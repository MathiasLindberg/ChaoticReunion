
using UnityEngine;
using System.Collections;

namespace LEGODeviceUnitySDK
{
	public interface ILEGOMotorWithTachoDelegate : ILEGOServiceDelegate
	{
        void DidUpdatePower(LEGOMotorWithTacho motor, LEGOValue oldPower, LEGOValue newPower);
		void DidUpdateSpeed(LEGOMotorWithTacho motor, LEGOValue oldSpeed, LEGOValue newSpeed);
		void DidUpdatePosition(LEGOMotorWithTacho motor, LEGOValue oldPosition, LEGOValue newPosition);
	}
}
