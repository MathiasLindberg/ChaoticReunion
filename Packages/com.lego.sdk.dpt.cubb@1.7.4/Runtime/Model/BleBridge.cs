using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using LEGO.Logger;

namespace CoreUnityBleBridge.Model
{
    internal class BleBridge: IBleBridge
    {
        private static readonly ILog Log = LogManager.GetLogger<BleBridge>();

        public event Action<AdapterScanState> AdapterScanStateChanged = delegate {};
        public event Action<IBleDevice> DeviceAppeared = delegate {};
        public event Action<IBleDevice> DeviceDisappeared = delegate {};
        public event Action<string> ErrorOccurred = delegate {};
        public event Action PacketSent = delegate {};
        public event Action PacketReceived = delegate {};
        public event Action PacketDropped = delegate {};
        
        private readonly INativeToUnity nativeToUnity;
        private readonly IUnityToNative unityToNative;

        internal BleBridge(INativeToUnity nativeToUnity, IUnityToNative unityToNative)
        {
            this.nativeToUnity = nativeToUnity;
            this.unityToNative = unityToNative;
            
            AddSubscriptions();
            
            #if UNITY_EDITOR_OSX
            EditorSetup();
            #endif
        }

        #region Setting up
        internal void EmitInitialEvents()
        {
            AdapterScanStateChanged(adapterScanState);
        }
        #endregion

        #region Actions
        public void SetScanState(bool enabled)
        {
            unityToNative.SetScanState(enabled);
        }
        #endregion

        #region Model state
        private AdapterScanState adapterScanState = AdapterScanState.BluetoothUnavailable;
        public AdapterScanState AdapterScanState { get { return adapterScanState; } }

        private readonly List<BleDevice> devices = new List<BleDevice>();
        private readonly Dictionary<string, BleDevice> devicesByID = new Dictionary<string, BleDevice>();
        public ReadOnlyCollection<IBleDevice> Devices { get { return devices.Cast<IBleDevice>().ToList().AsReadOnly(); } }
        #endregion

        #region Incoming events
        private void AddSubscriptions()
        {
            nativeToUnity.AdapterScanStateChanged += OnAdapterScanStateChanged;
            nativeToUnity.DeviceStateChanged += OnDeviceStateChanged;
            nativeToUnity.DeviceConnectionStateChanged += OnDeviceConnectionStateChanged;
            nativeToUnity.DeviceDisappeared += OnDeviceDisappeared;
            nativeToUnity.Error += OnError;
            nativeToUnity.Log += OnLog;
            nativeToUnity.PacketSent += OnPacketSent;
            nativeToUnity.PacketReceived += OnPacketReceived;
            nativeToUnity.PacketTransmitted += OnPacketTransmitted;
            nativeToUnity.PacketDropped += OnPacketDropped;
            nativeToUnity.WriteMTUSize += OnWriteMTUSize;
            nativeToUnity.MTUSizeChanged += OnMtuSizeChanged;
        }

        private void OnError(ErrorEventArgs args)
        {
            Log.Error("OnError: " + args);
            
            ErrorOccurred(args.Message);
        }

        private void OnLog(string message, LogLevel level)
        {
            Log.LogMessage(message, level);
        }        
        
        private void OnAdapterScanStateChanged(AdapterScanStateChangedEventArgs args)
        {
            Log.Debug("OnAdapterScanStateChanged: " + args);
            
            adapterScanState = args.AdapterScanState;
            AdapterScanStateChanged(adapterScanState);

            if(args.AdapterScanState == AdapterScanState.BluetoothDisabled ||
               args.AdapterScanState == AdapterScanState.BluetoothUnavailable)
            {
                foreach(var device in devicesByID)
                {
                    var newState = new DeviceConnectionStateChangedEventArgs();
                    newState.DeviceID = device.Key;
                    newState.DeviceConnectionState = DeviceConnectionState.Disconnected;
                    OnDeviceConnectionStateChanged(newState);
                }
            }
        }

        private void OnDeviceStateChanged(DeviceStateChangedEventArgs args)
        {
            BleDevice device;
            bool justAdded = false;
            if (!devicesByID.TryGetValue(args.DeviceID, out device))
            {
                device = new BleDevice(args.DeviceID, unityToNative);
                devicesByID.Add(args.DeviceID, device);
                devices.Add(device);
                justAdded = true;
            }

            // First update the device properties, then fire "device appeared", then fire any change events:
            device.OnPropertiesChanged(args);
            if (justAdded) DeviceAppeared(device);
            device.FirePendingEvents();
        }

