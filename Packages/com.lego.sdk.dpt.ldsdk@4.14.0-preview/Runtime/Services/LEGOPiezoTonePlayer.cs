using System;
using System.Collections.Generic;
using UnityEngine;

namespace LEGODeviceUnitySDK
{
	public class LEGOPiezoTonePlayer : LEGOService
	{
        public LEGOPiezoTonePlayer(LEGOService.Builder builder) : base(builder) {}
		
        public override string ServiceName { get { return "Piezo Tone Player"; } }

        public abstract class PiezoTonePlayerCommand : LEGOServiceCommand
        {
            public const int PIEZO_TONE_MAX_FREQUENCY = 1500;
            public const int PIEZO_TONE_MAX_DURATION = 65536;

            private static readonly ICollection<IOType> PiezoToneOnly = new List<IOType> { IOType.LEIOTypePiezoTone };
            public override ICollection<IOType> SupportedIOTypes {
                get { return PiezoToneOnly; }
            }
        }

        public class PlayNoteCommand : PiezoTonePlayerCommand 
        {
			public LEPiezoTonePlayerNote Note;
			public int Octave;
			public int Milliseconds;

            protected override CommandPayload MakeCommandPayload()
            {
                throw new NotImplementedException("Piezo commands are not supported in V.3.x of the LEGO BLE SDK");
            }
        }

        public class PlayFrequencyCommand : PiezoTonePlayerCommand 
        {
			public int Frequency;
			public int Milliseconds;

            protected override CommandPayload MakeCommandPayload()
            {
                throw new NotImplementedException("Piezo commands are not supported in V.3.x of the LEGO BLE SDK");
            }
        }

        public class StopPlayingCommand : PiezoTonePlayerCommand 
        {
            protected override CommandPayload MakeCommandPayload()
            {
                throw new NotImplementedException("Piezo commands are not supported in V.3.x of the LEGO BLE SDK");
            }
        }

		public enum LEPiezoTonePlayerNote
		{
			LEPiezoTonePlayerNoteC = 1,
			LEPiezoTonePlayerNoteCis = 2,
			LEPiezoTonePlayerNoteD = 3,
			LEPiezoTonePlayerNoteDis = 4,
			LEPiezoTonePlayerNoteE = 5,
			LEPiezoTonePlayerNoteF = 6,
			LEPiezoTonePlayerNoteFis = 7,
			LEPiezoTonePlayerNoteG = 8,
			LEPiezoTonePlayerNoteGis = 9,
			LEPiezoTonePlayerNoteA = 10,
			LEPiezoTonePlayerNoteAis = 11,
			LEPiezoTonePlayerNoteB = 12,
		};
	}
}
