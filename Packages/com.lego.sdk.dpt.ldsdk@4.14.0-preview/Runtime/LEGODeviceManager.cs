using UnityEngine;
using System.Collections.Generic;
using System;
using LEGO.Logger;
using CoreUnityBleBridge;

namespace LEGODeviceUnitySDK
{
    public class LEGODeviceManager : MonoBehaviour, ILEGODeviceManager
    {

        private static readonly ILog logger = LogManager.GetLogger(typeof(LEGODeviceManager));

#pragma warning disable 067
        public event Action<DeviceManagerState> OnScanStateUpdated = delegate {};
        public event Action<ILEGODevice, DeviceState, DeviceState> OnDeviceStateUpdated;
        public event Action<ILEGODevice, DeviceDisconnectReason> OnDeviceDisconnected;
        public event Action<DeviceFlashState> OnDeviceFlashStateUpdated;
        public event Action<FlashErrorCode, string /* error message */> OnDeviceFlashFailed;
        public event Action<float /* pct */ > OnFlashProgressPercentageUpdated;
        public event Action<IFirmwareUpgradeProcess,int> OnSizeOfFlashFileUpdated;

        public event Action OnPacketSent;
        public event Action OnPacketReceived;
        public event Action OnPacketDropped;
#pragma warning restore 067


        private static GameObject deviceManagerGameObject;
        public static LEGODeviceManager Instance;

        private IBleBridge bleBridge;

#region Sub-states
        public DeviceManagerState ScanState { get { return TranslateAdapterState(bleBridge.AdapterScanState); } }
        private DeviceManagerState TranslateAdapterState(AdapterScanState rawState)
        {
            switch (rawState) {
            case AdapterScanState.TurningOnScanning:
                return DeviceManagerState.ScanRequested;
            case AdapterScanState.Scanning:
                return DeviceManagerState.Scanning;
            case AdapterScanState.BluetoothUnavailable:
            case AdapterScanState.BluetoothDisabled:
            case AdapterScanState.NotScanning:
            default:
                return DeviceManagerState.NotScanning;
            }
        }

        private DeviceListTracker deviceListTracker = new DeviceListTracker();
#endregion

        #region Lifecycle
        //Should be called before ANY messages communicating the native libs
        public static void Initialize()
        {
            if (Instance != null)
            {
                logger.Warn("Ignoring call to Initialize for LEGODeviceManager - already initialized");
                return;
            }

            deviceManagerGameObject = new GameObject("LEGODeviceManager");

            Instance = deviceManagerGameObject.AddComponent<LEGODeviceManager>();
            DontDestroyOnLoad(deviceManagerGameObject);
            MainThreadDispatcher.Initialize();
            
            var bleFilters = new BleSettings.Filtering
            {
                GattServices = GattService.AllServices
            };
            
            var bleSettings = new BleSettings(bleFilters);
            BleFactory.Initialize(bleSettings, ((LEGODeviceManager) Instance).OnBleBridgeInitialized);
        }

        private void OnBleBridgeInitialized (InitializationEventArgs args)
        {
            this.bleBridge = args.BleBridge;

            // Setup callbacks:
            bleBridge.ErrorOccurred += (msg => logger.Error("BLE Bridge error: "+msg));
            bleBridge.AdapterScanStateChanged += (scanState) => {
                OnScanStateUpdated(ScanState);
            };

            bleBridge.DeviceAppeared += deviceListTracker.OnDeviceAppeared;
            bleBridge.DeviceDisappeared += deviceListTracker.OnDeviceDisappeared;

            bleBridge.PacketSent += PacketSent; 
            bleBridge.PacketReceived += PacketReceived;
            bleBridge.PacketDropped += PacketDropped;
        }

