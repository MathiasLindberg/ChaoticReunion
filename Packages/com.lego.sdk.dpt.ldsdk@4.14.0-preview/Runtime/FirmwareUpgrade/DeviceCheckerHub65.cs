namespace LEGODeviceUnitySDK
{
    public class DeviceCheckerHub65 : IDeviceChecker
    {
        FirmwareUpdateReadiness.Status IDeviceChecker.IsDeviceReady(ILEGODevice device)
        {

            //Pre - FW update - assumes that the device has already been confirmed to be of the type hub65

            // If there is a UART device connected to Port A then report problem otherwise report OK to update

            ILEGOService service = device.FindService(DevicePort.A.PortNumber(device));

            if(    service != null
                && IOTypes.HasUARTConnection.Contains(service.ioType))
            {
                return FirmwareUpdateReadiness.Status.Hub65_UART_device_in_port_A;
            }

            return FirmwareUpdateReadiness.Status.OK;
        }
    }
}
