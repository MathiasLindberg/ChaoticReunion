using System;
using System.Collections.Generic;
using System.Linq;
using LEGO.Logger;
using CoreUnityBleBridge;
using CoreUnityBleBridge.Model;

namespace LEGODeviceUnitySDK
{
    public abstract class AbstractLEGODevice : ILEGODevice
    {
        public sealed class DeviceTypeGroup // derive from IEquatable if you need to compare device type  groups
        {
            private static readonly NoAckParameters regularNoAckParameters = new NoAckParameters(1, 50);
            private static readonly NoAckParameters assetTransferNoAckParameters = new NoAckParameters(100, 0);
            public static readonly DeviceTypeGroup LEAF = new DeviceTypeGroup(new [] {DeviceType.Hub67, DeviceType.Hub68});
            public static readonly DeviceTypeGroup OAD = new DeviceTypeGroup(new [] {DeviceType.Hub67, DeviceType.Hub68, DeviceType.Hub132});

            public DeviceType[] DeviceTypes { private set; get; }

            public NoAckParameters RegularParameters => regularNoAckParameters;
            public NoAckParameters AssetTransferParameters => assetTransferNoAckParameters;
 
            public DeviceTypeGroup(DeviceType[] deviceTypes)
            {
                DeviceTypes = deviceTypes;
            }
            
            public static DeviceType DeviceTypeBy(DeviceSystemType deviceSystemType, int systemDeviceNumber)
            {
                return new DeviceType(deviceSystemType, systemDeviceNumber);
            }
            
            public bool Contains(DeviceType deviceType)
            {
                try
                {
                    return DeviceTypes.Contains(deviceType);
                }
                catch (ArgumentNullException)
                {
                    logger.Warn("deviceType is null");
                    return false;
                }
            }

            public class NoAckParameters
            {
                public readonly int PacketsPerWindow;
                public readonly int WindowLengthMs;

                public NoAckParameters(int packetsPerWindow, int windowLengthMs)
                {
                    PacketsPerWindow = packetsPerWindow;
                    WindowLengthMs = windowLengthMs;
                }
            }
        }
        
        public sealed class DeviceType : IEquatable<DeviceType>
        {
            public static readonly DeviceType Hub32  = new DeviceType(DeviceSystemType.LEGODuplo, systemDeviceNumber: 0);   //Duplo train
            public static readonly DeviceType Hub33  = new DeviceType(DeviceSystemType.LEGODuplo, systemDeviceNumber: 1);   //Dory hub
            public static readonly DeviceType Hub64  = new DeviceType(DeviceSystemType.LEGOSystem1, systemDeviceNumber: 0); //Boost hub
            public static readonly DeviceType Hub65  = new DeviceType(DeviceSystemType.LEGOSystem1, systemDeviceNumber: 1); //city hub
            public static readonly DeviceType Hub66  = new DeviceType(DeviceSystemType.LEGOSystem1, systemDeviceNumber: 2); //city remote
            public static readonly DeviceType Hub67  = new DeviceType(DeviceSystemType.LEGOSystem1, systemDeviceNumber: 3); //Leaf Mario hub
            public static readonly DeviceType Hub68  = new DeviceType(DeviceSystemType.LEGOSystem1, systemDeviceNumber: 4); //Leaf Bob hub
            public static readonly DeviceType Hub128 = new DeviceType(DeviceSystemType.LEGOTechnic1, systemDeviceNumber: 0); //technic/convoy hub
            public static readonly DeviceType Hub132 = new DeviceType(DeviceSystemType.LEGOTechnic1, systemDeviceNumber: 4); //Falcon hub 

            public DeviceSystemType DeviceSystemType { private set; get; }
            public int SystemDeviceNumber { private set; get; }

            public DeviceType(DeviceSystemType deviceSystemType, int systemDeviceNumber)
            {
                DeviceSystemType = deviceSystemType;
                SystemDeviceNumber = systemDeviceNumber;
            }
            
            public static DeviceType DeviceTypeBy(DeviceSystemType deviceSystemType, int systemDeviceNumber)
            {
                return new DeviceType(deviceSystemType, systemDeviceNumber);
            }

            public bool Equals(DeviceType other)
            {
                if (other == null)
                {
                    return false;
                }

                return other.DeviceSystemType == DeviceSystemType && other.SystemDeviceNumber == SystemDeviceNumber;
            }
            
