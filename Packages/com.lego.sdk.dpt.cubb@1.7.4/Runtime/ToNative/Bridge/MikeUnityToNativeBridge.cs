using System.Runtime.InteropServices;
using CoreUnityBleBridge.ToUnity;
using LEGO.Logger;
using LEGO.Logger.Appenders;
using AOT;

namespace CoreUnityBleBridge.ToNative.Bridge
{
	internal sealed class MikeUnityToNativeBridge : AbstractUnityToNativeBridge
	{
		public delegate void CubbSendMessageDelegate(string message);

		//[DllImport("DLLTEST")] 
		[DllImport("winrt1", CallingConvention = CallingConvention.Cdecl)]
        private static extern void setCallback(CubbSendMessageDelegate callback);
        //[DllImport ("DLLTEST")]
        [DllImport("winrt1", CallingConvention = CallingConvention.Cdecl)]
        private static extern void _cubbHandleMessageFromUnity(string message);

        public MikeUnityToNativeBridge()
		{
			LogManager.RootLevel = LogLevel.OFF; // NOTE: changed from VERBOSE to OFF.
			LogManager.AddAppender(new UnityEngineLogAppender());
			MikeMainThreadDispatcher.Initialize();
			MikeMainThreadDispatcher.onUpdate += () => _cubbHandleMessageFromUnity("Poll");
			MikeMainThreadDispatcher.EnqueueOnDestroy(() => {
				UnityEngine.Debug.Log("unsetting callback"); // todo still needed?
				setCallback(null);
			});
			setCallback(ReceiveMessageFromNative);
		}

		protected override void SendMessageToNative(string message)
		{
			_cubbHandleMessageFromUnity(message);
		}

		[MonoPInvokeCallback(typeof(CubbSendMessageDelegate))]
		private static void ReceiveMessageFromNative(string message)
		{
			// switched to "pull" so this runs on unity thread, call directly
			CUBBNativeMessageReceiver.ReceiveMessage(message);
			//MikeMainThreadDispatcher.Enqueue(() => CUBBNativeMessageReceiver.ReceiveMessage(message));
		}
	}
}
