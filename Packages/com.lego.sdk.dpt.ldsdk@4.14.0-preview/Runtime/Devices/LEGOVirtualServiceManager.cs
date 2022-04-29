using System;
using System.Collections.Generic;
using System.Linq;
using LEGO.Logger;

namespace LEGODeviceUnitySDK
{
    public class LEGOVirtualServiceManager
    { 
        static readonly ILog logger = LogManager.GetLogger(typeof(LEGOVirtualServiceManager));
        
        private ILEGODevice device;
        private List<DeviceVirtualMapping> DevicesVirtualMapping = new List<DeviceVirtualMapping>();
        public LEGOVirtualServiceManager(ILEGODevice targetDevice)
        {
            this.device = targetDevice;

            // connect to service changes event - we react to motor additions.
            Subscribe();
        }

        ~LEGOVirtualServiceManager()
        {
            Unsubscribe();
        }

        private void Subscribe()
        {
            device.OnServiceConnectionChanged += HandleServiceConnectionChanged;
        }

        public void Unsubscribe()
        {
            device.OnServiceConnectionChanged -= HandleServiceConnectionChanged;
        } 
        private void HandleServiceConnectionChanged(ILEGODevice targetDevice, ILEGOService service, bool connected)
        {
            if(!connected || targetDevice != device)
            {
                return;
            }

            device = targetDevice;

            if (service is ILEGOMotor || service is ILEGOTachoMotor)
            {
                SetupAppropriateVirtualServices(device, service);
            }
        }    

        private void SetupAppropriateVirtualServices(ILEGODevice inDevice, ILEGOService newService)
        {
            if (inDevice == null || newService == null)
            {
                logger.Warn("inDevice or newService is null");
                return;
            }

            if (!DevicePort.IsValidVirtualCandidateTypeForHub(inDevice, newService.ioType))
            {
                return;
            }

            if(IsAlreadyPaired(newService))
            {
                return;
            }

            uint[] potentialVirtualPair = DevicePort.GetPotentialVirtualPair(inDevice, newService);
            if (potentialVirtualPair != null && potentialVirtualPair.Length > 1)
            {
                // this is a valid pair choice - attempt to make a pair - making sure it's always 
                // in the same order eg. (A,B) not (B,A)
                inDevice.SetupVirtualMotorPair(
                    Math.Min(potentialVirtualPair[0], potentialVirtualPair[1]),
                    Math.Max(potentialVirtualPair[0], potentialVirtualPair[1]));
            }
        }

        private bool IsAlreadyPaired(ILEGOService testService)
        {
            if(testService.ConnectInfo.VirtualConnection)
            {
                return true;
            }

            foreach(ILEGOService service in device.Services)
            {
                if ( service.ConnectInfo.VirtualConnection )
                {
                    if (   service.ConnectInfo.PortID1 == testService.ConnectInfo.PortID
                        || service.ConnectInfo.PortID2 == testService.ConnectInfo.PortID)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
                
        public int ActualConnectedVirtualPortForExpectedVirtualPort(ILEGODevice device, int expectedVirtualPort)
        {
            //IF boost hub then just pass port though HACK until LCC-1886 is implemented
            if (DeviceSystemType.LEGOSystem1 == device.SystemType
                && 0 == device.SystemDeviceNumber)
                return expectedVirtualPort;
            if(DevicesVirtualMapping.Count > 0)
            {
                var deviceVirtualMapping = DevicesVirtualMapping.First(pair => pair.Device.DeviceID == device.DeviceID);
                var virtualMapping = deviceVirtualMapping.VirtualMappedServices.FirstOrDefault(mapping =>
                {
                    if (mapping.ExpectedVirtualPort == expectedVirtualPort)
                    {
                        return true;
                    }

                    return false;
                });
                if (virtualMapping != null)
                    return virtualMapping.ActualVirtualPort;
            }
            
            return -1;
        }

        public int ExpectedVirtualPortForPortPair(ILEGODevice device, int[] portPair)
        {
            var portMap = DevicePort.FindDevicePortMap(device);
            if (portMap.ValidVirtualPairs == null)
            {
                return -1;
            }

            try
            {
                var index = portMap.ValidVirtualPairs.Select((v, i) => new {virtualPair = v, index = i}).First(c=> c.virtualPair.Port1.PortLabelIndex == portPair[0] &&  c.virtualPair.Port2.PortLabelIndex == portPair[1]).index;
                
                var onlyVirtuals = portMap.PortMap.Where(kvPair => kvPair.Key.PortLabelString.Contains("Virtual")).ToList();
                return onlyVirtuals[index].Value;
            }
            catch (Exception e)
            {
                logger.Debug($"Exception in Linq expression when attempting to find port pairs \"{e.Message}\" ");

                logger.Warn($"Could not find valid virtual pair index (portPair={portPair[0]},{portPair[1]} ) - could be old hub FW ({device.SystemType},{device.SystemDeviceNumber}) FW={device.DeviceInfo.FirmwareRevision.ToString()}");
                return - 1;
            }
        }

        public void TryRemoveMappedVirtualPortPair(ILEGODevice device, int virtualPort)
        {
            var foundDevice = DevicesVirtualMapping.FirstOrDefault(dev => dev.Device == device);
            foundDevice?.VirtualMappedServices.RemoveAll(mapping => mapping.ActualVirtualPort == virtualPort);
        }

        public void TryAddMappedVirtualPortPair(ILEGODevice device, int virtualPort, int[] ports)
        {
            var foundDevice = DevicesVirtualMapping.FirstOrDefault(dev => dev.Device == device);
            if (foundDevice == null)
            {
                foundDevice = new DeviceVirtualMapping
                    {Device = device, VirtualMappedServices = new List<VirtualMapping>()};
                DevicesVirtualMapping.Add(foundDevice);
            }

            if (foundDevice.VirtualMappedServices.All(port => port.ActualVirtualPort != virtualPort))
            {
                var expectedVirtualPort = ExpectedVirtualPortForPortPair(foundDevice.Device, ports);
                foundDevice.VirtualMappedServices.Add(new VirtualMapping()
                    {ExpectedVirtualPort = expectedVirtualPort, PortPair = ports, ActualVirtualPort = virtualPort});
            }
            else
            {
                var virtualMapping =
                    foundDevice.VirtualMappedServices.First(port => port.ActualVirtualPort == virtualPort);
                virtualMapping.PortPair = ports;
                virtualMapping.ExpectedVirtualPort = ExpectedVirtualPortForPortPair(foundDevice.Device, ports);
                virtualMapping.ActualVirtualPort = virtualPort;
            }
        }
    }

    public class DeviceVirtualMapping
    {
        public ILEGODevice Device;
        public List<VirtualMapping> VirtualMappedServices = new List<VirtualMapping>();
    }

    public class VirtualMapping
    {
        public int ExpectedVirtualPort;
        public int ActualVirtualPort;
        public int[] PortPair;
    }

}
