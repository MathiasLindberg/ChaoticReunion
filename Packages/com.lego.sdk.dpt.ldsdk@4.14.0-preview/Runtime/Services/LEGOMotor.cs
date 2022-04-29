using System;
using System.Collections.Generic;
using dk.lego.devicesdk.bluetooth.V3.messages;

namespace LEGODeviceUnitySDK
{
    public class LEGOMotor : LEGOService, ILEGOMotor
    {
        public override string ServiceName { get { return "Motor"; } }
        protected override int DefaultModeNumber { get { return 0; } }
        protected override int DefaultInputFormatDelta { get { return 1; } }
        protected override bool DefaultInputFormatEnableNotifications { get { return false; } }

        internal LEGOMotor(LEGOService.Builder builder) : base(builder) {}
		
        public abstract class MotorCommand : LEGOServiceCommand
        {
            public override ICollection<IOType> SupportedIOTypes { get { return IOTypes.PowerOnlyMotors; } }                        
        }

        public class SetPowerCommand : MotorCommand 
        {
            public int Power;

            protected override CommandPayload MakeCommandPayload()
            {
                return new CommandPayload(PortOutputCommandSubCommandTypeEnum.DIRECT_MODE_WRITE,
                    new byte[] {0x00, EncodeSignedPower(Power)});
            }
        }

        public class BrakeCommand : MotorCommand 
        {
            protected override CommandPayload MakeCommandPayload()
            {
                return new CommandPayload(PortOutputCommandSubCommandTypeEnum.DIRECT_MODE_WRITE,
                    new byte[] {0x00, MOTOR_POWER_BRAKE});
            }
        }

        public class DriftCommand : MotorCommand 
        {
            protected override CommandPayload MakeCommandPayload()
            {
                return new CommandPayload(PortOutputCommandSubCommandTypeEnum.DIRECT_MODE_WRITE,
                    new byte[] {0x00, MOTOR_POWER_DRIFT});
            }
        }

        #region ILEGOMotorBase implementation
        LEGOServiceCommand ILEGOMotorBase.SetPowerCommand(int power)
        {
            return new SetPowerCommand { Power = power };
        }

        LEGOServiceCommand ILEGOMotorBase.BrakeCommand()
        {
            return new BrakeCommand { };
        }

        LEGOServiceCommand ILEGOMotorBase.DriftCommand()
        {
            return new DriftCommand { };
        }
        #endregion

    }
}