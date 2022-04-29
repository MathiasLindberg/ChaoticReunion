using System;
using System.Linq;
using LEGO.Logger;
using UnityEngine;

namespace CoreUnityBleBridge.Model
{
    public sealed class BleDevice : IBleDevice
    {
        private static readonly ILog Log = LogManager.GetLogger<BleDevice>();

        private readonly IUnityToNative unityToNative;

        internal BleDevice(string id, IUnityToNative unityToNative)
        {
            ID = id;
            this.unityToNative = unityToNative;
        }

        #region Device-level events
        public event Action Disappeared = delegate { };

        internal void OnDisappeared()
        {
            Disappeared();
        }
        #endregion

        #region Properties
        public event Action VisibilityStateChanged = delegate { };
        public event Action ConnectionStateChanged = delegate { };
        public event Action NameChanged = delegate { };
        public event Action ServiceGuidChanged = delegate { };
        public event Action ManufacturerDataChanged = delegate { };
        public event Action RssiChanged = delegate { };

        public string ID { get; private set; }
        public DeviceVisibilityState Visibility { get; private set; }
        public DeviceConnectionState Connectivity { get; private set; }
        public string Name { get; private set; }
        public Guid? ServiceGuid { get; private set; }
        public byte[] ManufacturerData { get; private set; }
        public int RSSI { get; private set; }
        public int MtuSize { get; private set; }

        #region Remembering which properties have changed
        /// <summary>
        /// Device property. Used for indexing bits in a bitmask.
        /// </summary>
        private enum DeviceProperty
        {
            Visibility, Name, ServiceGuid, ManufacturerData, Rssi
        }
        private int changedProperties = 0;

        private void MarkAsChanged(DeviceProperty property) 
        {
            changedProperties |= (1 << (int)property);
        }
        
        private bool CheckAndClear(DeviceProperty property) 
        {
            var bitmask = (1 << (int)property);
            bool changed = (changedProperties & bitmask) != 0;
            changedProperties &= ~bitmask;
            return changed;
        }
        #endregion

        internal void OnPropertiesChanged(DeviceStateChangedEventArgs args) 
        {
            OnManufacturerDataChanged(args.ManufaturerData);
            OnServiceGuidChanged(args.ServiceGuid);
            OnVisibilityStateChanged(args.DeviceVisibilityState);
            OnRssiChanged(args.Rssi);
            OnNameChanged(args.DeviceName);
        }

        internal void OnNameChanged(string value)
        {
            if (Name == value) 
                return;
            
            Name = value;
            MarkAsChanged(DeviceProperty.Name);
        }

        internal void OnVisibilityStateChanged(DeviceVisibilityState value)
        {
            if (Visibility == value) 
                return;
            
            Visibility = value;
            MarkAsChanged(DeviceProperty.Visibility);
        }
        
        internal void OnRssiChanged(int value)
        {
            if (RSSI == value) 
                return;
            
            RSSI = value;
            MarkAsChanged(DeviceProperty.Rssi);
        }
        
        internal void OnServiceGuidChanged(Guid? value)
        {
            if (ServiceGuid == value) 
                return;
            
            ServiceGuid = value;
            MarkAsChanged(DeviceProperty.ServiceGuid);
        }

        internal void OnManufacturerDataChanged(byte[] value)
        {
            if (ByteStringsAreEqual(ManufacturerData, value)) 
                return;
            
            ManufacturerData = value;
            MarkAsChanged(DeviceProperty.ManufacturerData);
        }
        
        private static bool ByteStringsAreEqual(byte[] data1, byte[] data2)
        {
            var isEqual = (data1 == null || data2 == null)
                ? data1 == data2
                : data1.SequenceEqual(data2);
            
            // INVARIANT: data1 != null && data2 != null
            return isEqual;
        }

        internal void OnConnectionStateChanged(DeviceConnectionState value)
        {
            Connectivity = value;
            ConnectionStateChanged();
        }