            public override bool Equals(object other)
            {
                if(other == null)
                    return false;

                var second = other as DeviceType;

                return Equals(second);
            }
            
            public static bool operator == (DeviceType lh, DeviceType rh)
            {
                if (ReferenceEquals(lh, rh))
                {
                    return true;
                }
                
                if (ReferenceEquals(lh, null) || ReferenceEquals(rh, null))
                {
                    return false;
                }

                return lh.Equals(rh);
            }
            
            public static bool operator != (DeviceType lh, DeviceType rh)
            {
                return !(lh == rh);
            }
            
            public override int GetHashCode(){
                unchecked{
                    int hash = 17;
                    hash = hash * 23 + SystemDeviceNumber;
                    hash = hash * 23 + DeviceSystemType.GetHashCode();
                    return hash;
                }
            }

            public override string ToString()
            {
                return "DeviceType info: DeviceSystemType: " + DeviceSystemType + " SystemDeviceNumber: " + SystemDeviceNumber;
            }
        }
        
        static readonly ILog logger = LogManager.GetLogger(typeof(AbstractLEGODevice));

        protected readonly IBleDevice bleDevice;

        protected LEGOVirtualServiceManager virtualServiceManger = null;

        public LEGOVirtualServiceManager VirtualServiceManger
        {
            get { return virtualServiceManger; }
        }

        #region Event hooks
        #pragma warning disable 067
        public virtual event Action<ILEGODevice, HubAlertType, bool> OnAlertStateUpdated;
        public event Action<ILEGODevice, DeviceState /* old state */, DeviceState /* new state */> OnDeviceStateUpdated;
        public virtual event Action<ILEGODevice, ILEGOService, bool /* connected */ > OnServiceConnectionChanged;
        public event Action<ILEGODevice, bool> OnButtonStateUpdated;
        public event Action<ILEGODevice, string> OnNameUpdated;
        public virtual event Action<ILEGODevice, string> OnFailToAddServiceWithError;
        public event Action<ILEGODevice, int> OnBatteryLevelPercentageUpdated;
        public event Action<ILEGODevice, int> OnRSSIValueUpdated;
        public event Action<ILEGODevice, int> OnHubCalculatedRSSIValueUpdated;
        public event Action<ILEGODevice, LEGODeviceInfo> OnDeviceInfoUpdated;
        public event Action<ILEGODevice, int> OnSpeakerOnVolumeUpdated;
        public virtual event Action<ILEGODevice> OnDeviceWillSwitchOff;
        public virtual event Action<ILEGODevice> OnDeviceWillDisconnect;
        public virtual event Action<ILEGODevice> OnDeviceWillGoIntoBootMode;
        public virtual event Action<ILEGODevice, MessageErrorType, string, MessageType> OnDidExperienceError;

        public virtual event Action<ILEGODevice, int> OnWriteMTUSizeReceived;
        


        
        internal event Action<ILEGODevice, DeviceDisconnectReason> OnDisconnected;
        #pragma warning restore 067
        #endregion

        public AbstractLEGODevice(IBleDevice bleDevice)
        {
            this.bleDevice = bleDevice;
        }
        
        internal void Subscribe()
        {
            bleDevice.NameChanged += DeviceNameUpdated;
            bleDevice.RssiChanged += RSSIValueUpdated;
            bleDevice.ManufacturerDataChanged += ManufacturerDataUpdated;
            bleDevice.VisibilityStateChanged += UpdateVisibilityState;
            bleDevice.ConnectionStateChanged += UpdateConnectionState;

            bleDevice.PacketReceived += OnPacketReceived;
            bleDevice.PacketTransmitted += OnPacketTransmitted;
            bleDevice.PacketDropped += OnPacketDropped;
            bleDevice.WriteMTUSize += OnWriteMTUSize;

            // Grab initial values:
            DeviceNameUpdated();
            RSSIValueUpdated();
            ManufacturerDataUpdated();
            UpdateVisibilityState(); // Doing this one last, once all the others are in place.
        }

