
namespace LEGODeviceUnitySDK
{
    public interface ILEGOColorSensorDelegate : ILEGOServiceDelegate
    {

        void DidUpdateColorIndexFrom(LEGOColorSensor colorSensor, LEGOValue oldColorIndex, LEGOValue newColorIndex);

        void DidUpdateTagFrom(LEGOColorSensor colorSensor, LEGOValue oldColorIndex, LEGOValue newColorIndex);

        void DidUpdateReflectionFrom(LEGOColorSensor colorSensor, LEGOValue oldReflection, LEGOValue newReflection);

    }
}