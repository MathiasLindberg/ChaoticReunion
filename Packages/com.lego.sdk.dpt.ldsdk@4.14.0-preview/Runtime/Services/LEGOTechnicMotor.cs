using System;
using UnityEngine;
using LEGO.Logger;

namespace LEGODeviceUnitySDK
{
    public enum TechnicMotorMode
    {
        Power = 0, /* Out only */
        Speed = 1,
        Position = 2,
        AbsolutePosition = 3,
        /*
        Load = 4, // LOAD/pct - XL only
        Calib = 5, // CALIB/raw - is 4 for L??
        */
        Unknown = -1
    }

    public abstract class LEGOTechnicMotor : LEGOTachoMotorCommon
    {
        public LEGOTechnicMotor(LEGOService.Builder builder) : base(builder) { }

        public override int PowerModeNo { get { return (int)TechnicMotorMode.Power; } }
        public override int SpeedModeNo { get { return (int)TechnicMotorMode.Speed; } }
        public override int PositionModeNo { get { return (int)TechnicMotorMode.Position; } }
        public override int AbsolutePositionModeNo { get { return (int)TechnicMotorMode.AbsolutePosition; } }
        protected override int DefaultModeNumber { get { return 0; } }
        public TechnicMotorMode TechnicMotorMode { get { return InputFormat == null ? TechnicMotorMode.Unknown : (TechnicMotorMode)InputFormat.Mode; } }

    }

    public class LEGOSingleTechnicMotor : LEGOTechnicMotor, ILEGOSingleTachoMotor
    {
        public override string ServiceName { get { return "Technic Tacho Motor"; } }

        public LEGOSingleTechnicMotor(LEGOService.Builder builder) : base(builder) { }
    }

    public class LEGODualTechnicMotor : LEGOTechnicMotor, ILEGODualTachoMotor
    {
        public override string ServiceName { get { return "Dual Technic Tacho Motor"; } }

        public LEGODualTechnicMotor(LEGOService.Builder builder) : base(builder) { }
    }

}

