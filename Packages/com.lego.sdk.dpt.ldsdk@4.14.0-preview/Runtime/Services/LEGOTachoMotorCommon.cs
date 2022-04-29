using System.Collections.Generic;
using dk.lego.devicesdk.bluetooth.V3.messages;

namespace LEGODeviceUnitySDK
{
    /// <summary>
    /// Base-class for all tacho motors.
    ///
    /// Note: We have omitted the didUpdateSpeed, etc. callback for the TachoMotors.
    /// They do not provide any value currently as they are not being used by the LCC, and soon this version
    /// of the SDK will be replaced by a native Unity/C# SDK.
    ///
    /// To reviceve updates register a as listener to the LEGOService.didUpdateValue method. 
    /// </summary>
    public abstract class LEGOTachoMotorCommon : LEGOService, ILEGOTachoMotor 
    {

		public const int MaxAllowedPower = 100;

		public bool IsInternalMotor { get { return IsInternalService; } }

		public LEGOValue Speed { get { return ValueForMode(SpeedModeNo); } }
		public LEGOValue Position { get { return ValueForMode(PositionModeNo); } }
		public LEGOValue Power { get { return ValueForMode(PowerModeNo); } }
	    public LEGOValue AbsolutePosition { get { return ValueForMode(AbsolutePositionModeNo); } }

        protected LEGOTachoMotorCommon(LEGOService.Builder builder) : base(builder) { }

		public abstract int PowerModeNo { get; }
		public abstract int SpeedModeNo { get; }
		public abstract int PositionModeNo { get; }
		public abstract int AbsolutePositionModeNo { get; }

        public abstract class MotorCommand : LEGOServiceCommand
		{			
			public override ICollection<IOType> SupportedIOTypes { get { return IOTypes.TachoMotors; } }
		}

		public class PresetEncoderCommand : MotorCommand
		{
			public int Preset;

            protected override CommandPayload MakeCommandPayload()
            {
                return new CommandPayload(PortOutputCommandSubCommandTypeEnum.DIRECT_MODE_WRITE,
                    new PayloadBuilder(5).Put((byte)MotorWithTachoMode.Position).PutInt32(Preset).GetBytes());
            }
		}

		/// <summary>
		/// An 'Acceleration Profile' determines how the motor should accelerate or decelerate. 
		/// The firwmware on the Hub may define a number of profiles (e.g. straight-curve, ease-in, ease-out)
		/// 
		/// Through this command you may define the acceleration profile and duration that should be applied for 
		/// following motor commands implementing the IMotorCommandWithProfileConfig interface. 
		/// When this command is executed the Hub will remember the profile-configuration until disconnect. 
		/// 
		/// Example scenario: 
		/// 1. Execute this command with ProfileNo=0 and Duration=1000ms 
		/// 2. Execute the SetDecProfileDurationCommand command with ProfileNo=0 and duration=2000ms
		/// 3. Execute a SetSpeedMilliSecondsCommand with Configuration = 'both start and end'
		/// 
		/// Observed behaviour: 
		/// 1. The motor will accelerate to the defined speed in a 'straight-curve' given by profile0 over a period of 1000 ms. 
		/// 2. The motor will decelerate from the defined speed over 2000 ms period. 
		/// 
		/// </summary>
		public class SetAccProfileDurationCommand : MotorCommand
		{
			public int Duration;
			public int ProfileNo;

            protected override CommandPayload MakeCommandPayload()
            {
                return new CommandPayload(PortOutputCommandSubCommandTypeEnum.SET_ACC_TIME,
                    new PayloadBuilder(3).PutUInt16(EncodeDuration(Duration, max:10000)).Put((byte)ProfileNo).GetBytes());
            }
		}

		public class SetDecProfileDurationCommand : MotorCommand
		{
			public int Duration;
			public int ProfileNo;

            protected override CommandPayload MakeCommandPayload()
            {
                return new CommandPayload(PortOutputCommandSubCommandTypeEnum.SET_DEC_TIME,
                    new PayloadBuilder(3).PutUInt16(EncodeDuration(Duration, max:10000)).Put((byte)ProfileNo).GetBytes());
            }
		}

