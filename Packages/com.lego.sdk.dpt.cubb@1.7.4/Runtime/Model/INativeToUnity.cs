using System;
using LEGO.Logger;

namespace CoreUnityBleBridge.Model
{
    /// <summary>
    /// Interface relaying native layer messages to the unity client abiding by the .net guide on
    /// handling and raising events: <seealso>
    ///         <cref>https://docs.microsoft.com/en-us/dotnet/standard/events/</cref>
    ///     </seealso>
    /// </summary>
    internal interface INativeToUnity
    {
        event Action<ErrorEventArgs> Error;
        event Action<string, LogLevel> Log;
        event Action<AdapterScanStateChangedEventArgs> AdapterScanStateChanged;
        event Action<DeviceDisappearedEventArgs> DeviceDisappeared;
        event Action<DeviceStateChangedEventArgs> DeviceStateChanged;
        event Action<DeviceConnectionStateChangedEventArgs> DeviceConnectionStateChanged;
        event Action<PacketTransmittedEventArgs> PacketSent;
        event Action<PacketReceivedEventArgs> PacketReceived;
        event Action<PacketTransmittedEventArgs> PacketTransmitted;
        event Action<PacketDroppedEventArgs> PacketDropped;
        event Action<WriteMTUSizeEventArgs> WriteMTUSize;
        event Action<MtuSizeChangedEventArgs> MTUSizeChanged;
    }
    
    
    #region EventArgs

    public sealed class ErrorEventArgs : EventArgs
    {
        public string Message;

        public override string ToString()
        {
            return string.Format("Message: {0}", Message);
        }
    }

    public sealed class AdapterScanStateChangedEventArgs : EventArgs
    {
        public AdapterScanState AdapterScanState;

        public override string ToString()
        {
            return string.Format("AdapterScanState: {0}", AdapterScanState);
        }
    }

    public sealed class DeviceDisappearedEventArgs : EventArgs
    {
        public string DeviceID;

        public override string ToString()
        {
            return string.Format("DeviceID: {0}", DeviceID);
        }
    }

    public sealed class DeviceStateChangedEventArgs : EventArgs
    {
        public string DeviceID;
        public DeviceVisibilityState DeviceVisibilityState;
        public string DeviceName;
        public Guid? ServiceGuid;
        public int Rssi;
        public byte[] ManufaturerData;

        public override string ToString()
        {
            return string.Format("DeviceID: {0}, DeviceVisibilityState: {1}, DeviceName: {2}, ServiceGuid: {3}, Rssi: {4}, ManufaturerData: {5}", DeviceID, DeviceVisibilityState, DeviceName, ServiceGuid, Rssi, BitConverter.ToString(ManufaturerData));
        }
    }

    public sealed class DeviceConnectionStateChangedEventArgs : EventArgs
    {
        public string DeviceID;
        public DeviceConnectionState DeviceConnectionState;
        public string ErrorMessage;

        public override string ToString()
        {
            return string.Format("DeviceID: {0}, DeviceConnectionState: {1}, ErrorMessage: {2}", DeviceID, DeviceConnectionState, ErrorMessage);
        }
    }

    public sealed class MtuSizeChangedEventArgs : EventArgs
    {
        public string DeviceID;
        public int MtuSize;
        
        public override string ToString()
        {
            return string.Format($"DeviceID: {DeviceID}, MtuSize: {MtuSize}");
        }
    }

    public class PacketEventArgs : EventArgs
    {
        public string DeviceID;
        public string Service;
        public string GattCharacteristic;

        public bool isMatchForV3(string deviceID)
        {
            return (DeviceID == deviceID &&
                    Service == Constants.LDSDK.V3_SERVICE &&
                    GattCharacteristic == Constants.LDSDK.V3_CHARACTERISTIC);
        }
    }

    public sealed class PacketReceivedEventArgs : PacketEventArgs
    {
        public byte[] Data;

        public override string ToString()
        {
            return string.Format("DeviceID: {0}, Service: {1}, GattCharacteristic: {2}, Data: {3}", DeviceID, Service, GattCharacteristic,
                Data==null ? "null" : BitConverter.ToString(Data));
        }
    }

    public sealed class PacketTransmittedEventArgs : PacketEventArgs
    {
        public int SequenceNumber;

        public override string ToString()
        {
            return string.Format("DeviceID: {0}, ServiceInterface: {1}, GattCharacteristic: {2}, SequenceNumber: {3}", DeviceID, Service, GattCharacteristic, SequenceNumber);
        }
    }

    public sealed class PacketDroppedEventArgs : PacketEventArgs
    {
        public int PacketID;

        public override string ToString()
        {
            return string.Format("DeviceID: {0}, Service: {1}, GattCharacteristic: {2}, PacketID: {3}", DeviceID, Service, GattCharacteristic, PacketID);
        }
    }
    
    public sealed class WriteMTUSizeEventArgs : PacketEventArgs
    {
        public int WriteMTUSize;

        public override string ToString()
        {
            return string.Format("DeviceID: {0}, Service: {1}, GattCharacteristic: {2}, PacketID: {3}", DeviceID, Service, GattCharacteristic, WriteMTUSize);
        }
    }

    #endregion
}