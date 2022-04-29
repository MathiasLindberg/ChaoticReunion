using LEGO.Logger;

namespace LEGODeviceUnitySDK
{
    public class LEGOTiltSensor : LEGOService
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(LEGOTiltSensor));

        public override string ServiceName { get { return "Tilt Sensor"; } }
        protected override int DefaultModeNumber { get { return (int)LETiltSensorMode.Angle; } }

        public LEGOValue Angle { get { return ValueForMode((int)LETiltSensorMode.Angle); } }
        public LEGOValue Crash { get { return ValueForMode((int)LETiltSensorMode.Crash); } }
        public LEGOValue Direction { get { return ValueForMode((int)LETiltSensorMode.Tilt); } }

        public LETiltSensorMode TiltSensorMode { get { return InputFormat == null ? LETiltSensorMode.Unknown : (LETiltSensorMode) InputFormat.Mode; } }

        public LEGOTiltSensor(LEGOService.Builder builder) : base(builder) { }

        protected override void NotifySpecificObservers(LEGOValue oldValue, LEGOValue newValue)
        {
            switch ((LETiltSensorMode)newValue.Mode)
            {
                case LETiltSensorMode.Angle:
                    _delegates.OfType<ILEGOTiltSensorDelegate>().ForEach( serviceDelegate => (serviceDelegate).DidUpdateAngle(this, oldValue, newValue) );
                    break;
                case LETiltSensorMode.Crash:
                    _delegates.OfType<ILEGOTiltSensorDelegate>().ForEach( serviceDelegate => (serviceDelegate).DidUpdateCrash(this, oldValue, newValue) );
                    break;
                case LETiltSensorMode.Tilt:
                    _delegates.OfType<ILEGOTiltSensorDelegate>().ForEach( serviceDelegate => (serviceDelegate).DidUpdateDirection(this, oldValue, newValue) );
                    break;
                default:
                    logger.Warn("Received value update for unknown mode: " + newValue.Mode);
                    break;
            }
        }

        public enum LETiltSensorDirection
        {
            Neutral = 0,
            Backward = 3,
            Right = 5,
            Left = 7,
            Forward = 9,
            Unknown = 10
        }

        public enum LETiltSensorMode
        {
            Angle = 0,
            Tilt = 1,
            Crash = 2,
            Unknown
        }
    }
}