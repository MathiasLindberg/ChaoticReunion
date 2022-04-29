using System.Collections.Generic;
using LEGO.Logger;
using System;
using System.Text;
using dk.lego.devicesdk.bluetooth.V3.messages;
using System.IO;
using System.Threading.Tasks;
using CoreUnityBleBridge;

namespace LEGODeviceUnitySDK
{
    public class LEGODevice : AbstractLEGODevice, LegoMessage_Visitor<object>
    {

        static readonly ILog logger = LogManager.GetLogger(typeof(LEGODevice));

        public const string V3_SERVICE = "00001623-1212-efde-1623-785feabcd123";
        public const string V3_CHARACTERISTIC = "00001624-1212-efde-1623-785feabcd123";

        #region Event hooks
        #pragma warning disable 0067
        public override event Action<ILEGODevice, HubAlertType, bool> OnAlertStateUpdated;
        public override event Action<ILEGODevice, ILEGOService, bool /* connected */ > OnServiceConnectionChanged;
        public override event Action<ILEGODevice, string> OnFailToAddServiceWithError;
        public override event Action<ILEGODevice> OnDeviceWillSwitchOff;
        public override event Action<ILEGODevice> OnDeviceWillDisconnect;
        public override event Action<ILEGODevice> OnDeviceWillGoIntoBootMode;
        public override event Action<ILEGODevice, MessageErrorType, string, MessageType> OnDidExperienceError;
        #pragma warning restore 0067        
        #endregion

        public override DeviceCompatibleProtocolSpecificationVersionType CompatibleProtocolSpecification
        {
            get {
                return DeviceCompatibleProtocolSpecificationVersionType.LEDeviceCompatibleProtocolVersionV3x;
            }
        }

        public override bool IsBootLoader { get { return false; } }

        private Dictionary<HubAlertType, bool> alerts = new Dictionary<HubAlertType, bool>();
        
        internal LEGODevice(IBleDevice bleDevice)
            : base(bleDevice)
        {
            DeviceID = bleDevice.ID;
            
            OnDeviceStateUpdated += HandleDeviceStateUpdated;
        }

        private void HandleDeviceStateUpdated(ILEGODevice device, DeviceState oldState, DeviceState newState)
        {
            if (newState == DeviceState.InterrogationFinished && OADClient != null)
            {
                OADClient.HandleInterrogationCompleted();
            }

            if (newState == DeviceState.InterrogationFailed)
            {
                logger.Warn("Interrogation failed - disconnecting " + device?.DeviceName + ":" + device?.DeviceID);
                Disconnect(); // the failure to interrogate successfully can lead to some strange "crashed" hub device service module states that does not respond to messages brutal is best here..
            }
        }

        #region Device state
        private DeviceInterrogationState interrogationState;

        protected override DeviceState ComputeConnectedDeviceState()
        {
            if (interrogationState != null)
            {
                switch (interrogationState.CurrentStatus)
                {
                    case DeviceInterrogationState.Status.Interrogating:
                        logger.Debug("DeviceInterrogationState.Status.Interrogating");
                        return DeviceState.Interrogating;

                    case DeviceInterrogationState.Status.Completed:
                        logger.Debug("DeviceInterrogationState.Status.Completed");
                        return DeviceState.InterrogationFinished;
                    
                    case DeviceInterrogationState.Status.Failed:
                        logger.Debug("DeviceInterrogationState.Status.Failed");
                        return DeviceState.InterrogationFailed;
                }
            }

            return DeviceState.Connecting; 
        }
        
        protected override void DidConnect()
        {
            this.interrogationState = new DeviceInterrogationState(this, UpdateDeviceState);
        }


        protected override void DidDisconnect()
        {
            // Clear state:
            _services.ForEach(s => s.ResetQuaDisconnect());
            _services.Clear();

            if (interrogationState != null) 
            {
                interrogationState.DidDisconnect();
                interrogationState = null;
            }
        }
        #endregion

