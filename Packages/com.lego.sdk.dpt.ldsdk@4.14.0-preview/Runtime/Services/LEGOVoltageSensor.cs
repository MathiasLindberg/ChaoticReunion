using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace LEGODeviceUnitySDK
{
	public class LEGOVoltageSensor : LEGOService
	{
        public override string ServiceName { get { return "Voltage Sensor"; } }
        protected override int DefaultModeNumber { get { return 0; } }
        protected override int DefaultInputFormatDelta { get { return 30; } }

        public LEGOValue MilliVolts { get { return ValueForMode(0 /* Only has one mode */); } }

        public LEGOVoltageSensor(LEGOService.Builder builder) : base(builder) { }

        protected override void NotifySpecificObservers(LEGOValue oldValue, LEGOValue newValue)
        {
            if (newValue.Mode == 0)
                _delegates.OfType<ILEGOVoltageSensorDelegate>().ForEach(  serviceDelegate => (serviceDelegate).DidUpdateMilliVolts(this, oldValue, newValue) );
        }

	}
}