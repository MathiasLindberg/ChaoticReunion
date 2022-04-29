namespace LEGODeviceUnitySDK
{
    /// <summary>
    /// An interface for the representation of specific hub firmware update.
    /// </summary>
    public interface IDeviceChecker
    {
        FirmwareUpdateReadiness.Status IsDeviceReady(ILEGODevice device);
    }
}
