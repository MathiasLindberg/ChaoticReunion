using LEGO.Logger;

namespace LEGODeviceUnitySDK
{
    public class LEGOMotionSensor : LEGOService
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(LEGOMotionSensor));

        public override string ServiceName { get { return "Motion Sensor"; } }
        protected override int DefaultModeNumber { get { return 0; } }

        public LEGOValue Distance { get { return ValueForMode((int)LEMotionSensorMode.Detect); } }
        public LEGOValue Count { get { return ValueForMode((int)LEMotionSensorMode.Count); } }

        public LEMotionSensorMode MotionSensorMode { get { return InputFormat == null ? LEMotionSensorMode.Unknown : (LEMotionSensorMode)InputFormat.Mode; } }

        internal LEGOMotionSensor(LEGOService.Builder builder) : base(builder) { }

        protected override void NotifySpecificObservers(LEGOValue oldValue, LEGOValue newValue)
        {
            switch ((LEMotionSensorMode)newValue.Mode)
            {
                case LEMotionSensorMode.Detect:
                    _delegates.OfType<ILEGOMotionSensorDelegate>().ForEach( serviceDelegate => serviceDelegate.DidUpdateCount(this, oldValue, newValue) );
                    break;
                case LEMotionSensorMode.Count:
                    _delegates.OfType<ILEGOMotionSensorDelegate>().ForEach( serviceDelegate => serviceDelegate.DidUpdateDistance(this, oldValue, newValue) );
                    break;
                default:
                    logger.Warn("Received value update for unknown mode: " + newValue.Mode);
                    break;
            }
        }

        public enum LEMotionSensorMode
        {
            Detect = 0,
            Count = 1,
            Unknown
        }
    }
}
