using UnityEngine;

namespace LEGO.Logger.Utilities
{
    public class CoroutineRunner : MonoBehaviour 
    {
        public static CoroutineRunner Create()
        {
            var instance = new GameObject("LoggerCoroutineRunner");
            var runner = instance.AddComponent<CoroutineRunner>();
            return runner;
        }
    }
}