        private void PacketSent()
        {
            OnPacketSent?.Invoke();
        }
        private void PacketReceived()
        {
            OnPacketReceived?.Invoke();
        }
        private void PacketDropped()
        {
            OnPacketDropped?.Invoke();
        }
        public static void Dispose()
        {
            Instance = null;
            Destroy(deviceManagerGameObject);
            //NativeBridge.Dispose();
        }

        public void Awake()
        {
            logger.Info("Calling Awake on LEGODeviceManager");
        }

        #endregion

        #region Device list
        public ILEGODevice[] Devices {
            get { return deviceListTracker.DevicesAsArray; }
        }
        public ILEGODevice FindLegoDevice(string deviceID)
        {
            return deviceListTracker.FindLegoDevice(deviceID);
        }

        public List<ILEGODevice> DevicesInState(DeviceState deviceState, bool includeBootLoaderDevices = false)
        {
            return deviceListTracker.DevicesInState(deviceState, includeBootLoaderDevices);
        }

        public List<ILEGODevice> DevicesConnected(bool connected)
        {
            return deviceListTracker.DevicesConnected(connected);
        }
        #endregion

        public LEGODeviceSDKVersionInfo SDKVersionInfo
        {
            get
            {
                return new LEGODeviceSDKVersionInfo(
                    version:     BuildConfig.SDK_VERSION,
                    buildNumber: BuildConfig.SDK_BUILD_NUMBER,
                    commit:      BuildConfig.SDK_GIT_HEAD_HASH,
                    v2FirmwareVersion: BuildConfig.SDK_TESTED_WITH_V2_FIRMWARE_VERSION,
                    v3FirmwareVersion: BuildConfig.SDK_TESTED_WITH_V3_FIRMWARE_VERSION);
            }
        }

        public BluetoothState BluetoothState
        {
            get
            {
                switch (bleBridge.AdapterScanState) {
                case AdapterScanState.NotScanning:
                case AdapterScanState.Scanning:
                case AdapterScanState.TurningOnScanning:
                    return BluetoothState.On;
                case AdapterScanState.BluetoothDisabled:
                case AdapterScanState.BluetoothUnavailable:
                default:
                    return BluetoothState.Off;
                }
            }
        }

        #region Discovery
        private bool lastScanStateRequested = false;
        public void Scan() 
        {
            if (ScanState != DeviceManagerState.Scanning 
                && ScanState != DeviceManagerState.ScanRequested)
            {    // not already in scanning/turning on scanning
                if (lastScanStateRequested == false && OnScanStateUpdated != null)
                {
                    OnScanStateUpdated(DeviceManagerState.ScanRequested);
                }
                lastScanStateRequested = true;
            
                bleBridge.SetScanState(true);
            }
        }
        public void StopScanning() 
        {
            lastScanStateRequested = false;
            bleBridge.SetScanState(false);
        }
        #endregion

        #region Connection establishment
        public void ConnectToDevice(ILEGODevice legoDevice)
        {
            logger.Warn("Connect to device");
            ConnectToDevice(legoDevice.DeviceID);
        }

        public void ConnectToDevice(string legoDeviceID)
        {
            ILEGODevice device = deviceListTracker.FindLegoDevice(legoDeviceID);
            if (device != null)
                ConnectDevice(device);
            else
                logger.Warn("Connect requested on unknown device \""+legoDeviceID+"\"");
        }

        public void DisconnectDevice(string legoDeviceID)
        {
            ILEGODevice device = deviceListTracker.FindLegoDevice(legoDeviceID);
            if (device != null)
                DisconnectDevice(device);
            else
                logger.Warn("Disconnect requested on unknown device \""+legoDeviceID+"\"");
        }

        public void ConnectDevice(ILEGODevice legoDevice)
        {
            if (legoDevice is AbstractLEGODevice)
                ((AbstractLEGODevice)legoDevice).Connect();
            else
                logger.Warn("Connect requested on device of unknown type "+legoDevice.GetType().Name);
        }

