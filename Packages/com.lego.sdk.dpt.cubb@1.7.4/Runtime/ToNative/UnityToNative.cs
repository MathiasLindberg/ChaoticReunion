using System.Diagnostics.CodeAnalysis;
using CoreUnityBleBridge.Model;
using CoreUnityBleBridge.ToNative.Bridge;

using LEGO.Logger;
using UnityEngine;

namespace CoreUnityBleBridge.ToNative
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    internal sealed class UnityToNative : IUnityToNative
    {
        private static readonly ILog Log = LogManager.GetLogger<UnityToNative>();
        
        private readonly IUnityToNativeBridge bridge;
        
        public UnityToNative(IUnityToNativeBridge bridge)
        {
            this.bridge = bridge;
        }

        public void Initialize(string services)
        {
            bridge.SendToNative("Initialize", Parameter.ID(services));
            SetLogLevel((int)LogManager.RootLevel);
        }

        public void SetScanState(bool enable)
        {
            bridge.SendToNative("SetScanState", Parameter.Bool(enable));
        }

        public void ConnectToDevice(string deviceID, SendStrategy? sendStrategy)
        {
            string connectionOptions = "";
            if (sendStrategy != null) {
                connectionOptions += "sendStrategy=" + sendStrategy + ";";
            }
            // NB: The native parts (esp. Android) understand more options: "connectionPriority" (Android), "autoConnect" (Android).
            // To support these, extend the connectionOptions string here.
            #if UNITY_ANDROID && !UNITY_EDITOR
            connectionOptions += "autoConnect=false;";
            #endif

            bridge.SendToNative("ConnectToDevice", Parameter.ID(deviceID), Parameter.String(connectionOptions));
        }

        public void SetNoAckParameters(string deviceID, int packetCount, int windowLengthMs )
        {
            // the parameters are ints and make no sense to be zero or negative - the maxmimums are to limit extreme values
            int legalPacketCount    = Mathf.Clamp(packetCount, 1, 20);
            int legalWindowLengthMs = Mathf.Clamp(windowLengthMs, 1, 5000);
            
            bridge.SendToNative("SetNoAckParameters", Parameter.ID(deviceID), Parameter.Int(legalPacketCount), Parameter.Int(legalWindowLengthMs));
        }
        
        public void GetWriteMTUSize(string deviceID, string service, string characteristic )
        {
            bridge.SendToNative("GetWriteMTUSize", Parameter.ID(deviceID), Parameter.ID(service), Parameter.ID(characteristic));
        }
        
        public void DisconnectFromDevice(string deviceID)
        {
            bridge.SendToNative("DisconnectFromDevice", Parameter.ID(deviceID));
        }

        public void SendPacket(string deviceID, string service, string characteristic, byte[] data, int group, SendFlags sendFlags, int packetID)
        {
            bridge.SendToNative("SendPacket", Parameter.ID(deviceID), Parameter.ID(service), Parameter.ID(characteristic), Parameter.Bytes(data), Parameter.Int(group), Parameter.Int((int)sendFlags), Parameter.Int(packetID));
        }

        public void SendPacketNotifyOnDataTransmitted(string deviceID, string service, string characteristic, byte[] data, int seqNr, bool softAck)
        {
            bridge.SendToNative("SendPacketNotifyOnDataTransmitted", Parameter.ID(deviceID), Parameter.ID(service), Parameter.ID(characteristic), Parameter.Bytes(data), Parameter.Int(seqNr), Parameter.Bool(softAck));
        }

        public void SetLogLevel(int logLevel)
        {
            Log.Debug("SetLogLevel(" + logLevel.ToString() + ")");
            bridge.SendToNative("SetLogLevel", Parameter.Int(logLevel));
        }

        public void RequestMtuSize(string deviceID, int mtuSize)
        {
            Log.Debug($"RequestMtuSize(deviceID: {deviceID}, mtuSize: {mtuSize})");
            bridge.SendToNative("RequestMtuSize", Parameter.ID(deviceID), Parameter.Int(mtuSize));
        }
    }
}