        internal void Unsubscribe()
        {
            bleDevice.NameChanged -= DeviceNameUpdated;
            bleDevice.RssiChanged -= RSSIValueUpdated;
            bleDevice.ManufacturerDataChanged -= ManufacturerDataUpdated;
            bleDevice.VisibilityStateChanged -= UpdateVisibilityState;
            bleDevice.ConnectionStateChanged -= UpdateConnectionState;

            bleDevice.PacketReceived -= OnPacketReceived;
            bleDevice.PacketTransmitted -= OnPacketTransmitted;
            bleDevice.PacketDropped -= OnPacketDropped;
            bleDevice.WriteMTUSize -= OnWriteMTUSize;

            virtualServiceManger?.Unsubscribe();
            virtualServiceManger = null;
        }

        #region Properties
        internal int SortingKey {get; set;}

        private string deviceName;
        public string DeviceName
        {
            get
            {
                return deviceName;
            }
            protected set
            {
                if (value != null && value.Length > 0 && !value.Equals(deviceName))
                {
                    deviceName = value;
                    if (OnNameUpdated != null)
                        OnNameUpdated(this, deviceName);
                }
            }

        }

        public string DeviceID { get; protected set; }

        public void SetNoAckParameters(int packetCount, int windowLengthMs)
        {
            bleDevice.SetNoAckParameters(packetCount, windowLengthMs);
        }
        
        public void GetWriteMTUSize()
        {
            bleDevice.GetWriteMTUSize(BTServiceUUID, BTCharacteristicUUID);
        }

        public void RequestMTUSize(int mtuSize)
        {
            bleDevice.RequestMTUSize(mtuSize);
        }

        public LEGODeviceInfo DeviceInfo { get; protected set; }

        public IOADClient OADClient { get; protected set; }
        
        internal void DidGetAllHubProperties()
        {
            if (OnDeviceInfoUpdated != null)
                OnDeviceInfoUpdated(this, DeviceInfo);
        }

        private bool buttonPressed = false;
        public bool ButtonPressed {
            get {return buttonPressed;}
            protected set {
                if (value == buttonPressed) return;
                buttonPressed = value;
                if (OnButtonStateUpdated != null)
                    OnButtonStateUpdated(this, buttonPressed);
            }
        }

        private int batteryLevel;
        public int BatteryLevel {
            get { return batteryLevel; }
            protected set {
                batteryLevel = value;
                if (OnBatteryLevelPercentageUpdated != null)
                    OnBatteryLevelPercentageUpdated(this, batteryLevel);
            }
        }

        private int rssiValue;
        public int RSSIValue { //TODO: Set this also when hub is connected
            get { return rssiValue; }
            protected set {
                rssiValue = value;
                if (OnRSSIValueUpdated != null)
                    OnRSSIValueUpdated(this, rssiValue);
            }
        }

        private int hubCalculatedRssiValue;
        public int HubCalculatedRSSIValue {
            get { return hubCalculatedRssiValue; }
            protected set {
                hubCalculatedRssiValue = value;
                if (OnHubCalculatedRSSIValueUpdated != null)
                    OnHubCalculatedRSSIValueUpdated(this, hubCalculatedRssiValue);
            }
        }
        
        private int speakerVolumeValue;
        public int SpeakerVolumeValue {
            get { return speakerVolumeValue; }
            protected set
            {
                speakerVolumeValue = value;
                if (OnSpeakerOnVolumeUpdated != null)
                {
                    OnSpeakerOnVolumeUpdated(this, speakerVolumeValue);
                }
            }
        }
        
        private DeviceSystemType _systemType = DeviceSystemType.Unknown;
        public DeviceSystemType SystemType {
            get { return _systemType; }
            protected set { _systemType = value; }
        }

        private int _systemDeviceNumber = -1;
        public int SystemDeviceNumber {
            get { return _systemDeviceNumber; }
            protected set { _systemDeviceNumber = value; }
        }

        protected void SetSystemTypeID(byte systemTypeID) {
            SystemType = (DeviceSystemType) (systemTypeID >> 5);
            SystemDeviceNumber = (systemTypeID & 0x1F);
        }

        private DeviceFunction _supportedFunctions = DeviceFunction.Unknown;
        public DeviceFunction SupportedFunctions {
            get { return _supportedFunctions; }
            protected set { _supportedFunctions = value; }
        }

        private int _lastConnectedNetworkId = -1;
        public int LastConnectedNetworkId {
            get { return _lastConnectedNetworkId; }
            protected set { _lastConnectedNetworkId = value; }
        }

