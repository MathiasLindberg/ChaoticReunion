using System;
using System.Collections;
using System.Collections.Generic;
using CoreUnityBleBridge.Model;
using CoreUnityBleBridge.ToUnity;
using CoreUnityBleBridge.Utilities;
using LEGO.Logger;
using UnityEngine;

namespace CoreUnityBleBridge.Verification
{
    public class BridgeChecker: IUnityToNative
    {
        private static readonly ILog Logger = LogManager.GetLogger<BridgeChecker>();

        // Exposed bridge:
        internal readonly NativeToUnity NativeToUnity;
        internal readonly IUnityToNative UnityToNative;

        // Wrapped bridge:
#pragma warning disable 0414
        private readonly INativeToUnity realN2U;
#pragma warning restore 0414
        
        private readonly IUnityToNative realU2N;

        internal BleBridge BleBridge { get; set; }

        private readonly List<Expectation> pendingExpectations = new List<Expectation>();

        internal BridgeChecker(INativeToUnity realN2U, IUnityToNative realU2N)
        {
            this.realN2U = realN2U;
            this.realU2N = realU2N;

            NativeToUnity = new NativeToUnity();
            { // Wire up to this:
                realN2U.AdapterScanStateChanged += this.OnAdapterScanStateChanged;
                realN2U.DeviceConnectionStateChanged += this.OnDeviceConnectionStateChanged;
                realN2U.DeviceDisappeared += this.OnDeviceDisappeared;
                realN2U.DeviceStateChanged += this.OnDeviceStateChanged;
                realN2U.Error += this.OnError;
                realN2U.PacketSent += this.OnPacketSent;
                realN2U.PacketReceived += this.OnPacketReceived;
                realN2U.PacketTransmitted += this.OnPacketTransmitted;
                realN2U.PacketDropped += this.OnPacketDropped;
                realN2U.MTUSizeChanged += this.OnMtuSizeChanged;
            }
            { // Wire up to exposed bridge:
                realN2U.AdapterScanStateChanged += NativeToUnity.OnAdapterScanStateChanged;
                realN2U.DeviceConnectionStateChanged += NativeToUnity.OnDeviceConnectionStateChanged;
                realN2U.DeviceDisappeared += NativeToUnity.OnDeviceDisappeared;
                realN2U.DeviceStateChanged += NativeToUnity.OnDeviceStateChanged;
                realN2U.Error += NativeToUnity.OnError;
                realN2U.Log += NativeToUnity.OnLog;
                realN2U.PacketSent += NativeToUnity.OnPacketSent;
                realN2U.PacketReceived += NativeToUnity.OnPacketReceived;
                realN2U.PacketTransmitted += NativeToUnity.OnPacketTransmitted;
                realN2U.PacketDropped += NativeToUnity.OnPacketDropped;
                realN2U.MTUSizeChanged += NativeToUnity.OnMtuSizeChanged;
            }

            UnityToNative = this;
        }



        #region State
        private bool isInitialized;
        private readonly Dictionary<string, DeviceState> knownDevices = new Dictionary<string, DeviceState>();
        private AdapterScanState? adapterState = null;
        #endregion

        #region Downward events
        void IUnityToNative.Initialize(string services)
        {
            if (isInitialized)
                ReportError("'Initialize' must only be called once!");
            
            isInitialized = true;

            AddExpectation(new Expectation("Initialize->AdapterScanStateChanged",
                ev => ev is AdapterScanStateChangedEventArgs,
                5.0f));

            realU2N.Initialize(services);
        }

        void IUnityToNative.SetScanState(bool enabled)
        {
            CommonU2NPreCheck();
            if (!enabled && BleBridge.AdapterScanState == AdapterScanState.Scanning)
                AddExpectation(new Expectation("stop scanning -> scanState != Scanning",
                    ev0 => {
                        var ev = ev0 as AdapterScanStateChangedEventArgs;
                        return (ev != null) && ev.AdapterScanState != AdapterScanState.Scanning;
                    }, 5.0f));

            if (enabled && BleBridge.AdapterScanState != AdapterScanState.Scanning) {
                switch (BleBridge.AdapterScanState) {
                case AdapterScanState.BluetoothUnavailable:
                case AdapterScanState.BluetoothDisabled:
                case AdapterScanState.TurningOnScanning:
                    break; // OK
                default:
                    AddExpectation(new Expectation("start scanning -> scanState = TurningOnScanning",
                        ev0 => {
                            var ev = ev0 as AdapterScanStateChangedEventArgs;
                            return (ev != null) && (ev.AdapterScanState == AdapterScanState.TurningOnScanning ||
                            ev.AdapterScanState == AdapterScanState.BluetoothDisabled ||
                            ev.AdapterScanState == AdapterScanState.BluetoothUnavailable);
                        }, 1.0f));
                    break;
                }
            }

            realU2N.SetScanState(enabled);
        }

        void IUnityToNative.ConnectToDevice(string deviceID, SendStrategy? sendStrategy)
        {
            CommonU2NPreCheck();
            realU2N.ConnectToDevice(deviceID, sendStrategy);
        }

