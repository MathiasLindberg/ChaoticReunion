using System;

namespace LEGODeviceUnitySDK
{

    /// <summary>
    /// This interface represent a LEGO BLE enabled device. Instances of this interface are created once the LEGODeviceManager
    /// discovers an advertising device. 
    /// 
    /// Some methods and properties on this interface are available as soon when the device is not connected (like name and button state), 
    /// and others are only available when we have established a successfull connection to the device, i.e. DeviceSate = InterrogationFinished.
    /// 
    /// </summary>
    public interface ILEGODevice
    {
        #region Basic Device 

        /// <summary>
        /// The DeviceID will always be the same, and unique, for this physical device. 
        /// 
        /// The ID is always avaiabe, regardless of the current DeviceState.  
        /// </summary>
        string DeviceID { get; }

        /// <summary>
        /// The Device System Type. You may want to allow connection to only certain Device System Types. 
        /// In this case, you will have to filter out 'unnwated' device types based on this property. 
        /// 
        /// Value always available, regardless of the current DeviceState.
        /// </summary>
        DeviceSystemType SystemType { get; }

        /// <summary>
        /// The System Device Number. You may want to allow connetion to only certain types of device numbers. 
        /// In this case, you will have to filter out 'unwanted' device numbers based on this property. 
        /// 
        /// Value always available, regardless of the current DeviceState.
        /// </summary>
        int SystemDeviceNumber { get; }

        /// <summary>
        /// Info about Hardware- and Firmware-revision of currently connected Device. 
        /// 
        /// Value only available when connected to Device. 
        /// </summary>
        /// <value>The device info.</value>
        LEGODeviceInfo DeviceInfo { get; }
        
        /// <summary>
        /// Called when DeviceInfo becomes available (after connect)
        /// </summary>
        event Action<ILEGODevice, LEGODeviceInfo> OnDeviceInfoUpdated;

        
        LEGOManufacturerData ManufactureData { get; }
        event Action<ILEGODevice, LEGOManufacturerData, LEGOManufacturerData> OnManufacturerDataUpdated;

        #endregion Basic System Info



        #region Device Properties

        /// <summary>
        /// Current State of the Device (advertising, connected, etc.)
        /// </summary>
        DeviceState State { get; }

        /// <summary>
        /// Set to true when response with acknowledge of DisconnectAndShutDown message
        /// </summary>
        bool WillShutdown{ get; set; }

        /// <summary>
        /// Access to the virtual motor pair manager
        /// </summary>
        LEGOVirtualServiceManager VirtualServiceManger{ get; }
        
        /// <summary>
        /// Called when the state of the Device is updated. 
        /// </summary>
        event Action<ILEGODevice, DeviceState /* old state */, DeviceState /* new state */> OnDeviceStateUpdated;

        /// <summary>
        /// Enable busy indication to signal that a operation with a long duration is in progress. 
        /// The RGB light on the Device will flash to indicate 'busy'. 
        /// Could for instance be used during callibration of the model, where the user should keep hands of. 
        /// 
        /// May only be called when DeviceSate is InterrogationFinished. 
        /// </summary>
        void EnableBusyIndication(bool enable);

        /// <summary>
        /// The name of the Device as it will appear when advertesing. 
        /// 
        /// Value always available, regardless of the current DeviceState.
        /// </summary>
        string DeviceName { get; }

        /// <summary>
        /// Returns true if name is not too long to be used as a device name (name less that 14 bytes UTF8 encoded)
        /// </summary>
        bool IsValidDeviceNameLength(string name);

        /// <summary>
        /// Set a new name for the Device. 
        /// This method will automatically truncate string valid length.
        /// 
        /// May only be called when DeviceSate is InterrogationFinished. 
        /// </summary>
        void UpdateDeviceName(string name);

        /// <summary>
        /// Invoked when the name of the device has been succesfully updated. 
        /// </summary>
        event Action<ILEGODevice, string> OnNameUpdated;

