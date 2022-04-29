using System;

namespace LEGODeviceUnitySDK
{
    public interface IFirmwareUpgradeProcess
    {
        event Action<IFirmwareUpgradeProcess, DeviceFlashState> OnDeviceFlashStateUpdated;
        event Action<IFirmwareUpgradeProcess, FlashErrorCode, string /* error message */> OnDeviceFlashFailed;
        event Action<IFirmwareUpgradeProcess, float /* pct */ > OnFlashProgressPercentageUpdated;
        DeviceFlashState FlashState { get; }
        void StartProcess();
    }
}