        #region Services
        public override ILEGOService[] Services { get { return _services.ToArray(); } }

        public override ILEGOService[] InterternalServices
        {
            get { return _services.FindAll(service => service.IsInternalService).ToArray(); }
        }

        public override ILEGOService[] ExternalServices
        {
            get { return _services.FindAll(service => !service.IsInternalService).ToArray(); }
        }

        private List<LEGOService> _services = new List<LEGOService>();

        public override ILEGOService FindService(IOType type, bool virtualConnection = false)
        {
            var legoService = _services.Find(service => (service.ConnectInfo.Type == type && service.ConnectInfo.VirtualConnection == virtualConnection));
            if (legoService == null)
                logger.Debug("Could not find service for IOType: " + type.ToString());
            return legoService;
        }

        public override ILEGOService FindVirtualService(int portID1, int portID2)
        {
            var legoService = _services.Find(
                service => (   service.ConnectInfo.VirtualConnection == true 
                            && ( (   service.ConnectInfo.PortID1 == portID1 
                                  && service.ConnectInfo.PortID2 == portID2 
                                 )
                               ||
                                 (   service.ConnectInfo.PortID1 == portID2
                                  && service.ConnectInfo.PortID2 == portID1 
                                 )
                               )
                            ));
            if (legoService == null)
            {
                logger.Debug("Could not find any virtual services");
            }
            return legoService;
        }

        public override ILEGOService FindService(int portID)
        {
            var legoService = _services.Find(service => service.ConnectInfo.PortID == portID);
            if (legoService == null)
            {
                logger.Debug("Could not find service for PortID: " + portID);// + "\n CALLSTACK:" + new System.Diagnostics.StackTrace(true).ToString());
            }
            return legoService;
        }
        #endregion

        #region - Callbacks from native - device advertising properties
        internal override void ManufacturerDataUpdated(byte[] data) {
          
            if (data == null) {
                logger.Debug("Got ManufacturerData=null for device "+DeviceID);
                return;
            }

            if (ManufactureData == null)
            {
                ManufactureData = new LEGOManufacturerData();
            }
                
            //CUBB excludes the first 4 bytes of the manufacturer data
            
            if (data.Length >= 1)
            {
                ButtonPressed = (data[0] != 0);
                ManufactureData.SetButtonState(data[0]);
            }
            if (data.Length >= 2) {
                SetSystemTypeID(data[1]);
                ManufactureData.SetDataTypeName(data[1]);
            }
            if (data.Length >= 3) {
                SupportedFunctions = (DeviceFunction) data[2];
                ManufactureData.SetCapabilities(data[2]);
            }
            if (data.Length >= 4) {
                LastConnectedNetworkId = data[3];
                ManufactureData.SetLastNetwork(data[3]);
            }
            
            if (data.Length >= 5)
            {
                ManufactureData.SetStatus(data[4]);
            }

            if (data.Length >= 6)
            {
                ManufactureData.SetOption(data[5]);
            }
            
            //System type updated, does it support OAD?
            DeviceType deviceType = new DeviceType(SystemType, SystemDeviceNumber);
            if(DeviceTypeGroup.OAD.Contains(deviceType) && 
               OADClient == null)
            {
                OADClient = new OADClient(bleDevice);
            }
        }
        #endregion

        #region - Callbacks from native - packets from connected device
        internal override string BTServiceUUID { get {return V3_SERVICE;} }
        internal override string BTCharacteristicUUID { get {return V3_CHARACTERISTIC;} }
        
