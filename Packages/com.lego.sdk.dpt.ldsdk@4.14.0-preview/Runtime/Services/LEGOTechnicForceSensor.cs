namespace LEGODeviceUnitySDK
{
    public class LEGOTechnicForceSensor : LEGOService
    {
        public override string ServiceName { get { return "Technic Force Sensor"; } }
        public LEGOTechnicForceSensor(LEGOService.Builder builder) : base(builder) { }
        
        protected override int DefaultModeNumber { get { return 0; } }
    }
}