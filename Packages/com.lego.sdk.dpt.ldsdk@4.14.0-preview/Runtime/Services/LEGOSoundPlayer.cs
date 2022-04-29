using System;
using System.Collections.Generic;
using dk.lego.devicesdk.bluetooth.V3.messages;

namespace LEGODeviceUnitySDK
{
    public class LEGOSoundPlayer : LEGOService
    {
        public override string ServiceName { get { return "Sound Player"; } }
        protected override int DefaultModeNumber { get { return (int)LESoundPlayerMode.Tone; } }

        public LEGOSoundPlayer(LEGOService.Builder builder) : base(builder) { }

        public LESoundPlayerMode SoundPlayerMode { get { return InputFormat == null ? LESoundPlayerMode.Unknown : (LESoundPlayerMode)InputFormat.Mode; } }

        public abstract class SoundPlayerCommand : LEGOServiceCommand
        {
            private static readonly ICollection<IOType> SoundPlayerOnly = new List<IOType> { IOType.LEIOTypeSoundPlayer };
            public override ICollection<IOType> SupportedIOTypes {
                get { return SoundPlayerOnly; }
            }
        }

        public class PlayToneIndexCommand : SoundPlayerCommand
        {
            public LESoundPlayerTone Value;

            protected override CommandPayload MakeCommandPayload()
            {
                //TODO: Check current mode
                return new CommandPayload(PortOutputCommandSubCommandTypeEnum.DIRECT_MODE_WRITE,
                    new byte[] {(byte)LESoundPlayerMode.Tone, (byte)Value});
            }
        }

        public class PlaySoundIndexCommand : SoundPlayerCommand
        {
            public LESoundPlayerSound Value;

            protected override CommandPayload MakeCommandPayload()
            {
                //TODO: Check current mode
                return new CommandPayload(PortOutputCommandSubCommandTypeEnum.DIRECT_MODE_WRITE,
                    new byte[] {(byte)LESoundPlayerMode.Sound, (byte)Value});
            }
        }


        public enum LESoundPlayerMode
        {
            Tone = 0,
            Sound = 1,
            Unknown
        }

        public enum LESoundPlayerTone
        {
            Stop = 0,
            Low = 3,
            Medium = 9,
            High = 10
        }

        public enum LESoundPlayerSound
        {
            Stop = 0,
            Sound1 = 3,
            Sound2 = 5,
            Sound3 = 7,
            Sound4 = 9,
            Sound5 = 10
        }


    }
}
