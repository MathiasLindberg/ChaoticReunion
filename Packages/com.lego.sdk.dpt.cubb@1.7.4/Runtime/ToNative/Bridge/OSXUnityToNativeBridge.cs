#if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
using System;
using System.Runtime.InteropServices;
using CoreUnityBleBridge.ToUnity;

namespace CoreUnityBleBridge.ToNative.Bridge
{
    internal sealed class OSXUnityToNativeBridge : AbstractUnityToNativeBridge
    {
        private delegate void UnityMessageDelegate(IntPtr objectName, IntPtr commandName, IntPtr commandData);

        [DllImport ("CUBBNativeOSX")]
        private static extern void _cubbHandleMessageFromUnity(string message);
        [DllImport ("CUBBNativeOSX")]
        private static extern void LEGODeviceSDK_setOSXUnityMessageCallback([MarshalAs(UnmanagedType.FunctionPtr)]UnityMessageDelegate messageCallback);
        
        public OSXUnityToNativeBridge()
        {
            // Hook up the message callback
            LEGODeviceSDK_setOSXUnityMessageCallback((objectNamePtr, commandNamePtr, commandDataPtr) => CUBBNativeMessageReceiver.ReceiveMessage(Marshal.PtrToStringAuto(commandDataPtr)) );
        }

        ~OSXUnityToNativeBridge()
        {
	        LEGODeviceSDK_setOSXUnityMessageCallback(null);
        }

        protected override void SendMessageToNative(string message)
        {
            _cubbHandleMessageFromUnity(message);
        }
    }
}
#endif