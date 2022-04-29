using System;

namespace LEGODeviceUnitySDK
{
    public interface ILEGOMoveSensorDelegate : ILEGOServiceDelegate
    {
        void DidUpdateSpeed(LEGOMoveSensor moveSensor, LEGOValue oldSpeed, LEGOValue newSpeed);

    }
}
