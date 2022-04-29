using UnityEngine;

namespace CoreUnityBleBridge.Utilities
{
    internal sealed class BleCoroutineRunner : MonoBehaviour
    {
        //Note: If we ever allow something like BleFactory.Close(), we might need to return here and figure out
        //a proper way of destroying these objects.
        private static BleCoroutineRunner bleCoroutineRunner;
        
        /// <summary>
        /// Returns a reference to the static BleCoroutineRunner object.
        /// This object persists through scene loads.
        /// </summary>
        /// <returns></returns>
        public static BleCoroutineRunner GetDefault()
        {
            if (bleCoroutineRunner != null)
                return bleCoroutineRunner;

            var name = typeof(BleCoroutineRunner).Name;
            bleCoroutineRunner = new GameObject(name).AddComponent<BleCoroutineRunner>();
            
            #if !UNITY_EDITOR // testing occurs in Unity editor - this call is not allowed there (causes System.InvalidOperationException: The following game object is invoking the DontDestroyOnLoad method: BleCoroutineRunner. Notice that DontDestroyOnLoad can only be used in play mode and, as such, cannot be part of an editor script."
                DontDestroyOnLoad(bleCoroutineRunner.gameObject);
            #endif
            
            return bleCoroutineRunner;
        }
    }
}