using System;
using System.Collections;

namespace LEGODeviceUnitySDK
{
    internal static class SafeEnumerator
    {

        /// The compiler won't let us try-catch while iterating. So we work around that...:
        /// (The point being to move "yield" out of "try-catch".)
        public static IEnumerator Create(IEnumerator org, Action<Exception> onException)
        {
            //UnityEngine.Debug.Log("SafeEnum.create");
            while (true) {
                try {
                    //UnityEngine.Debug.Log("SafeEnum step");
                    if (!org.MoveNext()) break;
                } catch (Exception e) {
                    //UnityEngine.Debug.Log("SafeEnum exception: "+e.Message);
                    onException(e);
                    break;
                }
                yield return org.Current;
            }
        }
    }

}

