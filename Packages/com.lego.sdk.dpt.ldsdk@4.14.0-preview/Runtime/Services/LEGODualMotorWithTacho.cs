using System;
using dk.lego.devicesdk.bluetooth.V3.messages;

namespace LEGODeviceUnitySDK
{

    public class LEGODualMotorWithTacho : LEGOMotorWithTacho, ILEGODualTachoMotor
    {
        public override string ServiceName { get { return "Virtual Motor With Tacho"; } }

        public LEGODualMotorWithTacho(LEGOService.Builder builder) : base(builder) { }

        public abstract class DualMotorCommand : LEGOSingleMotorWithTacho.MotorCommand
        {
        }


        public class SetDualSpeedCommand : DualMotorCommand, ITachoMotorCommandWithMaxPower, ITachoMotorCommandWithProfileConfig
        {
            public int Speed1;
            public int Speed2;
            public int MaxPower = 100;
            public MotorWithTachoProfileConfiguration ProfileConfig = MotorWithTachoProfileConfiguration.None;


            public void SetMaxPower(int maxPower)
            {
                MaxPower = maxPower;
            }

            public void SetProfileConfiguration(MotorWithTachoProfileConfiguration profileConfig)
            {
                ProfileConfig = profileConfig;
            }

            protected override CommandPayload MakeCommandPayload()
            {
                return new CommandPayload(PortOutputCommandSubCommandTypeEnum.START_SPEED_MOTOR_ONE_AND_TWO,
                    new byte[] { EncodeSignedSpeed(Speed1), EncodeSignedSpeed(Speed2), EncodePower(MaxPower), EncodeProfileConfig(ProfileConfig) });
            }
        }

        public class SetDualSpeedAndDegreesCommand : SetDualSpeedCommand, ITachoMotorCommandWithEndState
        {
            public int Degrees;
            public MotorWithTachoEndState EndState = MotorWithTachoEndState.Braking;

            public void SetEndState(MotorWithTachoEndState endState)
            {
                EndState = endState;
            }

            protected override CommandPayload MakeCommandPayload()
            {
                //TODO: normalize degrees sign
                return new CommandPayload(PortOutputCommandSubCommandTypeEnum.START_SPEED_FOR_DEGREES_MOTOR_ONE_AND_TWO,
                    new PayloadBuilder(9)
                    .PutUInt32(EncodeUnsignedDegrees(Degrees))
                    .Put(EncodeSignedSpeed(Speed1))
                    .Put(EncodeSignedSpeed(Speed2))
                    .Put(EncodePower(MaxPower))
                    .Put(EncodeEndState(EndState))
                    .Put(EncodeProfileConfig(ProfileConfig))
                    .GetBytes());
            }
        }

        public class SetDualSpeedPositionCommand : SetDualSpeedCommand, ITachoMotorCommandWithEndState
        {
            public int Position1;
            public int Position2;
            public MotorWithTachoEndState EndState = MotorWithTachoEndState.Braking;

            public void SetEndState(MotorWithTachoEndState endState)
            {
                EndState = endState;
            }

            protected override CommandPayload MakeCommandPayload()
            {
                //TODO: Zero-check speed.
                var speed = Math.Max(Speed1, Speed2); //TODO: Fix wrong inheritance.
                return new CommandPayload(PortOutputCommandSubCommandTypeEnum.START_SPEED_GOTO_ABSOLUTE_POSITION_MOTOR_ONE_AND_TWO,
                    new PayloadBuilder(12)
                    .PutInt32(EncodePosition(Position1))
                    .PutInt32(EncodePosition(Position2))
                    .Put(EncodeSignedSpeed(speed))
                    .Put(EncodePower(MaxPower))
                    .Put(EncodeEndState(EndState))
                    .Put(EncodeProfileConfig(ProfileConfig))
                    .GetBytes());
            }
        }

        public class SetDualSpeedAndSecondsCommand : SetDualSpeedCommand, ITachoMotorCommandWithEndState
        {
            public int MilliSeconds;
            public MotorWithTachoEndState EndState = MotorWithTachoEndState.Braking;

            public void SetEndState(MotorWithTachoEndState endState)
            {
                EndState = endState;
            }

            protected override CommandPayload MakeCommandPayload()
            {
                return new CommandPayload(PortOutputCommandSubCommandTypeEnum.SPEED_FOR_TIME_MOTOR_ONE_AND_TWO,
                    new PayloadBuilder(7)
                    .PutUInt16(EncodeDuration(MilliSeconds))
                    .Put(EncodeSignedSpeed(Speed1))
                    .Put(EncodeSignedSpeed(Speed2))
                    .Put(EncodePower(MaxPower))
                    .Put(EncodeEndState(EndState))
                    .Put(EncodeProfileConfig(ProfileConfig))
                    .GetBytes());
            }
        }

        public class SetDualPowerCommand : DualMotorCommand
        {
            public int Power1;
            public int Power2;

            protected override CommandPayload MakeCommandPayload()
            {
                return new CommandPayload(PortOutputCommandSubCommandTypeEnum.START_POWER_MOTOR_ONE_AND_TWO,
                    new byte[] { EncodeSignedPower(Power1), EncodeSignedPower(Power2) });
            }
        }

        public class SetDualPresetCommand : DualMotorCommand
        {
            public int Preset1;
            public int Preset2;

            protected override CommandPayload MakeCommandPayload()
            {
                return new CommandPayload(PortOutputCommandSubCommandTypeEnum.PRESET_ENCODER_MOTOR_ONE_AND_TWO,
                    new PayloadBuilder(8)
                    .PutInt32(EncodePosition(Preset1))
                    .PutInt32(EncodePosition(Preset2))
                    .GetBytes());
            }
        }
    }
}