        void IUnityToNative.DisconnectFromDevice(string deviceID)
        {
            CommonU2NPreCheck();
            realU2N.DisconnectFromDevice(deviceID);
        }

        public void SetNoAckParameters(string deviceID, int packetCount, int windowLengthMs)
        {
            CommonU2NPreCheck();
            realU2N.SetNoAckParameters(deviceID, packetCount, windowLengthMs);
        }
        
        public void GetWriteMTUSize(string deviceID, string service, string gattChar)
        {
            CommonU2NPreCheck();
            realU2N.GetWriteMTUSize(deviceID, service, gattChar);
        }
        
        void IUnityToNative.SendPacket(string deviceID, string service, string gattChar, byte[] data, int group, SendFlags sendFlags, int packetID)
        {
            CommonU2NPreCheck();
            realU2N.SendPacket(deviceID, service, gattChar, data, group, sendFlags, packetID);
        }

        void IUnityToNative.SendPacketNotifyOnDataTransmitted(string deviceID, string service, string gattChar, byte[] data, int seqNr, bool softAck)
        {
            CommonU2NPreCheck();
            realU2N.SendPacketNotifyOnDataTransmitted(deviceID, service, gattChar, data, seqNr, softAck);
        }

        public void SetLogLevel(int logLevel)
        {
            CommonU2NPreCheck();
            realU2N.SetLogLevel(logLevel);
        }

        public void RequestMtuSize(string deviceID, int mtuSize)
        {
            CommonU2NPreCheck();
            realU2N.RequestMtuSize(deviceID, mtuSize);
        }

        private void CommonU2NPreCheck()
        {
            if (!isInitialized) {
                ReportError("Bridge used before Initialize");
            }
        }

        #endregion

        #region Upward events

        private void OnError(ErrorEventArgs e)
        {
            UpdateExpectations(e);
            if (e.Message == null)
                ReportError("Invalid null field in Error event: " + e);
        }

        private void OnAdapterScanStateChanged(AdapterScanStateChangedEventArgs e)
        {
            UpdateExpectations(e);

            if (e.AdapterScanState == adapterState)
            {
                ReportError("Duplicate adapter scan state: " + e.AdapterScanState);
            }

            if (e.AdapterScanState == AdapterScanState.Scanning && adapterState != AdapterScanState.TurningOnScanning)
            {
                ReportError("Invalid AdapterScanState transition: " + adapterState + "->" + e.AdapterScanState);
            }

            if (adapterState == null) { // Initial state
                switch (e.AdapterScanState) {
                case AdapterScanState.BluetoothUnavailable:
                case AdapterScanState.BluetoothDisabled:
                case AdapterScanState.NotScanning:
                    break; // OK
                
                default:
                    ReportError("Invalid initial scan state: " + e.AdapterScanState);
                    break;
                }
            }

            adapterState = e.AdapterScanState;
        }

        private void OnDeviceDisappeared(DeviceDisappearedEventArgs e)
        {
            UpdateExpectations(e);
            var deviceID = e.DeviceID;
            DeviceState device;
            if (!knownDevices.TryGetValue(deviceID, out device)) {
                ReportError("Disappearance of unknown device: " + deviceID);
                return;
            }
                
            if (device.connectionState != DeviceConnectionState.Disconnected)
                ReportError("Device with ID " + deviceID + " disappeared while in connection state " + device.connectionState);

            knownDevices.Remove(deviceID);
        }

        private void OnDeviceStateChanged(DeviceStateChangedEventArgs e)
        {
            UpdateExpectations(e);

            var deviceID = e.DeviceID;
            DeviceState device;
            if (!knownDevices.TryGetValue(deviceID, out device)) {
                device = new DeviceState(deviceID);
                knownDevices.Add(deviceID, device);
            }

            bool changed =
                (e.DeviceName != device.name ||
                e.DeviceVisibilityState != device.visibilityState ||
                e.Rssi != device.rssi ||
                e.ServiceGuid != device.serviceGuid ||
                !ByteStringsAreEqual(e.ManufaturerData, device.manufacturerData));
            
            if (!changed) {
                ReportError("Duplicate DeviceStateChanged event on device " + deviceID + " containing no changes: " + e);
            }

            device.name = e.DeviceName;
            device.visibilityState = e.DeviceVisibilityState;
            device.rssi = e.Rssi;
            device.serviceGuid = e.ServiceGuid;
            device.manufacturerData = e.ManufaturerData;
        }

        private bool ByteStringsAreEqual(byte[] data1, byte[] data2)
        {
            if (data1 == data2)
                return true;
            
            if ((data1 == null) != (data2 == null))
                return false;
            
            // INVARIANT: data1 != null && data2 != null
            return Array.Equals(data1, data2);
        }

        private void OnDeviceConnectionStateChanged(DeviceConnectionStateChangedEventArgs e)
        {
            UpdateExpectations(e);

            var deviceID = e.DeviceID;
            DeviceState device;
            if (!knownDevices.TryGetValue(deviceID, out device)) {
                ReportError("DeviceConnectionStateChanged of unknown device: " + deviceID);
                return;
            }

            if (e.DeviceConnectionState == device.connectionState && e.ErrorMessage == null) {
                ReportError("Duplicate connection state of device " + deviceID + ": " + e.DeviceConnectionState);
            }

            device.connectionState = e.DeviceConnectionState;
        }

