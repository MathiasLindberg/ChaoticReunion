using System.Collections.Generic;
using dk.lego.devicesdk.bluetooth.V3.messages;
using UnityEngine;

namespace LEGODeviceUnitySDK
{
    public class LEGOTechnicDistanceSensor : LEGOService, ILEGOSingleColorLight
    {
        public override string ServiceName { get { return "Technic Distance Sensor"; } }

        public LEGOTechnicDistanceSensor(LEGOService.Builder builder) : base(builder) { }
        
        protected override int DefaultModeNumber { get { return 0; } }
        
        public class SetLightPercentageCommand : LEGOServiceCommand
        {
            private const byte MODE_DIRECT_PERCENT = 0x05;
            public int Percentage0; //UpperLeftLED
            public int Percentage1; //UpperRightLED
            public int Percentage2; //LowerLeftLED
            public int Percentage3; //LowerRightLED
            
            public override ICollection<IOType> SupportedIOTypes
            {
                get { return new List<IOType>() {IOType.LEIOTypeTechnicDistanceSensor};  }
            }

            protected override CommandPayload MakeCommandPayload()
            {
                return new CommandPayload(PortOutputCommandSubCommandTypeEnum.DIRECT_MODE_WRITE,
                    new byte[] { MODE_DIRECT_PERCENT, EncodePower(Percentage0), EncodePower(Percentage1), EncodePower(Percentage2), EncodePower(Percentage3) });
            }
        }
    }
}