        internal void FirePendingEvents() 
        {
            // Invoke all relevant change callbacks:
            try {
                if (CheckAndClear(DeviceProperty.Visibility)) VisibilityStateChanged();
                if (CheckAndClear(DeviceProperty.Name)) NameChanged();
                if (CheckAndClear(DeviceProperty.ServiceGuid)) ServiceGuidChanged();
                if (CheckAndClear(DeviceProperty.ManufacturerData)) ManufacturerDataChanged();
                if (CheckAndClear(DeviceProperty.Rssi)) RssiChanged();
            } catch (Exception e) {
                Log.Error("Exception occurred during callback: " +  e.Data, e);
            }
        }
        #endregion

        #region Connection and data transfer
        public event Action<PacketReceivedEventArgs> PacketReceived = delegate {};
        public event Action<PacketTransmittedEventArgs> PacketTransmitted = delegate {};
        public event Action<PacketTransmittedEventArgs> PacketSent = delegate {};
        public event Action<PacketDroppedEventArgs> PacketDropped = delegate {};
        public event Action<WriteMTUSizeEventArgs> WriteMTUSize = delegate {};
        public event Action<MtuSizeChangedEventArgs> MTUSizeChanged = delegate {}; 

        public void Connect(SendStrategy? sendStrategy)
        {
            unityToNative.ConnectToDevice(ID, sendStrategy);
        }

        public void Disconnect()
        {
            unityToNative.DisconnectFromDevice(ID);
        }

        public void WillShutDown()
        {
            //Devices shutdown acknowledge receive, set device to disconnect so we dont have to wait for timeout before disconnect
            OnConnectionStateChanged(DeviceConnectionState.Disconnected);
        }

        public void SetNoAckParameters(int packetCount, int windowLengthMs)
        {
            Log.Debug("SetNoAckParameters(" + packetCount + ", " + windowLengthMs + ")");
            unityToNative.SetNoAckParameters(ID, packetCount, windowLengthMs);
        }
        
        public void GetWriteMTUSize(string service, string gattChar)
        {
            Log.Debug("GetWriteMTUSize()");
            unityToNative.GetWriteMTUSize(ID,service, gattChar);
        }

        public void RequestMTUSize(int mtuSize)
        {
            Log.Debug("RequestMTUSize(" + mtuSize + ")");
            unityToNative.RequestMtuSize(ID, mtuSize);
        }

        public void SendPacket(string service, string gattChar, byte[] data, int group=0, SendFlags sendFlags=SendFlags.None, int packetID=-1)
        {
            unityToNative.SendPacket(ID, service, gattChar, data, group, sendFlags, packetID);
        }

        public void SendPacketNotifyOnDataTransmitted(string service, string gattChar, byte[] data, int SeqNr, bool softAck=false)
        {
            unityToNative.SendPacketNotifyOnDataTransmitted(ID, service, gattChar, data, SeqNr, softAck);
        }

        internal void OnPacketReceived(PacketReceivedEventArgs args)
        {
            PacketReceived(args);
        }

        internal void OnPacketTransmitted(PacketTransmittedEventArgs args)
        {
            PacketTransmitted(args);
        }
        internal void OnPacketSent(PacketTransmittedEventArgs args)
        {
            PacketSent(args);
        }

        internal void OnPacketDropped(PacketDroppedEventArgs args)
        {
            PacketDropped(args);
        }
        
        internal void OnWriteMTUSize(WriteMTUSizeEventArgs args)
        {
            Debug.Log("BleDevice:OnWriteMTUSize:" + args);
            WriteMTUSize(args);
        }

        internal void OnMTUSizeChanged(MtuSizeChangedEventArgs args)
        {
            Log.Debug($"BleDevice {args.DeviceID} MTUSizeChanged: {args.MtuSize}");
            MtuSize = args.MtuSize;
            MTUSizeChanged(args);
        }
        #endregion
    }
}