        /// <summary>
        /// Setup a virtual motor pair - initiates the creation of a new service 
        /// from the combination of the two port ids provided (assuming they are
        /// registered services and they are combinable). 
        /// 
        /// TODO NB: this will only work on later versions of the hub firmware 
        /// (version/hub list to be added)
        ///  
        /// May only be called when DeviceSate is InterrogationFinished. 
        /// </summary>
        void SetupVirtualMotorPair(uint portID1, uint portID2);

        /// <summary>
        /// disconnect a virtual motor pair - will result in the deregistration 
        /// of a previously created virtual motor pair service the creation of 
        /// a new service. PortID must be of a registered virtual motor pair 
        /// port.
        /// 
        /// TODO NB: this will only work on later versions of the hub firmware 
        /// (version/hub list to be added)
        ///  
        /// May only be called when DeviceSate is InterrogationFinished. 
        /// </summary>
        void DisconnectVirtualMotorPair(uint portID);


        /// <summary>
        /// Returns true if the button on the Device is currently pressed down. 
        /// 
        /// Value always available, regardless of the current DeviceState.
        /// </summary>
        bool ButtonPressed { get; }

        /// <summary>
        /// Invoked when the press-state of the Device button changes. 
        /// 
        /// Value always available, regardless of the current DeviceState.
        /// </summary>
        event Action<ILEGODevice, bool> OnButtonStateUpdated;

        /// <summary>
        /// Returns the current battery level of the Device in percentage. 
        /// 
        /// Value only available when DeviceSate is InterrogationFinished. 
        /// </summary>
        int BatteryLevel { get; }

        /// <summary>
        /// Invoked when the current battery level of the Device changes. 
        /// </summary>
        event Action<ILEGODevice, int> OnBatteryLevelPercentageUpdated;

        /// <summary>
        /// Returns the latest RSSI Value
        /// </summary>
        int RSSIValue { get; }

        /// <summary>
        /// Invoked if the device advertisement data results in an updated RSSI value.
        /// </summary>
        event Action<ILEGODevice, int> OnRSSIValueUpdated;


        /// <summary>
        /// Returns the latest WriteMTUSize Value
        /// </summary>
        int WriteMTUSize { get; }
        
        /// <summary>
        /// Invoked if the device responded to a GetWriteMTUSize request and updated the WriteMTUSizeValue.
        /// </summary>
        event Action<ILEGODevice, int> OnWriteMTUSizeReceived;
        
        
        /// <summary>
        /// Returns the latest Hub calculated RSSI value 
        /// </summary>
        int HubCalculatedRSSIValue { get; }

        /// <summary>
        /// Invoked if the device sends an updated RSSI value based on its own calculations.
        /// </summary>
        event Action<ILEGODevice, int> OnHubCalculatedRSSIValueUpdated;

        /// <summary>
        /// Returns true if the specified alert type is currently set (high). 
        /// </summary>
        bool IsAlertHigh(HubAlertType type);


        /// <summary>
        /// Invoked when the state of an alert is updated. 
        /// </summary>
        event Action<ILEGODevice, HubAlertType, bool /* true if the given alert is high */> OnAlertStateUpdated;

        /// <summary>
        /// Gets the supported functions.
        /// 
        /// Value always available, regardless of the current DeviceState.
        /// </summary>
        DeviceFunction SupportedFunctions { get; }

        /// <summary>
        /// Gets the Nework of the ID the Device was last connected to. 
        /// Most usefull before connecting to an advertising device, to determine if the ID was the same as you have last connected to. 
        /// 
        /// Value always available, regardless of the current DeviceState.
        /// </summary>
        int LastConnectedNetworkId { get; }

        /// <summary>
        /// Sets the Network ID. 
        /// Most usefull before connecting to an advertising device, to determine if the ID was the same as you have last connected to. 
        /// 
        /// </summary>
        /// <param name="networkID">Network identifier</param>
        void SetConnectedNetworkID(int networkID);

