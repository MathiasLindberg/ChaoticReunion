//using System.Diagnostics.CodeAnalysis;
//using System;
//using CoreUnityBleBridge.Model;
//using CoreUnityBleBridge.ToUnity;

//namespace CoreUnityBleBridge.ToNative
//{
//    [SuppressMessage("ReSharper", "UnusedMember.Global")]
//    internal sealed class UWPUnityToNative: IUnityToNative
//    {
//        private static readonly LEGO.Logger.ILog Logger = LEGO.Logger.LogManager.GetLogger<UWPUnityToNative>();
        
//#if UNITY_WSA_10_0
//        private readonly LEGODeviceSDK.TranslationLayer translationLayer;
//#endif
        
//        public UWPUnityToNative(NativeToUnity nativeToUnity)
//        {
//            #if UNITY_WSA_10_0
//            var connectionsCallbacks = new ConnectionsCallbacks(nativeToUnity);
//            var sdkLoggerAdapter = new SdkLoggerAdapter(Logger);

//            translationLayer = new LEGODeviceSDK.TranslationLayer();
//            translationLayer.Init(OnNativeActionReceived, null, connectionsCallbacks, sdkLoggerAdapter);
//            //2nd argument = null means the Translation layer will do its own seperation mechanism

//            var adapterCallbacks = new AdapterCallbacks(nativeToUnity);
//            translationLayer.Subscribe(adapterCallbacks);
//            #endif
//        } 
        
        
//        #if UNITY_WSA_10_0
//        private static void OnNativeActionReceived(Action tup)
//        {
//            if(tup == null)
//                return;

//            if (UnityEngine.WSA.Application.RunningOnAppThread())
//            {
//                tup();
//                return;
//            }
            
//            UnityEngine.WSA.Application.InvokeOnAppThread(() =>
//            {
//                tup();
//            }, false);
//        }
//        #endif

//        public void Initialize(string services) {}

//        public void SetScanState(bool enabled)
//        {
//            Logger.Debug("SetScanState " + enabled);
            
//            #if UNITY_WSA_10_0
//            translationLayer.SetScanning(enabled);
//            #endif
//        }

//        public void ConnectToDevice(string deviceID, SendStrategy? sendStrategy)
//        {
//            #if UNITY_WSA_10_0
//            Logger.Debug("ConnectToDevice: " + deviceID+" sendStrategy: " + sendStrategy);
//            translationLayer.Connect(deviceID); //TODO: pass connection options?
//            #endif
//        }

//        public void DisconnectFromDevice(string deviceID)
//        {
//            #if UNITY_WSA_10_0
//            Logger.Debug("DisconnectFromDevice: " + deviceID);
//            translationLayer.Disconnect(deviceID);
//            #endif
//        }
        
//        public void SetNoAckParameters(string deviceID, int packetCount, int windowLengthMs)
//        {
//            Logger.Debug("SetNoAckParameters: deviceID: " + deviceID + " packetCount=" + packetCount + " windowLengthMs=" + windowLengthMs);
//        }
//        public void GetWriteMTUSize(string deviceID, string service, string gattChar)
//        {
//            Logger.Debug("GetWriteMTUSize: deviceID: " + deviceID + " service: " + service + " gattChar: " + gattChar);
//        }
        
//        public void SendPacket(string deviceID, string service, string characteristic, byte[] data, int group, SendFlags sendFlags, int packetID)
//        {
//            #if UNITY_WSA_10_0
//            translationLayer.SendPacket(deviceID, service, characteristic, data); //TODO: Pass group + sendFlags + packetID
//            #endif
//        }
//        public void SendPacketNotifyOnDataTransmitted(string deviceID, string service, string characteristic, byte[] data, int seqNr, bool softAck)
//        {
//            #if UNITY_WSA_10_0
//            translationLayer.SendPacketNotifyOnDataTransmitted(deviceID, service, characteristic, data, seqNr); //TODO: Implement soft ack
//            #endif
//        }

//        public void SetLogLevel(int logLevel)
//        {
//            #if UNITY_WSA_10_0
//            Logger.Warn("Log level filtering is not implemented in WSA");
//            #endif
//        }

//        public void RequestMtuSize(string deviceID, int mtuSize)
//        {
//            #if UNITY_WSA_10_0
//            Logger.Debug($"RequestMtuSize(deviceID: {deviceID}, mtuSize: {mtuSize}");
//            #endif
//        }


//#if UNITY_WSA_10_0
//        private class ConnectionsCallbacks : LEGODeviceSDK.IConnectionsCallbacks
//        {
//            private readonly NativeToUnity nativeToUnity;
//            public ConnectionsCallbacks(NativeToUnity nativeToUnity)
//            {
//                this.nativeToUnity = nativeToUnity;
//            }

//            public void OnReceivedPacket(string deviceID, string serviceInterface, string gattCharacteristic, byte[] data)
//            {
//                nativeToUnity.OnPacketReceived(deviceID, serviceInterface, gattCharacteristic, data);
//            }
            
//            public void OnPacketTransmitted(string deviceID, string serviceInterface, string gattCharacteristic, int seqNr)
//            {
//                nativeToUnity.OnPacketTransmitted(deviceID, serviceInterface, gattCharacteristic, seqNr);
//            }
//        }
        
