namespace LEGODeviceUnitySDK
{
    /// <summary>
    /// Pre firmware update check.
    /// </summary>
    public class PreFirmwareUpdateCheck
    {
        private readonly ILEGODevice device;

        public PreFirmwareUpdateCheck(ILEGODevice device)
        {
            this.device = device;
        }

        public FirmwareUpdateReadiness.Status IsDeviceReadyForUpdate()
        {
            if (device == null)
            {
                return FirmwareUpdateReadiness.Status.InternalError;
            }

            // find the appropriate pre firmware update checker (too simple for a factory at the mo)

            IDeviceChecker deviceChecker;

            if (device.SystemType == DeviceSystemType.LEGOTechnic1
                     && device.SystemDeviceNumber == 0)
            {   //Hub128
                deviceChecker = new DeviceCheckerHub128();
            }
            else if (device.SystemType == DeviceSystemType.LEGOSystem1
                     && device.SystemDeviceNumber == 1)
            {   //Hub65
                deviceChecker = new DeviceCheckerHub65();
            }
            else
            {   //unknown device - have no pre check failure information - must be OK 
                return FirmwareUpdateReadiness.Status.OK;
            }

            return deviceChecker.IsDeviceReady(device);
        }
    }
}