        /// <summary>
        /// Returns true if the Device is in 'Boot Loaderø mode
        /// 
        /// Value always available, regardless of the current DeviceState.
        /// </summary>
        bool IsBootLoader { get; }

        /// <summary>
        /// Used to disable or enable updates for given ValueEnableUpdateType. 
        /// Updates are per default enabled for all types, so you should only use this if you wish to disable updates for some properties. 
        /// </summary>
        void EnableValueUpdates(bool enable, ValueEnableUpdateType type);

        /// <summary>
        /// Request value update for a given Property or Alert.
        /// </summary>
        void UpdateValueForType(ValueUpdateType type);

        /// <summary>
        /// Resets provided value type.
        /// </summary>
        void ResetValueForType(ValueResetType type);

        /// <summary>
        /// Invoked when a device sends a notification before it shuts down.
        /// </summary>
        event Action<ILEGODevice> OnDeviceWillSwitchOff;

        /// <summary>
        /// Invoked when a device sends a notification before it disconnects.
        /// </summary>
        event Action<ILEGODevice> OnDeviceWillDisconnect;

        /// <summary>
        /// Invoked when a device sends a notification before it goes into Boot Mode.
        /// </summary>
        event Action<ILEGODevice> OnDeviceWillGoIntoBootMode;

        /// <summary>
        /// Invoked when an error has occurred on the device.
        /// </summary>
        event Action<ILEGODevice, MessageErrorType, String, MessageType> OnDidExperienceError;

        /// <summary>
        /// Compatible Protocol Specification version
        /// </summary>
        DeviceCompatibleProtocolSpecificationVersionType CompatibleProtocolSpecification { get; }

        #endregion Device Properties


        #region Services
        /*
        A Service represent an IO made available by the Device, typically some motor or sensor. 
        all methods in this region are only available when the current DeviceState is InterrogationFinished.
        */


        /// <summary>
        /// Returns all Services currently made avaialble by the Device. 
        /// </summary>
        ILEGOService[] Services { get; }

        /// <summary>
        /// Returns all Internal Services currently made avaialble by the Device. 
        /// An internal service represents something that is build into the Device, 
        /// e.g. the Internal Drivebase Motor (as opposed to the external motor connected by a port)
        /// </summary>
        ILEGOService[] InterternalServices { get; }

        /// <summary>
        /// Returns all External Serivces current connected to the Device ports. 
        /// </summary>
        ILEGOService[] ExternalServices { get; }

        /// <summary>
        /// Returns any connected service with the given IOType, or null if none 
        /// is found. If multiple services exists with the same IOType, the 
        /// first such service is returned.
        ///
        /// </summary>
        ILEGOService FindService(IOType type, bool virtualConnection = false);

        /// <summary>
        /// Returns the service with the given PortID, or null if none is found. 
        /// </summary>
        ILEGOService FindService(int portID);


        /// <summary>
        /// Returns any connected service that is declared as a virtual pair 
        /// consisting of portID1 and portID2 or null if none is found. 
        /// </summary>
        ILEGOService FindVirtualService(int portID1, int portID2);

        /// <summary>
        /// Called whenever a service is connected or disconned to this Device. 
        /// </summary>
        event Action<ILEGODevice, ILEGOService, bool /* connected*/> OnServiceConnectionChanged;

        /// <summary>
        /// Called if the SDK failed to register a connected IO as a service. 
        /// </summary>
        event Action<ILEGODevice, string> OnFailToAddServiceWithError;


        #endregion Services


    }

    #region Enumerations

