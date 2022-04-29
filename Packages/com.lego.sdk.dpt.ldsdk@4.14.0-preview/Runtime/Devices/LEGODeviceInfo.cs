using System.Collections.Generic;
using dk.lego.devicesdk.device;

namespace LEGODeviceUnitySDK
{
    public class LEGODeviceInfo
    {

        public LEGORevision FirmwareRevision { get; private set; }

        public LEGORevision HardwareRevision { get; private set; }

        public LEGORevision SoftwareRevision { get; private set; }

        public string ManufacturerName { get; private set; }

        public string RadioFirmwareVersion { get; private set; }

        public LEGODeviceInfo() {
            FirmwareRevision = LEGORevision.Empty;
            HardwareRevision = LEGORevision.Empty;
            SoftwareRevision = LEGORevision.Empty;
        }

        internal void SetFirmwareRevision(LEGORevision value) {this.FirmwareRevision = value;}

        internal void SetHardwareRevision(LEGORevision value) {this.HardwareRevision = value;}

        internal void SetSoftwareRevision(LEGORevision value) {this.SoftwareRevision = value;}

        internal void SetManufacturerName(string value) {this.ManufacturerName = value;}

        internal void SetRadioFirmwareVersion(string value) {this.RadioFirmwareVersion = value;}

        public bool IsEqual(LEGODeviceInfo deviceInfo)
        {
            if (deviceInfo == null)
                return false;

            return (FirmwareRevision.Equals(deviceInfo.FirmwareRevision)
                    && HardwareRevision.Equals(deviceInfo.HardwareRevision)
                    && SoftwareRevision.Equals(deviceInfo.SoftwareRevision)
                    && ManufacturerName.Equals(deviceInfo.ManufacturerName)
                    && RadioFirmwareVersion.Equals(deviceInfo.RadioFirmwareVersion));
        }

		public override string ToString()
		{
			return string.Format("[LEGODeviceInfo: FW={0}, HW={1}, SW={2}, MF={3}, RadioFW={4}]", FirmwareRevision, HardwareRevision, SoftwareRevision, ManufacturerName, RadioFirmwareVersion);
		}

    }
}
