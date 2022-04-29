using UnityEngine;
using System.Collections;

namespace LEGODeviceUnitySDK
{
	public interface ILEGOCurrentSensorDelegate : ILEGOServiceDelegate
	{
        void DidUpdateMilliAmp(LEGOCurrentSensor currentSensor, LEGOValue oldMilliAmp, LEGOValue newMilliAmp);
	}
}
