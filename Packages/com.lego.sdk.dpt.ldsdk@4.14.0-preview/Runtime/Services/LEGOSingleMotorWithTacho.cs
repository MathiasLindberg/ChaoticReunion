
namespace LEGODeviceUnitySDK
{

    public class LEGOSingleMotorWithTacho : LEGOMotorWithTacho
    {
        public override string ServiceName { get { return "Single Motor With Tacho"; } }

        internal LEGOSingleMotorWithTacho(LEGOService.Builder builder) : base(builder) { }
    }

}