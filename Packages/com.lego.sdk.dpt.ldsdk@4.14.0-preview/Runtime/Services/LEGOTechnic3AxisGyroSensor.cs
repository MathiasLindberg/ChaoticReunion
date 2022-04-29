using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace LEGODeviceUnitySDK
{

    public class LEGOTechnic3AxisGyroSensor : LEGOService
    {
        public override string ServiceName { get { return "Technic 3-Axis Gyro Sensor"; } }
        public LEGOTechnic3AxisGyroSensor(LEGOService.Builder builder) : base(builder) { }

        public LEGOValue DegreesPerSecond { get { return ValueForMode( 0 /* only has one mode */ ); } }

        /* TODO - New specific observer has been postponed; that system may be on its way out anyway.
        protected override void NotifySpecificObservers(LEGOValue oldValue, LEGOValue newValue)
        {
            if (newValue.Mode == 0)
                _delegates.OfType<ILEGOTechnic3AxisGyroSensorDelegate>().ForEach(serviceDelegate => (serviceDelegate).DidUpdateGyration(this, oldValue, newValue) );
        }
        */
    }
    
}