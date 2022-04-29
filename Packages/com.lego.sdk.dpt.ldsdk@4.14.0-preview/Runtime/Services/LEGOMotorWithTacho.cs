 using System;
using LEGO.Logger;
using dk.lego.devicesdk.bluetooth.V3.messages;

namespace LEGODeviceUnitySDK
{
    public enum MotorWithTachoMode
    {
        Power = 0,
        /** Speed */
        Speed = 1,
        /** Position */
        Position = 2,
        /** Absolute Position */
        AbsolutePosition = 3,
        /** Unknown */
        Unknown = -1
    }

    public abstract class LEGOMotorWithTacho : LEGOTachoMotorCommon
    {
        // ADDED COMMENT TO MAKE BUILD HAPPEN TO TPA
        public override int PowerModeNo { get { return (int)MotorWithTachoMode.Power; } }
        public override int SpeedModeNo { get { return (int)MotorWithTachoMode.Speed; } }
        public override int PositionModeNo { get { return (int)MotorWithTachoMode.Position; } }
       
        public override int AbsolutePositionModeNo { get { return (int)MotorWithTachoMode.AbsolutePosition; } }

        //public MotorWithTachoMode MotorWithTachoMode { get { return InputFormat == null ? MotorWithTachoMode.Unknown : (MotorWithTachoMode)InputFormat.Mode; } }

        protected LEGOMotorWithTacho (LEGOService.Builder builder) : base(builder)
        {
        }

    }
}

