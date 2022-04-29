using dk.lego.devicesdk.bluetooth.V3.messages;
using UnityEngine;
using DeviceType = LEGODeviceUnitySDK.AbstractLEGODevice.DeviceType;

namespace LEGODeviceUnitySDK.LEGODeviceExtensions
{
    /// <summary>
    /// This extension is to add message sending extended capabilities or device information facilities that might be
    /// required by clients.
    /// To use these add:
    /// using LEGODeviceUnitySDK.LEGODeviceExtensions;
    /// to the using list of the class using the LEGODevice class and access the method as normal eg.
    ///     device.SetSpeakerVolume(10);
    /// </summary>
    public static class LEGODeviceExtensions
    {
        /// <summary>
        /// The speaker volume is a property on so far only the LEAF hub67. It can be set and
        /// its current value requested  
        /// </summary>
        public static void SetSpeakerVolume(this LEGODevice device, byte volume)
        {
            volume = (byte)Mathf.Clamp(volume, 0, 100);
            device.SendMessage(new MessageHubProperty(0, HubProperty.SPEAKER_VOLUME,
                        HubPropertyOperation.SET, new byte [] {volume}));
        }
        
        /// <summary>
        /// The speaker volume current setting can be requested - the resultant value can be obtained by subscribing to
        /// AbstractLEGODevice OnSpeakerOnVolumeUpdated. The last received volume value is also found in
        /// SpeakerVolumeValue also  
        /// </summary>
        public static void RequestSpeakerVolume(this LEGODevice device)
        {
            device.SendMessage(new MessageHubProperty(0, HubProperty.SPEAKER_VOLUME,
                HubPropertyOperation.REQUEST_UPDATE, new byte [] {}));
        }
        
        /// <summary>
        /// If the hub(device) does not support the type of service then this extension method will return false
        /// </summary>
        public static bool IsServiceSupported(this ILEGODevice device, IOType ioType)
        {
            // if there are more complicated FW version and HW version support issues then a Linq style database
            // should be used here to cope with the extra complexities - however at the time of writing there is
            // only 1 device/2 service incompatibilities and it is across all versions of the HW/FW at the moment so
            // we will keep it simple.
            if(new DeviceType(device.SystemType, device.SystemDeviceNumber) == DeviceType.Hub64)
            {
                if (ioType == IOType.LEIOTypeTechnicColorSensor ||
                    ioType == IOType.LEIOTypeTechnicDistanceSensor)
                {
                    return false;
                }
            }
            return true;
        }
    }
}