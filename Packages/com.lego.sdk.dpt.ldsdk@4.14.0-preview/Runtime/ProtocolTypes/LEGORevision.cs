using System;
using System.Globalization;

#if !NO_LOGGER
using LEGO.Logger;
#endif

namespace LEGODeviceUnitySDK {

    public enum LEGORevisionLevel
    {
        Major = 0,
        Minor,
        BugFix,
        BuildNumber
    }

    public class LEGORevision : IComparable<LEGORevision>
    {
        #if !NO_LOGGER
        private static readonly ILog Logger = LogManager.GetLogger(typeof(LEGORevision));
        #endif

        public readonly string RevisionString;
        public readonly int MajorVersion;
        public readonly int MinorVersion;
        public readonly int BugFixVersion;
        public readonly int BuildNumber;
        public readonly bool IsEmptyRevision;

        public static readonly LEGORevision Empty = new LEGORevision(null);

        public static bool IsNullOrEmpty(LEGORevision revision)
        {
            if (revision == null) return true;
            return revision.IsEmptyRevision;
        }

        public LEGORevision(int majorVersion, int minorVersion, int bugFixVersion, int buildNumber) {
            this.MajorVersion = majorVersion;
            this.MinorVersion = minorVersion;
            this.BugFixVersion = bugFixVersion;
            this.BuildNumber = buildNumber;

            this.RevisionString = String.Format("{0:D01}.{1:D01}.{2:D02}.{3:D04}", MajorVersion, MinorVersion, BugFixVersion, BuildNumber);
        }

        public LEGORevision(string revisionString)
        {
            RevisionString = revisionString;
            if (string.IsNullOrEmpty(revisionString))
            {
                RevisionString = "0.0.0.0";
                IsEmptyRevision = true;
            }

            char[] delimiters = { '.' };
            var items = RevisionString.Split(delimiters);

            if (items.Length != 4)
                throw new FormatException("RevisionString '" + RevisionString + "' does not have valid format");

            try
            {
                MajorVersion = Int32.Parse(items[0]);
                MinorVersion = Int32.Parse(items[1]);
                BugFixVersion = Int32.Parse(items[2]);
                BuildNumber = Int32.Parse(items[3]);
            }
            catch (FormatException)
            {
                #if !NO_LOGGER
                Logger.Error("RevisionString '" + RevisionString + "' could not be parsed");
                #else
                Console.WriteLine("RevisionString '" + RevisionString + "' could not be parsed");
                #endif
            }
        }

        #region Serialization
        public static LEGORevision parse(byte[] bytes) {
            int buildNumber = 0;
            if (bytes.Length >= 2) {
                buildNumber = BCDDecode(BitConverter.ToUInt16(bytes, 0));
            }
            int bugFixVersion = 0;
            if (bytes.Length >= 3) {
                bugFixVersion = BCDDecode(bytes[2]);
            }
            int minorVersion = 0;
            int majorVersion = 0;
            if (bytes.Length >= 4) {
                minorVersion = bytes[3] & 0x0F;
                majorVersion = (bytes[3] & 0xF0) >> 4;
            }
            return new LEGORevision(majorVersion, minorVersion, bugFixVersion, buildNumber);
        }

        public byte[] unparse() {
            // BCD encode the fields:
            int major =  BCDEncode(MajorVersion);
            int minor =  BCDEncode(MinorVersion);
            int bugFix = BCDEncode(BugFixVersion);
            int build =  BCDEncode(BuildNumber);
            byte[] result = new byte[4];
            result[0] = (byte)(build & 0xFF);
            result[1] = (byte)((build >> 8) & 0xFF);
            result[2] = (byte)bugFix;
            result[3] = (byte)((major << 4) | minor);
            return result;
        }

        private static int BCDDecode(int bcdValue) {
            return Convert.ToInt32(String.Format(CultureInfo.InvariantCulture, "{0:X}", bcdValue), CultureInfo.InvariantCulture);
        }
        private static int BCDEncode(int bcdValue) {
            return Convert.ToInt32(String.Format(CultureInfo.InvariantCulture, "{0}", bcdValue), 16);
        }
        #endregion

        #region Compare

        public static bool operator >= (LEGORevision version1, LEGORevision version2)
        {
            if (version1 == null && version2 == null)
            {
                return true;
            }
            return (version2 == null) ? true : version1.Compare(LEGORevisionLevel.BuildNumber, version2) >= 0; // 0=same, 1=newer
        }

        public static bool operator <= (LEGORevision version1, LEGORevision version2)
        {
            if (version1 == null && version2 == null)
            {
                return true;
            }
            return (version2 == null) ? false : version1.Compare(LEGORevisionLevel.BuildNumber, version2) <= 0; // 0=same, -1=older
        }

        public int Compare(LEGORevisionLevel level, LEGORevision other)
        {
            int older = -1;
            int newer = 1;
            int same = 0;

            if (other == null)
                return 1;

            if (MajorVersion > other.MajorVersion)
                return newer;
            if (MajorVersion < other.MajorVersion)
                return older;
            if (level == LEGORevisionLevel.Major)
                return same;

            if (MinorVersion > other.MinorVersion)
                return newer;
            if (MinorVersion < other.MinorVersion)
                return older;
            if (level == LEGORevisionLevel.Minor)
                return same;

            if (BugFixVersion > other.BugFixVersion)
                return newer;
            if (BugFixVersion < other.BugFixVersion)
                return older;
            if (level == LEGORevisionLevel.BugFix)
                return same;


            if (BuildNumber > other.BuildNumber)
                return newer;
            if (BuildNumber < other.BuildNumber)
                return older;

            return same;

        }

        public int CompareTo(LEGORevision other)
        {
            return Compare(LEGORevisionLevel.BugFix, other);
        }

        #endregion Compare


        #region - Equal, ToString

        public override bool Equals(Object obj)
        {
            LEGORevision other = (LEGORevision)obj;
            if (other == null)
                return false;

            return RevisionString.Equals(other.RevisionString);
        }

        public override int GetHashCode()
        {
            return RevisionString.GetHashCode();
        }

        public override string ToString()
        {
            return RevisionString;
        }

        #endregion
    }
}