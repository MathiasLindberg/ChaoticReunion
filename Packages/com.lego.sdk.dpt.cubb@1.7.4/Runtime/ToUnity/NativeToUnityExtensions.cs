using System;
using CoreUnityBleBridge.Model;

namespace CoreUnityBleBridge.ToUnity
{
    /// <summary>
    /// Extension methods adapting raw input methods to object wrapped event based methods.
    /// </summary>
    internal static class NativeToUnityExtensions
    {
        public static void OnError(this NativeToUnity nativeToUnity, string errorMessage)
        {
            var args = new ErrorEventArgs
            {
                Message = errorMessage
            };
            nativeToUnity.OnError(args);
        }

        public static void OnAdapterScanStateChanged(this NativeToUnity nativeToUnity, AdapterScanState newScanState)
        {
            var args = new AdapterScanStateChangedEventArgs
            {
                AdapterScanState = newScanState
            };
            nativeToUnity.OnAdapterScanStateChanged(args);
        }

        public static void OnDeviceDisappeared(this NativeToUnity nativeToUnity, string deviceID)
        {
            var args = new DeviceDisappearedEventArgs
            {
                DeviceID = deviceID
            };
            nativeToUnity.OnDeviceDisappeared(args);
        }

        public static void OnDeviceStateChanged(this NativeToUnity nativeToUnity, string deviceID,
            DeviceVisibilityState state, string deviceName, Guid? serviceGuid, int rssi, byte[] manufacturerData)
        {
            var args = new DeviceStateChangedEventArgs
            {
                DeviceID = deviceID,
                DeviceVisibilityState = state,
                DeviceName = deviceName,
                ServiceGuid = serviceGuid,
                Rssi = rssi,
                ManufaturerData = manufacturerData
            };
            nativeToUnity.OnDeviceStateChanged(args);
        }

        public static void OnDeviceConnectionStateChanged(this NativeToUnity nativeToUnity, string deviceID,
            DeviceConnectionState state, string errorMsg)
        {
            var args = new DeviceConnectionStateChangedEventArgs
            {
                DeviceID = deviceID,
                DeviceConnectionState = state,
                ErrorMessage = errorMsg
            };
            nativeToUnity.OnDeviceConnectionStateChanged(args);
        }

        public static void OnPacketReceived(this NativeToUnity nativeToUnity, string deviceID, string service,
            string gattChar, byte[] data)
        {
            var args = new PacketReceivedEventArgs
            {
                DeviceID = deviceID,
                Service = service,
                GattCharacteristic = gattChar,
                Data = data
            };
            nativeToUnity.OnPacketReceived(args);
        }

        public static void OnPacketTransmitted(this NativeToUnity nativeToUnity, string deviceID,
            string serviceInterface, string gattCharacteristic, int seqNr)
        {
            var args = new PacketTransmittedEventArgs
            {
                DeviceID = deviceID,
                Service = serviceInterface,
                GattCharacteristic = gattCharacteristic,
                SequenceNumber = seqNr
            };
            nativeToUnity.OnPacketTransmitted(args);
        }
        
        public static void OnPacketSent(this NativeToUnity nativeToUnity, string deviceID,
            string serviceInterface, string gattCharacteristic)
        {
            var args = new PacketTransmittedEventArgs
            {
                DeviceID = deviceID,
                Service = serviceInterface,
                GattCharacteristic = gattCharacteristic
            };
            nativeToUnity.OnPacketSent(args);
        }
    }
}