using System;
namespace LEGODeviceUnitySDK
{
    public class LEGODeviceSDKVersionInfo
    {

        public LEGODeviceSDKVersionInfo(string version, string buildNumber, string commit, string v2FirmwareVersion, string v3FirmwareVersion)
        {
            Version = version;
            BuildNumber = buildNumber;
            Commit = commit;
            V2FirmwareVersion = v2FirmwareVersion;
            V3FirmwareVersion = v3FirmwareVersion;
        }

        public string Version { get; private set; }

        public string BuildNumber { get; private set; }

        public string Commit { get; private set; }

        public string V2FirmwareVersion { get; private set; }

        public string V3FirmwareVersion { get; private set; }

        public override string ToString()
        {
            return string.Format("[LEGODeviceSDKVersionInfo: Version={0}, BuildNumber={1}, Commit={2}, V2FirmwareVersion={3}, V3FirmwareVersion={4}]", Version, BuildNumber, Commit, V2FirmwareVersion, V3FirmwareVersion);
        }

    }
}
