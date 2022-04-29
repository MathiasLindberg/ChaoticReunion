using System;
using dk.lego.devicesdk.bluetooth.V3.messages;
using LEGO.Logger;

namespace LEGODeviceUnitySDK
{

    /// <summary>
    /// Enquiring a device about its hub properties.
    /// </summary>
    internal class HubPropertyInterrogator : CheckListBase<HubProperty, Void, Void>
    {
        #region Constants guiding the interrogation process
        private static readonly HubProperty[] EXPECTED_HUB_PROPERTIES_V3 = {
            HubProperty.HARDWARE_VERSION,
            HubProperty.FIRMWARE_VERSION,
            HubProperty.RADIO_FIRMWARE_VERSION,
            HubProperty.MANUFACTURER_NAME,
            HubProperty.BATTERY_TYPE,
            HubProperty.WIRELESS_PROTOCOL_VERSION,
            HubProperty.PRIMARY_MAC_ADDRESS,
            //HubProperty.SECONDARY_MAC_ADDRESS,

            //HubProperty.HARDWARE_NETWORK_ID,
            //HubProperty.HARDWARE_SYSTEM_TYPE

        };

        private static readonly HubProperty[] SUBSCRIPTION_HUB_PROPERTIES_V3 = {
            HubProperty.BUTTON,
            HubProperty.RSSI,
            HubProperty.BATTERY_VOLTAGE,
            HubProperty.NAME
        };
        #endregion

        private static readonly ILog Logger = LogManager.GetLogger(typeof(HubPropertyInterrogator));

        private readonly LEGODevice device;

        public HubPropertyInterrogator(LEGODevice device, Action onMinorProgress, Action onMajorProgress)
            : base(onMinorProgress, (dummy)=>onMajorProgress())
        {
            Logger.Debug("HubPropertyInterrogator created for "+device.DeviceID);
            this.device = device;

            // Initialize the sets of things to wait for:
            foreach (var p in EXPECTED_HUB_PROPERTIES_V3) {
                if (p.CanBeSubscribedTo())
                    device.SendMessage(new MessageHubProperty(0, p, HubPropertyOperation.ENABLE_UPDATES, new byte[]{}));
                RegisterAndSendMessage(new MessageHubProperty(0, p, HubPropertyOperation.REQUEST_UPDATE, new byte[]{}));
            }

            foreach (var p in SUBSCRIPTION_HUB_PROPERTIES_V3) { // Don't wait for these.
                if (p.CanBeSubscribedTo())
                    device.SendMessage(new MessageHubProperty(0, p, HubPropertyOperation.ENABLE_UPDATES, new byte[]{}));
                device.SendMessage(new MessageHubProperty(0, p, HubPropertyOperation.REQUEST_UPDATE, new byte[]{}));
            }
        }

        #region Interrogation events
        public void HubPropertyReceived(HubProperty property)
        {
            CheckOff(property);
        }
        #endregion
        
        #region Resend functionality
        public override void ResendMissing()
        {   
            foreach (var p in WaitingFor.Keys)
            {
                if (p.CanBeSubscribedTo())
                {
                    device.SendMessage(
                        new MessageHubProperty(0, p, HubPropertyOperation.ENABLE_UPDATES, new byte[] { }));
                }

                device.SendMessage(new MessageHubProperty(0, p, HubPropertyOperation.REQUEST_UPDATE, new byte[]{}));
            }
        }
        
        #endregion
        
        #region Sending messages and registering the expectance of an answer.
        private void RegisterAndSendMessage(MessageHubProperty msg)
        {
            if (msg.operation != HubPropertyOperation.REQUEST_UPDATE) 
            {
                Logger.Error("Assertion error: op="+msg.operation);
                return;
            }
            MarkAsPending(msg.property, Void.Instance);
            device.SendMessage(msg);
        }
        #endregion
    }
    
}
