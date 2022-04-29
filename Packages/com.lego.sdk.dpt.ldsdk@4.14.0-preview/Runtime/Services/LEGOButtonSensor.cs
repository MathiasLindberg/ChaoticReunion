namespace LEGODeviceUnitySDK
{
    public class LEGOButtonSensor : LEGOService
    {
        public enum LEButtonSensorMode
        {
            RCKEY = 0,
            KEYA = 1,
            Unknown
        }

        public override string ServiceName
        {
            get { return "Button Sensor"; }
        }

        protected override int DefaultModeNumber
        {
            get { return (int) LEButtonSensorMode.KEYA; }
        }

        protected override int DefaultInputFormatDelta
        {
            get { return 1; }
        }

        public LEGOButtonSensor(Builder builder) : base(builder)
        {
        }

        protected override void NotifySpecificObservers(LEGOValue oldValue, LEGOValue newValue)
        {
            if (newValue.Mode == (int) LEButtonSensorMode.KEYA)
                _delegates.OfType<ILEGOButtonSensorDelegate>().ForEach(serviceDelegate =>
                    (serviceDelegate).DidUpdateButton(this, oldValue, newValue));
        }
    }
}