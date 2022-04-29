using UnityEngine;
using System.Collections;

namespace LEGODeviceUnitySDK
{
	public interface ILEGOMotionSensorDelegate : ILEGOServiceDelegate
	{
		void DidUpdateCount(LEGOMotionSensor motionSensor, LEGOValue oldCount, LEGOValue newCount);
		void DidUpdateDistance(LEGOMotionSensor motionSensor, LEGOValue oldDistance, LEGOValue newDistance);
	}
}