        internal override void ReceivedPacket(byte[] data)
        {
            if (ConnectionState != DeviceConnectionState.Connected)
            {
                logger.Warn( " Got packet while in state " + ConnectionState);
            }
            
            try 
            {
                var message = LegoMessage.parse(data);
                HandleMessage(message);
                //logger.Warn("ReceivedPacket, Message: " + message + " Type: " +message.msgType + " HubID: " + message.hubID);
            } 
            catch (Exception e) 
            {
                if (OADClient != null)
                {
                    logger.Warn("Let LEAF protocol attempt to handle this message first");
                }
                else
                {
                    logger.Error("Invalid message from device: " + e + "; message: " + BitConverter.ToString(data));
                }
                
            }
        }

        private void HandleMessage(LegoMessage message)
        {
            message.visitWith(this, null);
        }

        void LegoMessage_Visitor<object>.handle_MessageHubProperty(MessageHubProperty msg, object arg)
        {
            if (msg.operation != HubPropertyOperation.UPDATE) {
                logger.Warn("Did not expect HubProperty message with operation="+msg.operation);
                return;
            }

            if (interrogationState != null)
                interrogationState.HubPropertyReceived(msg.property);
            
            switch (msg.property) 
            {
            case HubProperty.HARDWARE_VERSION:
                DeviceInfo.SetHardwareRevision(LEGORevision.parse(msg.payload));
                break;

            case HubProperty.FIRMWARE_VERSION:
                DeviceInfo.SetFirmwareRevision(LEGORevision.parse(msg.payload));
                if (DevicePort.IsCapableOfVirtualPairing(this))
                {
                    if (virtualServiceManger != null)
                    {
                        virtualServiceManger.Unsubscribe(); // do the unsubscribe - can't rely on the destructor in GC
                    } 
                    virtualServiceManger = null;
                    virtualServiceManger = new LEGOVirtualServiceManager(this);
                }
                break;

            case HubProperty.RADIO_FIRMWARE_VERSION:
                DeviceInfo.SetRadioFirmwareVersion(ParseString(msg.payload));
                break;

            case HubProperty.MANUFACTURER_NAME:
                DeviceInfo.SetManufacturerName(ParseString(msg.payload));
                break;

            case HubProperty.BATTERY_VOLTAGE:
                if (msg.payload.Length==1) 
                {
                    BatteryLevel = msg.payload[0];
                } 
                else 
                {
                    logger.Error("Received bad battery level: "+BitConverter.ToString(msg.payload));
                }
                break;

            case HubProperty.BUTTON:
                if (msg.payload.Length==1) 
                {
                    ButtonPressed = (msg.payload[0] != 0);
                } 
                else 
                {
                    logger.Error("Received bad button state: "+BitConverter.ToString(msg.payload));
                }
                break;
            case HubProperty.HARDWARE_NETWORK_ID:
                if (msg.payload.Length==1) {
                    LastConnectedNetworkId = msg.payload[0];
                } else {
                    logger.Error("Received bad network ID: "+BitConverter.ToString(msg.payload));
                }
                break;

            case HubProperty.NAME:
                DeviceName = ParseString(msg.payload);
                break;

            case HubProperty.RSSI:
                if (msg.payload.Length==1) 
                {
                    HubCalculatedRSSIValue = (sbyte)msg.payload[0];
                } 
                else 
                {
                    logger.Error("Received bad RSSI value: "+BitConverter.ToString(msg.payload));
                }
                break;
            
            case HubProperty.SPEAKER_VOLUME:
                if (msg.payload.Length==1) 
                {
                    SpeakerVolumeValue = msg.payload[0];
                } 
                else 
                {
                    logger.Error("Received bad Speaker Volume value: "+BitConverter.ToString(msg.payload));
                }
                break;
            
            case HubProperty.HARDWARE_SYSTEM_TYPE:
            case HubProperty.BATTERY_TYPE:
            case HubProperty.WIRELESS_PROTOCOL_VERSION:
            case HubProperty.PRIMARY_MAC_ADDRESS:
            case HubProperty.SECONDARY_MAC_ADDRESS:
            case HubProperty.BATTERY_CHARGING_STATUS:
            case HubProperty.BATTERY_CHARGE_VOLTAGE_PRESENT:
                // Ignore.
                break;

            default:
                logger.Warn("Unhandled hub property: "+msg.property);
                break;
            }
        }

