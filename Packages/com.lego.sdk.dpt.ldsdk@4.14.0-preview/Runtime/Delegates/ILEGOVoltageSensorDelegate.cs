using UnityEngine;
using System.Collections;

namespace LEGODeviceUnitySDK
{
	public interface ILEGOVoltageSensorDelegate : ILEGOServiceDelegate
	{
        void DidUpdateMilliVolts(LEGOVoltageSensor voltageSensor, LEGOValue oldMilliVolts, LEGOValue newMilliVolts);
	}
}
