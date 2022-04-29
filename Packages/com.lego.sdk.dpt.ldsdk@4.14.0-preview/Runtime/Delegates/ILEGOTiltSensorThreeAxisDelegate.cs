using UnityEngine;
using System.Collections;

namespace LEGODeviceUnitySDK
{
	public interface ILEGOTiltSensorThreeAxisDelegate : ILEGOServiceDelegate
	{
		void DidUpdateAngle(LEGOTiltSensorThreeAxis tiltSensor, LEGOValue oldAngle, LEGOValue newAngle);
        void DidUpdateImpact(LEGOTiltSensorThreeAxis tiltSensor, LEGOValue oldImpact, LEGOValue newImpact);
        void DidUpdateDirection(LEGOTiltSensorThreeAxis tiltSensor, LEGOValue oldDirection, LEGOValue newDirection);
        void DidUpdateAcceleration(LEGOTiltSensorThreeAxis tiltSensor, LEGOValue oldAcceleration, LEGOValue newAcceleration);
        void DidUpdateOrientation(LEGOTiltSensorThreeAxis tiltSensor, LEGOValue oldOrientation, LEGOValue newOrientation);
	}
}