        private string ParseString(byte[] payload)
        {
            try {
                return Encoding.UTF8.GetString(payload);
            } catch (Exception) {
                logger.Error("Invalid string value: "+BitConverter.ToString(payload));
                return "???";
            }
        }

        async void LegoMessage_Visitor<object>.handle_MessageHubAction(MessageHubAction msg, object arg)
        {
            DeviceType deviceType = new DeviceType(SystemType, SystemDeviceNumber);
            Action<ILEGODevice> callback = null;
            switch (msg.action) 
            {
            case HubAction.WILL_DISCONNECT:
                Disconnect();
                callback = OnDeviceWillDisconnect;
                break;
            case HubAction.WILL_SWITCH_OFF:
                if (deviceType.Equals(DeviceType.Hub128))
                {   //:NOTE: Technic hubs react differently from the preceding hubs in that they don't issue a
                    // WILL_DISCONNECT after being told to shut down and will go into advertising mode if we attempt to
                    // disconnect them before they disconnect themselves without telling us. Craptastic - Anders Bang is
                    // raising bugfix on PT - This code will need to be removed when this is fixed - also make sure the
                    // "async" in the method signature above is removed
                    await Task.Delay(850); // give the hub chance for it to disconnect before we do. NB: 750 is too little
                    Disconnect();
                }
                callback = OnDeviceWillSwitchOff;
                break;
            case HubAction.WILL_GO_INTO_BOOT_MODE:
                if (!DeviceTypeGroup.OAD.Contains(deviceType))
                {
                    // Hub67/68 (aka LEAF/LEAF.Bob) does not go into boot mode - it doesn't have a boot mode - it uses this message
                    // as a way to signify it is ready for FW or asset transfer via OAD
                    Disconnect();
                }
                callback = OnDeviceWillGoIntoBootMode;
                break;

            default:
                ProtocolError(msg);
                break;
            }
            
            if (callback != null) 
                callback(this);
        }

        void LegoMessage_Visitor<object>.handle_MessageHubAlert(MessageHubAlert msg, object arg)
        {
            if (msg.operation != HubAlertOperation.UPDATE)
                ProtocolError(msg);
            else {
                var alertOn = msg.payload.Length >= 1 && msg.payload[0] != 0;
                alerts[(HubAlertType)msg.alert] = alertOn;
                
                if (interrogationState != null)
                    interrogationState.HubAlertReceived(msg.alert);
                
                if (OnAlertStateUpdated != null)
                    OnAlertStateUpdated(this, (HubAlertType)msg.alert, alertOn);
            }
        }

        void LegoMessage_Visitor<object>.handle_MessagePortConnectivityRelated(MessagePortConnectivityRelated msg, object arg)
        {
            if (interrogationState != null)
                interrogationState.HandleMessage(msg);
        }

        void LegoMessage_Visitor<object>.handle_MessagePortMetadataRelated(MessagePortMetadataRelated msg, object arg)
        {
            if (interrogationState != null)
                interrogationState.HandleMessage(msg);
        }

        void LegoMessage_Visitor<object>.handle_MessagePortDynamicsRelated(MessagePortDynamicsRelated msg, object arg)
        {
            PassOnToService(msg.portID, msg, service => service.HandleMessage(msg));
        }

        void LegoMessage_Visitor<object>.handle_MessageFirmwareBootMode(MessageFirmwareBootMode msg, object arg)
        {
            ProtocolError(msg);
        }

