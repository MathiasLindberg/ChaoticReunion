using System;
using System.Text;
using CoreUnityBleBridge.Model;
using LEGO.Logger;
using UnityEngine;

namespace CoreUnityBleBridge.ToUnity
{
    internal sealed class NativeMessageInterpreter
    {
        private readonly NativeToUnity nativeToUnity;
        private static readonly ILog logger = LogManager.GetLogger<NativeMessageInterpreter>();

        internal NativeMessageInterpreter(NativeToUnity nativeToUnity)
        {
            this.nativeToUnity = nativeToUnity;
        }
        
        public void HandleMessage(string message)
        {
            try 
            {
                DecodeAndInvoke(message, nativeToUnity);
            } 
            catch (Exception e)
            {
                logger.Error("Interpreting failed for the N2U message <" + message + ">", e);
            }
        }

        private static void DecodeAndInvoke(string messageStr, NativeToUnity toUnity) 
        {
            var message = new MessageToUnity(messageStr);
            switch (message.MessageType) 
            {
            case "Error": // (string msg)
                message.AssertParameterCount(1);
                var errorEventArgs = new ErrorEventArgs {Message = message.String(1)};
                toUnity.OnError(errorEventArgs);
                break;
            
            case "Log": // (string msg, string logLevel)
                message.AssertParameterCount(2);
                var logMessage = message.String(1);
                var logLevelStr = message.String(2);
                toUnity.OnLog(logMessage, LogLevelFromString(logLevelStr));
                break;
            
            case "ScanStateChanged": // (enum newScanState)
                message.AssertParameterCount(1);
                var adapterScanStateChangedArgs = new AdapterScanStateChangedEventArgs
                {
                    AdapterScanState = (AdapterScanState) Enum.Parse(typeof(AdapterScanState), message.Token(1))
                };
                toUnity.OnAdapterScanStateChanged(adapterScanStateChangedArgs);
                break;
            
            case "DeviceStateChanged": // (string deviceID, enum state, string deviceName, string serviceGUID, int rssi, byte[] manufacturerData)
                message.AssertParameterCount(6);
                var deviceStateChangedArgs = new DeviceStateChangedEventArgs
                {
                    DeviceID = message.Token(1),
                    DeviceVisibilityState = (DeviceVisibilityState) Enum.Parse(typeof(DeviceVisibilityState), message.Token(2)),
                    DeviceName = message.String(3),
                    ServiceGuid = message.OptionalGuid(4),
                    Rssi = message.Int(5),
                    ManufaturerData = message.Bytes(6)
                };
                toUnity.OnDeviceStateChanged(deviceStateChangedArgs);
                break;

            case "DeviceDisappeared": // (string deviceID)
                message.AssertParameterCount(1);
                var deviceDisappearedArgs = new DeviceDisappearedEventArgs
                {
                    DeviceID = message.Token(1)
                };
                toUnity.OnDeviceDisappeared(deviceDisappearedArgs);
                break;

            case "DeviceConnectionStateChanged": // (string deviceID, enum state, string errorMsg)
                message.AssertParameterCount(3);
                var connectionStateChangedArgs = new DeviceConnectionStateChangedEventArgs
                {
                    DeviceID = message.Token(1),
                    DeviceConnectionState = (DeviceConnectionState) Enum.Parse(typeof(DeviceConnectionState), message.Token(2)),
                    ErrorMessage = message.String(3)
                };
                toUnity.OnDeviceConnectionStateChanged(connectionStateChangedArgs);
                break;

            case "ReceivedPacket": // (string deviceID, string service, string gattCharacteristic, byte[] data)
                message.AssertParameterCount(4);
                var packetReceivedArgs = new PacketReceivedEventArgs
                {
                    DeviceID = message.Token(1),
                    Service = message.UUID(2),
                    GattCharacteristic = message.UUID(3),
                    Data = message.Bytes(4)
                };
                toUnity.OnPacketReceived(packetReceivedArgs);
                break;
            case "PacketSent": // (string deviceID, string service, string gattCharacteristic)
                message.AssertParameterCount(3);
                var packetSentArgs = new PacketTransmittedEventArgs
                {
                    DeviceID = message.Token(1),
                    Service = message.UUID(2),
                    GattCharacteristic = message.UUID(3),
                };
                toUnity.OnPacketSent(packetSentArgs);
                break;
            
            case "PacketTransmitted": // (string deviceID, string service, string gattCharacteristic, int seqNr)
                message.AssertParameterCount(4);
                var packetTransmittedArgs = new PacketTransmittedEventArgs
                {
                    DeviceID = message.Token(1),
                    Service = message.UUID(2),
                    GattCharacteristic = message.UUID(3),
                    SequenceNumber = message.Int(4)
                };
                toUnity.OnPacketTransmitted(packetTransmittedArgs);
                break;

            case "PacketDropped": // (string deviceID, string service, string gattCharacteristic, int packetID)
                message.AssertParameterCount(4);
                var packetDroppedArgs = new PacketDroppedEventArgs
                {
                    DeviceID = message.Token(1),
                    Service = message.UUID(2),
                    GattCharacteristic = message.UUID(3),
                    PacketID = message.Int(4)
                };
                toUnity.OnPacketDropped(packetDroppedArgs);
                break;
            
            case "WriteMTUSize": // (string deviceID, string service, string gattCharacteristic, int writeMTUSize)
                Debug.Log("NativeMessageInterpreter: WriteMTUSize:" +  message);
                message.AssertParameterCount(4);
                var writeMTUSizeArgs = new WriteMTUSizeEventArgs
                {
                    DeviceID = message.Token(1),
                    Service = message.UUID(2),
                    GattCharacteristic = message.UUID(3),
                    WriteMTUSize = message.Int(4)
                };
                toUnity.OnWriteMTUSize(writeMTUSizeArgs);
                break;
            
            case "MtuSizeChanged": // (string deviceID, int newMtuSize)
                message.AssertParameterCount(2);
                var mtuSizeChangedArgs = new MtuSizeChangedEventArgs()
                {
                    DeviceID = message.Token(1),
                    MtuSize = message.Int(2)
                };
                toUnity.OnMtuSizeChanged(mtuSizeChangedArgs);
                break;

            default:
                var errorArgs = new ErrorEventArgs
                {
                    Message = "Unhandled message-from-native: <" + messageStr + ">"
                };
                toUnity.OnError(errorArgs);
                break;
            }
            
        }
        
