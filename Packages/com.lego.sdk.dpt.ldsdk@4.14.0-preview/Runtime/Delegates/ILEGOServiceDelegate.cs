namespace LEGODeviceUnitySDK
{
    public interface ILEGOServiceDelegate
    {
    }

    public interface ILEGOGeneralServiceDelegate : ILEGOServiceDelegate
	{
		void DidChangeState(ILEGOService service, ServiceState oldState, ServiceState newState);
        void DidUpdateInputFormat(ILEGOService service, LEGOInputFormat oldFormat, LEGOInputFormat newFormat);
        void DidUpdateInputFormatCombined(ILEGOService service, LEGOInputFormatCombined oldFormat, LEGOInputFormatCombined newFormat);
        void DidUpdateValueData(ILEGOService service, LEGOValue oldValue, LEGOValue newValue);
	}
}