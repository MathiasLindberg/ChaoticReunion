using System;
using System.Collections.Generic;
using dk.lego.devicesdk.bluetooth.V3.messages;

namespace LEGODeviceUnitySDK
{
    public class LEGODualMotor : LEGOMotor
    {
        public override string ServiceName { get { return "Dual Plain Motor"; } }

        public LEGODualMotor(LEGOService.Builder builder) : base(builder) { }

        public abstract class DualMotorCommand : LEGOServiceCommand
	    {
            public override ICollection<IOType> SupportedIOTypes { get { return IOTypes.PowerOnlyMotors; } }
	    }

    	public class SetDualPowerCommand : DualMotorCommand 
    	{
    		public int Power1, Power2;

            protected override CommandPayload MakeCommandPayload ()
            {
                return new CommandPayload(PortOutputCommandSubCommandTypeEnum.START_POWER_MOTOR_ONE_AND_TWO,
                    new byte[] {EncodeSignedPower(Power1), EncodeSignedPower(Power2)});
            }
    	}
    }
}