        private void OnPacketSent(PacketTransmittedEventArgs e)
        {
            UpdateExpectations(e);

            var deviceID = e.DeviceID;
            DeviceState device;
            if (!knownDevices.TryGetValue(deviceID, out device)) {
                ReportError("PacketSent event on unknown device: " + deviceID + ": " + e);
                return;
            }

            if (e.Service == null || e.GattCharacteristic == null)
                ReportError("Invalid null field in PacketSent event: " + e);
        }
        
        private void OnPacketReceived(PacketReceivedEventArgs e)
        {
            UpdateExpectations(e);

            var deviceID = e.DeviceID;
            DeviceState device;
            if (!knownDevices.TryGetValue(deviceID, out device)) {
                ReportError("PacketReceived event on unknown device: " + deviceID + ": " + e);
                return;
            }

            if (e.Data == null || e.Service == null || e.GattCharacteristic == null)
                ReportError("Invalid null field in PacketReceived event: " + e);
        }

        private void OnPacketTransmitted(PacketTransmittedEventArgs e)
        {
            UpdateExpectations(e);

            var deviceID = e.DeviceID;
            DeviceState device;
            if (!knownDevices.TryGetValue(deviceID, out device)) {
                ReportError("PacketTransmitted event on unknown device: " + deviceID + ": " + e);
                return;
            }

            if (e.Service == null || e.GattCharacteristic == null)
                ReportError("Invalid null field in PacketTransmitted event: " + e);

            /*
            if (e.SequenceNumber <= 0)
                ReportError("Invalid sequence number field in PacketTransmitted event: " + e);
            */
        }

        private void OnPacketDropped(PacketDroppedEventArgs e)
        {
            UpdateExpectations(e);

            var deviceID = e.DeviceID;
            DeviceState device;
            if (!knownDevices.TryGetValue(deviceID, out device)) {
                ReportError("PacketDropped event on unknown device: " + deviceID + ": " + e);
                return;
            }

            if (e.Service == null || e.GattCharacteristic == null)
                ReportError("Invalid null field in PacketDropped event: " + e);

            /*
            if (e.SequenceNumber <= 0)
                ReportError("Invalid sequence number field in PacketDropped event: " + e);
            */
        }
        
        private void OnMtuSizeChanged(MtuSizeChangedEventArgs args)
        {
            if (!knownDevices.TryGetValue(args.DeviceID, out var device))
            {
                ReportError("MtuSizeChanged event on unknown device: " + args);
            }
        }

        #endregion

        private void AddExpectation(Expectation expectation)
        {
            pendingExpectations.Add(expectation);
        }

        private void UpdateExpectations(EventArgs ev)
        {
            //Logger.Info("Checker: UpdateExpectations on "+ev);
            pendingExpectations.RemoveAll(ex => ex.IsMetBy(ev));
        }

        internal static void ReportError(string message)
        {
            Logger.Error(message);
            UnityEngine.Debug.LogError(message); //TODO: Remove once logging works.
        }

        private class DeviceState
        {
            internal readonly string id;
            internal string name;
            internal DeviceConnectionState connectionState;
            internal DeviceVisibilityState visibilityState;
            internal int rssi;
            internal Guid? serviceGuid;
            internal byte[] manufacturerData;

            public DeviceState(string id)
            {
                this.id = id;
            }
                
        }
    }

    internal class Expectation
    {
        private static readonly ILog Logger = LogManager.GetLogger<Expectation>();
        
        private readonly string description;
        private readonly Func<EventArgs, bool> predicate;
        private readonly float timeout;
        // In seconds
        private readonly Coroutine timeOutRoutine;

        public Expectation(string description, Func<EventArgs, bool> predicate, float timeout)
        {
            this.description = description;
            this.predicate = predicate;
            this.timeout = timeout;
            
            timeOutRoutine = BleCoroutineRunner.GetDefault()
                                               .StartCoroutine(ExpectationTimeout());
        }

        private IEnumerator ExpectationTimeout()
        {
            Logger.Debug("Expectation " + description + " timer started (" + timeout + "s)");
            yield return new WaitForSeconds(timeout);
            BridgeChecker.ReportError("Expectation timeout: " + description);
        }

        public bool IsMetBy(EventArgs ev)
        {
            try {
                var expectationIsMet = predicate(ev);
                //Logger.Debug("expectationIsMet = " + expectationIsMet + " on " + ev);
                if (expectationIsMet)
                {
                    BleCoroutineRunner.GetDefault()
                                      .StopCoroutine(timeOutRoutine);

                    Logger.Debug("Expectation " + description + " timer stopped");
                }

                return expectationIsMet;
            } catch (Exception e) {
                Logger.Warn("Expectation predicate threw. description=" + description, e);
                return false;
            }
        }
    }

}