        private int writeMTUSize = -1;
        public int WriteMTUSize 
        { 
            get { return writeMTUSize; }
            protected set 
            {
                writeMTUSize = value;
                if (OnWriteMTUSizeReceived != null)
                {
                    OnWriteMTUSizeReceived(this, writeMTUSize);
                }
            }
        }

        
        #endregion

        #region - Callbacks from native - device advertising properties
        private void DeviceNameUpdated() {
            this.DeviceName = bleDevice.Name;
        }

        private void RSSIValueUpdated() {
            this.RSSIValue = bleDevice.RSSI;
        }

        private void WriteMTUSizeReceived(int writeMTUSize)
        {
            this.WriteMTUSize = writeMTUSize;
        }
        
        private void ManufacturerDataUpdated()
        {
         
            var prevManufacturerData = new LEGOManufacturerData( ManufactureData);

            ManufacturerDataUpdated(bleDevice.ManufacturerData);
            OnManufacturerDataUpdated?.Invoke(this,prevManufacturerData,ManufactureData);

            
            DeviceType deviceType = new DeviceType(SystemType, SystemDeviceNumber);
            if(DeviceTypeGroup.OAD.Contains(deviceType))
            {
                if(OADClient == null)
                {
                    OADClient = new OADClient(bleDevice);
                    logger.Debug("TIOADClient created");
                }
            }
            else if(OADClient != null)
            {
                OADClient = null;
            }
        }

        public LEGOManufacturerData ManufactureData { get; private protected set; }
        public event Action<ILEGODevice, LEGOManufacturerData,LEGOManufacturerData> OnManufacturerDataUpdated;
        
        internal abstract void ManufacturerDataUpdated(byte[] data);
        #endregion

        #region - Callbacks from native - packets from connected device
        internal abstract string BTServiceUUID { get; }
        internal abstract string BTCharacteristicUUID { get; }
        internal abstract void ReceivedPacket(byte[] data);

        private void OnPacketReceived(PacketReceivedEventArgs args)
        {
            if (args.Service != BTServiceUUID || args.GattCharacteristic != BTCharacteristicUUID)  
            {
                return;
            }
            
            ReceivedPacket(args.Data);
        }
        private void OnPacketTransmitted(PacketTransmittedEventArgs args)
        {
            if (args.Service != BTServiceUUID || args.GattCharacteristic != BTCharacteristicUUID) {
                logger.Error("Received packet on unknown service/characteristic: "+args.Service+"/"+args.GattCharacteristic+" from device "+DeviceID);
                return;
            }
            PacketTransmitted(args.SequenceNumber);
        }

        /// <summary>
        /// Sends a packet.
        /// </summary>
        /// <param name="">.</param>
        /// <param name="group">Group.</param>
        internal/*protected*/ void SendPacket(byte[] data, PacketDroppingInfo? packetDroppingInfo) {
            int group;
            int packetID;
            if (packetDroppingInfo != null) {
                var pdi = packetDroppingInfo.Value;
                group = pdi.Group;
                packetID = AllocatePacketID(pdi.Feedback);
            } else {
                group = 0;
                packetID = -1;
            }
            bleDevice.SendPacket(BTServiceUUID, BTCharacteristicUUID, data, group: group, sendFlags: SendFlags.None, packetID: packetID);
        }

        protected void SendPacketWithFeedback(byte[] data, int seqNr, bool useSoftAck) {
            bleDevice.SendPacketNotifyOnDataTransmitted(BTServiceUUID, BTCharacteristicUUID, data, seqNr, softAck: useSoftAck);
        }

        internal virtual void PacketTransmitted(int seqNr) {}
        #endregion

        #region Handling of packet replacement
        private readonly PacketDroppedMap packetDroppedMap = new PacketDroppedMap();
        private int packetID = 1;

        private int AllocatePacketID(IPacketDropFeedback feedback)
        {
            do {
                packetID = (packetID == int.MaxValue) ? 1 : packetID + 1;
            } while (packetDroppedMap.ContainsKey(packetID));
            packetDroppedMap.Add(packetID, feedback);
            return packetID;
        }

        private void OnPacketDropped(PacketDroppedEventArgs args) {
            IPacketDropFeedback feedback;
            if (packetDroppedMap.TryGetAndRemoveValue(args.PacketID, out feedback)) {
                feedback.OnDropped();
            }
        }
        
