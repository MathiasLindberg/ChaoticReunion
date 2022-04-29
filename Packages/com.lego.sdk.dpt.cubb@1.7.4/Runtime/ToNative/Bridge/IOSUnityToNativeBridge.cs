using System.Runtime.InteropServices;
using AOT;
using CoreUnityBleBridge.ToUnity;

namespace CoreUnityBleBridge.ToNative.Bridge
{
	internal sealed class IOSUnityToNativeBridge : AbstractUnityToNativeBridge
	{
        
#if UNITY_IOS && !UNITY_EDITOR
        [DllImport ("__Internal")]
        private static extern void _cubbHandleMessageFromUnity(string message);
#else
		private static void _cubbHandleMessageFromUnity(string message) { }
#endif
	    
		public delegate void CubbSendMessageDelegate(string message);
 
		[DllImport("__Internal")]
		public static extern void setCallback(CubbSendMessageDelegate callback);

		public IOSUnityToNativeBridge()
		{
			setCallback(ReceiveMessageFromNative);
		}
	    
		[MonoPInvokeCallback(typeof(CubbSendMessageDelegate))]
		private static void ReceiveMessageFromNative(string message)
		{
			CUBBNativeMessageReceiver.ReceiveMessage(message);
		}

		protected override void SendMessageToNative(string message)
		{
			// Extern method which sends a message to native
			_cubbHandleMessageFromUnity(message);
		}
	}
}