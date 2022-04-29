namespace CoreUnityBleBridge.Model
{
    /// <summary>
    /// Interface for the "downward"/Unity-to-native direction.
    /// All operations here are synchronous, but non-blocking.
    /// </summary>
    internal interface IUnityToNative
    {
        /// <summary>
        /// Configure and initialize the bridge.
        /// </summary>
        /// <param name="services">Gatt services in as json string</param>
        void Initialize(string services);


        /// <summary>
        /// Change whether to search for BLE devices.
        /// </summary>
        /// <param name="enabled">If set to <c>true</c> enabled.</param>
        void SetScanState(bool enabled);

        /// <summary>
        /// Connect to a known device.
        /// </summary>
        /// <param name="deviceID">Device I.</param>
        void ConnectToDevice(string deviceID, SendStrategy? sendStrategy);
        /// <summary>
        /// Disconnect from a device (which we're already connected to).
        /// </summary>
        /// <param name="deviceID">Device I.</param>
        void DisconnectFromDevice(string deviceID);

        
        /// <summary>
        /// Set the no ack parameters (only affects send strategy no ack - must be connected).
        /// </summary>
        /// <param name="deviceID">Device ID</param>
        /// <param name="packetCount">packet count setting</param>
        /// <param name="windowLengthMs">window length in milliseconds</param>
        void SetNoAckParameters(string deviceID, int packetCount, int windowLengthMs);
        
        /// <summary>
        /// Request the current write MTU size - provokes a "WriteMTUSize" message from CUBB. To receive the answer
        /// message subscribe to BleDevice.OnWriteMTUSize
        /// </summary>
        /// <param name="deviceID">Device ID</param>
        /// <param name="service">The target BLE service.</param>
        /// <param name="gattChar">The target GATT chararcteristic.</param>
        void GetWriteMTUSize(string deviceID, string service, string gattChar);
        
        /// <summary>
        /// Send a packet to a given (connected!) device, through a specific BLE service and GATT characteristic, using the BLE "Write without response" mechanism.
        /// </summary>
        /// <param name="deviceID">The ID of the destination device.</param>
        /// <param name="service">The target BLE service.</param>
        /// <param name="gattChar">The target GATT chararcteristic.</param>
        /// <param name="data">The data to send.</param>
        /// <param name="group">The packet replacement group.</param>
        /// <param name="sendFlags">Flags giverning the details of the transmission, in particular the packet replacement policy.</param>
        /// <param name="packetID">An identifier which is used for feedback if the packet is replaced instead of sent. Negative means no feedback.</param>
        void SendPacket(string deviceID, string service, string gattChar, byte[] data, int group, SendFlags sendFlags, int packetID);
        /// <summary>
        /// Send a packet to a given (connected!) device, through a specific BLE service and GATT characteristic, using the BLE "Write with response" mechanism.
        /// </summary>
        /// <param name="deviceID">The ID of the destination device.</param>
        /// <param name="service">The target BLE service.</param>
        /// <param name="gattChar">The target GATT chararcteristic.</param>
        /// <param name="data">The data to send.</param>
        /// <param name="seqNr">A sequence number. A PacketTransmitted with the sequence number will occur once this packet has been transmitted and ack'ed by the device.</param>
        /// <param name="softAck">Whether soft acknowledgement (more lenient, about 4x faster) should be used.</param>
        void SendPacketNotifyOnDataTransmitted(string deviceID, string service, string gattChar, byte[] data, int seqNr, bool softAck);

        /// <summary>
        /// Sets the log level in the native implementation
        /// </summary>
        /// <param name="logLevel"></param>
        void SetLogLevel(int logLevel);

        /// <summary>
        /// Request a GATT Mtu size 
        /// </summary>
        /// <param name="deviceID">The ID of the device to request on</param>
        /// <param name="mtuSize">Requested MTU size</param>
        void RequestMtuSize(string deviceID, int mtuSize);
    }
}