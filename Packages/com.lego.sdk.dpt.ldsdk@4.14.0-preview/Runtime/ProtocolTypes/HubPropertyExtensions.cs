using dk.lego.devicesdk.bluetooth.V3.messages;

namespace LEGODeviceUnitySDK
{
    public static class HubPropertyExtensions
    {
        /// <summary>
        /// Whether a proper can be set or reset.
        /// </summary>
        public static bool CanBeSet(this HubProperty property) {
            switch (property) {
            case HubProperty.NAME:
            case HubProperty.HARDWARE_NETWORK_ID:
            case HubProperty.SPEAKER_VOLUME:
                return true;
            default:
                return false;
            }
        }

        /// <summary>
        /// Can a property be subscribed to?
        /// </summary>
        public static bool CanBeSubscribedTo(this HubProperty property) {
            switch (property) {
            case HubProperty.NAME:
            case HubProperty.BUTTON:
            case HubProperty.RSSI:
            case HubProperty.BATTERY_VOLTAGE:
                return true;
            default:
                return false;
            }
        }

    }
}

