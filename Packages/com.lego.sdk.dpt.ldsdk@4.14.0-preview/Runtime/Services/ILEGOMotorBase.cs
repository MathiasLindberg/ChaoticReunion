namespace LEGODeviceUnitySDK
{
	public interface ILEGOMotorBase : ILEGOService
	{
		LEGOServiceCommand SetPowerCommand(int power);
		LEGOServiceCommand BrakeCommand();
        LEGOServiceCommand DriftCommand();
	}
}
