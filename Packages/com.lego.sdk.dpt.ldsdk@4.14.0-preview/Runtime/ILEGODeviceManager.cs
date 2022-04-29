using System;
using System.Collections.Generic;

namespace LEGODeviceUnitySDK
{

    /// <summary>
    /// This is the entry point for all access to LEGO BlE enabled devices.
    /// Use this manager to scan and connect to to advertising devices.
    ///
    /// You must initialize the LEGODeviceManager before calling Scan or any other method that invokes
    /// methods on the native side. The initialize method will add the LEGODeviceManager game-boject under DontDestroyOnLoad
    ///
    /// </summary>
    public interface ILEGODeviceManager
    {
        #region Scanning for Advertising Devices

        /// <summary>
        /// Start scanning for advertising LEGO BLE Devices
        /// </summary>
        void Scan();

        /// <summary>
        /// Stop scanning for advertising LEGO BLE Devices
        /// </summary>
        void StopScanning();

        /// <summary>
        /// Used to check bluetooth capbailities
        /// Note, if bluetooth is turned on but there is no Low Energy capabilities, this will return NotSupported.
        /// Hence, normally you will want to check for 'NotSupported' before checking for off.
        ///
        /// For iOS this property will always return Unknown - iOS will prompt the user to turn on Bluetooth itself if necessary.
        ///
        /// </summary>
        /// <value>The state of the bluetooth.</value>
        BluetoothState BluetoothState { get; }


        /// <summary>
        /// Get the current Scan State of the DeviceManager.
        /// </summary>
        DeviceManagerState ScanState { get; }


        /// <summary>
        /// Called whenever state changes from 'Scanning for' / 'Stopped scanning for' Devices
        /// Normally this will be triggered by a call to Scan or StopScanning, but it may also
        /// be triggered by the SDK itself, for instance in relation to updating firmware, where
        /// the SDK will force 'Scanning' to connect to the Bootloader.
        /// </summary>
        event Action<DeviceManagerState> OnScanStateUpdated;

        #endregion Scanning for Advertising Devices

        #region Discovering Devices

        /// <summary>
        /// Use this method to get all Devices in a specific state.
        /// </summary>
        List<ILEGODevice> DevicesInState(DeviceState deviceState, bool includeBootLoaderDevices = false);

        /// <summary>
        /// Register for this update to get nofications as LEGODevices start advertising, stops advertising,
        /// starts connecting, etc.
        ///
        /// Once you get an event for 'DisconnectedAdvertising' you may call Connect on that device.
        /// </summary>
        event Action<ILEGODevice, DeviceState /* old state */, DeviceState /* new state */> OnDeviceStateUpdated;

        #endregion Discovering Devices

        #region Connect / Disconnect to Devices

        /// <summary>
        /// Connect to a LEGO Device
        /// </summary>
        /// <param name="legoDeviceID">Lego device identifier.</param>
        void ConnectToDevice(ILEGODevice legoDeviceID);

        /// <summary>
        /// Disconnect from a LEGO Device
        /// </summary>
        /// <param name="legoDeviceID">Lego device identifier.</param>
        void DisconnectDevice(ILEGODevice legoDeviceID);
     
        /// <summary>
        /// Request the hub to power off.
        /// </summary>
        /// <param name="legoDeviceID">Lego device identifier.</param>
        void RequestDeviceToSwitchOff(ILEGODevice legoDeviceID);
        
        /// <summary>
        /// Request the hub to disconnect. This will trigger the OnDeviceWillDisconnect delegate method in LEGOServiceCallbacks
        /// </summary>
        /// <param name="legoDeviceID">Lego device identifier.</param>
        void RequestDeviceToDisconnect(ILEGODevice legoDeviceID);

        
        /// <summary>
        /// Returns the devices current flash state if firmwareUpgradeProcess is not null.
        /// Else it returns DeviceFlashState.Idle
        /// 
        /// Value always available, regardless of the current flashingUpgradeProcess.
        /// </summary>
        DeviceFlashState GetCurrentFlashState();
        
        /// <summary>
        /// Returns the current IFirmwareUpgradeProcess, else return null
        /// 
        /// Returns null, if not existing
        /// </summary>
        IFirmwareUpgradeProcess GetFlashProcess();
        
        /// <summary>
        /// Called whenever the connection to a Device is closed, either by the user or due to a failure.
        /// The DeviceConnectReasons states why the connection was closed.
        /// </summary>
        event Action<ILEGODevice, DeviceDisconnectReason> OnDeviceDisconnected;

        #endregion Connect / Disconnect

        #region Getting known Devices

        event Action OnPacketSent;
        event Action OnPacketReceived;
        event Action OnPacketDropped;
        
        /// <summary>
        /// All devices currently known by the Manager.
        ///
        /// A device is added to this list once it is discovered advertising (state == DisconnectedAdvertising)
        /// A device is removed from this list once it stops advertising (state == DiconnectedNotAdvertising)
        ///
        /// </summary>
        ILEGODevice[] Devices { get; }

        /// <summary>
        /// Finds a lego device by its DeviceID.
        /// </summary>
        ILEGODevice FindLegoDevice(string deviceID);

