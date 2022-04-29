/**
 *
 * -- Ported Code --
 * Ported C# version of SimpleLink BLE OAD iOS from Texas Instruments.
 *
 *  !!-- ATTENTION --!!
 * Functionality regarding RSSI and timers has been omitted from the ported code below.
 * 
 * Ported SimpleLink Version: 1.0.1
 * 
 * Source URL: http://git.ti.com/simplelink-ble-oad-ios/simplelink-ble-oad-ios/trees/master/ti_oad/Classes
 *
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using CoreUnityBleBridge;
using CoreUnityBleBridge.Model;
using JetBrains.Annotations;
using LEGO.Logger;

// ReSharper disable once CheckNamespace
namespace LEGODeviceUnitySDK
{
    public class OADClient : IOADClient
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(OADClient));

        public OADClientProgressValues progress; 

        private IOADImageReader imgData;
        [UsedImplicitly] private byte lastErrorCode;

        public event Action<OADClient, OADClientProgressValues> OnProgressUpdated;
        public event Action<OADClient, OADClientState> OnProcessStateChanged;
        
        public event Action RequestMTUSizeForTransfer = delegate {};

        private readonly IBleDevice bleDevice;
        private OADClientState state;

        public OADFirmwareUpgradeProcess.ImageDataType currentTransferingImageDataType = OADFirmwareUpgradeProcess.ImageDataType.Firmware;

        private OADTransferStats transferStats;
        public OADTransferStats TransferStats => transferStats;

        public OADClient(IBleDevice bleDevice)
        {
            state = OADClientState.Initializing;
            this.bleDevice = bleDevice;
            this.transferStats = new OADTransferStats();
            Subscribe();
        }

        private void Subscribe()
        {
            bleDevice.ConnectionStateChanged += BleDeviceOnConnectionStateChanged;
            bleDevice.PacketReceived += BleDeviceOnPacketReceived;
        }

        // ReSharper disable once UnusedMember.Local
        private void Unsubscribe()
        {
            bleDevice.ConnectionStateChanged -= BleDeviceOnConnectionStateChanged;
            bleDevice.PacketReceived -= BleDeviceOnPacketReceived;
        }
        
        public uint OADDeviceID { get; private set; }

        public void HandleInterrogationCompleted()
        {
            DeviceStateReady();
        }
        
        public void HandleTransferCompleted()
        {
            DeviceStateReady();
        }

        private void DeviceStateReady()
        {
            state = OADClientState.Ready;
        }

        // ReSharper disable once UnusedMember.Local
        private void DidUpdateNotificationStateForCharacteristic(string characteristic)
        {
            const bool isNotifying = false;
            Logger.Debug("OADClient: Notification for " + characteristic + " set to : " + isNotifying);
            if (characteristic.Equals(OADDefines.OAD_CONTROL_CHARACTERISTIC))
            {
                //We can start sending commands here
                SendStateChangedWithoutErrorBasedOnState();
            }
        }


        public void StartOAD(IOADImageReader imageData, OADFirmwareUpgradeProcess.ImageDataType imgDataType)
        {
            Logger.Warn("Starting OAD image transfer with data type " + imgDataType);
            currentTransferingImageDataType = imgDataType;
            
            if (state != OADClientState.Ready )
            {
                Logger.Error("Device is not ready OADClientState:" + state);
            }
            
            
            if (imageData == null)
            {
                throw new NullReferenceException("ITOADImageReader argument to StartOAD cannot be null");
            }

            imgData = imageData;
            progress.TotalBytes = (UInt32) imageData.GetRAWData().Length;
            progress.StartDownloadTime = DateTime.Now.ToOADate();
            progress.LastBlockSent = UInt32.MaxValue;
            //Init says we are ready, then start by requesting device type etc.
            SendGetDeviceTypeCommand();
            RequestMTUSizeForTransfer();
        }

        public void CancelOAD()
        {
            SendOadControlCancelOadCmd();
        }

        private void SendStateChangedWithoutErrorBasedOnState()
        {
            Logger.Warn(string.Format("State changed to: {0}({1})", state, (byte) state));
            ProcessStateChanged(state);
        }

        private void SendStateChangedWithErrorBasedOnState(byte statusByte)
        {
            switch (state)
            {
                case OADClientState.PeripheralNotConnected:
                    Logger.Error("Peripheral is not connected, OADClient cannot continue. Please connect and discover services and characteristics before calling OADClient init !");
                    ProcessStateChanged(OADClientState.PeripheralNotConnected);
                    break;
                case OADClientState.OADServiceMissingOnPeripheral:
                    Logger.Error("Peripheral is missing the correct characteristics, OADClient cannot continue. Please connect and discover services and characteristics before calling OADClient init !");
                    ProcessStateChanged(OADClientState.OADServiceMissingOnPeripheral);
                    break;
                case OADClientState.ImageTransferFailed:
                    Logger.Error("Image transfer failed during programming ...");
                    ProcessStateChanged(OADClientState.ImageTransferFailed);
                    break;
                case OADClientState.CompleteFeedbackFailed:
                    Logger.Error("Download to peripheral went OK, but peripheral would not start, please initialize a new OADClient and run the process again !");
                    ProcessStateChanged(OADClientState.CompleteFeedbackFailed);
                    break;
                case OADClientState.HeaderFailed:
                    Logger.Error("Peripheral received header but would not accept, try another image and initialize again ... Peripheral status : " +GetStatusStringFromStatusByte(statusByte));
                    ProcessStateChanged(OADClientState.HeaderFailed);
                    break;
                case OADClientState.DisconnectedDuringDownload:
                    Logger.Error("Peripheral disconnected during download .. Please try and reconnect.\nReceived status byte: " + statusByte +"\n" + GetStatusStringFromStatusByte(statusByte));
                    ProcessStateChanged(OADClientState.DisconnectedDuringDownload);
                    break;
                default:
                    Logger.Error("Unknown error code during programming ...\nReceived status byte: " + statusByte +"\n" + GetStatusStringFromStatusByte(statusByte));
                    var abstractLegoDevice = (bleDevice as AbstractLEGODevice);
                    if (abstractLegoDevice != null)
                    {
                        Logger.Error("Printing out device details for debugging ...\nManufacturerName:" +
                                     abstractLegoDevice.DeviceInfo.ManufacturerName +
                                     "\nFirmwareRevision:" + abstractLegoDevice.DeviceInfo.FirmwareRevision +
                                     "\nHardwareRevision:" + abstractLegoDevice.DeviceInfo.HardwareRevision +
                                     "\nSoftwareRevision:" + abstractLegoDevice.DeviceInfo.SoftwareRevision +
                                     "\nRadioFirmwareVersion:" + abstractLegoDevice.DeviceInfo.RadioFirmwareVersion +
                                     "\nDeviceState:" + abstractLegoDevice.State +
                                     "\nHubCalculatedRSSIValue:" + abstractLegoDevice.HubCalculatedRSSIValue +
                                     "\nRSSIValue:" + abstractLegoDevice.RSSIValue +
                                     "\nBatteryLevel:" + abstractLegoDevice.BatteryLevel);
                    }
                    Logger.Error("Unknown error code during programming ... State received: " + state);
                    ProcessStateChanged(state);
                    break;
            }
        }

        private void ProcessStateChanged(OADClientState newState)
        {
            MainThreadDispatcher.Enqueue(() => OnProcessStateChanged?.Invoke(this, newState));
        }
        
        #region OAD Control command senders below (Private functions)

        private void SendGetDeviceTypeCommand()
        {
            const byte data = OADDefines.TOAD_CONTROL_CMD_GET_DEVICE_TYPE_CMD;
            SendPacket(OADDefines.OAD_CONTROL_CHARACTERISTIC, data);
            state = OADClientState.GetDeviceTypeCommandSent;
            Logger.Debug(string.Format("SendGetDeviceTypeCommand: OADClient: OAD Control TX: {0:X2}", data));
            Logger.Warn("Send GetDeviceType Command");
        }

        private void SendOadControlGetOadBlockSizeCmd()
        {
            const byte data = OADDefines.TOAD_CONTROL_CMD_GET_BLOCK_SIZE;
            SendPacket(OADDefines.OAD_CONTROL_CHARACTERISTIC, data);
            state = OADClientState.BlockSizeRequestSent;
            Logger.Debug(string.Format("OADClient: OAD Control TX: {0:X2}", data));
            Logger.Warn("Send OadControlGetOadBlockSize Command");
        }

        // ReSharper disable once UnusedMember.Local
        private byte[] Combine(params byte[][] arrays)
        {
            var rv = new byte[arrays.Sum(a => a.Length)];
            var offset = 0;
            foreach (byte[] array in arrays)
            {
                Buffer.BlockCopy(array, 0, rv, offset, array.Length);
                offset += array.Length;
            }

            return rv;
        }

        // ReSharper disable once UnusedMember.Local
        private static byte[] AddByteToArray(byte[] bArray, byte newByte)
        {
            byte[] newArray = new byte[bArray.Length + 1];
            bArray.CopyTo(newArray, 1);
            newArray[0] = newByte;
            return newArray;
        }

        // ReSharper disable once UnusedMember.Global
        public byte[] Concatenate(byte[] aArray, byte[] bArray)
        {
            byte[] newArray = new byte[aArray.Length + bArray.Length];
            aArray.CopyTo(newArray, 0);
            bArray.CopyTo(newArray, aArray.Length);
            return newArray;
        }

        private void SendOADImageHeader()
        {
            if (imgData == null)
            {
                Logger.Error("Cannot send OAD image header, image data is NULL!");
                return;
            }
            var header = imgData.GetHeader();
            var imageLength = header.OADImageLength;
            var bufferSize = Buffer.ByteLength(header.OADImageIdentificationValue) + header.OADImageBIMVersion +
                             header.OADImageHeaderVersion + Buffer.ByteLength(header.OADImageInformation) +
                             Marshal.SizeOf(imageLength) + Buffer.ByteLength(header.OADImageSoftwareVersion);
            var buffer = new BinaryWriter(new MemoryStream(bufferSize));
            buffer.Write(header.OADImageIdentificationValue);
            buffer.Write(header.OADImageBIMVersion);
            buffer.Write(header.OADImageHeaderVersion);
            buffer.Write(header.OADImageInformation);
            buffer.Write(header.OADImageLength);
            buffer.Write(header.OADImageSoftwareVersion);
            var headerReq = ((MemoryStream) buffer.BaseStream).ToArray();
            SendPacket(OADDefines.OAD_IMAGE_NOTIFY_CHARACTERISTIC, headerReq);
            state = OADClientState.HeaderSent;
            Logger.Debug(string.Format("OADClient: OAD Image Identify TX: {0}", StringFromData(headerReq)));
        }

        private void SendOadControlStartOadProcessCmd()
        {
            const byte data = OADDefines.TOAD_CONTROL_CMD_START_OAD_PROCESS;
            SendPacket(OADDefines.OAD_CONTROL_CHARACTERISTIC, data);
            state = OADClientState.OADProcessStartCommandSent;
            progress.LastBlockSent = UInt32.MaxValue;
            transferStats.StartSession( currentBlockSize:progress.CurrentBlockSize, dataSizeBytes:progress.TotalBytes, statsMode:OADTransferStats.StatsMode.WindowCaptureOnly);
            Logger.Warn("Sending OAD Start Process Command!");
            Logger.Debug(string.Format("OADClient: OAD Control TX: {0:X2}", data));
        }
        
        private void SendOadControlCancelOadCmd()
        {
            const byte data = OADDefines.TOAD_CONTROL_CMD_CANCEL_OAD;
            SendPacket(OADDefines.OAD_CONTROL_CHARACTERISTIC, data);
            state = OADClientState.Ready;
            Logger.Warn("Sending OAD Cancel Command!");
            Logger.Debug(string.Format("OADClient: OAD Control TX: {0:X2}", data));
        }

        private void SendOadNextImageBlock()
        {
            progress.CurrentByte = progress.CurrentBlock * progress.CurrentBlockSize;
            progress.PercentProgress =  progress.CurrentByte * 100.0f /  progress.TotalBytes;
            var currentBlockSize = progress.CurrentBlockSize;
            var currentBlockPosition = progress.CurrentByte;
            
            var imgRawData = imgData.GetRAWData();
            if (progress.CurrentByte + currentBlockSize > imgRawData.Length)
            {
                currentBlockSize = (uint)imgRawData.Length - progress.CurrentByte;
            }
            
            var imgBlock = new byte[sizeof(uint) + currentBlockSize];
            var currentBlockBytes = BitConverter.GetBytes(progress.CurrentBlock);
            Buffer.BlockCopy(currentBlockBytes, 0, imgBlock, 0, currentBlockBytes.Length);
            Buffer.BlockCopy(imgRawData, (int)currentBlockPosition, imgBlock, currentBlockBytes.Length, (int)currentBlockSize);
            
            SendPacket(OADDefines.OAD_IMAGE_BLOCK_REQUEST_CHARACTERISTIC, imgBlock);
            transferStats.SentBlock(progress.CurrentBlock, progress.CurrentBlockSize);
            Logger.Debug(string.Format("OADClient: OAD Block TX: {0}, length {1}", StringFromData(imgBlock), imgBlock.Length));
        }

        private void SendOadControlEnableOadImageCmd()
        {
            const byte data = OADDefines.TOAD_CONTROL_CMD_ENABLE_OAD_IMAGE_CMD;
            SendPacket(OADDefines.OAD_CONTROL_CHARACTERISTIC, data);
            state = OADClientState.EnableOADImageCommandSent;
            Logger.Warn("Sending OAD Enable Image Command!");
            Logger.Debug(string.Format("OADClient: OAD Control TX: {0:X2}", data));
        }


        public void SendPacket(string gattChar, params byte[] data)
        {
            bleDevice.SendPacket(OADDefines.OAD_SERVICE, gattChar, data);
        }

        #endregion

        #region OAD State machine (Private)

        private void OadStatMachineIterate()
        {
            Logger.Debug("OadStatMachineIterate, state: " + state);
            switch (state)
            {
                case OADClientState.GetDeviceTypeResponseReceived:
                    SendOadControlGetOadBlockSizeCmd();
                    SendStateChangedWithoutErrorBasedOnState();
                    break;
                case OADClientState.GotBlockSizeResponse:
                    //We have block size, send header
                    SendOADImageHeader();
                    SendStateChangedWithoutErrorBasedOnState();
                    break;
                case OADClientState.HeaderOK:
                    //Header sent OK, we send start of OAD process command...
                    SendOadControlStartOadProcessCmd();
                    SendStateChangedWithoutErrorBasedOnState();
                    break;
                case OADClientState.ImageTransfer:
                    //Image block transfer
                    SendOadNextImageBlock();
                    MainThreadDispatcher.Enqueue(() => OnProgressUpdated?.Invoke(this, progress));
                    break;
                case OADClientState.ImageTransferOK:
                    //Image transferred OK !
                    progress.CurrentBlock = progress.TotalBlocks;
                    progress.CurrentByte = progress.TotalBytes;
                    MainThreadDispatcher.Enqueue(() => OnProgressUpdated?.Invoke(this, progress));
                    SendStateChangedWithoutErrorBasedOnState();
                    SendOadControlEnableOadImageCmd();
                    if (currentTransferingImageDataType == OADFirmwareUpgradeProcess.ImageDataType.Assetpack)
                    {
                        SendOadControlCancelOadCmd();
                    } 
                    break;
            }
        }

        #endregion

        private string GetStatusStringFromStatusByte(byte statusByte)
        {
            return OADDefines.EOAD_STATUS_STRINGS[statusByte];
        }
        
        public string GetStateStringFromState()
        {
            return state <= OADClientState.RSSIGettingLow ? OADDefines.EOAD_STATE_STRINGS[(int) state] : string.Format("Unknown state: {0}", state);
        }


        #region BLEDevice call-back handlers (corresponds to CBPeripheralDelegate and CBCentralManagerDelegate on iOS)

        private void BleDeviceOnConnectionStateChanged()
        {
            var connectionState = bleDevice.Connectivity;
            Logger.Warn("BleDeviceOnConnectionStateChanged connectionState: " +connectionState );
            if (connectionState == DeviceConnectionState.Disconnected)
            {
                Logger.Warn("Connection state is Disconnected, OADClient state is " + state);
                if (state != OADClientState.CompleteFeedbackOK && state != OADClientState.EnableOADImageCommandSent)
                {
                    state = OADClientState.DisconnectedDuringDownload;
                    SendStateChangedWithErrorBasedOnState(6);
                }
                else
                {
                    state = OADClientState.OADCompleteIncludingDisconnect;
                    SendStateChangedWithoutErrorBasedOnState();
                }
                Logger.Warn("Connection state modified, new state: " + state);
            }
        }

        public void BleDeviceOnPacketReceived(PacketReceivedEventArgs obj)
        {
            if (! IsEqualUUID(obj.Service, OADDefines.OAD_SERVICE))
            {
                //not a package for the OADService
                return;
            }
            
            if (IsEqualUUID(obj.GattCharacteristic, OADDefines.OAD_CONTROL_CHARACTERISTIC))
            {
                var data = obj.Data;

                Logger.Debug(string.Format("OADClient: OAD Control RX: {0}", StringFromData(data)));
                if (obj.GattCharacteristic.Length < 1)
                    return;

                if (data == null || data.Length < 1)
                {
                    Logger.Error("Contents of received packet is null/empty!");
                    return;
                }
                switch (data[0])
                {
                    case OADDefines.TOAD_CONTROL_CMD_GET_BLOCK_SIZE:
                        progress.CurrentBlockSize = (((UInt32) data[2] << 8) | (data[1])) - 4;
                        progress.TotalBlocks = progress.TotalBytes / progress.CurrentBlockSize;
                        Logger.Debug(string.Format("OADClient: OAD block size response: {0}", progress.CurrentBlockSize));
                        state = OADClientState.GotBlockSizeResponse;
                        SendStateChangedWithoutErrorBasedOnState();
                        OadStatMachineIterate();
                        break;
                    case OADDefines.TOAD_CONTROL_CMD_IMAGE_BLOCK_WRITE_CHAR_RESPONSE:
                        progress.CurrentBlock = ((UInt32) data[5] << 24) | ((UInt32) data[4] << 16) | ((UInt32) data[3] << 8) | (data[2]);
                        
                        if (progress.LastBlockSent >= progress.CurrentBlock)
                        {
                            transferStats.RepeatRequest(progress.CurrentBlock, progress.CurrentBlockSize);
                            Logger.Debug(string.Format("Repeat Requested - LastBlockSent({0}) >= CurrentBlock({1})",progress.LastBlockSent, progress.CurrentBlock));
                        }
                        progress.LastBlockSent = progress.CurrentBlock;

                        Logger.Debug(string.Format("OADClient: OAD Image Block write char response: {0} last status {1}", progress.CurrentBlock,
                            data[1]));

                        switch (data[1])
                        {
                            case 0x00:
                                if (state != OADClientState.ImageTransfer)
                                {
                                    state = OADClientState.ImageTransfer;
                                    SendStateChangedWithoutErrorBasedOnState();
                                }

                                state = OADClientState.ImageTransfer;
                                OadStatMachineIterate();
                                break;

                            case 0x0e:
                                state = OADClientState.ImageTransferOK;
                                OadStatMachineIterate();
                                transferStats.EndSession();
                                break;
                            default:
                                state = OADClientState.ImageTransferFailed;
                                lastErrorCode = data[1];
                                SendStateChangedWithErrorBasedOnState(data[1]);
                                transferStats.EndSession();
                                break;
                        }

                        break;
                    case OADDefines.TOAD_CONTROL_CMD_ENABLE_OAD_IMAGE_CMD:
                        progress.TotalDownloadTime = DateTime.Now.ToOADate() - progress.StartDownloadTime;
                        switch (data[1])
                        {
                            case 0x00:
                                state = OADClientState.CompleteFeedbackOK;
                                SendStateChangedWithoutErrorBasedOnState();

                                break;
                            default:
                                state = OADClientState.CompleteFeedbackFailed;
                                SendStateChangedWithErrorBasedOnState(data[1]);
                                break;
                        }

                        break;
                    case OADDefines.TOAD_CONTROL_CMD_GET_DEVICE_TYPE_CMD:
                        OADDeviceID = (uint) data[1] << 24 | (uint) data[2] << 16 | data[3] | data[4];
                        state = OADClientState.GetDeviceTypeResponseReceived;
                        SendStateChangedWithoutErrorBasedOnState();
                        OadStatMachineIterate();
                        break;
                }
            }

            else if (IsEqualUUID(obj.GattCharacteristic, OADDefines.OAD_IMAGE_BLOCK_REQUEST_CHARACTERISTIC))
            {
                Logger.Debug(string.Format("OADClient: OAD Block RX: {0}", obj.GattCharacteristic));
            }

            else if (IsEqualUUID(obj.GattCharacteristic, OADDefines.OAD_IMAGE_NOTIFY_CHARACTERISTIC))
            {
                var data = obj.Data;
                Logger.Debug(string.Format("OADClient: OAD Identify RX: {0}", obj.GattCharacteristic));

                if (data == null || data.Length < 1)
                {
                    Logger.Error("Contents of received packet is null/empty!");
                    return;
                }
                
                switch (data[0])
                {
                    case 0x00:
                        Logger.Debug("Success, we can start the OAD process !");
                        state = OADClientState.HeaderOK;
                        OadStatMachineIterate();
                        break;

                    default:
                        Logger.Debug("Failed, we have to stop");
                        state = OADClientState.HeaderFailed;
                        lastErrorCode = data[0];
                        SendStateChangedWithErrorBasedOnState(data[0]);
                        break;
                }
            }
        }

        private bool IsEqualUUID(string uuid1, string uuid2)
        {
            return string.Equals(uuid1, uuid2, StringComparison.OrdinalIgnoreCase);
        }

        private string StringFromData(byte[] data)
        {
            if(data != null)
            {
                var str = "<" + BitConverter.ToString(data).Replace("-", string.Empty) + ">";
                str = System.Text.RegularExpressions.Regex.Replace(str, ".{8}", "$0 ");
                return str;
            }

            return "NULL";
        }
        
        #endregion
    }
}