		public class BrakeCommand : MotorCommand
		{
            protected override CommandPayload MakeCommandPayload()
            {
                return new CommandPayload(PortOutputCommandSubCommandTypeEnum.DIRECT_MODE_WRITE,
                    new byte[] {(byte)MotorWithTachoMode.Power, MOTOR_POWER_BRAKE});
            }
		}

		public class DriftCommand : MotorCommand
		{
            protected override CommandPayload MakeCommandPayload()
            {
                return new CommandPayload(PortOutputCommandSubCommandTypeEnum.DIRECT_MODE_WRITE,
                    new byte[] {(byte)MotorWithTachoMode.Power, MOTOR_POWER_DRIFT});
            }
		}

		public class HoldCommand : MotorCommand, ITachoMotorCommandWithMaxPower
		{
			public int MaxPower = 100;

            public void SetMaxPower(int maxPower)
            {
                MaxPower = maxPower;
            }

            protected override CommandPayload MakeCommandPayload()
            {
                return new CommandPayload(PortOutputCommandSubCommandTypeEnum.START_SPEED,
                    new byte[] {0, EncodePower(MaxPower), 0});
            }
		}

		public class SetPowerCommand : MotorCommand
		{
			public int Power;

            protected override CommandPayload MakeCommandPayload()
            {
                return new CommandPayload(PortOutputCommandSubCommandTypeEnum.DIRECT_MODE_WRITE,
                    new byte[] {(byte)MotorWithTachoMode.Power, EncodeSignedPower(Power)});
            }
		}

		public class SetSpeedCommand : MotorCommand, ITachoMotorCommandWithMaxPower, ITachoMotorCommandWithProfileConfig
		{
			public int Speed;
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
                return new CommandPayload(PortOutputCommandSubCommandTypeEnum.START_SPEED,
                    new byte[] {EncodeSignedSpeed(Speed), EncodePower(MaxPower), EncodeProfileConfig(ProfileConfig)});
            }
		}

		public class SetSpeedDegreesCommand : SetSpeedCommand, ITachoMotorCommandWithEndState
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
                return new CommandPayload(PortOutputCommandSubCommandTypeEnum.START_SPEED_FOR_DEGREES,
                    new PayloadBuilder(8)
                    .PutInt32(Degrees)
                    .Put(EncodeSignedSpeed(Speed))
                    .Put(EncodePower(MaxPower))
                    .Put(EncodeEndState(EndState))
                    .Put(EncodeProfileConfig(ProfileConfig))
                    .GetBytes());
            }
		}

		public class SetSpeedPositionCommand : SetSpeedCommand, ITachoMotorCommandWithEndState
		{
			public int Position;
			public MotorWithTachoEndState EndState = MotorWithTachoEndState.Braking;

			public void SetEndState(MotorWithTachoEndState endState)
			{
				EndState = endState;
			}

            protected override CommandPayload MakeCommandPayload()
            {
                return new CommandPayload(PortOutputCommandSubCommandTypeEnum.START_SPEED_GOTO_ABSOLUTE_POSITION,
                    new PayloadBuilder(8)
                    .PutInt32(EncodePosition(Position))
                    .Put(EncodeSignedSpeed(Speed))
                    .Put(EncodePower(MaxPower))
                    .Put(EncodeEndState(EndState))
                    .Put(EncodeProfileConfig(ProfileConfig))
                    .GetBytes());
            }
		}

		public class SetSpeedMilliSecondsCommand : SetSpeedCommand, ITachoMotorCommandWithEndState
		{
			public int MilliSeconds;
			public MotorWithTachoEndState EndState = MotorWithTachoEndState.Braking;

			public void SetEndState(MotorWithTachoEndState endState)
			{
				EndState = endState;
			}

            protected override CommandPayload MakeCommandPayload()
            {
                return new CommandPayload(PortOutputCommandSubCommandTypeEnum.SPEED_FOR_TIME,
                    new PayloadBuilder(6).PutUInt16(EncodeDuration(MilliSeconds))
                    .Put(EncodeSignedSpeed(Speed))
                    .Put(EncodePower(MaxPower))
                    .Put(EncodeEndState(EndState))
                    .Put(EncodeProfileConfig(ProfileConfig))
                    .GetBytes());
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