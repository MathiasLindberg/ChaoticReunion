using System;
using System.Collections.Generic;
using System.Globalization;
using LEGODeviceUnitySDK;

namespace LEGODeviceUnitySDK
{
    public class FirmwareUpdateReadiness
    {
        public enum Status // this should be in IDeviceChecker - universal set
        {
            OK,
            InternalError,
            Hub128_FW_too_old_and_any_UARTs_connected,
            Hub65_UART_device_in_port_A
        }
    }
}
