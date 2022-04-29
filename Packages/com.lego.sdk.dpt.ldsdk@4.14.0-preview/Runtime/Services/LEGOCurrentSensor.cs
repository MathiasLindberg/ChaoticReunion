using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace LEGODeviceUnitySDK
{
	public class LEGOCurrentSensor : LEGOService
	{
        public override string ServiceName { get { return "Current Sensor"; } }
        protected override int DefaultModeNumber { get { return 0; } }
        protected override int DefaultInputFormatDelta { get { return 30; } }

        public LEGOValue MilliAmp { get { return ValueForMode( 0 /* only has one mode */ ); } }

        internal LEGOCurrentSensor(LEGOService.Builder builder) : base(builder) { }

        protected override void NotifySpecificObservers(LEGOValue oldValue, LEGOValue newValue)
        {
            if (newValue.Mode == 0)
                _delegates.OfType<ILEGOCurrentSensorDelegate>().ForEach(serviceDelegate => (serviceDelegate).DidUpdateMilliAmp(this, oldValue, newValue) );
        }

	}
}