using System;
using System.Collections.Generic;
using LEGO.Logger;
using dk.lego.devicesdk.bluetooth.V3.messages;
using UnityEngine;

namespace LEGODeviceUnitySDK
{

    public enum LETiltSensorThreeAxisOrientationConfiguration
    {
        Normal = 0,
        Forward = 1,
        Backward = 2,
        Left = 3,
        Right = 4,
        UpsideDown = 5,
        AutoActualAsNormal = 6,
        Unknown = 7
    }

    public enum LETiltSensorThreeAxisMode
    {
        Angle = 0,
        Tilt = 1,
        Orientation = 2,
        Impact = 3,
        Acceleration = 4,
        Unknown
    };

    public enum LETiltSensorThreeAxisOrientation
    {
        Normal = 0,
        Forward = 1,
        Backward = 2,
        Left = 3,
        Right = 4,
        UpsideDown = 5,
        Unknown = 7,
    }

    public class LEGOTiltSensorThreeAxis : LEGOService
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(LEGOTiltSensorThreeAxis));

        public override string ServiceName { get { return "Tilt Sensor Three Axis"; } }
        protected override int DefaultModeNumber { get { return (int)LETiltSensorThreeAxisMode.Angle; } }

        public LEGOValue Direction    { get { return ValueForMode((int)LETiltSensorThreeAxisMode.Tilt); } }
        public LEGOValue Orientation  { get { return ValueForMode((int)LETiltSensorThreeAxisMode.Orientation); } }
        public LEGOValue Angle        { get { return ValueForMode((int)LETiltSensorThreeAxisMode.Angle); } }
        public LEGOValue Impact       { get { return ValueForMode((int)LETiltSensorThreeAxisMode.Impact); } }
        public LEGOValue Acceleration { get { return ValueForMode((int)LETiltSensorThreeAxisMode.Acceleration); } }

        public LETiltSensorThreeAxisMode TiltSensorMode { get { return InputFormat == null ? LETiltSensorThreeAxisMode.Unknown : (LETiltSensorThreeAxisMode) InputFormat.Mode; } }

        public LEGOTiltSensorThreeAxis(LEGOService.Builder builder) : base(builder) { }

        protected override void NotifySpecificObservers(LEGOValue oldValue, LEGOValue newValue)
        {
            switch ((LETiltSensorThreeAxisMode)newValue.Mode)
            {
                case LETiltSensorThreeAxisMode.Tilt:
                    _delegates.OfType<ILEGOTiltSensorThreeAxisDelegate>().ForEach( serviceDelegate => (serviceDelegate).DidUpdateDirection(this, oldValue, newValue) );
                    break;
                case LETiltSensorThreeAxisMode.Orientation:
                    _delegates.OfType<ILEGOTiltSensorThreeAxisDelegate>().ForEach( serviceDelegate => (serviceDelegate).DidUpdateOrientation(this, oldValue, newValue) );
                    break;
                case LETiltSensorThreeAxisMode.Angle:
                    _delegates.OfType<ILEGOTiltSensorThreeAxisDelegate>().ForEach( serviceDelegate => (serviceDelegate).DidUpdateAngle(this, oldValue, newValue) );
                    break;
                case LETiltSensorThreeAxisMode.Impact:
                    _delegates.OfType<ILEGOTiltSensorThreeAxisDelegate>().ForEach( serviceDelegate => (serviceDelegate).DidUpdateImpact(this, oldValue, newValue) );
                    break;
                case LETiltSensorThreeAxisMode.Acceleration:
                    _delegates.OfType<ILEGOTiltSensorThreeAxisDelegate>().ForEach( serviceDelegate => (serviceDelegate).DidUpdateAcceleration(this, oldValue, newValue) );
                    break;
                default:
                    logger.Warn("Received value update for unknown mode: " + newValue.Mode);
                    break;
            }
        }

        public abstract class TiltSensorThreeAxisCommand : LEGOServiceCommand
        {
            protected const byte IMPACT_PRESET_MODE = 3;
            protected const byte CONFIG_ORIENTATION_MODE = 5;
            protected const byte CONFIG_IMPACT_MODE = 6;

            private static readonly ICollection<IOType> Tilt3AxisOnly = new List<IOType> { IOType.LEIOTypeInternalTiltSensorThreeAxis };
            public override ICollection<IOType> SupportedIOTypes {
                get { return Tilt3AxisOnly; }
            }

        }

        public class PresetImpactCountCommand : TiltSensorThreeAxisCommand
        {
            public int Count;

            protected override CommandPayload MakeCommandPayload()
            {
                return new CommandPayload(PortOutputCommandSubCommandTypeEnum.DIRECT_MODE_WRITE,
                    new PayloadBuilder(5).Put(IMPACT_PRESET_MODE).PutUInt32((uint)Math.Max(0, Count)).GetBytes());
            }
        }

        public class ConfigureImpactThresholdHoldOffCommand : TiltSensorThreeAxisCommand
        {
            public int Threshold;
            public int HoldOff;

            protected override CommandPayload MakeCommandPayload()
            {
                return new CommandPayload(PortOutputCommandSubCommandTypeEnum.DIRECT_MODE_WRITE,
                    new byte[] {
                        (byte)CONFIG_IMPACT_MODE,
                        (byte)Mathf.Clamp(Threshold, 0, 127),
                        (byte)Mathf.Clamp(HoldOff, 1, 127)
                    });
            }
        }

        public class ConfigureOrientationCommand : TiltSensorThreeAxisCommand
        {
            public LETiltSensorThreeAxisOrientationConfiguration Orientation;
            protected override CommandPayload MakeCommandPayload()
            {
                return new CommandPayload(PortOutputCommandSubCommandTypeEnum.DIRECT_MODE_WRITE,
                    new byte[] {
                        (byte)CONFIG_ORIENTATION_MODE,
                        (byte)Orientation}
                );
            }
       }
    }
}