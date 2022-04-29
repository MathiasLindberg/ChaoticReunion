using UnityEngine;

namespace LEGODeviceUnitySDK
{
	public interface ILEGOVisionSensorDelegate : ILEGOServiceDelegate
	{
		void DidUpdateColorIndexFrom(LEGOVisionSensor visionSensor, LEGOValue oldColorIndex, LEGOValue newColorIndex);
		void DidUpdateMeasuredColorFrom(LEGOVisionSensor visionSensor, LEGOValue oldColor, LEGOValue newColor);
		void DidUpdateDetectFrom(LEGOVisionSensor visionSensor, LEGOValue oldDetect, LEGOValue newDetect);
		void DidUpdateCountFrom(LEGOVisionSensor visionSensor, LEGOValue oldCount, LEGOValue newCount);
		void DidUpdateReflectionFrom(LEGOVisionSensor visionSensor, LEGOValue oldReflection, LEGOValue newReflection);
		void DidUpdateAmbientFrom(LEGOVisionSensor visionSensor, LEGOValue oldAmbient, LEGOValue newAmbient);
		void DidUpdateRGBFrom(LEGOVisionSensor visionSensor, LEGOValue oldRGB, LEGOValue newRGB);
	}
}
