using System.Collections.Generic;
using System.Linq;

namespace LEGODeviceUnitySDK
{
    public class DevicePort
    {
        public string PortLabelString;

        public int PortLabelIndex { get; private set; }

        public static readonly DevicePort Unknown = new DevicePort {PortLabelString = "Unknown"};
        public static readonly DevicePort A = new DevicePort {PortLabelString = "A", PortLabelIndex = 0};
        public static readonly DevicePort B = new DevicePort {PortLabelString = "B", PortLabelIndex = 1};
        public static readonly DevicePort C = new DevicePort {PortLabelString = "C", PortLabelIndex = 2};
        public static readonly DevicePort D = new DevicePort {PortLabelString = "D", PortLabelIndex = 3};
        public static readonly DevicePort Virtual1 = new DevicePort {PortLabelString = "Virtual1", PortLabelIndex = 4};
        public static readonly DevicePort Virtual2 = new DevicePort {PortLabelString = "Virtual2", PortLabelIndex = 5};
        public static readonly DevicePort Virtual3 = new DevicePort {PortLabelString = "Virtual3", PortLabelIndex = 6};
        public static readonly DevicePort Virtual4 = new DevicePort {PortLabelString = "Virtual4", PortLabelIndex = 7};
        public static readonly DevicePort Virtual5 = new DevicePort {PortLabelString = "Virtual5", PortLabelIndex = 8};
        public static readonly DevicePort Virtual6 = new DevicePort {PortLabelString = "Virtual6", PortLabelIndex = 9};
        public static readonly DevicePort InternalRGBLight = new DevicePort {PortLabelString = "InternalRGBLight", PortLabelIndex = 10};
        public static readonly DevicePort InternalColorSensor = new DevicePort {PortLabelString = "InternalColorSensor", PortLabelIndex = 11};
        public static readonly DevicePort InternalAccelerometer = new DevicePort {PortLabelString = "InternalAccelerometer", PortLabelIndex = 12};
        public static readonly DevicePort InternalOrientationAngleSensor = new DevicePort {PortLabelString = "InternalOrientationAngleSensor", PortLabelIndex = 13};
        public static readonly DevicePort InternalCurrentSensor = new DevicePort {PortLabelString = "InternalCurrentSensor", PortLabelIndex = 14};
        public static readonly DevicePort InternalVoltageSensor = new DevicePort {PortLabelString = "InternalVoltageSensor", PortLabelIndex = 15};
        
        private static readonly List<DevicePort> allPorts = new List<DevicePort>
        {
            A, B, C, D,
            Virtual1, Virtual2, Virtual3,
            Virtual4, Virtual5, Virtual6,
            InternalRGBLight, InternalColorSensor,
            InternalAccelerometer, InternalOrientationAngleSensor,
            InternalCurrentSensor, InternalVoltageSensor
        };