        public void DisconnectDevice(ILEGODevice legoDevice)
        {
            if (legoDevice is AbstractLEGODevice)
                ((AbstractLEGODevice)legoDevice).Disconnect();
            else
                logger.Warn("Disconnect requested on device of unknown type "+legoDevice.GetType().Name);
        }

        internal void DeviceDidDisconnect(ILEGODevice device, DeviceDisconnectReason reason) {
            if (OnDeviceDisconnected != null)
                OnDeviceDisconnected(device, reason);
        }            

        #endregion

        public void RequestDeviceToSwitchOff(ILEGODevice legoDevice)
        {
            var device = legoDevice as LEGODevice;
            if (device != null)
                ((LEGODevice)device).RequestToSwitchOff();
            else
                logger.Warn("RequestDeviceToSwitchOff requested on device of unknown type "+legoDevice.GetType().Name);
        }

        public void RequestDeviceToDisconnect(ILEGODevice legoDevice)
        {
            var device = legoDevice as LEGODevice;
            if (device != null)
                ((LEGODevice)device).RequestToDisconnect();
            else
                logger.Warn("RequestDeviceToDisconnect requested on device of unknown type "+legoDevice.GetType().Name);
        }

        public void ClearPersistedModeInformationCache()
        {
            InterrogationCacheManager.Instance.ClearCache();
        }


        public void RefreshDeviceList()
        {
            deviceListTracker.OrderDevicesByRSSI();
        }


        #region Firmware update
        private IFirmwareUpgradeProcess firmwareUpgradeProcess;

        public void FlashFirmware(string legoDeviceID, FlashCompletionAction completionAction,
            byte[] firmwareData, DeviceSystemType imageSystemType = DeviceSystemType.LEGOSystem1, int imageSystemNumber = 0)
        {
            if (firmwareData == null || firmwareData.Length == 0)
            {
                logger.Error("Firmware data is null or empty, ignoring call to start flashing");
                if (OnDeviceFlashFailed != null)
                    OnDeviceFlashFailed(FlashErrorCode.General, "Invalid firmware image data");

                return;
            }

            
            if (firmwareUpgradeProcess != null) {
                logger.Warn("FirmwareUpgradeProcess exists, currently in state: " + firmwareUpgradeProcess.FlashState);
                if(firmwareUpgradeProcess.FlashState != DeviceFlashState.Idle)
                {
                    logger.Error("A firmware upgrade is already in progress.");
                    if (OnDeviceFlashFailed != null)
                        OnDeviceFlashFailed(FlashErrorCode.General,
                            "A firmware upgrade has state " + firmwareUpgradeProcess.FlashState +". Can't start another now.");
                    return;
                }
            }

            var device = FindLegoDevice(legoDeviceID);
            if (device == null) {
                logger.Error("Can't flash unknown device: "+legoDeviceID);
                if (OnDeviceFlashFailed != null)
                    OnDeviceFlashFailed(FlashErrorCode.General, "The device \""+legoDeviceID+"\" cannot be found.");
                return;
            }

            logger.Debug("Starting firmware upgrade process with firmware image of size: " + firmwareData.Length);
            StartFirmwareUpgradeProcess(device, firmwareData, imageSystemType, imageSystemNumber, completionAction);
        }

