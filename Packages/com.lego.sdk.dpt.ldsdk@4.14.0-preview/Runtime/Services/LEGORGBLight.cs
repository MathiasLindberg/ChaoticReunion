using UnityEngine;
using System.Collections.Generic;
using System;
using dk.lego.devicesdk.bluetooth.V3.messages;
//using Programming.Hardware;

namespace LEGODeviceUnitySDK
{
    public class LEGORGBLight : LEGOService, ILEGORGBLight
    {
        public override string ServiceName { get { return "RGB Light"; } }

        protected override int DefaultModeNumber { get { return (int)RGBLightMode.Discrete; } }

        public RGBLightMode RGBMode { get { return InputFormat == null ? RGBLightMode.Unknown : (RGBLightMode)InputFormat.Mode; } }

        public Color DefaultColor { get; private set; }
        public static int DefaultColorIndex
        {
            get
            {
                return defaultColorIndex;
            }
            set
            {
                defaultColorIndex = value;
            } 
        }
        private static int defaultColorIndex = 3;

        private Color[] _defaultColorSet = new Color[]
            {
                LEGORGBLight.Color0(),
                LEGORGBLight.Color1(),
                LEGORGBLight.Color2(),
                LEGORGBLight.Color3(),
                LEGORGBLight.Color4(),
                LEGORGBLight.Color5(),
                LEGORGBLight.Color6(),
                LEGORGBLight.Color7(),
                LEGORGBLight.Color8(),
                LEGORGBLight.Color9(),
                LEGORGBLight.Color10(),

            };
        public Color[] DefaultColorSet { get { return _defaultColorSet; } }

        internal LEGORGBLight(LEGOService.Builder builder) : base(builder) {}

        #region Internal
        public abstract class RGBLightCommand : LEGOServiceCommand
        {
            protected byte MODE_DIRECT_COLOR_INDEX = 0x00;
            protected byte MODE_DIRECT_COLOR_RGB = 0x01;
            
            public override ICollection<IOType> SupportedIOTypes {
                get { return IOTypes.RGBLights; }
            }
        }

        public class SetColorCommand : RGBLightCommand 
        {
            private RGBColor RGBColor = new RGBColor();

            public Color Color
            {
                get { return RGBColor.Color; }
                set { RGBColor.Color = value; }
            }

            protected override CommandPayload MakeCommandPayload()
            {
                return new CommandPayload(PortOutputCommandSubCommandTypeEnum.DIRECT_MODE_WRITE,
                    new byte[] {
                        MODE_DIRECT_COLOR_RGB,
                        (byte)Mathf.Clamp(RGBColor.R, 0, 255),
                        (byte)Mathf.Clamp(RGBColor.G, 0, 255),
                        (byte)Mathf.Clamp(RGBColor.B, 0, 255)
                    });
            }
        }

        public class RGBColor 
        {
            public float R; 
            public float G;
            public float B;

            private Color color;
            public Color Color
            {
                get { return color; }
                set
                {
                    color = value;
                    R = color.r;
                    G = color.g;
                    B = color.b;
                }
            }
        }

        public class SetColorIndexCommand : RGBLightCommand 
        {
            public int ColorIndex;

            protected override CommandPayload MakeCommandPayload()
            {
                return new CommandPayload(PortOutputCommandSubCommandTypeEnum.DIRECT_MODE_WRITE,
                    new byte[] {
                        MODE_DIRECT_COLOR_INDEX,
                        (byte)Mathf.Clamp(ColorIndex, 0, 255)
                    });
            }
        }

        public class SetDefaultColorCommand : RGBLightCommand 
        {
            protected override CommandPayload MakeCommandPayload()
            {
                //TODO: Ought to preserve mode discrete-ness - i.e. set color index or RGB depending on the current mode.
                return new CommandPayload(PortOutputCommandSubCommandTypeEnum.DIRECT_MODE_WRITE,
                    new byte[] {
                        MODE_DIRECT_COLOR_INDEX,
                        (byte)Mathf.Clamp(DefaultColorIndex, 0, 255)
                    });
            }
        }

        public class SwitchOffCommand : RGBLightCommand 
        {
            protected override CommandPayload MakeCommandPayload()
            {
                //TODO: Ought to preserve mode discrete-ness - i.e. set color index or RGB depending on the current mode.
                byte offColorIndex = 0;
                return new CommandPayload(PortOutputCommandSubCommandTypeEnum.DIRECT_MODE_WRITE,
                    new byte[] {
                        MODE_DIRECT_COLOR_INDEX,
                        (byte)Mathf.Clamp(offColorIndex, 0, 255)
                    });
            }
        }

        #endregion

        #region Default Colors
        private static Color Color10()
        {
            return new Color32(255, 110, 60, 255); 
        }

        private static Color Color9()
        {
            return new Color32(255, 0, 0, 255); 
        }

        private static Color Color8()
        { 
            return new Color32(255, 20, 0, 255); 
        }

        private static Color Color7()
        {
            return new Color32(255, 55, 0, 255); 
        }

        private static Color Color6()
        {
            return new Color32(0, 200, 5, 255); 
        }

        private static Color Color5()
        {
            return new Color32(0, 255, 60, 255); 
        }

        private static Color Color4()
        {
            return new Color32(70, 155, 140, 255); 
        }

        private static Color Color3()
        {
            return new Color32(0, 0, 255, 255); 
        }

        private static Color Color2()
        {
            return new Color32(145, 0, 130, 255); 
        }

        private static Color Color1()
        {
            return new Color32(255, 10, 18, 255); 
        }

        private static Color Color0()
        {
            //aka. off
            return new Color32(0, 0, 0, 255); 
        }

        public enum RGBLightMode
        {
            /** Discrete */
            Discrete = 0,
            /** Absolute */
            Absolute = 1,

            Unknown
        }

        #endregion

    }



}
