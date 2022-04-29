using System;
using dk.lego.devicesdk.bluetooth.V3.messages;
using LEGO.Logger;
using System.Text;
using dk.lego.devicesdk.bluetooth.V3bootloader.messages;

namespace LEGODeviceUnitySDK
{
    /*********************************************************************
     * PURPOSE: Coordination of device interrogation.
     * See class DeviceInterrogationState.
     *********************************************************************/
    /* Structure:
    *    BootloaderDeviceInterrogationState ----> ConnectInterrogationTimer
    *               |
    *               v
    *    BootloaderPropertyInterrogator
    */
    public class V3BootloaderDeviceInterrogationState {
        private static readonly ILog logger = LogManager.GetLogger(typeof(V3BootloaderDeviceInterrogationState));

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
        private readonly V3BootloaderPropertyInterrogator propertyInterrogator;

        public V3BootloaderDeviceInterrogationState(V3BootloaderDevice device, Action onComplete) 
        {
            CurrentStatus = Status.Interrogating;
            
            interrogationTimer = new ConnectInterrogationTimer(LEGODeviceManager.Instance,
                () => true,
                (success) => 
                {
                    interrogationTimer = null;
                    if (!success) 
                    {
                        var sb = new StringBuilder();
                        propertyInterrogator.ReportMissing(sb);
                        logger.Debug("Missing in boot loader interrogation: "+sb.ToString());
                        CurrentStatus = Status.Failed;
                    }
                    else
                    {
                        CurrentStatus = Status.Completed;
                    }
                    if (onComplete != null) 
                        onComplete();
                });

            propertyInterrogator = new V3BootloaderPropertyInterrogator(device,
                () => { if (interrogationTimer != null) interrogationTimer.MinorProgressTick(); },
                () => {
                    device.DidGetAllHubProperties();
                    if (interrogationTimer != null) interrogationTimer.GotHubProperties();
                }
            );

            logger.Debug("BootloaderDeviceInterrogationState created for "+device.DeviceID);
        }

        #region Events
        public void DidDisconnect() {
            if (interrogationTimer != null) 
                interrogationTimer.Cancel();
            interrogationTimer = null;
        }

        public void HandleMessage(GetInfoResp msg)
        {
            propertyInterrogator.HandleMessage(msg);
        }
        #endregion
    }

    internal class V3BootloaderPropertyInterrogator : CheckListBase<string, Void, Void> {
        private static readonly ILog logger = LogManager.GetLogger(typeof(V3BootloaderPropertyInterrogator));

        //private readonly V3BootloaderDevice device;

        public V3BootloaderPropertyInterrogator(V3BootloaderDevice device, Action onMinorProgress, Action onMajorProgress)
            : base(onMinorProgress, (dummy)=>onMajorProgress())
        {
            logger.Debug("BootloaderPropertyInterrogator created for "+device.DeviceID);
            //this.device = device;

            // Initialize the sets of things to wait for:
            MarkAsPending("GET_INFO", Void.Instance);
            device.SendMessage(new GetInfoReq());
        }

        #region Interrogation events
        public void HandleMessage(GetInfoResp resp)
        {
            CheckOff("GET_INFO");
        }
        #endregion
    }
}