        private void OnWriteMTUSize(WriteMTUSizeEventArgs args)
        {
            WriteMTUSize = args.WriteMTUSize;
        }
        #endregion


        #region Device state
        private DeviceVisibilityState visibilityState = DeviceVisibilityState.Invisible;
        private DeviceConnectionState connectionState = DeviceConnectionState.Disconnected;
        protected DeviceConnectionState ConnectionState { get { return connectionState; } }

        private bool expectingConnectionToClose = false;

        private DeviceState deviceState = DeviceState.DisconnectedNotAdvertising;

        private bool willShutDown;
        public bool WillShutdown
        {
            get { return willShutDown; }
            set
            {
                willShutDown = value;
                if(willShutDown)
                {
                    bleDevice.WillShutDown();
                    UpdateConnectionState();
                }
            }
        }

        public DeviceState State { get { return deviceState; } }

        internal SendStrategy SendStrategyForDevice 
        {
            get
            {
                //The CUBB does not allow us to use NoAck on some services and SoftAck/HardAck on others
                //Until that is fixed we will have to use NoAck for LEAF as the OAD service only support write-without-response 
                DeviceType deviceType = new DeviceType(SystemType, SystemDeviceNumber);
                if( deviceType.Equals(DeviceType.Hub65) || deviceType.Equals(DeviceType.Hub66)) // city/powered up hub
                {
                    return (IsBootLoader) ? SendStrategy.NoAck : SendStrategy.HardAck;
                }
                else if (deviceType.Equals(DeviceType.Hub32) || deviceType.Equals(DeviceType.Hub33)) // dory hub
                {
                    return SendStrategy.HardAck;
                }
                else if (DeviceTypeGroup.OAD.Contains(deviceType))
                {
                    return SendStrategy.NoAck;
                }
                else if (deviceType.Equals(DeviceType.Hub64))
                {
                    return (IsBootLoader) ? SendStrategy.SoftAck : SendStrategy.HardAck;
                } 
                
                return SendStrategy.SoftAck;
            }
        }

        internal void Connect() {
            var sendStrategy = SendStrategyForDevice;
            logger.Debug("Connecting using send strategy " + sendStrategy);
            bleDevice.Connect(sendStrategy);
        }
        
        internal void Disconnect() {
            logger.Warn("Device was disconnected" );
            bleDevice.Disconnect();
            MarkAsExpectingConnectionToClose();
        }

        protected void MarkAsExpectingConnectionToClose() {
            expectingConnectionToClose = true;
        }

        internal void UpdateVisibilityState()
        {
            visibilityState = bleDevice.Visibility;
            UpdateDeviceState();
        }
        internal void UpdateConnectionState()
        {
            DeviceConnectionState state = bleDevice.Connectivity;
            
            if (state == this.connectionState) return;

            var oldState = state;
            this.connectionState = state;

            var reason = oldState==DeviceConnectionState.Connecting ? DeviceDisconnectReason.FailedToConnect
                : expectingConnectionToClose ? DeviceDisconnectReason.Closed
                : DeviceDisconnectReason.ConnectionLost;

            if (state == DeviceConnectionState.Connected)
            {
                HandleConnected();
            }
            else if (state == DeviceConnectionState.Disconnected)
            {
                HandleDisconnected(reason);
            }

            // Invoke callbacks once the state is updated:
            UpdateDeviceState();
            if (state == DeviceConnectionState.Disconnected) 
            {
                if (OnDisconnected != null)
                { 
                    OnDisconnected(this, reason);
                }
                
                LEGODeviceManager.Instance.DeviceDidDisconnect(this, reason);
                expectingConnectionToClose = false;
            }
        }

        private void HandleConnected()
        {
            DeviceType deviceType = new DeviceType(SystemType, SystemDeviceNumber);
            if(DeviceTypeGroup.OAD.Contains(deviceType) &&
               SendStrategyForDevice == SendStrategy.NoAck)
            {
                var noAckParams = DeviceTypeGroup.OAD.RegularParameters;
                bleDevice.SetNoAckParameters(noAckParams.PacketsPerWindow, noAckParams.WindowLengthMs); 
            }

            logger.Debug("HandleConnected: "+DeviceID);
            this.DeviceInfo = new LEGODeviceInfo();
            this.DidConnect();
        }

