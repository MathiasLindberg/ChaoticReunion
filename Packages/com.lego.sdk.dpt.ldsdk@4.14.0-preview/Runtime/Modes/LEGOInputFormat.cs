using System.Collections.Generic;
using System;

namespace LEGODeviceUnitySDK
{
    public class LEGOInputFormat
    {
        public string Revision { get; private set; }

        [Obsolete("Use ConnectInfo.PortID instead.")]
        public int PortID { get; private set; }

        [Obsolete("Use ConnectInfo.Type instead.")]
        public IOType Type { get; private set; }

        [Obsolete("Use ConnectInfo.Type instead.")]
        public int TypeNumber { get { return (int)Type; } }

        public int Mode { get; private set; }

        public int DeltaInterval { get; private set; }

        public InputFormatUnit Unit { get; private set; }

        public bool NotificationsEnabled { get; private set; }

        public LEGOInputFormat(int portID, IOType type, int mode, int deltaInterval, InputFormatUnit unit, bool notificationsEnabled)
        {
            #pragma warning disable 0618 // Obsolete properties
            PortID = portID;
            Type = type;
            #pragma warning restore 0618
            Mode = mode;
            DeltaInterval = deltaInterval;
            Unit = unit;
            NotificationsEnabled = notificationsEnabled;
        }


        public enum InputFormatUnit
        {
            LEInputFormatUnitRaw = 0,
            LEInputFormatUnitPercentage = 1,
            LEInputFormatUnitSI = 2,
            LEInputFormatUnitUnknown,
        }

        public override string ToString()
        {
            #pragma warning disable 0618 // Obsolete properties
            return string.Format("[LEGOInputFormat: Revision={0}, PortID={1}, TypeNumber={2}, Type={3}, Mode={4}, DeltaInterval={5}, Unit={6}, NotificationsEnabled={7}]", Revision, PortID, TypeNumber, Type, Mode, DeltaInterval, Unit, NotificationsEnabled);
            #pragma warning restore 0618 // Obsolete properties
        }


    }

}