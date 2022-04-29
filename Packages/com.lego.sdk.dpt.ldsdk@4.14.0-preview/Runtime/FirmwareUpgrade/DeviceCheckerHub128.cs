using System.Collections.Generic;

namespace LEGODeviceUnitySDK
{
    public class DeviceCheckerHub128 : IDeviceChecker
    {
        FirmwareUpdateReadiness.Status IDeviceChecker.IsDeviceReady(ILEGODevice device)
        {
            //Pre - FW update - assumes that the device has already been confirmed to be of the type hub128

            // If the hub firmware version is higher than 1.0.0.20 then return OK to proceed
            // If any port have a connected service which has UART sensors present(tacho motors, colour sensors etc.) return not ok - must remove UART peripherals
            // otherwise return OK to proceed

            // We can not know what is connected if interrogation has not finished
            if (device.State != DeviceState.InterrogationFinished)
            {
                return FirmwareUpdateReadiness.Status.InternalError;
            }
            
            if(device.DeviceInfo.FirmwareRevision >=  new LEGORevision(1,0,0,30))
            {
                return FirmwareUpdateReadiness.Status.OK;
            }

            uint UARTConnectPortCount = 0;

            List<ILEGOService> services = new List<ILEGOService>
            {
                device.FindService(DevicePort.A.PortNumber(device)),
                device.FindService(DevicePort.B.PortNumber(device)),
                device.FindService(DevicePort.C.PortNumber(device)),
                device.FindService(DevicePort.D.PortNumber(device))
            };

            foreach (var service in services)
            {
                if (service == null)
                {
                    continue;
                }
                
                if (IOTypes.HasUARTConnection.Contains(service.ioType))
                {
                    UARTConnectPortCount++;
                }
            }

            if (UARTConnectPortCount > 0)
            {
                return FirmwareUpdateReadiness.Status.Hub128_FW_too_old_and_any_UARTs_connected;
            }

            return FirmwareUpdateReadiness.Status.OK;
        }
    }
}