        void LegoMessage_Visitor<object>.handle_MessageError(MessageError msg, object arg)
        {
            if (OnDidExperienceError != null) {
                var msgType = (MessageType)msg.commandID;
                var errorType = (MessageErrorType)msg.errorType;
                var errMsg = String.Format("Received error message: Type: 0x{0:X} HubID: {1:X} ErrorType: {2} Payload: {3}",
                    msgType, msg.hubID, errorType.ToString(), BitConverter.ToString(msg.payload));
                OnDidExperienceError(this, errorType, errMsg, msgType);
            }
        }

        void LegoMessage_Visitor<object>.handle_MessagePortValueSingle(MessagePortValueSingle msg, object arg)
        {
            var stream = new MemoryStream(msg.rawInputValue);
            var reader = new BinaryReader(stream);
            while (stream.Position < stream.Length) {
                var portID = stream.Position==0 ? msg.portID : reader.ReadByte();
                var ok = false;
                PassOnToService(portID, msg, service => {ok = service.DidUpdateValueData(reader);});
                if (!ok) {
                    logger.Warn("Service not found on port "+portID+". Cannot parse the rest of PortValueSingle message.");
                    break;
                }
            }
        }

        void LegoMessage_Visitor<object>.handle_MessageVirtualPortSetupDisconnect(MessageVirtualPortSetupDisconnect msg, object arg)
        {
            ProtocolError(msg);
        }

        void LegoMessage_Visitor<object>.handle_MessageVirtualPortSetupConnect(MessageVirtualPortSetupConnect msg, object arg)
        {
            ProtocolError(msg);
        }

        void LegoMessage_Visitor<object>.handle_MessagePortOutputCommandFeedback(MessagePortOutputCommandFeedback msg, object arg)
        {
            PassOnToService(msg.portID, msg, (service)=>service.DidReceiveFeedback(msg.feedback));

            var extra = msg.optionalFeedback;
            var extraCnt = extra.Length / 2;
            for (var i=0; i<extraCnt; i++) {
                var portID = extra[2*i];
                var feedback = extra[2*i+1];

                PassOnToService(portID, msg, (service)=>service.DidReceiveFeedback(feedback));
            }
        }

        internal void ProtocolError(LegoMessage msg) {
            logger.Error("Protocol error: Received message of type "+msg.GetType().Name);
        }

        internal static void ProtocolErrorS(LegoMessage msg) {
            logger.Error("Protocol error: Received message of type "+msg.GetType().Name);
        }

        private void PassOnToService(byte portID, LegoMessage msg, Action<LEGOService> action) {
            var service = (LEGOService)FindService(portID);
            if (service != null) {
                action(service);
            } else {
                logger.Warn(String.Format("Received {0} from unknown service on port {1}", msg.GetType().Name, portID));
            }
        }

        internal void AddService(LEGOService service)
        {
            _services.Add(service);
            service.DidChangeState(ServiceState.Connected);
            //Fixes a timing issues where addservice are being called before virtualServiceManager exists, meaning virtual pair wouldnt get created
            if (virtualServiceManger == null)
            {
                virtualServiceManger = new LEGOVirtualServiceManager(this);
            }
            if (OnServiceConnectionChanged != null)
            {
                OnServiceConnectionChanged(this, service, true);
            }
        }

        internal void RemoveService(int portID)
        {
            var service = _services.Find(existing => (existing.ConnectInfo.PortID == portID));
            if (service == null) return;
            RemoveService(service);
        }

        internal void RemoveService(LEGOService service)
        {
            service.DidChangeState(ServiceState.Disconnected);
            _services.Remove(service);
            if (OnServiceConnectionChanged != null)
            {
                OnServiceConnectionChanged(this, service, false);
            }
        }

        public override bool IsAlertHigh(HubAlertType type)
        {
            bool alertHigh = false;
            alerts.TryGetValue(type, out alertHigh);
            return alertHigh;
        }
        #endregion


        #region - Send commands to Native

        public readonly static int maxDeviceNameBytes = 14;

