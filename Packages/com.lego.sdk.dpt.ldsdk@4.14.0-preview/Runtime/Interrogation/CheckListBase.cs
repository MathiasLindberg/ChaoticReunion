using System;
using System.Collections.Generic;
using LEGO.Logger;
using System.Text;

namespace LEGODeviceUnitySDK
{

    /*********************************************************************************
     * PURPOSE: Track when we're done receiving all the information about something. *
     *********************************************************************************/
    internal class CheckListBase<TKey, TValue, TResult> {
        private static readonly ILog logger = LogManager.GetLogger(typeof(CheckListBase<TKey, TValue, TResult>));

        #region Callbacks
        /// Called when progress happens (which is not major progress).
        private readonly Action onMinorProgress;
        /// Called when the interrogator becomes stable.
        private readonly Action<TResult> onMajorProgress;
        #endregion

        public CheckListBase(Action onMinorProgress, Action<TResult> onMajorProgress)
        {
            this.onMinorProgress = onMinorProgress;
            this.onMajorProgress = onMajorProgress;
        }

        #region "Waiting for" set
        public bool IsStable {get { return waitingFor.Count == 0 && !suspended; } }

        /// Things we're waiting for.
        private readonly Dictionary<TKey, TValue> waitingFor = new Dictionary<TKey, TValue>();
        
        protected Dictionary<TKey, TValue> WaitingFor
        {
            get { return waitingFor; }
        }
        
        private bool suspended = false;
        protected bool Suspended {
            get { return suspended; }
            set {
                suspended = value;
                if (!suspended) CheckWhetherStable();
            }
        }
        #endregion

        protected void MinorProgressDidHappen() {
            onMinorProgress();
        }

        protected void MarkAsPending(TKey key, TValue value) {
            waitingFor.Add(key, value);
        }

        protected bool OnPendingDo(TKey key, Action<TValue> action) {
            TValue value;
            if (waitingFor.TryGetValue(key, out value)) {
                action(value);
                return true;
            } else {
                return false;
            }
        }

        protected void CheckOff(TKey key) {
            TValue value;
            if (!waitingFor.TryGetValue(key, out value)) 
                return;
            waitingFor.Remove(key);

            logger.Debug("Checking off "+key+"; still waiting for "+waitingFor.Count);

            CheckWhetherStable();
        }

        private void CheckWhetherStable() {
            if (IsStable) {
                DidBecomeStable();
                onMajorProgress(Result());
            } else
                onMinorProgress();
        }

        protected virtual TResult Result() { return default(TResult); }
        protected virtual void DidBecomeStable() { }

        public void ReportMissing(StringBuilder sb) {
            if (waitingFor.Count > 0) {
                sb.Append("Missing: {\n");
                foreach (var k in waitingFor.Keys) {
                    sb.Append("- "+k);
                    ReportMissingDetails(sb, k, waitingFor[k]);
                    sb.Append("\n");
                }
                sb.Append("}");
            }
        }
        protected virtual void ReportMissingDetails(StringBuilder sb, TKey key, TValue value) {}

        public virtual void ResendMissing() { }
    }
    
}
