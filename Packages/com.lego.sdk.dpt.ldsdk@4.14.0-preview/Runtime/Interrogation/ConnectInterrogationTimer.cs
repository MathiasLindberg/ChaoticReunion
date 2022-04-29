using System;
using UnityEngine;
using System.Collections;
using LEGO.Logger;

namespace LEGODeviceUnitySDK
{

    /******************************************************************************
     * PURPOSE: Determining when interrogation-at-connect can be considered done. *
     ******************************************************************************/
    /* ==== Interrogation policy:
    * Interrogation is completed as soon as all of the following are satisfied:
    * - A minimum time has passed since the start, *or* these has been a certain idle time.
    * - We have received the entire set of expected device properties.
    *   - For V3, these are: hardwareRevision, firmwareRevision, radioFirmwareVersion, manufacturerName, batteryType, wirelessProtocolVersion, primaryMacAddress, secondaryMacAddress
    * - We have received complete mode information for all of the known services.
    * 
    * However, we abort - declaring an interrogation failure - when either of the following:
    * - The device has disconnected.
    * - After the minimum time, we have not yet received all of the device properties.
    * - A certain amount of time has passed without any progress (no packets received).
    * 
    * The algorithm is:
    * - Once connected, we enable updates and request the current value for all hub properties (as applicable); see other class.
    * - We maintain two timers:
    *   - a global timer, started on the reception of the first packet
    *   - and a local one, started at each relevant received packet.
    * - On local timer timeout, we declare minimum time to have passed (sic); if we're still missing something, interrogation has failed.
    * - On global timer timeout, we declare minimum time to have passed; if we're still missing the hub properties, interrogation has failed.
    */
    internal class ConnectInterrogationTimer 
    {
        #region Constants guiding the interrogation process
        private const float MINIMUM_INTERROGATION_TIME = 3f;
        private const float MAX_TIME_TO_WAIT_FOR_NEXT_PACKAGE_DURING_INTERROGATION = 2.5f;
        public enum InterrogationState
        {
            InProgress, 
            Complete, 
            Failed 
        }
        #endregion

        public InterrogationState CurrentState 
        {
            get;
            private set;
        }
        
        
        private static readonly ILog logger = LogManager.GetLogger(typeof(ConnectInterrogationTimer));

        private readonly MonoBehaviour coroutineHelper;

        private Func<bool> servicesAreFullyKnown;

        #region Timers & state
        private bool gotHubAlerts = false;
        private bool gotHubProperties = false;
        private bool minimumTimePassed = false;
        private bool done = false;
        private readonly Action<bool> onCompletion;
        private Coroutine globalTimer, localTimer;
        #endregion

        public ConnectInterrogationTimer(MonoBehaviour coroutineHelper, Func<bool> servicesAreFullyKnown, Action<bool> onCompletion)
        {
            logger.Debug("ConnectInterrogationTimer created");
            this.coroutineHelper = coroutineHelper;
            this.onCompletion = onCompletion;
            this.servicesAreFullyKnown = servicesAreFullyKnown;
            
            CurrentState = InterrogationState.InProgress;
            
            StartGlobalTimer();
        }

        #region Events from outside

        public void Restart()
        {
            done = false;
            minimumTimePassed = false;
            
            StopTimers();
            
            CurrentState = InterrogationState.InProgress;
            
            StartGlobalTimer();
        }
        
        public void Cancel() 
        {
            AbortInterrogation("Cancelled");
        }
        
        public void GotHubAlerts()
        {
            gotHubAlerts = true;
            MajorProgressTick();
        }
        public void GotHubProperties()
        {
            gotHubProperties = true;
            MajorProgressTick();
        }

        public void MinorProgressTick() 
        {
            RestartLocalTimer();
        }

        public void MajorProgressTick() 
        {
            CheckWhetherDone();
            if (!done) 
                RestartLocalTimer();
        }
        #endregion

        #region Timers
        private void StartGlobalTimer()
        {
            globalTimer = coroutineHelper.StartCoroutine(GlobalTimer());
        }

        private void RestartLocalTimer()
        {
            if (localTimer != null) 
                coroutineHelper.StopCoroutine(localTimer);
            localTimer = coroutineHelper.StartCoroutine(LocalTimer());
        }

        private void StopTimers() 
        {
            if (globalTimer != null) 
            {
                coroutineHelper.StopCoroutine(globalTimer);
                globalTimer = null;
            }
            if (localTimer != null) 
            {
                coroutineHelper.StopCoroutine(localTimer);
                localTimer = null;
            }
        }

        private IEnumerator GlobalTimer() 
        {
            yield return new WaitForSeconds(MINIMUM_INTERROGATION_TIME);
            logger.Debug("Interrogation minimum-timer triggered");
            minimumTimePassed = true;
            if(!gotHubAlerts)
            {
                AbortInterrogation("Hub alerts missing at global timeout");
            }
            else if (!gotHubProperties) 
            {
                AbortInterrogation("Hub properties missing at global timeout");
            }
            else 
            {
                MajorProgressTick();
            }
        }

        private IEnumerator LocalTimer() 
        {
            yield return new WaitForSeconds(MAX_TIME_TO_WAIT_FOR_NEXT_PACKAGE_DURING_INTERROGATION);
            logger.Debug("Interrogation idle-timer triggered");
            // Either we trigger completion, or we abort:
            minimumTimePassed = true;
            MajorProgressTick();
            if (!done) 
            {
                AbortInterrogation("Idle timeout.");
            }
        }
        #endregion

        private void AbortInterrogation(string reason)
        {
            logger.Warn("Interrogation aborted: "+reason);
            TerminateInterrogation(false);
        }

        private void TerminateInterrogation(bool success)
        {
            if (done) { // Paranoia
                logger.Error("Double done! "+success);
                return;
            }

            CurrentState = success ? InterrogationState.Complete : InterrogationState.Failed;
            
            StopTimers();
            done = true;
            
            onCompletion(success);
        }

        private void CheckWhetherDone()
        {
            if (   minimumTimePassed 
                && gotHubAlerts 
                && gotHubProperties 
                && servicesAreFullyKnown()
                && CurrentState == InterrogationState.InProgress) 
            {
                logger.Info("Interrogation completed.");
                TerminateInterrogation(true);
            }
        }

    }

    /**********************************************************
     * PURPOSE: Controlling the interrogation flow.           *
     **********************************************************/
    
}