        internal void SendMessage(LegoMessage message, PacketDroppingInfo? packetDroppingInfo = null) {
            byte[] data;
            try {
                data = message.unparse();
            } catch (Exception e) {
                logger.Error("Failed to serialize message "+message, e);
                return;
            }

            SendPacket(data, packetDroppingInfo: packetDroppingInfo);
        }

        internal void RequestToSwitchOff()
        {
            SendMessage(new MessageHubAction(0, HubAction.SWITCH_OFF));
            MarkAsExpectingConnectionToClose();
        }

        internal void RequestToDisconnect()
        {
            SendMessage(new MessageHubAction(0, HubAction.DISCONNECT));
            MarkAsExpectingConnectionToClose();
        }

        public void RequestToGoIntoFlashLoaderMode()
        {
            SendMessage(new MessageFirmwareBootMode(0, Encoding.ASCII.GetBytes("LPF2-Boot")));
            MarkAsExpectingConnectionToClose();
        }

        public void RequestReboot()
        {
            SendMessage(new MessageHubAction(0, HubAction.REBOOT));
            MarkAsExpectingConnectionToClose();
        }
        
        public override void UpdateDeviceName(string name)
        {
            SendMessage(new MessageHubProperty(0, HubProperty.NAME, HubPropertyOperation.SET, Encoding.UTF8.GetBytes(Truncate(name))));
        }

        public override bool IsValidDeviceNameLength(string name)
        {
            return Encoding.UTF8.GetByteCount(name) <= maxDeviceNameBytes && Encoding.UTF8.GetByteCount(name) > 0;
        }

        public string Truncate(string value)
        {
            if(String.IsNullOrWhiteSpace(value))
                return String.Empty;
                
            var newValue = (value.Length <= maxDeviceNameBytes) ? value : value.Substring(0, maxDeviceNameBytes);
            while (!IsValidDeviceNameLength(newValue))
            {
                newValue = newValue.Substring(0, newValue.Length - 1);
            }
            return newValue;
        }

        public override void EnableValueUpdates(bool enable, ValueEnableUpdateType updateType)
        {
            HubProperty hubProperty;
            HubAlert hubAlert;

            switch (updateType) {
            case ValueEnableUpdateType.AdvName:
                hubProperty = HubProperty.NAME; goto sendEnablePropertyUpdate;
            case ValueEnableUpdateType.BatteryVoltage:
                hubProperty = HubProperty.BATTERY_VOLTAGE; goto sendEnablePropertyUpdate;
            case ValueEnableUpdateType.ButtonState:
                hubProperty = HubProperty.BUTTON; goto sendEnablePropertyUpdate;
            case ValueEnableUpdateType.UpdateRSSI:
                hubProperty = HubProperty.RSSI; goto sendEnablePropertyUpdate;

            case ValueEnableUpdateType.HighCurrentAlert:
                hubAlert = HubAlert.HIGH_CURRENT; goto sendEnableAlert;
            case ValueEnableUpdateType.HighPowerUseAlert:
                hubAlert = HubAlert.HIGH_POWER_USE; goto sendEnableAlert;
            case ValueEnableUpdateType.LowBatteryVoltageAlert:
                hubAlert = HubAlert.LOW_VOLTAGE; goto sendEnableAlert;
            case ValueEnableUpdateType.LowSignalStrengthAlert:
                hubAlert = HubAlert.LOW_SIGNAL_STRENGTH; goto sendEnableAlert;

            default:
                logger.Error("Invalid update type: "+updateType);
                break;

            sendEnablePropertyUpdate:
                SendMessage(new MessageHubProperty(0, hubProperty, enable ? HubPropertyOperation.ENABLE_UPDATES : HubPropertyOperation.DISABLE_UPDATES, new byte[0]));
                if (enable)
                {
                    SendMessage(new MessageHubProperty(0, hubProperty, HubPropertyOperation.REQUEST_UPDATE, new byte[0]));
                }
                break;

            sendEnableAlert:
                SendMessage(new MessageHubAlert(0, hubAlert, enable ? HubAlertOperation.ENABLE_UPDATES : HubAlertOperation.DISABLE_UPDATES, new byte[0]));
                if(enable)
                {
                    SendMessage(new MessageHubAlert(0, hubAlert, HubAlertOperation.REQUEST_UPDATE, new byte[0]));
                }
                break;
            }
        }

