using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace LEGODeviceUnitySDK
{

    public class LEGOTechnic3AxisOrientationSensor : LEGOService
    {
        public override string ServiceName { get { return "Technic 3-Axis Orientation Sensor"; } }
        public LEGOTechnic3AxisOrientationSensor(LEGOService.Builder builder) : base(builder) { }

        public LEGOValue Degrees { get { return ValueForMode( 0 /* only has one mode */ ); } }

        /* TODO - New specific observer has been postponed; that system may be on its way out anyway.
        protected override void NotifySpecificObservers(LEGOValue oldValue, LEGOValue newValue)
        {
            if (newValue.Mode == 0)
                _delegates.OfType<ILEGOTechnic3AxisOrientationSensorDelegate>().ForEach(serviceDelegate => (serviceDelegate).DidUpdateOrientation(this, oldValue, newValue) );
        }
        */
    }
    
}