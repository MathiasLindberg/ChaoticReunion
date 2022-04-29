using System;

namespace LEGODeviceUnitySDK
{
    /// <summary>
    /// Sensor mode names.
    /// </summary>
    namespace LEGOModeNames
    {

        public static class TachoMotor
        {
            public const string PowerModeName    = "LPF2-M-POW";
            public const string SpeedModeName    = "LPF2-M-SPD";
            public const string PositionModeName = "LPF2-M-POS";
        }

        public static class DrivebaseTachoMotor
        {
            public const string PowerModeName    = "Power";
            public const string SpeedModeName    = "Speed";
            public const string PositionModeName = "Pos";
        }

        public static class Tilt3D
        {
            public const string TiltModeName          = "TILT";
            public const string OrientationModeName   = "ORIENT";
            public const string AngleModeName         = "ANGLE";
            public const string ImpactModeName        = "IMPACT";
            public const string AccelerationModeName  = "ACCEL";
            public const string OrientaionCfgModeName = "ORICFG";
            public const string ImpactCfgModeName     = "IMPCFG";
            public const string FacCalModeName        = "FACCAL";
        }

        public static class Vision
        {
            public const string ColorModeName          = "LPF2-COLOR";
            public const string DetectModeName         = "LPF2-DETECT";
            public const string CountModeName          = "LPF2-COUNT";
            public const string ReflectionModeName     = "LPF2-REFL";
            public const string AmbientModeName        = "LPF2-AMB";
            public const string ColorOutModeName       = "LPF2-COLOUT";
            public const string RawRGBModeName         = "LPF2-RAWRGB";
            public const string IRTransmitModeName     = "LPF2-IRTX";
            public const string DebugModeName          = "LPF2-DEBUG";
            public const string CalibrateModeName      = "LPF2-CAL";
        }

        #region Legacy sensors
        public static class LegacyMotor
        {
            public const string PowerModeName   = "MOTOR-PWR";
        }

        public static class LegacyTilt
        {
            public const string AngleModeName   = "LPF2-ANGLE";
            public const string TiltModeName    = "LPF2-TILT";
            public const string CrashModeName   = "LPF2-CRASH";
        }

        public static class LegacyMotion
        {
            public const string DetectModeName  = "LPF2-DETECT";
            public const string CountModeName   = "LPF2-COUNT";
        }

        public static class LegacyRGB
        {
            public const string ColorModeName   = "RGB-COLOR";
            public const string ValueModeName   = "RGB-VALUE";
        }

        public static class LegacyCurrent
        {
            public const string CurrentModeName = "CURR-VAL";
        }

        public static class LegacyVoltage
        {
            public const string VoltageModeName = "VOLTAGE-VAL";
        }
        #endregion
    }
}

