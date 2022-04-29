using LEGO.Logger;

namespace LEGODeviceUnitySDK
{
    public class LEGOColorSensor : LEGOService
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(LEGOColorSensor));

        public override string ServiceName { get { return "Color Sensor"; } }
        protected override int DefaultModeNumber { get { return (int)LEColorSensorMode.Color; } }

        internal LEGOColorSensor(LEGOService.Builder builder) : base(builder) { }

        public LEGOValue MeasuredColor { get { return ValueForMode((int)LEColorSensorMode.Color); } }
        public LEGOValue Tag { get { return ValueForMode((int)LEColorSensorMode.Tag); } }
        public LEGOValue Reflection { get { return ValueForMode((int)LEColorSensorMode.Reflection); } }

        protected override void NotifySpecificObservers(LEGOValue oldValue, LEGOValue newValue)
        {
            switch ((LEColorSensorMode)newValue.Mode)
            {
                case LEColorSensorMode.Color:
                    _delegates.OfType<ILEGOColorSensorDelegate>().ForEach(serviceDelegate => serviceDelegate.DidUpdateColorIndexFrom(this, oldValue, newValue));
                    break;
                case LEColorSensorMode.Tag:
                    _delegates.OfType<ILEGOColorSensorDelegate>().ForEach(serviceDelegate => serviceDelegate.DidUpdateTagFrom(this, oldValue, newValue));
                    break;
                case LEColorSensorMode.Reflection:
                    _delegates.OfType<ILEGOColorSensorDelegate>().ForEach(serviceDelegate => serviceDelegate.DidUpdateReflectionFrom(this, oldValue, newValue));
                    break;
                case LEColorSensorMode.RGBRaw:
                    logger.Warn("The LEGOColorSensor unity wrapper does not support RGB raw yet");
                    break;
            }
        }

        public enum LEColorSensorMode
        {
            Color = 0,
            /** Range detection */
            Tag = 1,
            /** Obstruction count */
            Reflection = 2,
            /** Reflection measure */
            RGBRaw = 3,

            Unknown
        }


    }

}