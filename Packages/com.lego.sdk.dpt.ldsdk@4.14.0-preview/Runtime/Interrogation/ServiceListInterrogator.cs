using System;
using dk.lego.devicesdk.bluetooth.V3.messages;
using LEGO.Logger;
using System.Text;
using LEGODeviceUnitySDK.LEGODeviceExtensions;

namespace LEGODeviceUnitySDK
{

    /// <summary>
    /// Enquiring a device about the set of services.
    /// (TKey is the port ID)
    /// </summary>
    internal class ServiceListInterrogator
        : CheckListBase<uint, ServiceInterrogator, Void>, MessagePortConnectivityRelated_Visitor<Void>
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(ServiceListInterrogator));

        private readonly LEGODevice device;

        public ServiceListInterrogator(LEGODevice device, Action onMinorProgress, Action onMajorProgress)
            : base(onMinorProgress, (dummy)=>onMajorProgress())
        {
            logger.Debug("ServiceListInterrogator created for "+device.DeviceID);
            this.device = device;
        }

        protected override void ReportMissingDetails(StringBuilder sb, uint key, ServiceInterrogator value)
        {
            sb.Append(" [");
            value.ReportMissing(sb);
            sb.Append(" ]");
        }
        #region Resend functionality
        public override void ResendMissing()
        {  
            foreach (var serviceInterrogator in WaitingFor.Values)
            {
                serviceInterrogator.ResendMissing();
            }
        }
        
        #endregion
        #region Interrogation events
        
        public void HandleMessage(MessagePortConnectivityRelated msg)
        {
            msg.visitWith(this, Void.Instance);
        }
        
        public void HandleMessage(MessagePortMetadataRelated msg)
        {
            OnPendingDo(msg.portID, x=>x.HandleMessage(msg));
        }

        void MessagePortConnectivityRelated_Visitor<Void>.handle_MessageHubAttachedIOAttached(MessageHubAttachedIOAttached msg, Void arg)
        {
            logger.Warn("IOAttached: "+msg.portID + " IOType: " +  msg.ioType);
            
            if(device != null)
            {
                var portID = msg.portID;
                // can this device support this service properly? If not don't set up service interrogator - ie disable service
                if (!device.IsServiceSupported((IOType)msg.ioType))
                {
                    logger.Warn($@"Device {device.DeviceName} ({device.SystemType},{device.SystemDeviceNumber}) does not support service for IOtype {(IOType)msg.ioType} attached to port {portID}");
                    return;
                }
                
                var serviceInterr = new ServiceInterrogator(device, msg, MinorProgressDidHappen,
                    (service) =>
                    {
                        device.AddService(service);
                        CheckOff(portID);
                    });
                
                MarkAsPending(portID, serviceInterr);
                serviceInterr.Initialize();
            }
            else
            {
                logger.Warn("Device is disconnected, cannot create service interrogator");
            }
        }

        void MessagePortConnectivityRelated_Visitor<Void>.handle_MessageHubAttachedIOVirtualAttached(MessageHubAttachedIOVirtualAttached msg, Void arg)
        {
            logger.Warn("VirtualIOAttached: "+msg.portID+ " IOType: " +  msg.ioType);

            if (device != null)
            {
                var portID = msg.portID;

                LEGORevision hwRev, fwRev;
                if (!FindVirtualRevision(msg.portA, out hwRev, out fwRev) &&
                    !FindVirtualRevision(msg.portB, out hwRev, out fwRev))
                {
                    fwRev = hwRev = LEGORevision.Empty;
                }

                var serviceInterr = new ServiceInterrogator(device, msg, hwRev, fwRev, () => { },
                    (service) =>
                    {
                        device.VirtualServiceManger.TryAddMappedVirtualPortPair(device, portID,
                            new int[] {msg.portA, msg.portB});
                        device.AddService(service);
                        CheckOff(portID);
                    });
                MarkAsPending(portID, serviceInterr);
                serviceInterr.Initialize();
            }
            else
            {
                logger.Warn("Device is disconnected, cannot create service interrogator");
            }
        }

        void MessagePortConnectivityRelated_Visitor<Void>.handle_MessageHubAttachedIODetached(MessageHubAttachedIODetached msg, Void arg)
        {
            logger.Debug("IODetached: "+msg.portID);
            var portID = msg.portID;
            device.RemoveService(portID);
            CheckOff(portID);

            //If detached IO was pair of a virtual pair, remove that mapped pair from the VirtualServiceManager
            device.VirtualServiceManger.TryRemoveMappedVirtualPortPair(device,portID);
        }

        private bool FindVirtualRevision(byte portID, out LEGORevision hwRev, out LEGORevision fwRev)
        {
            // Case: Service is being interrogated
            ServiceInterrogator serviceInterr = null;
            if (OnPendingDo(portID, s => serviceInterr = s)) {
                hwRev = serviceInterr.HardwareRevision;
                fwRev = serviceInterr.FirmwareRevision;
                return true;
            }

            // Case: Service has completed interrogation
            ILEGOService service = device.FindService(portID);
            if (service != null) {
                hwRev = service.ConnectInfo.HardwareRevision;
                fwRev = service.ConnectInfo.SoftwareRevision;
                return true;
            }

            // Case: Service not found (somehow - virtual device attached before underlying?)
            hwRev = fwRev = null;
            return false;
        }
        #endregion
    }
    
}
