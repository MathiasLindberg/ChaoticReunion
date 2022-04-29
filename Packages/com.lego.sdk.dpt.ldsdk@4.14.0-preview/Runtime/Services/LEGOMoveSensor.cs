
namespace LEGODeviceUnitySDK
{
    public class LEGOMoveSensor : LEGOService
    {
        public override string ServiceName { get { return "Move Sensor"; } }
        protected override int DefaultModeNumber { get { return 0; } }

        public LEGOValue Speed { get { return ValueForMode(0 /* Only has one mode */); } }

        public LEGOMoveSensor(LEGOService.Builder builder) : base(builder) { }

        protected override void NotifySpecificObservers(LEGOValue oldValue, LEGOValue newValue)
        {
            if (newValue.Mode == 0)
                _delegates.OfType<ILEGOMoveSensorDelegate>().ForEach(serviceDelegate => (serviceDelegate).DidUpdateSpeed(this, oldValue, newValue));
        }

    }
}