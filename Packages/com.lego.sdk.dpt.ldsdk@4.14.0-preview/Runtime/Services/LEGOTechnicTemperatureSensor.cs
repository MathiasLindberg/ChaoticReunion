using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace LEGODeviceUnitySDK
{

    public class LEGOTechnicTemperatureSensor : LEGOService
    {
        public override string ServiceName { get { return "Technic Temperatur Sensor"; } }
        public LEGOTechnicTemperatureSensor(LEGOService.Builder builder) : base(builder) { }

        public LEGOValue DegreesCelcius { get { return ValueForMode( 0 /* only has one mode */ ); } }

        /* TODO - New specific observer has been postponed; that system may be on its way out anyway.
        protected override void NotifySpecificObservers(LEGOValue oldValue, LEGOValue newValue)
        {
            if (newValue.Mode == 0)
                _delegates.OfType<ILEGOTechnicTemperatureSensorDelegate>().ForEach(serviceDelegate => (serviceDelegate).DidUpdateTemperature(this, oldValue, newValue) );
        }
        */
    }
}