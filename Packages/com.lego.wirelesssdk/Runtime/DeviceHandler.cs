using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace LEGODeviceUnitySDK
{
    public enum HubType
    {
        Any,
        Boost,
        City,
        Technic,
        DuploTrain,
        RemoteControl
    }

    public class DeviceHandler : MonoBehaviour
    {
        public Action<ILEGODevice> OnDeviceDisconnected;
        public Action<ILEGODevice> OnDeviceInterrogating;
        public Action<ILEGODevice> OnDeviceInitialized;
        public Action<ILEGODevice> OnDeviceAppeared;
        public Action<ILEGODevice> OnDeviceDisappeared;

        public bool isScanning { get; private set; }

        public static AbstractLEGODevice.DeviceType[] hubOptions = {
            null,
            AbstractLEGODevice.DeviceType.Hub64,    // Boost hub
            AbstractLEGODevice.DeviceType.Hub65,    // City hub
            AbstractLEGODevice.DeviceType.Hub128,   // Technic hub
            AbstractLEGODevice.DeviceType.Hub32,    // Duplo train
            AbstractLEGODevice.DeviceType.Hub66     // City remote
            };

        public HubType hubType = 0;

        private bool isAutoConnecting = false;

        private void Awake()
        {
            LEGODeviceManager.Initialize();
            LEGODeviceManager.Instance.OnDeviceStateUpdated += OnDeviceStateUpdated;
        }

        void OnApplicationQuit()
        {
            Debug.Log("Application ending after " + Time.time + " seconds");
            StopScanning();
            foreach (var device in LEGODeviceManager.Instance.DevicesConnected(true))
            {
                LEGODeviceManager.Instance.RequestDeviceToDisconnect(device);
            }
            LEGODeviceManager.Instance.OnDeviceStateUpdated -= OnDeviceStateUpdated;
        }

        public void StartScanning()
        {
            if (!isScanning)
            {
                StartCoroutine(StartScanningWhenReady());
                isScanning = true;
            }
        }

        public void AutoConnectToDeviceOfType(HubType type)
        {
            Debug.LogFormat("AutoConnectToDeviceOfType {0}", type);
            this.hubType = type;
            StartScanning();
            StartCoroutine(ConnectToStrongestSignalWithDelay());
        }

        IEnumerator StartScanningWhenReady()
        {
            // Debug.Log("Waiting to scan : ScanState " + LEGODeviceManager.Instance.ScanState + " - BluetoothState: " + LEGODeviceManager.Instance.BluetoothState);
            // yield return new WaitUntil(() =>
            // {
            //   return true; // todo dumb fix. otherwise never stats scanning LEGODeviceManager.Instance.BluetoothState == BluetoothState.On;
            // });

            yield return null;

            Debug.Log("Start scanning : State " + LEGODeviceManager.Instance.ScanState + " - Bluetooth state: " + LEGODeviceManager.Instance.BluetoothState);

            LEGODeviceManager.Instance.OnScanStateUpdated += OnScanStateUpdated;
            LEGODeviceManager.Instance.Scan();

        }

        public void StopScanning()
        {
            Debug.Log("Stop scanning");
            LEGODeviceManager.Instance.StopScanning();

            LEGODeviceManager.Instance.OnScanStateUpdated -= OnScanStateUpdated;
            isScanning = false;
        }

        public void ConnectToDevice(ILEGODevice device)
        {
            Debug.LogFormat("Connect to device {0}", device.DeviceName);
            LEGODeviceManager.Instance.ConnectToDevice(device);
        }

        public void DisconnectDevice(ILEGODevice device)
        {
            Debug.LogFormat("Disconnect device {0}", device.DeviceName);
            LEGODeviceManager.Instance.DisconnectDevice(device);
        }

        IEnumerator ConnectToStrongestSignalWithDelay(float delay = 1f)
        {
            isAutoConnecting = true;
            yield return new WaitForSeconds(delay);
            ConnectToStrongestSignal();
        }

        void ConnectToStrongestSignal()
        {
            Debug.LogFormat("ConnectToStrongestSignal");
            var discoveredDevices = LEGODeviceManager.Instance.DevicesInState(DeviceState.DisconnectedAdvertising);
            if (discoveredDevices != null && discoveredDevices.Count > 0)
            {
                Debug.LogFormat("Found devices - testing types");
                ILEGODevice device = discoveredDevices.OrderBy(d => new RSSIComparer()).First();

                var selectedHubType = hubOptions[(int)hubType];

                if (device != null && selectedHubType != null && selectedHubType.DeviceSystemType == device.SystemType && selectedHubType.SystemDeviceNumber == device.SystemDeviceNumber)
                {
                    Debug.Log("Connect to " + device);
                    LEGODeviceManager.Instance.ConnectToDevice(device);
                }
                else
                {
                    Debug.Log("No device found - restarting");
                    StartCoroutine(ConnectToStrongestSignalWithDelay());
                }
            }
            else
            {
                Debug.Log("No devices found - restarting");
                StartCoroutine(ConnectToStrongestSignalWithDelay());
            }
        }

        void OnScanStateUpdated(DeviceManagerState state)
        {
            Debug.Log("Scan state updated " + state);
        }

        void OnDeviceStateUpdated(ILEGODevice device, DeviceState prevState, DeviceState newState)
        {
            Debug.Log("Instance_OnDeviceStateUpdated " + device + "[" + device.DeviceID + "] " + device.SystemDeviceNumber + " " + device.SystemType + " => " + newState);

            // TODO Filter devices based on hubType
            var selectedHubType = hubOptions[(int)hubType];
            if (selectedHubType == null || (selectedHubType.DeviceSystemType == device.SystemType && selectedHubType.SystemDeviceNumber == device.SystemDeviceNumber))
            {
                switch (newState)
                {
                    case DeviceState.DisconnectedAdvertising:
                        Debug.Log("device appeared");
                        if (OnDeviceAppeared != null) OnDeviceAppeared(device);
                        break;
                    case DeviceState.DisconnectedNotAdvertising:
                        Debug.Log("device disconnected");
                        if (OnDeviceDisconnected != null) OnDeviceDisconnected(device);
                        if (OnDeviceDisappeared != null) OnDeviceDisappeared(device);
                        break;
                    case DeviceState.Interrogating:
                        Debug.Log("device interrogating");
                        if (OnDeviceInterrogating != null) OnDeviceInterrogating(device);
                        if (OnDeviceDisappeared != null) OnDeviceDisappeared(device);
                        break;
                    case DeviceState.InterrogationFinished:
                        Debug.Log("device intetrogation finished");
                        if (OnDeviceInitialized != null) OnDeviceInitialized(device);
                        if (isAutoConnecting)
                        {
                            StopAllCoroutines();
                            StopScanning();
                            isAutoConnecting = false;
                        }
                        break;
                }
            }
            else
            {
                Debug.LogFormat("Not valid device {0} => {1}, {2} => {3}", selectedHubType.DeviceSystemType, device.SystemType, selectedHubType.SystemDeviceNumber, device.SystemDeviceNumber);
            }
        }

        public class RSSIComparer : Comparer<ILEGODevice>
        {
            public override int Compare(ILEGODevice x, ILEGODevice y)
            {
                return x.RSSIValue.CompareTo(y.RSSIValue);
            }
        }
    }
}