        // NOTE: 
        // DevicePort now needs the firmware revision as in the case of the boost hub earlier versions of the firmware have a 
        // different mapping of port id (A->D) to device service port IDs
        private static readonly List<DevicePortMap> portMaps = new List<DevicePortMap>
        {
            //Hub64 - Move
            new DevicePortMap(new LEGORevision(0, 0, 0, 0),
                DeviceSystemType.LEGOSystem1, 0, new Dictionary<DevicePort, int> {{A, 55}, {B, 56}, {C, 1}, {D, 2}, {InternalRGBLight, 50}, {Virtual1, 57},
                    {InternalAccelerometer, 58}, {InternalOrientationAngleSensor, 58}, {InternalCurrentSensor, 59}, {InternalVoltageSensor, 60}},
                new[] {IOType.LEIOTypeInternalMotorWithTacho},
                new[] {new VirtualPortPair(A, B)}),
            new DevicePortMap(new LEGORevision(1, 0, 0, 224), //  ie. 1.0.0.224 and onwards 
                DeviceSystemType.LEGOSystem1, 0, new Dictionary<DevicePort, int> {{A, 0}, {B, 1}, {C, 2}, {D, 3}, {Virtual1, 16}, {Virtual2, 17}, {InternalRGBLight, 50},
                    {InternalAccelerometer, 58}, {InternalOrientationAngleSensor, 58}, {InternalCurrentSensor, 59}, {InternalVoltageSensor, 60}}, //On hub64 the accelerometer and orientation is on the same port, but uses different modes
                new[] {IOType.LEIOTypeInternalMotorWithTacho, IOType.LEIOTypeMotorWithTacho, IOType.LEIOTypeMotor},
                new[] {new VirtualPortPair(A, B)}),
            
            //Hub65 - City
            new DevicePortMap(new LEGORevision(0, 0, 0, 0),
                DeviceSystemType.LEGOSystem1, 1, new Dictionary<DevicePort, int> {{A, 0}, {B, 1}, {InternalRGBLight, 50}, {Virtual1, 57}, {InternalCurrentSensor, 59}, {InternalVoltageSensor, 60}},
                new[] {IOType.LEIOTypeTrainMotor, IOType.LEIOTypeMotor, IOType.LEIOTypeMotorWithTacho},
                new[] {new VirtualPortPair(A, B)}),
            new DevicePortMap(new LEGORevision(1, 1, 0, 0),
                DeviceSystemType.LEGOSystem1, 1, new Dictionary<DevicePort, int> {{A, 0}, {B, 1}, {InternalRGBLight, 50}, {Virtual1, 16}, {InternalCurrentSensor, 59}, {InternalVoltageSensor, 60}},
                new[] {IOType.LEIOTypeTrainMotor, IOType.LEIOTypeMotor, IOType.LEIOTypeMotorWithTacho},
                new[] {new VirtualPortPair(A, B)}),

            //Hub66 - City remote
            new DevicePortMap(new LEGORevision(0, 0, 0, 0),
                DeviceSystemType.LEGOSystem1, 2, new Dictionary<DevicePort, int> {{A, 0}, {B, 1}, {InternalRGBLight, 52}, {InternalVoltageSensor, 59}},
                null,
                null),
            
            //Hub128 - Technic
            new DevicePortMap(new LEGORevision(0, 0, 0, 0),
                DeviceSystemType.LEGOTechnic1, 0, new Dictionary<DevicePort, int> {{A, 0}, {B, 1}, {C, 2}, {D, 3}, {Virtual1, 16}, {Virtual2, 17},{InternalRGBLight, 50}, {InternalCurrentSensor, 59}, {InternalVoltageSensor, 60}, {InternalAccelerometer, 97}, {InternalOrientationAngleSensor, 99}},
                new[] {IOType.LEIOTypeMotorWithTacho, IOType.LEIOTypeTechnicMotorL, IOType.LEIOTypeTechnicMotorXL, IOType.LEIOTypeTechnicAzureAngularMotorM, IOType.LEIOTypeTechnicAzureAngularMotorL, IOType.LEIOTypeTechnicGreyAngularMotorM, IOType.LEIOTypeTechnicGreyAngularMotorL, IOType.LEIOTypeTechnicAzureAngularMotorS},
                new[] {new VirtualPortPair(A, B), new VirtualPortPair(C, D)}),
            
            //Hub32 - Duplo Train
            new DevicePortMap(new LEGORevision(0, 0, 0, 0),
                DeviceSystemType.LEGODuplo, 0, new Dictionary<DevicePort, int> {{A, 0}, {InternalRGBLight, 17}, {InternalColorSensor, 18}},
                null,
                null),
            
            //Hub33 - DORY
            new DevicePortMap(new LEGORevision(0, 0, 0, 0),
                DeviceSystemType.LEGODuplo, 1, new Dictionary<DevicePort, int> {{A, 0}, {B, 1}, {Virtual1, 16}, {InternalRGBLight, 50}, {InternalColorSensor, 63}},
                new[] {IOType.LEIOTypeMotorWithTacho, IOType.LEIOTypeInternalMotorWithTacho, IOType.LEIOTypeTechnicMotorL, IOType.LEIOTypeTechnicMotorXL, IOType.LEIOTypeTechnicAzureAngularMotorM, IOType.LEIOTypeTechnicAzureAngularMotorL, IOType.LEIOTypeTechnicAzureAngularMotorS},
                new[] {new VirtualPortPair(A, B)}),
            
            //Hub67 - LEAF
            new DevicePortMap(new LEGORevision(0, 0, 0, 0),
                DeviceSystemType.LEGOSystem1, 3, new Dictionary<DevicePort, int> {{A, 0}, {B, 1}, {C, 2},{D, 3}, {InternalVoltageSensor, 6} },
                null,
                null),
            
            //Hub68 - LEAF Bob
            new DevicePortMap(new LEGORevision(0, 0, 0, 0),
                DeviceSystemType.LEGOSystem1, 4, new Dictionary<DevicePort, int> {{A, 0}, {B, 1}, {C, 2},{D, 3}, {InternalVoltageSensor, 6} },
                null,
                null)

        };

        public static DevicePort DevicePortByIndex(int portIndex)
        {
            return (portIndex > -1 && portIndex < allPorts.Count) ? allPorts[portIndex] : Unknown;
        }
        
        public static DevicePort DevicePortFromNumber(int portNumber, ILEGODevice device)
        {
            var portMap = FindDevicePortMap(device);
            return portMap == null ? Unknown : portMap.DevicePortFromNumber(portNumber);
        }

        public int PortNumber(ILEGODevice device)
        {
            var portMap = FindDevicePortMap(device);
            return portMap == null ? -1 : portMap.PortNumberFromDevicePort(this);
        }

        public static bool IsCapableOfVirtualPairing(ILEGODevice device)
        {
            var portMap = FindDevicePortMap(device);
            if (portMap == null || portMap.ValidVirtualCandidates == null)
            {
                return false;
            }

            return portMap.ValidVirtualCandidates.Length > 0;
        }

        public static bool IsValidVirtualCandidateTypeForHub(ILEGODevice device, IOType ioType)
        {
            var portMap = FindDevicePortMap(device);
            if (portMap == null || portMap.ValidVirtualCandidates == null)
            {
                return false;
            }

            return portMap.ValidVirtualCandidates.Contains(ioType);
        }

