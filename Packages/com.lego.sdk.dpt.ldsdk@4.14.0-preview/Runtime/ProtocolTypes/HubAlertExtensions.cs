using System;
using dk.lego.devicesdk.bluetooth.V3.messages;

namespace LEGODeviceUnitySDK
{
    public static class HubAlertExtensions
    {
        /// <summary>
        /// Can an alert be subscribed on?
        /// </summary>
        public static bool CanBeSubscribedTo(this HubAlert alert) 
        {
            switch (alert) 
            {
            case HubAlert.HIGH_CURRENT:
            case HubAlert.HIGH_POWER_USE:
            case HubAlert.LOW_SIGNAL_STRENGTH:
            case HubAlert.LOW_VOLTAGE:
                return true;
            default:
                return false;
            }
        }
        
        public static bool IsSupportedByHub(this HubAlert alert, AbstractLEGODevice.DeviceType deviceType) 
        {
            switch (alert) 
            {
                case HubAlert.HIGH_CURRENT:
                case HubAlert.HIGH_POWER_USE:
                    if (deviceType == AbstractLEGODevice.DeviceType.Hub66)
                    {
                        return false;
                    }
                    return true;
                
                case HubAlert.LOW_SIGNAL_STRENGTH:
                case HubAlert.LOW_VOLTAGE:
                    return true;
                default:
                    return false;
            }
        }

    }
}