        private static LogLevel LogLevelFromString(string logLevel)
        {
            if (string.Equals("verbose", logLevel, StringComparison.OrdinalIgnoreCase))
                return LogLevel.VERBOSE;
            if (string.Equals("debug", logLevel, StringComparison.OrdinalIgnoreCase))
                return LogLevel.DEBUG;
            if (string.Equals("info", logLevel, StringComparison.OrdinalIgnoreCase))
                return LogLevel.INFO;
            if (string.Equals("warn", logLevel, StringComparison.OrdinalIgnoreCase))
                return LogLevel.WARN;
            if (string.Equals("error", logLevel, StringComparison.OrdinalIgnoreCase))
                return LogLevel.ERROR;
            if (string.Equals("fatal", logLevel, StringComparison.OrdinalIgnoreCase))
                return LogLevel.FATAL;     
            
            logger.Warn(logLevel + " is not a recognized log category (check the names of log-categories send from the native side)");
            return LogLevel.DEBUG;
        }

        private struct MessageToUnity 
        {
            private static readonly ILog Logger = LogManager.GetLogger<MessageToUnity>();

            private readonly string[] parts;
            public MessageToUnity(string message)
            {
                this.parts = message.Split('|');
            }

            public string MessageType
            {
                get { return Token(0); }
            }

            public void AssertParameterCount(int expectedCount)
            {
                if (parts.Length != expectedCount + 1) 
                {
                    throw new ArgumentException("Message has the wrong number of arguments: "+MessageType+"/"+parts.Length);
                }
            }

            public string Token(int index) { return parts[index]; }

            public String UUID(int index) {
                // Normalize casing:
                return parts[index].ToLower();
            }

            public Guid? OptionalGuid(int index) 
            { 
                var s = parts[index];
                if (s == "%" || s == "")
                    return null;
                    
                try 
                {
                    return new Guid(s);
                } 
                catch (Exception e) 
                {
                    Logger.Warn("Not a valid GUID: '" + s + "'", e);
                    return null;
                }
            }

            public int Int(int index) 
            {
                return Convert.ToInt32(parts[index]);
            }

            public byte[] Bytes(int index) 
            {
                var s = parts[index];
                return s == "%" ? null : Convert.FromBase64String(s);
            }

            public string String(int index) 
            {
                var bytes = Bytes(index);
                return bytes == null ? null : Encoding.UTF8.GetString(bytes);
            }
        }
    }
}

