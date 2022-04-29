using System;
using CoreUnityBleBridge.Model;
using LEGO.Logger;
using UnityEngine;

namespace CoreUnityBleBridge.ToUnity
{
    /// <summary>
    /// Utility class to trigger native event calls.
    /// This class is publicly exposed in order to simulate native callbacks without an actual connection. 
    /// </summary>
    public sealed class NativeToUnity : INativeToUnity
    {
        public event Action<ErrorEventArgs> Error = delegate {};
        public event Action<string, LogLevel> Log = delegate {};
        public event Action<AdapterScanStateChangedEventArgs> AdapterScanStateChanged = delegate {};
        public event Action<DeviceDisappearedEventArgs> DeviceDisappeared = delegate {};
        public event Action<DeviceStateChangedEventArgs> DeviceStateChanged = delegate {};
        public event Action<DeviceConnectionStateChangedEventArgs> DeviceConnectionStateChanged = delegate {};
        public event Action<PacketReceivedEventArgs> PacketReceived = delegate {};
        public event Action<PacketTransmittedEventArgs> PacketTransmitted = delegate {};
        public event Action<PacketDroppedEventArgs> PacketDropped = delegate {};
        public event Action<PacketTransmittedEventArgs> PacketSent = delegate {};
        public event Action<WriteMTUSizeEventArgs> WriteMTUSize = delegate {};
        public event Action<MtuSizeChangedEventArgs> MTUSizeChanged = delegate {};

        public void OnError(ErrorEventArgs e)
        {
            Error(e);
        }

        public void OnLog(string message, LogLevel logLevel)
        {
            Log(message, logLevel);
        }
        
        
        public void OnAdapterScanStateChanged(AdapterScanStateChangedEventArgs e)
        {
            AdapterScanStateChanged(e);
        }

        public void OnDeviceDisappeared(DeviceDisappearedEventArgs e)
        {
            DeviceDisappeared(e);
        }

        public void OnDeviceStateChanged(DeviceStateChangedEventArgs e)
        {
            DeviceStateChanged(e);
        }

        public void OnDeviceConnectionStateChanged(DeviceConnectionStateChangedEventArgs e)
        {
            DeviceConnectionStateChanged(e);
        }

        public void OnPacketReceived(PacketReceivedEventArgs e)
        {
            PacketReceived(e);
        }

        public void OnPacketTransmitted(PacketTransmittedEventArgs e)
        {
            PacketTransmitted(e);
        }

        public void OnPacketDropped(PacketDroppedEventArgs e)
        {
            PacketDropped(e);
        }

        public void OnPacketSent(PacketTransmittedEventArgs e)
        {
            PacketSent(e);
        }
        
        public void OnWriteMTUSize(WriteMTUSizeEventArgs e)
        {
            Debug.Log("NativeToUnity:OnWriteMTUSize: WriteMTUSize rxd:" + e.WriteMTUSize);
            WriteMTUSize(e);
        }

        public void OnMtuSizeChanged(MtuSizeChangedEventArgs e)
        {
            MTUSizeChanged(e);
        }
    }
}