        public override void EnableBusyIndication(bool enable)
        {
            SendMessage(new MessageHubAction(0, enable ? HubAction.ACTIVATE_BUSY_INDICATION : HubAction.DEACTIVATE_BUSY_INDICATION));
        }

        public override void SetConnectedNetworkID(int networkID)
        {
            if (networkID != (byte)networkID)
                throw new ArgumentException("Network ID out of range: "+networkID);
            SendMessage(new MessageHubProperty(0, HubProperty.HARDWARE_NETWORK_ID, HubPropertyOperation.SET, new byte[]{(byte)networkID}));
        }


        public override void UpdateValueForType(ValueUpdateType updateType)
        {
            HubProperty hubProperty;
            HubAlert hubAlert;

            switch (updateType) {
            case ValueUpdateType.LEDeviceRequestUpdateBatteryVoltage:
                hubProperty = HubProperty.BATTERY_VOLTAGE; goto sendPropertyValueRequest;
            case ValueUpdateType.LEDeviceRequestUpdateButtonState:
                hubProperty = HubProperty.BUTTON; goto sendPropertyValueRequest;
            case ValueUpdateType.LEDeviceRequestUpdateRSSI:
                hubProperty = HubProperty.RSSI; goto sendPropertyValueRequest;

            case ValueUpdateType.LEDeviceRequestUpdateHighCurrentAlert:
                hubAlert = HubAlert.HIGH_CURRENT; goto sendAlertValueRequest;
            case ValueUpdateType.LEDeviceRequestUpdateHighPowerUseAlert:
                hubAlert = HubAlert.HIGH_POWER_USE; goto sendAlertValueRequest;
            case ValueUpdateType.LEDeviceRequestUpdateLowSignalStrengthAlert:
                hubAlert = HubAlert.LOW_SIGNAL_STRENGTH; goto sendAlertValueRequest;
            case ValueUpdateType.LEDeviceRequestUpdateLowVoltageAlert:
                hubAlert = HubAlert.LOW_VOLTAGE; goto sendAlertValueRequest;

            default:
                logger.Error("Invalid update type: "+updateType);
                break;

            sendPropertyValueRequest:
                SendMessage(new MessageHubProperty(0, hubProperty, HubPropertyOperation.REQUEST_UPDATE, new byte[0]));
                break;

            sendAlertValueRequest:
                SendMessage(new MessageHubAlert(0, hubAlert, HubAlertOperation.REQUEST_UPDATE, new byte[0]));
                break;
            }
        }

        public override void ResetValueForType(ValueResetType resetType)
        {
            HubProperty hubProperty;

            switch (resetType) {
            case ValueResetType.LEDeviceResetTypeAdvName:
                hubProperty = HubProperty.NAME; goto sendResetProperty;
            case ValueResetType.LEDeviceResetTypeHardwareNetworkID:
                hubProperty = HubProperty.HARDWARE_NETWORK_ID; goto sendResetProperty;

            default:
                logger.Error("Invalid reset type: "+resetType);
                break;

            sendResetProperty:
                SendMessage(new MessageHubProperty(0, hubProperty, HubPropertyOperation.RESET, new byte[0]));
                break;
            }
        }

        public override void SetupVirtualMotorPair(uint portID1, uint portID2)
        {
            SendMessage(new MessageVirtualPortSetupConnect(0, portID1, portID2));
        }

        public override void DisconnectVirtualMotorPair(uint portID)
        {
            SendMessage(new MessageVirtualPortSetupDisconnect(0, portID));
        }

        #endregion
    }
}