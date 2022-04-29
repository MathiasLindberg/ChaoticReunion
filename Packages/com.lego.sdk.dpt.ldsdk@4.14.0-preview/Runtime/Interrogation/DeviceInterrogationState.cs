using System;
using dk.lego.devicesdk.bluetooth.V3.messages;
using LEGO.Logger;
using System.Text;
using dk.lego.devicesdk.bluetooth.V3bootloader.messages;

namespace LEGODeviceUnitySDK
{
    /*********************************************************************
     * PURPOSE: Coordination of device interrogation.
     * Device interrogation involves:
     * - On connect, enquire for hub alerts and properties.
     * - On attachment of new services, enquire for their mode info.
     * - On connect, start timers, trying to track when we're done.
     * These tasks are split into different classes.
     *********************************************************************/
    /* Structure:
     *              InterrogationState---->ConnectInterrogationTimer
     *               |              |
     *    HubAlertInterrogator      |
     *               v              |
     *    HubPropertyInterrogator   |
     *                              v
     *                     ServiceListInterrogator
     *                              |
     *                              v*
     *                     ServiceInterrogator
     *                    /         |
     *                   /          v*
     *                  :   ServiceModeInterrogator
     *                  :      :
     *                  v      v
     *        ServiceMetadataCache
     */
    public class DeviceInterrogationState {
        private static readonly ILog logger = LogManager.GetLogger(typeof(DeviceInterrogationState));
        
        public enum Status
        {
            Interrogating,
            Completed,
            Failed 
        }

        public Status CurrentStatus
        {
            get; private set;
        }
 
        private ConnectInterrogationTimer interrogationTimer;
        private readonly HubAlertInterrogator hubAlertInterrogator;
        private readonly HubPropertyInterrogator hubPropertyInterrogator;
        private readonly ServiceListInterrogator serviceListInterrogator;

        private int retryCount = 0;
        private readonly int maxRetries = 2;
        private LEGODevice legoDevice;
        
        public DeviceInterrogationState(LEGODevice device, Action onComplete)
        {
            legoDevice = device;
            retryCount = 0;
            CurrentStatus = Status.Interrogating;

            OnComplete = onComplete;

            interrogationTimer = new ConnectInterrogationTimer(LEGODeviceManager.Instance,
                () => serviceListInterrogator.IsStable, 
                OnInterrogationFinishedAction );

            hubAlertInterrogator = new HubAlertInterrogator(device,
                () => { if (interrogationTimer != null) interrogationTimer.MinorProgressTick(); },
                () => { if (interrogationTimer != null) interrogationTimer.GotHubAlerts();}
            );

            hubPropertyInterrogator = new HubPropertyInterrogator(device,
                () => { if (interrogationTimer != null) interrogationTimer.MinorProgressTick(); },
                () => {
                    device.DidGetAllHubProperties();
                    if (interrogationTimer != null) interrogationTimer.GotHubProperties();
                }
            );

            serviceListInterrogator = new ServiceListInterrogator(device,
                () => { if (interrogationTimer != null) interrogationTimer.MinorProgressTick(); },
                () => { if (interrogationTimer != null) interrogationTimer.MajorProgressTick(); }
            );                

            logger.Debug("DeviceInterrogationState created for "+device.DeviceID);
        }

        private Action OnComplete = null;
        private void OnInterrogationFinishedAction (bool success)
        {
            if (!success) 
            {
                var sb = new StringBuilder();
                hubAlertInterrogator.ReportMissing(sb);
                sb.Append("\n");
                hubPropertyInterrogator.ReportMissing(sb);
                sb.Append("\n");
                serviceListInterrogator.ReportMissing(sb);
                logger.Debug("Missing in interrogation: " + sb.ToString());
                
                if (retryCount >= maxRetries)
                {
                    interrogationTimer = null;
                    CurrentStatus = Status.Failed;
                    logger.Debug("Maximum retry interrogation exceeded interrogation failed: retries:" + retryCount);
                    if (OnComplete != null)
                    {
                        OnComplete();
                    }
                    legoDevice?.RequestToDisconnect();
                }
                else
                {
                    if (interrogationTimer != null)
                    {
                        if (legoDevice?.State == DeviceState.Interrogating &&
                            interrogationTimer.CurrentState == ConnectInterrogationTimer.InterrogationState.InProgress)
                        {   // try a retry
                            retryCount++;

                            logger.Debug("Retrying missing items in interrogation: retry:" + retryCount);

                            interrogationTimer.Restart();

                            hubAlertInterrogator.ResendMissing();
                            hubPropertyInterrogator.ResendMissing();
                            serviceListInterrogator.ResendMissing();
                        }
                        else
                        {   // no longer in interrogation phase - probably disconnected/hanging or cancelled - terminate
                            logger.Debug("device no longer in interrogation phase - interrogation failed");
                            interrogationTimer = null;
                            CurrentStatus = Status.Failed;
                    
                            if (OnComplete != null)
                            {
                                OnComplete();
                            }
                            //legoDevice?.Disconnect();
                            legoDevice?.RequestToDisconnect();
                        }
                    }
                    else
                    {   //Interrogation timer is null!!
                        logger.Debug("interrogation timer is null! Interrogation failed");
                        
                        CurrentStatus = Status.Failed;
                        if (OnComplete != null)
                        {
                            OnComplete();
                        }
                        //legoDevice?.Disconnect();
                        legoDevice?.RequestToDisconnect(); // just in case - it should be closed already can use more aggressive legoDevice?.Disconnect();
                    }
                }
            }
            else
            {   // Interrogation complete with success
                interrogationTimer = null;
                CurrentStatus = Status.Completed;
                logger.Debug("Interrogation phase complete");
                if (OnComplete != null)
                {
                    OnComplete();
                }
            }
        }
        
        
        #region Events
        public void DidDisconnect() 
        {
            if (interrogationTimer != null) 
                interrogationTimer.Cancel();
            //interrogationTimer = null;
        }

        public void HubPropertyReceived(HubProperty property) 
        {
            hubPropertyInterrogator.HubPropertyReceived(property);
        }
        
        public void HubAlertReceived(HubAlert alert) 
        {
            hubAlertInterrogator.HubAlertReceived(alert);
        }

        public void HandleMessage(MessagePortConnectivityRelated msg)
        {
            serviceListInterrogator.HandleMessage(msg);
        }

        public void HandleMessage(MessagePortMetadataRelated msg)
        {
            serviceListInterrogator.HandleMessage(msg);
        }
        #endregion
    }
}
