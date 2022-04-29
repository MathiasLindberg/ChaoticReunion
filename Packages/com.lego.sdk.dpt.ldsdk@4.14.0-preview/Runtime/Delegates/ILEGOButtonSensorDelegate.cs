using UnityEngine;
using System.Collections;

namespace LEGODeviceUnitySDK
{
	public interface ILEGOButtonSensorDelegate : ILEGOServiceDelegate
	{
        void DidUpdateButton(LEGOButtonSensor buttonSensor, LEGOValue oldButtonVal, LEGOValue newButtonVal);
	}
}
