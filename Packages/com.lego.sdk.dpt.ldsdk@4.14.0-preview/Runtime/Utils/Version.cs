using System;
namespace LEGODeviceUnitySDK
{
    public class Version
    {
        private int major = -1, minor = -1, bugfix = -1, build = -1;
        public int Major { get { return major; } set { major = value; } }
        public int Minor { get { return minor; } set { minor = value; } }
        public int Bugfix { get { return bugfix; } set { bugfix = value; } }
        public int Build { get { return build; } set { build = value; } }

        bool IsValid()
        {
            return (Major >= 0 && Minor >= 0 && Bugfix >= 0 && Build >= 0);
        }

        public static bool operator >(Version lhs, Version rhs)
        {
            if (lhs.Major > rhs.Major)
            {
                return true;
            }

            if (lhs.Major < rhs.Major)
            {
                return false;
            }
            // major == rhs.major

            if (lhs.Minor > rhs.Minor)
            {
                return true;
            }

            if (lhs.Minor < rhs.Minor)
            {
                return false;
            }

            // (major == rhs.major) && (minor == rhs.minor)
            if (lhs.Bugfix > rhs.Bugfix)
            {
                return true;
            }

            if (lhs.Bugfix < rhs.Bugfix)
            {
                return false;
            }

            // (major == rhs.major) && (minor == rhs.minor) && (bugfix == rhs.bugFix)
            if (lhs.Build > rhs.Build)
            {
                return true;
            }

            return false;
        }

        public static bool operator ==(Version lhs, Version rhs)
        {
            return (lhs.Major == rhs.Major
                    && lhs.Minor == rhs.Minor
                    && lhs.Bugfix == rhs.Bugfix
                    && lhs.Build == rhs.Build);
        }

        public static bool operator !=(Version lhs, Version rhs)
        {
            return (lhs == rhs) ? false : true;
        }

        public static bool operator <(Version lhs, Version rhs)
        {
            return (lhs > rhs || lhs == rhs) ? false : true;
        }

        public static bool operator >=(Version lhs, Version rhs)
        {
            return (lhs > rhs || lhs == rhs);
        }

        public static bool operator <=(Version lhs, Version rhs)
        {
            return (lhs < rhs || lhs == rhs);
        }

        public override string ToString()
        {
            return Major + "," + Minor + "," + Bugfix + "," + Build;
        }

        public override bool Equals(object obj)
        {
            var version = obj as Version;
            return version != null && this == version;

        }

        public override int GetHashCode()
        {
            return -199171022 + Major.GetHashCode() + Minor.GetHashCode() + Bugfix.GetHashCode() + Build.GetHashCode();
        }
    }
}
