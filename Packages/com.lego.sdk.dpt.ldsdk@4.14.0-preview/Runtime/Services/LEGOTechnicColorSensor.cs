using System.Collections.Generic;
using dk.lego.devicesdk.bluetooth.V3.messages;
using UnityEngine;

namespace LEGODeviceUnitySDK
{
    public class LEGOTechnicColorSensor : LEGOService
    {
        public enum LEGOTechnicColorSensorMode
        {
            Color = 0,
            Reflection = 1, 
            Ambient = 2,
            RREFL = 4,
            RGBI = 5,
            HSV = 6,
            SHVS = 7,
            DEBUG = 8
        }
        
        public override string ServiceName { get { return "Technic Color Sensor"; } }

        public LEGOTechnicColorSensor(LEGOService.Builder builder) : base(builder) { }
        
        protected override int DefaultModeNumber { get { return 0; } } // color sensor is default
        

        protected override void NotifySpecificObservers(LEGOValue oldValue, LEGOValue newValue)
        {
            switch ((LEGOTechnicColorSensorMode)newValue.Mode)
            {
                case LEGOTechnicColorSensorMode.Color:
                    _delegates.OfType<ILEGOTechnicColorSensorDelegate>().ForEach( serviceDelegate => serviceDelegate.DidUpdateColor(this, oldValue, newValue) );                    
                    break;
                case LEGOTechnicColorSensorMode.Reflection:
                    _delegates.OfType<ILEGOTechnicColorSensorDelegate>().ForEach( serviceDelegate => serviceDelegate.DidUpdateReflection(this, oldValue, newValue) );
                    break;
                case LEGOTechnicColorSensorMode.RGBI:
                    _delegates.OfType<ILEGOTechnicColorSensorDelegate>().ForEach( serviceDelegate => serviceDelegate.DidUpdateRGBI(this, oldValue, newValue) );
                    break;
                case LEGOTechnicColorSensorMode.HSV:
                    _delegates.OfType<ILEGOTechnicColorSensorDelegate>().ForEach( serviceDelegate => serviceDelegate.DidUpdateHSV(this, oldValue, newValue) );
                    break;
            }
        }

        public class SetLightPercentageCommand : LEGOServiceCommand
        {
            private const byte MODE_DIRECT_PERCENT = 0x03;
            public int Percentage;
            
            public override ICollection<IOType> SupportedIOTypes
            {
                get { return new List<IOType>() {IOType.LEIOTypeTechnicColorSensor};  }
            }

            protected override CommandPayload MakeCommandPayload()
            {
                return new CommandPayload(PortOutputCommandSubCommandTypeEnum.DIRECT_MODE_WRITE,
                    new byte[] { MODE_DIRECT_PERCENT, EncodePower(Percentage), EncodePower(Percentage), EncodePower(Percentage), EncodePower(Percentage) });
            }
        }
    }
}