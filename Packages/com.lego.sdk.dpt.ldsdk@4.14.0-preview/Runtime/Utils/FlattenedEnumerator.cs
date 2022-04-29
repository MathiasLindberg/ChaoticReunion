using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace LEGODeviceUnitySDK
{
    // The compiler won't let us try-catch while iterating. This is a work-around:
    internal class FlattenedEnumerator : IEnumerator
	{
        private Stack<IEnumerator> stack;

        public object Current { get; private set; }

        public FlattenedEnumerator(IEnumerator org)
        {
            stack = new Stack<IEnumerator>();
            stack.Push(org);
        }

        /// <summary>
        /// Returns whether done.
        /// </summary>
        public bool MoveNext()
        {
            //UnityEngine.Debug.Log("Flatten: |stack|="+stack.Count);

            while (true) {
                if (stack.Count == 0) return false;

                var iter = stack.Peek();
                if (iter.MoveNext()) {
                    var cur = iter.Current;
                    if (cur is IEnumerable)
                        cur = ((IEnumerable)cur).GetEnumerator();
                    if (cur is IEnumerator) {
                        //Recurse!
                        stack.Push((IEnumerator)cur);
                    } else {
                        // Yield.
                        this.Current = cur;
                        return true;
                    }
                } else {
                    stack.Pop();
                }
            }
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }

	}
}

