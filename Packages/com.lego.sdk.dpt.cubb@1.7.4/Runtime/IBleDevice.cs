
using System;
using CoreUnityBleBridge.Model;

namespace CoreUnityBleBridge
{
    public interface IBleDevice
    {
        #region Device-level events
        event Action Disappeared;
        #endregion

        #region Properties
        event Action VisibilityStateChanged; 
        event Action ConnectionStateChanged;
        event Action NameChanged;
        event Action ServiceGuidChanged;
        event Action ManufacturerDataChanged; 
        event Action RssiChanged;

        string ID { get; }
        DeviceVisibilityState Visibility { get; }
        DeviceConnectionState Connectivity { get; }
        string Name { get; }
        Guid? ServiceGuid { get; }
        byte[] ManufacturerData { get; }
        int RSSI { get; }
        int MtuSize { get; }
        #endregion

        #region Connection and data transfer
        event Action<PacketReceivedEventArgs> PacketReceived;
        event Action<PacketTransmittedEventArgs> PacketTransmitted;
        event Action<PacketDroppedEventArgs> PacketDropped;
        event Action<WriteMTUSizeEventArgs> WriteMTUSize;
        event Action<MtuSizeChangedEventArgs> MTUSizeChanged; 
        
        
        void Connect(SendStrategy? sendStrategy = null);
        void Disconnect();
        void WillShutDown();
        void SetNoAckParameters(int packetCount, int windowLengthMs);

        void GetWriteMTUSize(string service, string gattChar);

        void RequestMTUSize(int mtuSize);
        
        void SendPacket(string service, string gattChar, byte[] data, int group=0, SendFlags sendFlags=SendFlags.None, int packetID=-1);
        void SendPacketNotifyOnDataTransmitted(string service, string gattChar, byte[] data, int SeqNr, bool softAck = false);
        #endregion
    }

    public enum SendStrategy {
        NoAck, // Write Without Response always
        SoftAck, // Write Without Response when possible without dropping packets
        HardAck, // Write With Response always
    }
    public static class SendStrategyExtensions {
        public static string ToString(this SendStrategy strategy) {
            switch (strategy) {
            case SendStrategy.NoAck:
            case SendStrategy.SoftAck:
            case SendStrategy.HardAck:
            default: return "";
            }
        }
    }

}