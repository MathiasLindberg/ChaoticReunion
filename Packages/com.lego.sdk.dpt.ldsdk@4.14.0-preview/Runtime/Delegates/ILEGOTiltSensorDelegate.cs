using UnityEngine;
using System.Collections;

namespace LEGODeviceUnitySDK
{
	public interface ILEGOTiltSensorDelegate : ILEGOServiceDelegate
	{
		void DidUpdateAngle(LEGOTiltSensor tiltSensor, LEGOValue oldAngle, LEGOValue newAngle);
		void DidUpdateCrash(LEGOTiltSensor tiltSensor, LEGOValue oldCrash, LEGOValue newCrash);
		void DidUpdateDirection(LEGOTiltSensor tiltSensor, LEGOValue oldDirection, LEGOValue newDirection);
	}
}
