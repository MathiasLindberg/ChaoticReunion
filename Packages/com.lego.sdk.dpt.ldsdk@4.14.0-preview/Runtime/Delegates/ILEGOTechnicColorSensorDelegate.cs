namespace LEGODeviceUnitySDK
{
    public interface ILEGOTechnicColorSensorDelegate : ILEGOServiceDelegate
    {
        void DidUpdateColor(LEGOTechnicColorSensor colorSensor, LEGOValue oldAngle, LEGOValue newAngle);
        void DidUpdateReflection(LEGOTechnicColorSensor colorSensor, LEGOValue oldAngle, LEGOValue newAngle);
        void DidUpdateRGBI(LEGOTechnicColorSensor colorSensor,LEGOValue oldAngle, LEGOValue newAngle);
        void DidUpdateHSV(LEGOTechnicColorSensor colorSensor,LEGOValue oldAngle, LEGOValue newAngle);
    }
}