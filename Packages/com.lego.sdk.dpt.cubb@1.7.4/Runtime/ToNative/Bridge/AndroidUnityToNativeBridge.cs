using System.Threading;
using UnityEngine;

namespace CoreUnityBleBridge.ToNative.Bridge
{
    internal sealed class AndroidUnityToNativeBridge : AbstractUnityToNativeBridge
    {        
        private const string javaClassName = "dk.lego.cubb.unity.CUBBNative";
        private const string javaMethodName = "handleMessageFromUnity";
        private delegate object JNIAction(AndroidJavaObject javaObject);

        private static int mainThreadId;
        private AndroidJavaObject nativeWrapper;
        
        
        public AndroidUnityToNativeBridge()
        {
            mainThreadId = Thread.CurrentThread.ManagedThreadId;
            PerformWrapperAction(javaClassName,
                wrapper =>
                {
                    nativeWrapper = wrapper;
                    return null;
                });

            InitializeNative();
        }

        protected override void SendMessageToNative(string message)
        {
            var attached = IsBackgroundThread() ? AndroidJNI.AttachCurrentThread() == 0 : false;
            try
            {
                nativeWrapper.Call(javaMethodName, message);
            }
            finally
            {
                if (attached)
                    AndroidJNI.DetachCurrentThread();
            }
        }

        
        private static void InitializeNative()
        {
            var attached = IsBackgroundThread() ? AndroidJNI.AttachCurrentThread() == 0 : false;

            try
            {
                using (AndroidJavaClass wrapperClass =
                        new AndroidJavaClass(javaClassName),
                    playerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                {
                    using (var activity = playerClass.GetStatic<AndroidJavaObject>("currentActivity"))
                    {
                        var application = activity.Call<AndroidJavaObject>("getApplication");
                        var wrapper = wrapperClass.CallStatic<AndroidJavaObject>("getInstance");
                        wrapper.Call("setApplication", application);
                    }
                }
            }
            finally
            {
                if (attached)
                    AndroidJNI.DetachCurrentThread();
            }
        }        
                
        private static object PerformWrapperAction(string wrapperClassName, JNIAction action)
        {
            var attached = IsBackgroundThread() ? AndroidJNI.AttachCurrentThread() == 0 : false;

            try
            {
                using (var wrapperClass = new AndroidJavaClass(wrapperClassName))
                {
                    var wrapper = wrapperClass.CallStatic<AndroidJavaObject>("getInstance");
                    return action(wrapper);
                    // Don't dispose.
                }
            }
            finally
            {
                if (attached)
                    AndroidJNI.DetachCurrentThread();
            }
        }

        
        private static bool IsBackgroundThread()
        {
            return Thread.CurrentThread.ManagedThreadId != mainThreadId;
        }

        /*
        In order to call Java methods of the Android LEGO Device SDK the Unity-defined JNI helpers are used (AndroidJavaClass, AndroidJavaObject).
    
        Only the main/UI thread, on which Unity does most of its stuff, is attached to a Java (Dalvik) VM by default. Other threads are still
        allowed to call the following methods directly since background threads are attached and detached to a VM when necessary.
        It is crucial to attach a thread to a VM before making any Java calls, and to detach it before it is terminated. Otherwise crashes will occur.
        */
        
    }
}