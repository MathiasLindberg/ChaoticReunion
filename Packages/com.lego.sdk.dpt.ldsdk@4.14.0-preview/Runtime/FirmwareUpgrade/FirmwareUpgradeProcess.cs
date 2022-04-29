using System;
using UnityEngine;
using System.Collections;
using LEGO.Logger;
using dk.lego.devicesdk.bluetooth.V3bootloader.messages;

namespace LEGODeviceUnitySDK
{
    /// <summary>
    /// Represents and controls the process of firmware-upgrading a device.
    /// </summary>
    internal class FirmwareUpgradeProcess : IFirmwareUpgradeProcess, UpstreamMessage_Visitor<Void>
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(FirmwareUpgradeProcess));

        #region Constants governing the flashing process

        /// For how long should we wait for a device to reappear after disconnect?
        public const float REAPPEAR_PATIENCE_SECONDS = 65.0f;

        public const float CONNECT_PATIENCE_SECONDS = 10.0f;
        public const float INTERROGATE_PATIENCE_SECONDS = 20.0f;
        public const float GET_INFO_PATIENCE_SECONDS = 5.0f;
        public const float ERASE_PATIENCE_SECONDS = 20.0f;
        public const float INITIATE_PATIENCE_SECONDS = 10.0f;
        public const float PROGRESS_PATIENCE_SECONDS = 25.0f; // Patience per percentage progress

        public const float CHECK_PERIOD_SECONDS = 0.1f; // Normal check period
        public const float RESPONSE_CHECK_PERIOD_SECONDS = 0.005f; // Check period for request-response

        public const uint MAX_DATA_PAYLOAD_SIZE = 14; // Max message size: 20; minus (1+1+4) other bytes in the message.

        #endregion

        private readonly MonoBehaviour coroutineHelper;

        #region Info about the original device

        private readonly string originalDeviceID;
        private readonly DeviceSystemType originalSystemType;
        private readonly int originalSystemDeviceNumber;

        private string originalDeviceName; // Null if unknown.
        private int? originalDeviceNetworkID; // Null if unknown.

        #endregion

        #region Static process properties

        private readonly byte[] firmwareData;
        private readonly DeviceSystemType imageSystemType;
        private readonly int imageSystemNumber;
        private readonly FlashCompletionAction completionAction;
        private readonly Action<IFirmwareUpgradeProcess> onCompletion;
        private readonly DateTime startTime;

        #endregion

        #region Other dynamic properties

        private ILEGODevice device;
        private bool seekingFlashloader; // Used by FindDevice
        private ILEGODevice lastAppearedDevice; // Updated by events
        private IFlashLoaderDevice flashLoaderDevice; // Possibly make this local?
        private DateTime transferStartTime;
        private FlashErrorCode? errorCode;

        #endregion

        private DeviceFlashState _flashState = DeviceFlashState.Idle;

        

        public DeviceFlashState FlashState
        {
            get { return _flashState; }
            private set
            {
                _flashState = value;
                if (OnDeviceFlashStateUpdated != null)
                    OnDeviceFlashStateUpdated(this, _flashState);
            }
        }

        public FirmwareUpgradeProcess(ILEGODevice device, byte[] firmwareData, DeviceSystemType imageSystemType,
            int imageSystemNumber,
            FlashCompletionAction completionAction, MonoBehaviour coroutineHelper,
            Action<IFirmwareUpgradeProcess> onCompletion)
        {
            this.device = device;
            this.originalDeviceID = device.DeviceID;
            this.originalSystemType = device.SystemType;
            this.originalSystemDeviceNumber = device.SystemDeviceNumber;

            this.firmwareData = firmwareData;
            this.imageSystemType = imageSystemType;
            this.imageSystemNumber = imageSystemNumber;
            this.coroutineHelper = coroutineHelper;
            this.onCompletion = onCompletion;
            this.completionAction = SanitizeCompletionAction(completionAction, device);

            this.startTime = DateTime.UtcNow;
        }

        private static FlashCompletionAction SanitizeCompletionAction(FlashCompletionAction completionAction,
            ILEGODevice originalDevice)
        {
            switch (completionAction)
            {
                case FlashCompletionAction.AdvertiseAndAutoConnect:
                    if (originalDevice.IsBootLoader)
                    {
                        logger.Warn(
                            "Device to flash is already in bootloader mode, so we can't autoconnect to it after restart." +
                            " Will use AdvertiseNewImage instead of AdvertiseAndAutoConnect.");
                        completionAction = FlashCompletionAction.AdvertiseNewImage;
                    }

                    break;
                case FlashCompletionAction.AdvertiseNewImage:
                case FlashCompletionAction.TurnOff:
                    break;
                default: // Including NOT_CONFIGURED:
                    completionAction = FlashCompletionAction.AdvertiseNewImage;
                    break;
            }

            return completionAction;
        }

        #region Events

        public event Action<IFirmwareUpgradeProcess, DeviceFlashState> OnDeviceFlashStateUpdated;
        public event Action<IFirmwareUpgradeProcess, FlashErrorCode, string /* error message */> OnDeviceFlashFailed;
        public event Action<IFirmwareUpgradeProcess, float /* pct */> OnFlashProgressPercentageUpdated;

        private void FireError(FlashErrorCode errorCode, string errorMessage)
        {
            if (OnDeviceFlashFailed != null)
                OnDeviceFlashFailed(this, errorCode, errorMessage);
        }

        #endregion

        #region Flashing process

        public void StartProcess()
        {
            coroutineHelper.StartCoroutine(ProcessSupervisor(ProcessMain()));
        }

        /// <summary>
        /// Error handling wrapper. Ensures graceful handling of exceptions.
        /// </summary>
        private IEnumerator ProcessSupervisor(IEnumerator body)
        {
            // Robustly handle exceptions at any enumerator level:
            LEGODeviceManager.Instance.OnDeviceStateUpdated += DeviceStateUpdated;
            yield return SafeEnumerator.Create(new FlattenedEnumerator(body),
                exception =>
                {
                    if (exception is FlashingException)
                    {
                        var e = (FlashingException) exception;
                        logger.Warn("Flashing failed: " + e.Message);
                        FireError(e.ErrorCode, e.Message);
                    }
                    else
                    {
                        logger.Error("Flashing process crashed: " + exception + "\nTrace: " + exception.StackTrace);
                        FireError(FlashErrorCode.General, "Error encountered while flashing");
                    }
                });

            CleanAndWrapUp();
        }

        private void CleanAndWrapUp()
        {
            if (flashLoaderDevice != null)
            {
                flashLoaderDevice.OnPacketSent -= DidSendPacket;
                flashLoaderDevice.OnReceivedMessage -= DidReceiveMessage;
            }

            LEGODeviceManager.Instance.OnDeviceStateUpdated -= DeviceStateUpdated;
            FlashState = DeviceFlashState.Idle;
            var duration = DateTime.UtcNow - startTime;
            logger.Info("Firmware upgrade total duration: " + duration.TotalMilliseconds + "ms");

            if (onCompletion != null) onCompletion(this);
        }

        private void DeviceStateUpdated(ILEGODevice device, DeviceState oldState, DeviceState newState)
        {
            bool match = device.IsBootLoader ||
                         (device.DeviceID == originalDeviceID);
            
            logger.Debug("Device state updated, new state: " + newState);
            if (newState == DeviceState.DisconnectedNotAdvertising)
            {
                // Device was disconnected while flashing
                if (match && FlashState == DeviceFlashState.Flashing)
                {
                    logger.Debug("Device was disconnected while flashing. Stopping process and cleaning up.");
                    coroutineHelper.StopAllCoroutines();
                    CleanAndWrapUp();
                }
                
                return; // Is not visible.
            }

            if (IsConnectedTo(device)) return; // Is connected.

            if (device.SystemType != originalSystemType ||
                device.SystemDeviceNumber != originalSystemDeviceNumber)
            {
                logger.Debug("The device " + device + "/" + device.GetType().Name +
                             " has appeared, but has wrong system type.");
                return;
            }

            if (device.IsBootLoader != seekingFlashloader)
            {
                logger.Debug("The device " + device + "/" + device.GetType().Name +
                             " has appeared, but does not have the type we're interested in.");
                return;
            }


            if (match)
            {
                logger.Info("The device " + device + "/" + device.GetType().Name + " has appeared.");
                this.lastAppearedDevice = device;
            }
        }

        private IEnumerator ProcessMain()
        {
            yield return GetOriginalDeviceInfo();
            logger.Debug("Flashing process: Got necessary device info.");

            yield return EnsureConnectedToAFlashloaderDevice();
            logger.Debug("Flashing process: Ensured that we're connected to a bootloader device.");

            yield return SendImage();
            logger.Debug("Flashing process: Image sent.");

            yield return PerformCompletionAction();
            logger.Debug("Flashing process: Completion action is done.");
        }

        private IEnumerator GetOriginalDeviceInfo()
        {
            if (device.IsBootLoader ||
                completionAction != FlashCompletionAction.AdvertiseAndAutoConnect)
            {
                // We do not need interrogated info.
                yield break;
            }

            yield return Await(INTERROGATE_PATIENCE_SECONDS,
                () => device.State == DeviceState.InterrogationFinished,
                () => { throw new FlashingException("Timeout (interrogating)"); });

            this.originalDeviceName = device.IsBootLoader ? null : device.DeviceName;
            this.originalDeviceNetworkID = device.IsBootLoader ? (int?) null : device.LastConnectedNetworkId;
        }

        private IEnumerator EnsureConnectedToAFlashloaderDevice()
        {
            if (!IsConnectedTo(device))
            {
                throw new FlashingException("Must be connected to the device");
            }

            if (device is IFlashLoaderDevice)
            {
                flashLoaderDevice = (IFlashLoaderDevice) device;
            }
            else
            {
                if (device is LEGODevice)
                    ((LEGODevice) device).RequestToGoIntoFlashLoaderMode();
                else
                {
                    throw new FlashingException("Device is of a unrecognized type.");
                }

                FlashState = DeviceFlashState.WaitingToConnectToBootLoader;
                IFlashLoaderDevice foundDevice = null;
                yield return FindDevice<IFlashLoaderDevice>(d => { foundDevice = d; },
                    FlashErrorCode.TimedOutConnectingToBootLoader,
                    "Timed out waiting to discover device in flash loader mode.",
                    seekingFlashloader: true);
                device = flashLoaderDevice = foundDevice;

                FlashState = DeviceFlashState.ConnectingToBootLoader;
                yield return ConnectToDevice(flashLoaderDevice,
                    FlashErrorCode.TimedOutConnectingToBootLoader,
                    "Timed out connecting to device in flash loader mode.");
            }

            FlashState = DeviceFlashState.ConnectedToBootLoader;
        }

        private IEnumerator SendImage()
        {
            FlashState = DeviceFlashState.Flashing;
            this.transferStartTime = DateTime.UtcNow;
            yield return DoSendImage();
            var duration = DateTime.UtcNow - transferStartTime;
            logger.Info("Firmware upgrade transfer duration: " + duration.TotalMilliseconds + "ms");
            FlashState = DeviceFlashState.FlashingCompleted;
        }

        private IEnumerator PerformCompletionAction()
        {
            switch (completionAction)
            {
                case FlashCompletionAction.TurnOff:
                    SendAndDisconnect(new Disconnect());
                    break;

                case FlashCompletionAction.AdvertiseNewImage:
                    SendAndDisconnect(new StartAppReq());
                    break;

                case FlashCompletionAction.AdvertiseAndAutoConnect:
                    SendAndDisconnect(new StartAppReq());
                    // Auto-connect:
                    FlashState = DeviceFlashState.WaitingForNewlyFlashedDeviceToAdvertise;
                {
                    LEGODevice foundDevice = null;
                    yield return FindDevice<LEGODevice>(d => { foundDevice = d; },
                        FlashErrorCode.TimeOutConnectingToFlashedDevice,
                        "Timed out waiting to discover upgraded device.",
                        seekingFlashloader: false);
                    device = foundDevice;
                }
                    FlashState = DeviceFlashState.ConnectingToNewlyFlashedDevice;
                    yield return ConnectToDevice(device,
                        FlashErrorCode.TimeOutConnectingToFlashedDevice, "Timed out connecting to upgraded device.");

                    if (device is LEGODevice)
                    {
                        logger.Info("Restoring device properties");
                        if (originalDeviceName != null) device.UpdateDeviceName(originalDeviceName);
                        if (originalDeviceNetworkID.HasValue)
                            device.SetConnectedNetworkID(originalDeviceNetworkID.Value);
                    }

                    break;
            }

            yield break;
        }

        private void SendAndDisconnect(DownstreamMessage msg)
        {
            flashLoaderDevice.SendMessage(msg);
            // For good measure, disconnect from this ends as well:
            //flashLoaderDevice.Disconnect();
        }

        #endregion

        #region Send-image flow

        /* Flow: 
         * 1) Send "Erase flash"; await answer
         * 2) Send "Initiate loader"; await answer
         * 3) Send "Program flash" messages; await final response
         * 4) Error-checking of response
         */
        private GetInfoResp getInfoResponse;
        private EraseFlashResp eraseFlashResponse;
        private InitiateLoaderResp initiateLoaderResponse;
        private ProgramFlashDone programFlashResponse;
        private DateTime progressDeadline;

        private IEnumerator DoSendImage()
        {
            // Preparations:
            byte[] firmware = PadFirmware(firmwareData);
            byte checksum = CalculateChecksum(firmware);
            flashLoaderDevice.OnPacketSent += DidSendPacket;
            flashLoaderDevice.OnReceivedMessage += DidReceiveMessage;

            // Emit "0%":
            if (OnFlashProgressPercentageUpdated != null)
                OnFlashProgressPercentageUpdated(this, 0);

            // Get starting address & system info:
            getInfoResponse = null;
            flashLoaderDevice.SendMessage(new GetInfoReq());
            yield return Await(GET_INFO_PATIENCE_SECONDS,
                () => getInfoResponse != null,
                () => { throw new FlashingException("Timeout (getting system info)"); },
                checkPeriod: RESPONSE_CHECK_PERIOD_SECONDS);
            uint startingAddress = getInfoResponse.flashStartAddress;
            logger.Info(String.Format("Firmware starting address: 0x{0:X8}", startingAddress));
            if (startingAddress == 0)
            {
                //TODO: Keep this sanity check? Mostly paranoia ATM. --ESS
                throw new FlashingException(String.Format("Bad starting address: 0x{0:X8}", startingAddress));
            }

            {
                uint availableSpace = getInfoResponse.flashEndAddress - getInfoResponse.flashStartAddress + 1;
                if (firmware.Length > availableSpace ||
                    getInfoResponse.flashEndAddress < getInfoResponse.flashStartAddress)
                {
                    throw new FlashingException(String.Format(
                        "Firmware image does not fit into available address range (which has size {0})",
                        availableSpace));
                }
            }

            {
                var devSystemType = (DeviceSystemType) (getInfoResponse.systemTypeID >> 5);
                var devSystemDeviceNumber = (getInfoResponse.systemTypeID & 0x1F);
                if (devSystemType != imageSystemType ||
                    devSystemDeviceNumber != imageSystemNumber)
                {
                    throw new FlashingException("The firmware image is not appropriate for this device.");
                }
            }

            // Erase flash:
            eraseFlashResponse = null;
            flashLoaderDevice.SendMessage(new EraseFlashReq());
            yield return Await(ERASE_PATIENCE_SECONDS,
                () => eraseFlashResponse != null,
                () => { throw new FlashingException("Timeout (clearing flash memory)"); },
                checkPeriod: RESPONSE_CHECK_PERIOD_SECONDS);
            if (eraseFlashResponse.errorCode != 0)
                throw new FlashingException("Could not clear memory (error code: " + eraseFlashResponse.errorCode +
                                            ")");

            // Initiate loader:
            initiateLoaderResponse = null;
            flashLoaderDevice.SendMessage(new InitiateLoaderReq((uint) firmware.Length));
            yield return Await(INITIATE_PATIENCE_SECONDS,
                () => initiateLoaderResponse != null,
                () => { throw new FlashingException("Timeout (initiating loader)"); },
                checkPeriod: RESPONSE_CHECK_PERIOD_SECONDS);
            if (initiateLoaderResponse.errorCode != 0)
                throw new FlashingException("Could not initiate loader (error code: " +
                                            initiateLoaderResponse.errorCode + ")");
            var mayUseHardAck = (device as AbstractLEGODevice).SendStrategyForDevice !=
                                CoreUnityBleBridge.SendStrategy.NoAck;

            // Send (enqueue) image:
            programFlashResponse = null;
            uint offset = 0;
            int lastPct = 0; // We already sent "0%".
            this.lastPctFeedbackReceived = -1;
            while (offset < firmware.Length)
            {
                uint chunkSize = Math.Min(MAX_DATA_PAYLOAD_SIZE, (uint) firmware.Length - offset);
                var chunk = new byte[chunkSize];
                Array.Copy(firmware, (int) offset, chunk, 0, (int) chunkSize);

                bool isLastPacket = offset + chunkSize >= firmware.Length;
                int pct = (int) (offset + chunkSize) * 100 / firmware.Length;

                if (pct > lastPctFeedbackReceived + 2)
                    yield return null; // Give it a frame to progress.

                int seqNr;
                if (pct > lastPct)
                {
                    seqNr = pct;
                    lastPct = pct;
                }
                else
                {
                    seqNr = -1;
                }

                uint address = startingAddress + offset;
                flashLoaderDevice.SendMessage(new ProgramFlashData(address, chunk), seqNr,
                    useSoftAck: !(isLastPacket && mayUseHardAck));

                offset += chunkSize;
            }

            MoveDeadline();
            logger.Info("Transfer initiated (" + firmware.Length + " bytes)");

            // Await completion - or timeout:
            yield return Await(101 * PROGRESS_PATIENCE_SECONDS,
                () => (programFlashResponse != null ||
                       DateTime.UtcNow > progressDeadline),
                () => { throw new FlashingException("Timeout transferring firmware"); });
            logger.Info("Transfer complete - checking result");

            // Checking result:
            if (programFlashResponse == null)
            {
                throw new FlashingException("Progress is too slow. Aborting.");
            }

            if (programFlashResponse.byteCount != firmware.Length)
            {
                logger.Error("Transfer failed - wrong byte count. Actual: " + programFlashResponse.byteCount +
                             "; expected: " + firmware.Length);
                throw new FlashingException("Transfer failed :-( (wrong byte count)");
            }

            if (programFlashResponse.checkSum != checksum)
            {
                logger.Error("Transfer failed - wrong checksum. Actual: " + programFlashResponse.checkSum +
                             "; expected: " + checksum);
                throw new FlashingException("Transfer failed :-( (checksum error)");
            }
            else
            {
                logger.Info("Transfer completed succesfully :-)");
            }
        }

        private static byte[] PadFirmware(byte[] unpadded)
        {
            bool is16bitMultiple = (unpadded.Length & 0x1) == 0;
            if (is16bitMultiple)
            {
                return unpadded;
            }
            else
            {
                byte[] padded = new byte[unpadded.Length + 1];
                Array.Copy(unpadded, padded, unpadded.Length);
                padded[unpadded.Length] = 0xFF;
                return padded;
            }
        }

        private static byte CalculateChecksum(byte[] data)
        {
            byte checksum = 0xff;
            for (int i = 0; i < data.Length; i++)
                checksum ^= data[i];
            return checksum;
        }

        private void MoveDeadline()
        {
            progressDeadline = System.DateTime.UtcNow.AddSeconds(PROGRESS_PATIENCE_SECONDS);
        }

        #endregion

        #region Feedback handling

        private int lastPctFeedbackReceived;

        private void DidSendPacket(int pct)
        {
            if (pct < 0) return; // Ignore

            MoveDeadline();

            if (pct < lastPctFeedbackReceived) return; // Feedback may come out-of-order. Be robust about this.
            if (OnFlashProgressPercentageUpdated != null)
                OnFlashProgressPercentageUpdated(this, pct);
            lastPctFeedbackReceived = pct;
        }

        private void DidReceiveMessage(UpstreamMessage msg)
        {
            msg.visitWith(this, Void.Instance);
        }

        public void handle_EraseFlashResp(EraseFlashResp msg, Void arg)
        {
            this.eraseFlashResponse = msg;
        }

        public void handle_ProgramFlashDone(ProgramFlashDone msg, Void arg)
        {
            this.programFlashResponse = msg;
        }

        public void handle_InitiateLoaderResp(InitiateLoaderResp msg, Void arg)
        {
            this.initiateLoaderResponse = msg;
        }

        public void handle_GetInfoResp(GetInfoResp msg, Void arg)
        {
            this.getInfoResponse = msg;
            logger.Debug(String.Format("Bootloader flash address range: {0:X8}-{1:X8}", msg.flashStartAddress,
                msg.flashEndAddress));
            logger.Debug(String.Format("Bootloader system type: {0:X2}", msg.systemTypeID));
            logger.Info("Bootloader flash loader version: " + msg.flashLoaderVersion);
        }

        public void handle_GetChecksumResp(GetChecksumResp msg, Void arg)
        {
        }

        public void handle_GetFlashStateResp(GetFlashStateResp msg, Void arg)
        {
        }

        #endregion

        #region Process building blocks

        private IEnumerator FindDevice<DeviceType>(Action<DeviceType> returnDevice, FlashErrorCode timeoutErrorCode,
            string timeoutMsg, bool seekingFlashloader)
            where DeviceType : class, ILEGODevice
        {
            // Setup:
            this.seekingFlashloader = seekingFlashloader;

            // Start scanning in order to find bootloader device:
            LEGODeviceManager.Instance.Scan();

            // Await reappearance:
            DeviceType foundDevice = null;
            yield return Await(REAPPEAR_PATIENCE_SECONDS,
                () =>
                {
                    foundDevice = lastAppearedDevice as DeviceType;
                    return (foundDevice != null);
                },
                () => { throw new FlashingException(timeoutMsg, timeoutErrorCode); });
            logger.Info("Refound device: " + foundDevice + " id=" + foundDevice.DeviceID + " type=" +
                        foundDevice.SystemType + "#" + foundDevice.SystemDeviceNumber);

            // Stop scanning:
            LEGODeviceManager.Instance.StopScanning();

            returnDevice(foundDevice);
        }

        private static IEnumerator ConnectToDevice(ILEGODevice theDevice, FlashErrorCode timeoutErrorCode,
            string timeoutMsg)
        {
            LEGODeviceManager.Instance.ConnectToDevice(theDevice);
            yield return Await(CONNECT_PATIENCE_SECONDS,
                () => IsConnectedTo(theDevice),
                () => { throw new FlashingException(timeoutMsg, timeoutErrorCode); });
        }

        #endregion

        #region Helper functions

        private static IEnumerable Await(float patience, Func<bool> predicate, Action onTimeout,
            float checkPeriod = CHECK_PERIOD_SECONDS)
        {
            var startTime = Time.time;
            do
            {
                if (predicate()) yield break;
                yield return new WaitForSecondsRealtime(checkPeriod);
            } while (Time.time < startTime + patience);

            onTimeout();
        }

        private static bool IsConnectedTo(ILEGODevice device)
        {
            switch (device.State)
            {
                case DeviceState.Interrogating:
                case DeviceState.InterrogationFinished:
                    return true;
                default:
                    return false;
            }
        }

        #endregion

        private class FlashingException : Exception
        {
            public FlashErrorCode ErrorCode { get; private set; }

            public FlashingException(string message, FlashErrorCode errorCode = FlashErrorCode.General)
                : base(message)
            {
                this.ErrorCode = errorCode;
            }
        }
    }
}