        public static uint[] GetPotentialVirtualPair(ILEGODevice device, ILEGOService service)
        {
            var portMap = FindDevicePortMap(device);

            if (portMap == null || portMap.ValidVirtualPairs == null || portMap.ValidVirtualCandidates == null)
            {
                return null;
            }

            if (!portMap.ValidVirtualCandidates.Contains(service.ioType))
            {
                return null;
            }

            foreach (var portPair in portMap.ValidVirtualPairs)
            {
                DevicePort potentialPairingPort;
                if (service.ConnectInfo.PortID == portPair.Port1.PortNumber(device))
                {
                    potentialPairingPort = portPair.Port2;
                }
                else if (service.ConnectInfo.PortID == portPair.Port2.PortNumber(device))
                {
                    potentialPairingPort = portPair.Port1;
                }
                else
                {
                    continue;
                }

                var otherService = device.FindService(potentialPairingPort.PortNumber(device));
                if (otherService == null)
                {
                    continue;
                }

                if (service.ioType == otherService.ioType)
                {
                    var portID1 = (uint) portPair.Port1.PortNumber(device);
                    var portID2 = (uint) portPair.Port2.PortNumber(device);
                    return new[] {portID1, portID2};
                }
            }

            return null;
        }
        
        
        //!brief find the appropriate portmap for the supplied device.
        //       passing of the whole device instead of an extra FW version 
        //       parameter may be a better mechanism from a testing perspective,
        //       and future iterations may want to consider this.
        //!param device
        //!return null if none found or the highest firmware qualifying device portmap 
        public static DevicePortMap FindDevicePortMap(ILEGODevice device)
        {
            if (device == null)
            {
                return null;
            }
            
            var devicePortMaps = portMaps.FindAll(p => p.IsForDevice(device)).OrderByDescending(fw => fw.Version);
            return devicePortMaps.Any() ? devicePortMaps.First() : null;
        }

        public override string ToString()
        {
            return PortLabelString;
        }

        public class DevicePortMap
        {
            private readonly LEGORevision version;
            private readonly DeviceSystemType systemType;
            private readonly int deviceNumber;
            private readonly Dictionary<DevicePort, int> portMap;
            public VirtualPortPair[] ValidVirtualPairs;
            public readonly IOType[] ValidVirtualCandidates;
            

            public LEGORevision Version { get { return version; } }
            public DeviceSystemType SystemType { get { return systemType; } }
            public int DeviceNumber { get { return deviceNumber; } }
            public Dictionary<DevicePort, int> PortMap { get { return portMap; } }

            public DevicePortMap(LEGORevision version, 
                DeviceSystemType systemType,
                int deviceNumber, 
                Dictionary<DevicePort, int> portMap, 
                IOType[] validVirtualCandidates, 
                VirtualPortPair[] validVirtualPairs)
            {
                this.version = version;
                this.systemType = systemType;
                this.deviceNumber = deviceNumber;
                this.portMap = portMap;
                ValidVirtualPairs = validVirtualPairs;
                ValidVirtualCandidates = validVirtualCandidates;
            }

            //!brief  This method is a predicate used in the DevicePortMap.FindDevicePortMap method to find all firmware versions in the portMap list that are less than or 
            //        equal to the passed in devices firmware version. Example: using the List<DevicePortMap> portMaps as defined at the time of writing, 
            //        and a device 64 (Boost hub, systemType = 2, deviceNumber = 0) with a firmware of 1,0,0,221, then this predicate will return true for only entry 0.0.0.0.
            //        in the case of a device with firmware 1.0.0.232 it would return a true for 0.0.0.0 and 1.0.0.232. This is used by the calling linq to make a list to 
            //        select the topmost portmap in an ordered list. If a device had a firmware version of 2.0.0.2 then this predicate would answer true for both 
            //!param  device the device to compare against this DevicePortMap
            //!return true if device is they same system type and device number and its firmware version is higher or the same. 
            public bool IsForDevice(ILEGODevice device)
            {
                if (device == null)
                {
                    return false;
                }

                return (systemType == device?.SystemType
                        && deviceNumber == device?.SystemDeviceNumber
                        && version.Compare(LEGORevisionLevel.BuildNumber, device.DeviceInfo?.FirmwareRevision) <= 0);
            }

            public DevicePort DevicePortFromNumber(int portNumber)
            {
                foreach (var keyValue in portMap)
                {
                    if (keyValue.Value == portNumber) 
                    {
                        return keyValue.Key;
                    }
                }
                
                return Unknown;
            }

            public int PortNumberFromDevicePort(DevicePort port)
            {
                int portNumber;
                return portMap.TryGetValue(port, out portNumber) ? portNumber : -1;
            }

        }      
            
        public struct VirtualPortPair
        {
            public readonly DevicePort Port1;
            public readonly DevicePort Port2;

            public VirtualPortPair(DevicePort port1, DevicePort port2)
            {
                Port1 = port1;
                Port2 = port2;
            }
        }
    }


    
}