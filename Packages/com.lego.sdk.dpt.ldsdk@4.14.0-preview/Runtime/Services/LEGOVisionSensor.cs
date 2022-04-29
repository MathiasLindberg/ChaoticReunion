using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using dk.lego.devicesdk.bluetooth.V3.messages;


namespace LEGODeviceUnitySDK
{
    public class LEGOVisionSensor : LEGOService, ILEGORGBLight
    {
        public override string ServiceName { get { return "Vision Sensor"; } }
        protected override int DefaultModeNumber { get { return (int)LEVisionSensorMode.Spec1; } }

        public LEGOValue MeasuredColor    { get { return ValueForMode((int)LEVisionSensorMode.Color); }  }
        public LEGOValue Detect           { get { return ValueForMode((int)LEVisionSensorMode.Detect); }  }
        public LEGOValue Count            { get { return ValueForMode((int)LEVisionSensorMode.Count); }  }
        public LEGOValue Reflection       { get { return ValueForMode((int)LEVisionSensorMode.Reflection); }  }
        public LEGOValue Ambient          { get { return ValueForMode((int)LEVisionSensorMode.Ambient); }  }
        public LEGOValue MeasuredColorRGB { get { return ValueForMode((int)LEVisionSensorMode.RGBRaw); }  }

        internal LEGOVisionSensor(LEGOService.Builder builder) : base(builder) { }

        protected override void NotifySpecificObservers(LEGOValue oldValue, LEGOValue newValue)
        {
            switch ((LEVisionSensorMode)newValue.Mode)
            {
                case LEVisionSensorMode.Color:
                    _delegates.OfType<ILEGOVisionSensorDelegate>().ForEach( serviceDelegate => serviceDelegate.DidUpdateMeasuredColorFrom(this, oldValue, newValue) );                    
                    break;
                case LEVisionSensorMode.Detect:
                    _delegates.OfType<ILEGOVisionSensorDelegate>().ForEach( serviceDelegate => serviceDelegate.DidUpdateDetectFrom(this, oldValue, newValue) );                                        
                    break;
                case LEVisionSensorMode.Count:
                    _delegates.OfType<ILEGOVisionSensorDelegate>().ForEach( serviceDelegate => serviceDelegate.DidUpdateCountFrom(this, oldValue, newValue) );                                        
                    break;
                case LEVisionSensorMode.Reflection:
                    _delegates.OfType<ILEGOVisionSensorDelegate>().ForEach( serviceDelegate => serviceDelegate.DidUpdateReflectionFrom(this, oldValue, newValue) );                                        
                    break;
                case LEVisionSensorMode.Ambient:
                    _delegates.OfType<ILEGOVisionSensorDelegate>().ForEach( serviceDelegate => serviceDelegate.DidUpdateAmbientFrom(this, oldValue, newValue) );                                        
                    break;
                case LEVisionSensorMode.RGBRaw:
                    _delegates.OfType<ILEGOVisionSensorDelegate>().ForEach( serviceDelegate => serviceDelegate.DidUpdateRGBFrom(this, oldValue, newValue) );                                        
                    break;
            }
        }

        public abstract class VisionSensorCommand : LEGOServiceCommand
        {
            protected CommandPayload PayloadForColorIndex(byte index) {
                return new CommandPayload(PortOutputCommandSubCommandTypeEnum.DIRECT_MODE_WRITE,
                    new byte[] {(byte)LEVisionSensorMode.ColorIndexOut, index});
            }

            private static readonly ICollection<IOType> VisionSensorOnly = new List<IOType> { IOType.LEIOTypeVisionSensor };
            public override ICollection<IOType> SupportedIOTypes {
                get { return VisionSensorOnly; }
            }

        }

        public class SetColorIndexCommand : VisionSensorCommand 
        {
            public int ColorIndex;

            protected override CommandPayload MakeCommandPayload()
            {
                return PayloadForColorIndex((byte)Mathf.Clamp(ColorIndex, 0, 255));
            }
        }

        public class SetDefaultColorCommand : VisionSensorCommand 
        {
            protected override CommandPayload MakeCommandPayload()
            {
                const byte defaultColor = 3; // Blue
                return PayloadForColorIndex(defaultColor);
            }
        }
        
        public class SwitchOffCommand : VisionSensorCommand 
        {
            protected override CommandPayload MakeCommandPayload()
            {
                return PayloadForColorIndex(0xFF);
            }
        }
    }

    public enum LEVisionSensorMode
    {
        Color = 0,
        /** Range detection */
        Detect = 1,
        /** Obstruction count */
        Count = 2,
        /** Reflection measure */
        Reflection = 3,
        /** Ambient lighting measure */
        Ambient = 4,
        /** LED color control (Out) */
        ColorIndexOut = 5,
        /** Color measure (RGB) */
        RGBRaw = 6,
        /** Infrared **/
        IRTx = 7,
        /** Special "combined mode" for the vision sensor **/
        Spec1 = 8,
        /** Debug **/
        Debug = 9,
        /** Calibration **/
        Calib = 10,
    }
}