        // Start upgrading process - once preconditions have been established.
        private void StartFirmwareUpgradeProcess(ILEGODevice device,
            byte[] firmwareData, DeviceSystemType imageSystemType, int imageSystemNumber,
            FlashCompletionAction completionAction)
        {
            var deviceType = new AbstractLEGODevice.DeviceType(device.SystemType, device.SystemDeviceNumber);
            
            Action<IFirmwareUpgradeProcess> onCompletion = (process) => 
            {
                // Done! Unsubscribe & reset
                process.OnDeviceFlashStateUpdated -= DidUpdateFlashState;
                process.OnDeviceFlashFailed -= DidFailDeviceFlash;
                process.OnFlashProgressPercentageUpdated -= DidUpdateFlashProgressPercentage;

                if (process == firmwareUpgradeProcess)
                {
                    firmwareUpgradeProcess = null;
                }
                
                var legoDevice = device as AbstractLEGODevice;
                var noAckParams = AbstractLEGODevice.DeviceTypeGroup.OAD.RegularParameters;
                legoDevice.SetNoAckParameters(noAckParams.PacketsPerWindow, noAckParams.WindowLengthMs); 
            };
            
            if(AbstractLEGODevice.DeviceTypeGroup.OAD.Contains(deviceType) && 
               device is AbstractLEGODevice)
            {
                var legoDevice = device as AbstractLEGODevice;
                var noAckParams = AbstractLEGODevice.DeviceTypeGroup.OAD.AssetTransferParameters;
                legoDevice.SetNoAckParameters(noAckParams.PacketsPerWindow, noAckParams.WindowLengthMs); 
                firmwareUpgradeProcess = new OADFirmwareUpgradeProcess(device, ((AbstractLEGODevice)device).OADClient, firmwareData, onCompletion);
            }
            else
            {
                if(deviceType.Equals(AbstractLEGODevice.DeviceType.Hub65) && device is AbstractLEGODevice)
                {
                    // CITY uses NoAck during fw update - setting optimal packet count / window for it
                    var legoDevice = device as AbstractLEGODevice;
                    legoDevice.SetNoAckParameters(10,100);
                }
                
                firmwareUpgradeProcess = new FirmwareUpgradeProcess(device,
                    firmwareData, imageSystemType, imageSystemNumber,
                    completionAction, this, onCompletion);
            }

            OnSizeOfFlashFileUpdated?.Invoke(firmwareUpgradeProcess, firmwareData.Length);

            firmwareUpgradeProcess.OnDeviceFlashStateUpdated += DidUpdateFlashState;
            firmwareUpgradeProcess.OnDeviceFlashFailed += DidFailDeviceFlash;
            firmwareUpgradeProcess.OnFlashProgressPercentageUpdated += DidUpdateFlashProgressPercentage;

            firmwareUpgradeProcess.StartProcess();
        }

        public IFirmwareUpgradeProcess GetFlashProcess()
        {
            return firmwareUpgradeProcess;
        }
        
        public DeviceFlashState GetCurrentFlashState()
        {
            if (firmwareUpgradeProcess != null)
            {
                return firmwareUpgradeProcess.FlashState;
            }

            return DeviceFlashState.Idle;
        }
        
        private void DidUpdateFlashState(IFirmwareUpgradeProcess process, DeviceFlashState newState) {
            if (process != firmwareUpgradeProcess) return;
            if (OnDeviceFlashStateUpdated != null)
                OnDeviceFlashStateUpdated(newState);

            if (newState == DeviceFlashState.Idle)
                firmwareUpgradeProcess = null; // Do this last.
        }
        private void DidFailDeviceFlash(IFirmwareUpgradeProcess process, FlashErrorCode errorCode, string errorMessage) {
            if (process != firmwareUpgradeProcess) return;
            
            firmwareUpgradeProcess = null;

            OnDeviceFlashFailed?.Invoke(errorCode, errorMessage);
        }
        private void DidUpdateFlashProgressPercentage(IFirmwareUpgradeProcess process, float pct) {
            if (process != firmwareUpgradeProcess) return;
            OnFlashProgressPercentageUpdated?.Invoke(pct);
        }

        public DeviceFlashState FlashState => firmwareUpgradeProcess?.FlashState ?? DeviceFlashState.Idle;

        #endregion

        /// Forward the device update state from the Device
        internal void HandleDeviceStateUpdated(ILEGODevice legoDevice, DeviceState oldState, DeviceState newState)
        {
            if (OnDeviceStateUpdated != null)
                OnDeviceStateUpdated(legoDevice, oldState, newState);
        }
    }
}