//        private class AdapterCallbacks : LEGODeviceSDK.IAdapterCallbacks
//        {
//            private readonly string context = typeof(AdapterCallbacks).Name;
//            private readonly NativeToUnity nativeToUnity;
//            public AdapterCallbacks(NativeToUnity nativeToUnity)
//            {
//                Logger.Debug(context + ": Constructor");
                
//                this.nativeToUnity = nativeToUnity;
//            }

//            public void OnAdapterStateChanged(LEGODeviceSDK.AdapterState newAdapterState)
//            {  
//                Logger.Debug(context + ".OnAdapterStateChanged: "+ newAdapterState);
                
//                AdapterScanState state;
//                switch(newAdapterState)
//                {
//                    case LEGODeviceSDK.AdapterState.Created:
//                    case LEGODeviceSDK.AdapterState.StoppingScanning:
//                    case LEGODeviceSDK.AdapterState.Stopped:
//                    case LEGODeviceSDK.AdapterState.Aborted: // TODO KLI: is this correct for the error state
//                        state = AdapterScanState.NotScanning;
//                        break;
                    
//                    case LEGODeviceSDK.AdapterState.Scanning:
//                    case LEGODeviceSDK.AdapterState.AllInitialDevicesFound:
//                        state = AdapterScanState.Scanning;
//                        break;
                    
//                    case LEGODeviceSDK.AdapterState.BLEDisabled:
//                        state = AdapterScanState.BluetoothDisabled;
//                        break;
                    
//                    case LEGODeviceSDK.AdapterState.NoBLE:
//                        state = AdapterScanState.BluetoothUnavailable;
//                        break;
                    
//                    default:
//                        state = AdapterScanState.BluetoothUnavailable;
//                        break;
//                }
//                System.Diagnostics.Debug.Assert(nativeToUnity != null);
//                nativeToUnity.OnAdapterScanStateChanged(state);
//            }

//            public void OnDeviceConnectionStateChanged(string deviceID, LEGODeviceSDK.DeviceConnectionState state, string errorMsg)
//            { 
//                Logger.Debug(context + ".OnDeviceConnectionStateChanged: " + state);
                
//                DeviceConnectionState dcState;
//                switch (state)
//                {
//                    case LEGODeviceSDK.DeviceConnectionState.Start:
//                    case LEGODeviceSDK.DeviceConnectionState.Disconnected:
//                        dcState = DeviceConnectionState.Disconnected;
//                        break;
                    
//                    case LEGODeviceSDK.DeviceConnectionState.Connected:
//                        dcState = DeviceConnectionState.Connected;
//                        break;
                    
//                    case LEGODeviceSDK.DeviceConnectionState.Connecting:
//                        dcState = DeviceConnectionState.Connecting;
//                        break;
                    
//                    default:
//                        throw new NotImplementedException();
//                }

//                nativeToUnity.OnDeviceConnectionStateChanged(deviceID, dcState, errorMsg);
//            }


//            public void OnDeviceStateChanged(string deviceID, string serviceGuid, LEGODeviceSDK.DeviceAdvertisingState deviceAdvertisingState, string deviceName, int rssi, byte[] manufacturerData)
//            {
//                Logger.Debug(context + ".OnDeviceStateChanged; " + deviceAdvertisingState + ", btaddress: " + deviceID + ", serviceGuid: " + serviceGuid);
                
//                DeviceVisibilityState deviceVisibilityState;
//                switch(deviceAdvertisingState)
//                {
//                    case LEGODeviceSDK.DeviceAdvertisingState.NotAdvertising: 
//                        deviceVisibilityState = DeviceVisibilityState.Invisible;
//                        break;
                    
//                    case LEGODeviceSDK.DeviceAdvertisingState.Advertising:
//                        deviceVisibilityState = DeviceVisibilityState.Visible;
//                        break;
                    
//                    default:
//                        deviceVisibilityState = DeviceVisibilityState.Invisible;
//                        break;
//                }
                
//                nativeToUnity.OnDeviceStateChanged(deviceID, deviceVisibilityState, deviceName, new Guid(serviceGuid), rssi, manufacturerData);
//            }

//            public void OnDeviceDisappeared(string deviceID)
//            {
//                Logger.Debug(context + ".OnDeviceDisappeared");
                
//                nativeToUnity.OnDeviceDisappeared(deviceID);
//            }
//        }
        
//        private class SdkLoggerAdapter: LEGODeviceSDK.ILog
//        {
//            private readonly LEGO.Logger.ILog unityLogger;
//            public SdkLoggerAdapter(LEGO.Logger.ILog unityLogger)
//            {
//                this.unityLogger = unityLogger;
//            }

//            public void Debug(object message)
//            {
//                unityLogger.Debug(message);
//            }

//            public void Error(object message)
//            {
//                unityLogger.Error(message);
//            }

//            public void Fatal(object message)
//            {
//                unityLogger.Fatal(message);
//            }

//            public void Info(object message)
//            {
//                unityLogger.Info(message);
//            }

//            public void Verbose(object message)
//            {
//                unityLogger.Verbose(message);
//            }

//            public void Warn(object message)
//            {
//                unityLogger.Warn(message);
//            }
//        }
//        #endif
//    }
//}