    public enum DeviceState
    {
        /** The Device is disconnected and no connection attempt is in progress */
        DisconnectedAdvertising,
        /** The Device is disconnected and no longer advertising - the device also returns to this state after a successful connection has been closed  */
        DisconnectedNotAdvertising,
        /** A connection attempt is in progress */
        Connecting,
        /** Connected and interrogating Device for required services */
        Interrogating,
        /** Connected and interrogation complete - device is ready for use */
        InterrogationFinished,
        /** Interrogation failed - device is not ready for use */
        InterrogationFailed
    }

    public enum DeviceDisconnectReason
    {
        /** The connection was closed intentionally and without any failure */
        Closed,
        /** The connection was lost, e.g. due to out-of-range */
        ConnectionLost,
        /** Failure during the connection process  */
        FailedToConnect
    }

    public enum DeviceSystemType
    {
        Unknown = -1,

        /** Miscellaneous LEGO (e.g. WeDo)  */
        MiscLEGO = 0,
        /** LEGO Duplo */
        LEGODuplo = 1,
        /** LEGO System 1 */
        LEGOSystem1 = 2,
        /** LEGO System 2 */
        LEGOSystem2 = 3,
        /** LEGO Technic 1 */
        LEGOTechnic1 = 4,
        /** LEGO Technic 2 */
        LEGOTechnic2 = 5,
        /** Third Party 1 */
        LEGOThirdParty1 = 6,
        /** Third Party 2 */
        LEGOThirdParty2 = 7,
    }

    public enum DeviceFunction
    {
        /** Central Mode */
        CentralMode = 0,
        /** Peripheral Mode */
        PeripheralMode = 1,
        /** Device IO Ports Mode */
        IOPorts = 2,
        /** Device acts as a remote controller */
        ActsAsRemoteController = 3,
        /** Unknown */
        Unknown = 4
    }

    public enum ValueEnableUpdateType
    {
        // Properties
        /** Advertisement name */
        AdvName,
        /** Button State */
        ButtonState,
        /** RSSI */
        UpdateRSSI,
        /** Battery Voltage */
        BatteryVoltage,

        // Alerts
        /** Low Battery Voltage Alert */
        LowBatteryVoltageAlert,
        /** High Current Alert */
        HighCurrentAlert,
        /** Low Signal Strength Alert */
        LowSignalStrengthAlert,
        /** High Power Use Alert */
        HighPowerUseAlert
    }

    /**
    * Properties and Alerts which can be requested a value update
    */
    public enum ValueUpdateType
    {
        // Properties
        /** Button State */
        LEDeviceRequestUpdateButtonState,
        /** RSSI */
        LEDeviceRequestUpdateRSSI,
        /** Battery Voltage  */
        LEDeviceRequestUpdateBatteryVoltage,

        // Alerts
        /** Low Battery Voltage Alert */
        LEDeviceRequestUpdateLowVoltageAlert,
        /** High Current Alert */
        LEDeviceRequestUpdateHighCurrentAlert,
        /** Low Signal Strength Alert */
        LEDeviceRequestUpdateLowSignalStrengthAlert,
        /** High Power Use Alert */
        LEDeviceRequestUpdateHighPowerUseAlert
    }

    /**
     * Properties which can be reset
     */
    public enum ValueResetType
    {
        // Properties
        /** Advertisement name */
        LEDeviceResetTypeAdvName,
        /** Hardware Reference ID */
        LEDeviceResetTypeHardwareNetworkID
    }

    public enum HubAlertType
    {
        Unknown = 0,
        LowVoltage = 1,
        HighCurrent = 2,
        LowSignalStrength = 3,
        HighPowerUse = 4
    }

    public enum DeviceCompatibleProtocolSpecificationVersionType
    {
        /** V.2.x  */
        LEDeviceCompatibleProtocolVersionV2x,
        /** V.3.x */
        LEDeviceCompatibleProtocolVersionV3x,
        /** V.3.x - Boot Loader*/
        LEDeviceCompatibleProtocolVersionBootLoaderV3x,
        /** Unknown */
        Unknown
    }

    #endregion Enumerations

}

