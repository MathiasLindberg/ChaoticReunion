using System;
using JetBrains.Annotations;
using LEGO.Logger;
using UnityEngine;

namespace CoreUnityBleBridge.ToUnity
{
    internal sealed class CUBBNativeMessageReceiver : MonoBehaviour
    {
        public event Action<string> MessageReceived = delegate {};
        private static Action<string> InternalMessageReceived = delegate {};

        public void Awake()
        {
            InternalMessageReceived += OnInternalMessageReceived;
        }

        private void OnDestroy()
        {
            InternalMessageReceived -= OnInternalMessageReceived;
        }

        private void OnInternalMessageReceived(string message)
        {
            MessageReceived?.Invoke(message);
        }

        [UsedImplicitly]
        public void OnMessageReceived(string msg)
        {
            MessageReceived(msg);
        }

        public static void ReceiveMessage(string message)
        {
            InternalMessageReceived?.Invoke(message);
        }
    }
    
    internal static class CUBBNativeMessageReceiverFactory
    {
        private static CUBBNativeMessageReceiver cubbNativeMessageReceiver;
        public static CUBBNativeMessageReceiver Create()
        {
            if (cubbNativeMessageReceiver != null)
                return cubbNativeMessageReceiver;
            
            //NOTE: Do not change the game object name, since this is utilized directly within the native layers through
            //UnityEngine.SendMessage(<GameObjectName>, <MethodName>, <Message>)
            var gameObject = new GameObject("CUBBNativeMessageReceiver");
            
            cubbNativeMessageReceiver = gameObject.AddComponent<CUBBNativeMessageReceiver>();
            UnityEngine.Object.DontDestroyOnLoad(gameObject);

            return cubbNativeMessageReceiver;
        }
    }
}