        private void OnDeviceConnectionStateChanged(DeviceConnectionStateChangedEventArgs args)
        {
            Log.Debug("OnDeviceConnectionStateChanged: " + args);
            
            BleDevice bleDevice;
            if (!devicesByID.TryGetValue(args.DeviceID, out bleDevice))
            {
                return;
            }

            bleDevice.OnConnectionStateChanged(args.DeviceConnectionState);
        }

        private void OnDeviceDisappeared(DeviceDisappearedEventArgs args)
        {
            Log.Debug("OnDeviceDisappeared: " + args);
            
            BleDevice bleDevice;
            if (!devicesByID.TryGetValue(args.DeviceID, out bleDevice))
            {
                return;
            }
            devicesByID.Remove(args.DeviceID);
            devices.Remove(bleDevice);
            bleDevice.OnDisappeared();
            DeviceDisappeared(bleDevice);
        }


        private void OnPacketReceived(PacketReceivedEventArgs args)
        {
            PacketReceived();
            
            if(Log.IsVerboseEnabled)
                Log.Verbose("OnPacketReceived: " + args);

            BleDevice bleDevice;
            if (!devicesByID.TryGetValue(args.DeviceID, out bleDevice))
            {
                return;
            }
            bleDevice.OnPacketReceived(args);
        }

        private void OnPacketTransmitted(PacketTransmittedEventArgs args)
        {
            if(Log.IsVerboseEnabled)
                Log.Verbose("OnPacketTransmitted: " + args);
            
            BleDevice bleDevice;
            if (!devicesByID.TryGetValue(args.DeviceID, out bleDevice))
            {
                return;
            }
            bleDevice.OnPacketTransmitted(args);
        }

        private void OnPacketSent(PacketTransmittedEventArgs args)
        {
            PacketSent();
            
            if(Log.IsVerboseEnabled)
                Log.Verbose("OnPacketSent: " + args);
            
            BleDevice bleDevice;
            if (!devicesByID.TryGetValue(args.DeviceID, out bleDevice))
            {
                return;
            }
            bleDevice.OnPacketSent(args);
        }
        
        private void OnPacketDropped(PacketDroppedEventArgs args)
        {
            PacketDropped();
            
            if(Log.IsVerboseEnabled)
                Log.Verbose("OnPacketDropped: " + args);

            BleDevice bleDevice;
            if (!devicesByID.TryGetValue(args.DeviceID, out bleDevice))
            {
                return;
            }
            bleDevice.OnPacketDropped(args);
        }
        
        private void OnWriteMTUSize(WriteMTUSizeEventArgs args)
        {
            if (Log.IsVerboseEnabled)
            {
                Log.Verbose("WriteMTUSize: " + args);
            }

            BleDevice bleDevice;
            if (!devicesByID.TryGetValue(args.DeviceID, out bleDevice))
            {
                return;
            }
            bleDevice.OnWriteMTUSize(args);
        }
        
        private void OnMtuSizeChanged(MtuSizeChangedEventArgs args)
        {
            Log.Verbose($"BleBridge.OnMtuSizeChanged: " + args);
            
            if (!devicesByID.TryGetValue(args.DeviceID, out var bleDevice))
            {
                Log.Warn("Unable to find device with id: " + args.DeviceID);
                return;
            }
                
            bleDevice?.OnMTUSizeChanged(args);
        }
        #endregion

        #region Editor setup
        #if UNITY_EDITOR_OSX
        // When CMD + Q in OSX builds, the app will close but throw "Program quit unexpectedly" from finalizer
        ~BleBridge() => TearDown();

        private void TearDown()
        {
            if (devices != null)
            {
                foreach (var bleDevice in devices)
                {
                    unityToNative?.DisconnectFromDevice(bleDevice.ID);
                }
            }
            devices?.Clear();
            devicesByID?.Clear();
            
            SetScanState(false);
        }
        private void EditorSetup()
        {
            UnityEditor.EditorApplication.playModeStateChanged += OnEditorPlayModeStateChanged;
        }

        private void OnEditorPlayModeStateChanged(UnityEditor.PlayModeStateChange change)
        {
            if (change == UnityEditor.PlayModeStateChange.ExitingPlayMode)
            {
                TearDown();
                UnityEditor.EditorApplication.playModeStateChanged -= OnEditorPlayModeStateChanged;
            }
        }
        #endif
        #endregion
    }
}