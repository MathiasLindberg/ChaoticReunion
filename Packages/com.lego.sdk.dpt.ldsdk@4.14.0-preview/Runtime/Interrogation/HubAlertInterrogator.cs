using System;
using dk.lego.devicesdk.bluetooth.V3.messages;
using LEGO.Logger;

namespace LEGODeviceUnitySDK
{

    /// <summary>
    /// Enquiring a device about its hub alerts.
    /// </summary>
    internal class HubAlertInterrogator : CheckListBase<HubAlert, Void, Void>
    {
        #region Constants guiding the interrogation process
        private static readonly HubAlert[] EXPECTED_HUB_ALERTS_V3 = {
            HubAlert.HIGH_CURRENT,
            HubAlert.HIGH_POWER_USE,
            HubAlert.LOW_VOLTAGE,
            HubAlert.LOW_SIGNAL_STRENGTH,
        };
        #endregion

        private static readonly ILog Logger = LogManager.GetLogger(typeof(HubAlertInterrogator));

        private readonly LEGODevice device;

        public HubAlertInterrogator(LEGODevice device, Action onMinorProgress, Action onMajorProgress)
            : base(onMinorProgress, (dummy) => onMajorProgress())
        {
            Logger.Debug("HubAlertInterrogator created for " + device.DeviceID);
            this.device = device;

            AbstractLEGODevice.DeviceType deviceType = new AbstractLEGODevice.DeviceType( device.SystemType, device.SystemDeviceNumber);
            
            // Initialize the sets of things to wait for:
            foreach (var p in EXPECTED_HUB_ALERTS_V3)
            {
                if (p.IsSupportedByHub(deviceType))
                {
                    if (p.CanBeSubscribedTo())
                    {
                        device.SendMessage(new MessageHubAlert(0, p, HubAlertOperation.ENABLE_UPDATES, new byte[] { }));
                    }

                    RegisterAndSendMessage(new MessageHubAlert(0, p, HubAlertOperation.REQUEST_UPDATE, new byte[] { }));
                }
            }
        }

        #region Interrogation events

        public void HubAlertReceived(HubAlert alert)
        {
            CheckOff(alert);
        }
        #endregion

        #region Resend functionality
        public override void ResendMissing()
        {
            foreach (var a in WaitingFor.Keys)
            {
                if (a.CanBeSubscribedTo())
                {
                    device.SendMessage(new MessageHubAlert(0, a, HubAlertOperation.ENABLE_UPDATES, new byte[] { }));
                }
                device.SendMessage(new MessageHubAlert(0, a, HubAlertOperation.REQUEST_UPDATE, new byte[] { }));
            }
        }
        
        #endregion

        #region Sending messages and registering the expectance of an answer.
        private void RegisterAndSendMessage(MessageHubAlert msg)
        {
            if (msg.operation != HubAlertOperation.REQUEST_UPDATE)
            {
                Logger.Error("Assertion error: op=" + msg.operation);
                return;
            }
            MarkAsPending(msg.alert, Void.Instance);
            device.SendMessage(msg);
        }

        #endregion
    }

}
