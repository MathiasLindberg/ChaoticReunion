using UnityEngine;
using System.Collections.Generic;
using System;
using dk.lego.devicesdk.bluetooth.V3.messages;
using LEGODeviceUnitySDK;

namespace LEGODeviceUnitySDK
{
    public class LEGOSingleColorLight : LEGOService, ILEGOSingleColorLight
    {
        public override string ServiceName { get { return "Light"; } }
        protected override int DefaultModeNumber { get { return (int)LightMode.Percentage; } }

        internal LEGOSingleColorLight(LEGOService.Builder builder) : base(builder) { }

        public class SetPercentCommand : LEGOServiceCommand
        {
            private const byte MODE_DIRECT_PERCENT = 0x00;
            public int Percentage;
            
            public override ICollection<IOType> SupportedIOTypes
            {
                get { return new List<IOType>() {IOType.LEIOTypeLight};  }
            }

            protected override CommandPayload MakeCommandPayload()
            {
                return new CommandPayload(PortOutputCommandSubCommandTypeEnum.DIRECT_MODE_WRITE,
                    new byte[] { MODE_DIRECT_PERCENT, EncodePower(Percentage) });
            }
        }
        
        public enum LightMode
        {
            Percentage = 0,
            Unknown = -1
        }
    }
}