        /// <summary>
        /// If true, returns all conneted devices, that is devices in state 'Connecting', 'Interrogating' or 'Interrogating finished'
        /// If false, returns all knwon disconneted devices, that is devices in state 'DisconnectedAdvertising' and 'DisconnectedNotAdvertising'
        /// </summary>
        List<ILEGODevice> DevicesConnected(bool connected);


        /// <summary>
        /// This method will syncrohize the Devices property with the list of devices maintained in the native SDK.
        /// The only real use of this method right now is to make sure we get a list that is also sorted according to most recent RSSI value.
        /// Otherwise, there should be no need to explicitely refresh the list of Devices as this is automatically maintaned based on DeviceState update events.
        /// </summary>
        void RefreshDeviceList();

        #endregion Getting known Devices

        #region Firmware Flashing

        /// <summary>
        /// Flashs new firmware onto a Device.
        ///
        /// May only be called when DeviceSate is InterrogationFinished.
        ///
        /// Scenario 1:
        /// The connected device is in 'App Mode'
        ///
        /// In 'App Mode' you may specify any completionAction.
        /// If completionAction is AdvertiseAndAutoconnect, the following process takes place:
        /// 1) The Device will automatically restart in Boot Loader mode (FlashState: Idle => WaitingToConnectToBootLoader)
        /// 2) The SDK connect to the Boot Loader Hub (FlashState: WaitingToConnectToBootLoader => ConnectingToBootLoader)
        /// 3) The SDK automtically connects to the Boot Loader Hub and starts flashing (FlashState: ConnectingToBootLoader => ConnectedToBootLoader)
        /// 4) The Device disconnects and starts advertising in 'App Mode' (FlashState: ConnectedToBootLoader => WaitingForNewlyFlashedDeviceToAdvertise)
        /// 5) The SDK connects to the Hub and restores the name  (FlashState: WaitingForNewlyFlashedDeviceToAdvertise => Idle)
        ///
        /// That is, once the DeviceFlashState returns to 'Idle' the flashing has completed. The state may also change to 'Idle' due to some error along this process.
        /// For instance, if the SDK never manages to connect to the Boot Loader in step 2. In this case the OnDeviceFlashFailed is invoked right
        /// before the OnDeviceFlashStateUpdated callback.
        ///
        ///
        /// Scenario 2:
        /// The connected device is 'Boot Loader' mode.
        ///
        /// In 'Boot Loader' mode you may not specify the AdvertiseAndAutoconnect (but only TurnOff or AdvertiseNewImage).
        /// That is, we cannot automatically ask the SDK to connect to the Hub when it starts up in 'App mode' after successful flashing.
        /// This means that after sted 3 above, the FlashState returns to Idle.
        ///
        void FlashFirmware(string legoDeviceID, FlashCompletionAction completionAction, byte[] firmwareData,
            DeviceSystemType imageSystemType = DeviceSystemType.LEGOSystem1, int imageSystemNumber = 0);

        /// <summary>
        /// Gets the state of the flashing progress.
        /// If no flashing is in progress, the state is Idle.
        /// </summary>
        DeviceFlashState FlashState { get; }

        /// <summary>
        /// Called when the FlashState is updated during Device Flashing.
        /// </summary>
        event Action<DeviceFlashState> OnDeviceFlashStateUpdated;

        /// <summary>
        /// Called if Flashing of the Device failed somehere along the process.
        /// </summary>
        event Action<FlashErrorCode, string /* error message */> OnDeviceFlashFailed;

        /// <summary>
        /// Called when Firmware is being flashed to incdicate flash progress in percentage.
        /// </summary>
        event Action<float> OnFlashProgressPercentageUpdated;
        
        
        /// <summary>
        /// Called when Firmware transfer is initiated to report size of file
        /// </summary>
        event Action<IFirmwareUpgradeProcess,int> OnSizeOfFlashFileUpdated;
        
        #endregion Firmware Flashing

        #region LEGO Wireless SDK Info

        /// <summary>
        /// Gets info about the SDK version and which Firmware.
        /// </summary>
        /// <value>The SDKV ersion info.</value>
        LEGODeviceSDKVersionInfo SDKVersionInfo { get; }

    #endregion LEGO Wireless SDK Info

    /// <summary>
    /// Deletes the persisted cache of mode information from persisted cache
    /// </summary>
    void ClearPersistedModeInformationCache();
    }

    public enum BluetoothState
    {
        Unknown,
    On,
        Off,
        MissingLocationPermission
    }
    public enum DeviceManagerState
    {
        ScanRequested = 0,
        Scanning = 1,
        NotScanning = 2
    }

    public enum DeviceFlashState
    {
        Idle,
        WaitingToConnectToBootLoader,
        ConnectingToBootLoader,
        ConnectedToBootLoader,
        Flashing,
        FlashingCompleted,
        FlashingCompletedWithDisconnect,
        WaitingForNewlyFlashedDeviceToAdvertise,
        ConnectingToNewlyFlashedDevice
    }

    public enum FlashCompletionAction
    {
        NotConfigured,
        TurnOff,
        AdvertiseNewImage,
        AdvertiseAndAutoConnect
    };

    public enum FlashErrorCode
    {
        General = 200,
        TimedOutConnectingToBootLoader = 201,
        TimeOutConnectingToFlashedDevice = 202,
        DisconnectedDuringDownload = 203,
        TransferFailed = 204
    }
}