        private void HandleDisconnected(DeviceDisconnectReason reason)
        {
            logger.Debug("HandleDisconnected: "+DeviceID + " Disconnected reason: " + reason);
            this.DidDisconnect();
        }

        protected abstract void DidConnect();
        protected abstract void DidDisconnect();

        protected void UpdateDeviceState() {
            var oldState = deviceState;
            var newState = ComputeDeviceState();
            if (newState != oldState) {
                deviceState = newState;
                if (OnDeviceStateUpdated != null) 
                    OnDeviceStateUpdated(this, oldState, newState);
                LEGODeviceManager.Instance.HandleDeviceStateUpdated(this, oldState, newState);
            }
        }

        private DeviceState ComputeDeviceState() {
            switch (connectionState) {
            case DeviceConnectionState.Connected:
                return ComputeConnectedDeviceState();
            case DeviceConnectionState.Connecting:
                return DeviceState.Connecting;
            case DeviceConnectionState.Disconnected:
                switch (visibilityState) {
                case DeviceVisibilityState.Visible: return DeviceState.DisconnectedAdvertising;
                case DeviceVisibilityState.Invisible: return DeviceState.DisconnectedNotAdvertising;
                }
                break;
            }
            logger.Error("Bad device state: "+visibilityState+"/"+connectionState);
            return DeviceState.DisconnectedNotAdvertising; // Fallback.
        }

        protected abstract DeviceState ComputeConnectedDeviceState();
        #endregion

        #region Postponed implementation
        public abstract void EnableBusyIndication(bool enable);

        public abstract bool IsValidDeviceNameLength(string name);

        public abstract void UpdateDeviceName(string name);

        public abstract bool IsAlertHigh(HubAlertType type);

        public abstract void SetConnectedNetworkID(int networkID);

        public abstract void EnableValueUpdates(bool enable, ValueEnableUpdateType type);

        public abstract void UpdateValueForType(ValueUpdateType type);

        public abstract void ResetValueForType(ValueResetType type);

        public abstract ILEGOService FindService(IOType type, bool virtualConnection = false);

        public abstract ILEGOService FindService(int portID);

        public abstract ILEGOService FindVirtualService(int portID1, int portID2);

        public abstract void SetupVirtualMotorPair(uint portID1, uint portID2);

        public abstract void DisconnectVirtualMotorPair(uint portID);

        public abstract bool IsBootLoader { get; }

        public abstract DeviceCompatibleProtocolSpecificationVersionType CompatibleProtocolSpecification { get; }

        public abstract ILEGOService[] Services { get; }

        public abstract ILEGOService[] InterternalServices { get; }

        public abstract ILEGOService[] ExternalServices { get; }
        #endregion
    }

    internal interface IPacketDropFeedback {
        void OnDropped();
    }

    internal struct PacketDroppingInfo {
        public int Group;
        public IPacketDropFeedback Feedback;
    }

    /// <summary>
    /// A dictionary mapping integers to weak references to IDroppable.
    /// </summary>
    class PacketDroppedMap {
        private const int TRIVIALITY_THRESHOLD = 16; // Don't bother pruning below this level.

        private readonly Dictionary<int, WeakReference/*<IDroppable>*/> table = new Dictionary<int, WeakReference/*<IDroppable>*/>();
        private int nextPruningSize = TRIVIALITY_THRESHOLD;

        public void Add(int key, IPacketDropFeedback value) {
            if (table.Count >= nextPruningSize) {
                Prune();
                nextPruningSize = Math.Max(table.Count * 2, TRIVIALITY_THRESHOLD);
            }
            table.Add(key, new WeakReference(value));
        }

        public bool ContainsKey(int key) {
            return table.ContainsKey(key);
        }

        public bool TryGetAndRemoveValue(int key, out IPacketDropFeedback value) {
            WeakReference wr;
            bool found = table.TryGetValue(key, out wr);
            if (found) {
                table.Remove(key);

                var v = wr.Target;
                if (v != null) {
                    value = (IPacketDropFeedback)v;
                    return true;
                }
            }
            // Unknown or invalid.
            value = null;
            return false;
        } 

        private void Prune() {
            var toBeDeleted = new List<int>();
            foreach (var entry in table) {
                if (!entry.Value.IsAlive) {
                    toBeDeleted.Add(entry.Key);
                }
            }
            foreach (var key in toBeDeleted) {
                table.Remove(key);
            }
        }
    } 
}

