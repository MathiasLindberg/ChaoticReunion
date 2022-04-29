using System;
using dk.lego.devicesdk.bluetooth.V3bootloader.messages;
using LEGO.Logger;
using CoreUnityBleBridge;

namespace LEGODeviceUnitySDK
{
    public class V3BootloaderDevice : AbstractLEGODevice, IFlashLoaderDevice
    {
        public const string V3_BOOTLOADER_SERVICE = "00001625-1212-efde-1623-785feabcd123";
        public const string V3_BOOTLOADER_CHARACTERISTIC = "00001626-1212-efde-1623-785feabcd123";

        private static readonly ILog logger = LogManager.GetLogger(typeof(V3BootloaderDevice));

        internal V3BootloaderDevice (IBleDevice bleDevice)
            : base(bleDevice)
        {
            DeviceID = bleDevice.ID;
            DeviceInfo = new LEGODeviceInfo();
        }

        #region Bootloader related
        public override bool IsBootLoader {
            get { return true; }
        }

        public override DeviceCompatibleProtocolSpecificationVersionType CompatibleProtocolSpecification {
            get {
                return DeviceCompatibleProtocolSpecificationVersionType.LEDeviceCompatibleProtocolVersionBootLoaderV3x;
            }
        }
        #endregion

        #region Lifecycle
        private V3BootloaderDeviceInterrogationState interrogationState;

        protected override void DidConnect()
        {
            this.interrogationState = new V3BootloaderDeviceInterrogationState(this, UpdateDeviceState);
        }

        protected override void DidDisconnect()
        {
            // Clear state:
            if (interrogationState != null) 
            {
                interrogationState.DidDisconnect();
                interrogationState = null;
            }
        }

        protected override DeviceState ComputeConnectedDeviceState()
        {
            if (interrogationState != null)
            {
                switch (interrogationState.CurrentStatus)
                {
                    case V3BootloaderDeviceInterrogationState.Status.Interrogating:
                        return DeviceState.Interrogating;

                    case V3BootloaderDeviceInterrogationState.Status.Completed:
                        return DeviceState.InterrogationFinished;
                    
                    case V3BootloaderDeviceInterrogationState.Status.Failed:
                        //In case of devices stuck in bootloader mode re-connects, we dont return InterrogationFailed but instead InterrogationFinished, for connections to succeed
                        return DeviceState.InterrogationFinished;
                }
            }

            return DeviceState.Connecting;
            
        }
        #endregion


        #region - Callbacks from native - device advertising properties
        internal override void ManufacturerDataUpdated(byte[] data) {
            if (data == null) {
                logger.Debug("Got ManufacturerData=null for device "+DeviceID);
                return;
            }

            if (data.Length >= 4) {
                var tmp = new byte[4];
                Array.Copy(data, 0, tmp, 0, tmp.Length);
                DeviceInfo.SetFirmwareRevision(LEGORevision.parse(tmp));
            }
            if (data.Length >= 5) {
                SetSystemTypeID(data[4]);
            }
            if (data.Length >= 6) {
                SupportedFunctions = (DeviceFunction) data[5];
            }
        }
        #endregion

        #region Communication with device
        internal override string BTServiceUUID { get {return V3_BOOTLOADER_SERVICE;} }
        internal override string BTCharacteristicUUID { get {return V3_BOOTLOADER_CHARACTERISTIC;} }

        public event Action<UpstreamMessage> OnReceivedMessage;
        public event Action<int> OnPacketSent;

        internal override void ReceivedPacket(byte[] data)
        {
            UpstreamMessage msg;
            try {
                msg = UpstreamMessage.parse(data);
            } catch (Exception) {
                logger.Error("Received invalid message: "+BitConverter.ToString(data));
                return;
            }

            HandleReceivedMessage(msg);

            if (OnReceivedMessage != null) {
                try {
                    OnReceivedMessage(msg);
                } catch (Exception e) {
                    logger.Error("Exception occurred when handling upstream message: "+e+"\nTrace: "+e.StackTrace);
                }
            }
        }

        private void HandleReceivedMessage(UpstreamMessage msg)
        {
            if (msg is GetInfoResp)
                HandleReceivedMessage((GetInfoResp)msg);
        }

        private void HandleReceivedMessage(GetInfoResp msg)
        {
            DeviceInfo.SetFirmwareRevision(msg.flashLoaderVersion);
            SetSystemTypeID(msg.systemTypeID);

            interrogationState.HandleMessage(msg);
        }

        public void SendMessage(DownstreamMessage msg, int? seqNr = null, bool useSoftAck = false)
        {
            DoSendMessage(msg, seqNr, useSoftAck);
            if (msg is Disconnect || msg is StartAppReq)
                MarkAsExpectingConnectionToClose();
        }

        internal void DoSendMessage(DownstreamMessage message, int? seqNr, bool useSoftAck) {
            byte[] data;
            try {
                data = message.unparse();
            } catch (Exception e) {
                logger.Error("Failed to serialize message "+message, e);
                return;
            }

            if (seqNr.HasValue)
                SendPacketWithFeedback(data, seqNr.Value, useSoftAck);
            else
                SendPacket(data, packetDroppingInfo: null);
        }

        internal override void PacketTransmitted(int seqNr) {
            logger.Debug("PacketTransmitted: "+seqNr);
            if (OnPacketSent != null)
                OnPacketSent(seqNr);
        }

        void IFlashLoaderDevice.Disconnect() {
            base.Disconnect();
        }

        #endregion

        #region Dynamic device properties
        public override bool IsValidDeviceNameLength (string name) { throw new NotImplementedException (); }
        public override void UpdateDeviceName (string name) { throw new NotImplementedException (); }

        public override bool IsAlertHigh (HubAlertType type) { return false; }
        public override void SetConnectedNetworkID (int networkID) { throw new NotImplementedException (); }

        public override void EnableValueUpdates (bool enable, ValueEnableUpdateType type) { throw new NotImplementedException (); }

        public override void UpdateValueForType (ValueUpdateType type) { throw new NotImplementedException (); }

        public override void ResetValueForType (ValueResetType type) { throw new NotImplementedException (); }

        public override void EnableBusyIndication (bool enable) { throw new NotImplementedException (); }

        public override void SetupVirtualMotorPair(uint portID1, uint portID2) { throw new NotImplementedException(); }
        public override void DisconnectVirtualMotorPair(uint portID){ throw new NotImplementedException(); }
        #endregion

        #region Services
        private ILEGOService[] _services = new ILEGOService[0];

        public override ILEGOService FindService (IOType type, bool virtualConnection) { return null; }
        public override ILEGOService FindService (int portID) { return null; }
        public override ILEGOService FindVirtualService(int portID1, int portID2) { return null; }

        public override ILEGOService[] Services { get { return _services; } }
        public override ILEGOService[] InterternalServices { get { return _services; } }
        public override ILEGOService[] ExternalServices { get { return _services; } }
        #endregion

        #region Event hooks
        #pragma warning disable 067
        public override event Action<ILEGODevice, HubAlertType, bool> OnAlertStateUpdated;
        public override event Action<ILEGODevice, ILEGOService, bool /* connected */ > OnServiceConnectionChanged;
        public override event Action<ILEGODevice, string> OnFailToAddServiceWithError;
        public override event Action<ILEGODevice> OnDeviceWillSwitchOff;
        public override event Action<ILEGODevice> OnDeviceWillDisconnect;
        public override event Action<ILEGODevice> OnDeviceWillGoIntoBootMode;
        public override event Action<ILEGODevice, MessageErrorType, string, MessageType> OnDidExperienceError;
        #pragma warning restore 067
        #endregion
    }
}

