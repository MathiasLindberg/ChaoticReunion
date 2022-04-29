using System;

namespace LEGODeviceUnitySDK
{
    public class LEGODeviceStub : ILEGODevice
    {
        public LEGODeviceStub(string deviceID, string name, int batteryLevel)
        {
            DeviceID = deviceID;
            DeviceName = name;
            BatteryLevel = batteryLevel;
            State = DeviceState.InterrogationFinished;
            SystemType = DeviceSystemType.LEGOSystem1; //Boost
        }


        #region ILEGODevice implementation
        public ILEGOService FindService(IOType type, bool virtualConnection = false) { throw new NotImplementedException(); }
        public ILEGOService FindService(int portID) { throw new NotImplementedException(); }
        public ILEGOService FindVirtualService(int portID1, int portID2) { return null; }

        public bool IsBootLoader { get; private set; }

        public void AddService(ILEGOService service) { }
        public void RemoveService(ILEGOService service) { }
        public void RemoveService(int portID) { }

        public void UpdateDeviceName(string name) { DeviceName = name; }

        //public void HandleMessageFromNative(string commandName, JSONNode messagNode) { }

        public DeviceFlashState GetCurrentFlashState()
        {
            return DeviceFlashState.Idle;
        }

        public void EnableValueUpdates(bool enable, ValueEnableUpdateType type) { }

        public void EnableBusyIndication(bool enable) { }

#pragma warning disable 067
        public LEGOVirtualServiceManager VirtualServiceManger { get; }
        public event Action<ILEGODevice, HubAlertType, bool> OnAlertStateUpdated;
        public event Action<ILEGODevice, DeviceState, DeviceState> OnDeviceStateUpdated;
        public event Action<ILEGODevice, ILEGOService, bool> OnServiceConnectionChanged;
        public event Action<ILEGODevice, bool> OnButtonStateUpdated;
        public event Action<ILEGODevice, string> OnNameUpdated;
        public event Action<ILEGODevice, string> OnFailToAddServiceWithError;
        public event Action<ILEGODevice, int> OnBatteryLevelPercentageUpdated;
        public event Action<ILEGODevice, LEGODeviceInfo> OnDeviceInfoUpdated;
        public LEGOManufacturerData ManufactureData { get; }
        public event Action<ILEGODevice, LEGOManufacturerData,LEGOManufacturerData> OnManufacturerDataUpdated;
        public event Action<ILEGODevice, int> OnRSSIValueUpdated;
        public int WriteMTUSize { get; }
        public event Action<ILEGODevice, int> OnWriteMTUSizeReceived;
        public event Action<ILEGODevice, int> OnHubCalculatedRSSIValueUpdated;
        public event Action<ILEGODevice> OnDeviceWillSwitchOff;
        public event Action<ILEGODevice> OnDeviceWillDisconnect;
        public event Action<ILEGODevice> OnDeviceWillGoIntoBootMode;
        public void HandleInterrogationSucceeded(bool success)
        {
            
        }

        public event Action<ILEGODevice, MessageErrorType, string, MessageType> OnDidExperienceError;
#pragma warning restore 067

        public bool IsAlertHigh(HubAlertType type)
        {
            return false;
        }

        public bool IsValidDeviceNameLength(string name)
        {
            return true;
        }

        public void UpdateValueForType(ValueUpdateType type)
        {
        }

        public void ResetValueForType(ValueResetType type)
        {
        }

        public void SetConnectedNetworkID(int networkID)
        {

        }

        public void SetupVirtualMotorPair(uint portID1, uint portID2)
        {
        }

        public void DisconnectVirtualMotorPair(uint portID)
        {
        }

        public string DeviceName { get; private set; }

        public string DeviceID { get; private set; }

        public LEGODeviceInfo DeviceInfo { get { throw new NotImplementedException(); } }
        

        public bool ButtonPressed { get { throw new NotImplementedException(); } }

        public int BatteryLevel { get; private set; }

        public bool LowVoltageState { get; private set; }

        public DeviceState State { get; set; }
        public bool WillShutdown { get; set; }


        public DeviceSystemType SystemType { get; private set; }

        public int SystemDeviceNumber { get; private set; }

        public DeviceFunction SupportedFunctions { get; private set; }

        public int LastConnectedNetworkId { get; private set; }

        public ILEGOService[] Services { get { throw new NotImplementedException(); } }
        public ILEGOService[] InterternalServices { get { throw new NotImplementedException(); } }
        public ILEGOService[] ExternalServices { get { throw new NotImplementedException(); } }

        public int RSSIValue { get; private set; }

        public int HubCalculatedRSSIValue { get; private set; }

        public DeviceCompatibleProtocolSpecificationVersionType CompatibleProtocolSpecification { get; private set; }
        
        #endregion
    }
}