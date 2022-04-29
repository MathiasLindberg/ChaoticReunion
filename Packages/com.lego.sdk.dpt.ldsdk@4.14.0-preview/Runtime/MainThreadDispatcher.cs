using System;
using UnityEngine;

namespace LEGODeviceUnitySDK
{
    internal class MainThreadDispatcher : MonoBehaviour
    {
        private static Action _queue;
        private static object _lockObject = new object();
        private static MainThreadDispatcher _instance;

        private void Update()
        {
            lock (_lockObject)
            {
                _queue?.Invoke();
                _queue = null;
            }
        }

        public static void Enqueue(Action action)
        {
            lock (_lockObject)
            {
                _queue += action;
            }
        }

        public static void Initialize()
        {
            if (_instance == null)
            {
                _instance = new GameObject("LDSDK MainThreadDispatcher").AddComponent<MainThreadDispatcher>();
                DontDestroyOnLoad(_instance.gameObject);
            }        
        }
    }
}
