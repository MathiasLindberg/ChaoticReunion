using System;
using dk.lego.devicesdk.bluetooth.V3.messages;
using LEGO.Logger;
using dk.lego.devicesdk.device;
using System.Text;

namespace LEGODeviceUnitySDK
{

    /// <summary>
    /// Enquiring a device about a specific service on a given port.
    /// </summary>
    internal class ServiceInterrogator
        : CheckListBase<string, object, LEGOService>, MessagePortMetadataRelated_Visitor<Void>, MessagePortInformation_Visitor<Void>
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(ServiceInterrogator));

        private readonly LEGODevice device;
        private readonly byte portID;
        private readonly LEGOService.Builder builder;
        private readonly ServiceInfoCache cacheItem;

        public ServiceInterrogator(LEGODevice device, MessageHubAttachedIOAttached attachMsg, Action onMinorProgress, Action<LEGOService> onMajorProgress)
            : this(device, attachMsg, onMinorProgress, onMajorProgress,
                attachMsg.hardwareRevision, attachMsg.firmwareRevision, false, 0, 0)
        {}

        public ServiceInterrogator(LEGODevice device, MessageHubAttachedIOVirtualAttached attachMsg,
            LEGORevision hardwareRevision, LEGORevision firmwareRevision,
            Action onMinorProgress, Action<LEGOService> onMajorProgress)
            : this(device, attachMsg, onMinorProgress, onMajorProgress,
                hardwareRevision, firmwareRevision, true, attachMsg.portA, attachMsg.portB)
        {}

        private ServiceInterrogator(LEGODevice device, MessageHubAttachedIOAbstractAttached attachMsg, Action onMinorProgress, Action<LEGOService> onMajorProgress,
            LEGORevision hardwareRevision, LEGORevision firmwareRevision,
            bool isVirtual, int portA, int portB)
            : base(onMinorProgress, onMajorProgress)
        {
            logger.Debug("ServiceInterrogator created for "+device.DeviceID+" port "+attachMsg.portID);
            this.device = device;
            this.portID = attachMsg.portID;

            var connectInfo = new LEGOConnectInfo(
                attachMsg.portID,
                hardwareRevision,
                firmwareRevision,
                (IOType)attachMsg.ioType,
                isVirtual,
                portA, portB);
            this.builder = new LEGOService.Builder(device, connectInfo);

            cacheItem = InterrogationCacheManager.Instance.CacheForService(
                (IOType)attachMsg.ioType,
                hwRev:hardwareRevision,
                fwRev:firmwareRevision);
        }

        public LEGORevision HardwareRevision {
            get { return builder.connectInfo.HardwareRevision; }
        }
        public LEGORevision FirmwareRevision {
            get { return builder.connectInfo.SoftwareRevision; }
        }

        public void Initialize()
        {
            Suspended = true; // Avoid premature completion
            RegisterAndSendMessage(new MessagePortInformationRequest(0, portID, PortInformationType.MODE_INFO));
            Suspended = false;
        }

        protected override void DidBecomeStable()
        {
            cacheItem.CompletedPopulating();
        }

        protected override LEGOService Result()
        {
            return builder.Build();
        }
    
        #region Interrogation events
        
        public void HandleMessage(MessagePortMetadataRelated msg)
        {
            msg.visitWith(this, Void.Instance);
        }

        void MessagePortMetadataRelated_Visitor<Void>.handle_MessagePortInformationRequest(MessagePortInformationRequest msg, Void arg)
        {
            device.ProtocolError(msg);
        }

        void MessagePortMetadataRelated_Visitor<Void>.handle_MessagePortModeInformationRequest(MessagePortModeInformationRequest msg, Void arg)
        {
            device.ProtocolError(msg);
        }

        void MessagePortMetadataRelated_Visitor<Void>.handle_MessagePortInformation(MessagePortInformation msg, Void arg)
        {
            msg.visitWith(this, Void.Instance);
        }

        void MessagePortInformation_Visitor<Void>.handle_MessagePortInformationModeInfo(MessagePortInformationModeInfo msg, Void arg)
        {
            HandlePortInformationModeInfo(msg);
            cacheItem.PopulateWith(msg);
            // Check off last.
            CheckOff(PORT_INFO_PREFIX + msg.informationType);
        }
        private void HandlePortInformationModeInfo(MessagePortInformationModeInfo msg)
        {
            builder.SetCoarseModeInfo(msg.modeCount,
                ProtocolUtils.IntegerArrayFromBitSet(msg.inputModes),
                ProtocolUtils.IntegerArrayFromBitSet(msg.outputModes));

            if (((ServiceCapabilities)msg.capabilities & ServiceCapabilities.Combinable) != 0) 
            {
                RegisterAndSendMessage(new MessagePortInformationRequest(0, portID, PortInformationType.ALLOWED_MODE_COMBINATIONS));
            }

            for (uint modeNr=0; modeNr < msg.modeCount; modeNr++) 
            {
                var key = PORT_MODE_INFO_PREFIX + modeNr;
                var theModeNr = modeNr; // Necessary for the closure to work.
                var modeInterrogator = new ServiceModeInterrogator(device, portID, modeNr, cacheItem, MinorProgressDidHappen,
                    (modeInfo)=> {
                        builder.SetModeInfo(theModeNr, modeInfo);
                        CheckOff(key);
                    });
                MarkAsPending(key, modeInterrogator);
                modeInterrogator.Initialize();
            }
        }

        void MessagePortInformation_Visitor<Void>.handle_MessagePortInformationAllowedCombinations(MessagePortInformationAllowedCombinations msg, Void arg)
        {
            HandlePortInformationAllowedCombinations(msg);
            cacheItem.PopulateWith(msg);
            CheckOff(PORT_INFO_PREFIX + msg.informationType);
        }
        private void HandlePortInformationAllowedCombinations(MessagePortInformationAllowedCombinations msg)
        {
            var combinations = msg.ParseAllowedCombinations();
            var combinationMasks = msg.ParseAllowedCombinationMasks();
            builder.SetAllowedModeCombinations(combinations, combinationMasks);
        }

        void MessagePortMetadataRelated_Visitor<Void>.handle_MessagePortModeInformation(MessagePortModeInformation msg, Void arg)
        {
            cacheItem.PopulateWith(msg);
            
            OnPendingDo(PORT_MODE_INFO_PREFIX + msg.mode, 
                (modeInterrogator) =>
                    {
                        var smi = modeInterrogator as ServiceModeInterrogator;
                        smi?.DidReceivePortModeInformation(msg);
                    }
                );
        }
        #endregion

        #region Sending messages and registering the expectance of an answer.
        private const string PORT_INFO_PREFIX = "PI:";
        private const string PORT_MODE_INFO_PREFIX = "PMI:";

        private void RegisterAndSendMessage(MessagePortInformationRequest msg)
        {
            MessagePortInformation cachedAnswer = cacheItem.LookupInfo(msg);
            logger.Debug("Using cached answer for "+msg.portID+"/"+msg.information+" ? "+(cachedAnswer != null));
            if (cachedAnswer != null) {
                try {
                    if (cachedAnswer is MessagePortInformationModeInfo)
                        HandlePortInformationModeInfo((MessagePortInformationModeInfo)cachedAnswer);
                    else if (cachedAnswer is MessagePortInformationAllowedCombinations)
                        HandlePortInformationAllowedCombinations((MessagePortInformationAllowedCombinations)cachedAnswer);
                    else
                        logger.Warn("Unexpected answer type from cache: "+cachedAnswer.informationType);
                    return;
                } catch (Exception e) {
                    logger.Warn("Error using cache lookup result: "+e);
                }
            }

            // Still here? Then the cache could not be used.
            MarkAsPending(PORT_INFO_PREFIX + msg.information, msg);
            device.SendMessage(msg);
        }
        #endregion

        protected override void ReportMissingDetails(StringBuilder sb, string key, object value)
        {
            if (value != null)
            {
                switch (value)
                {
                    case ServiceModeInterrogator smi:
                        sb.Append(" [");
                        smi.ReportMissing(sb);
                        sb.Append(" ]");
                        break;
                }
            }
        }

        public override void ResendMissing()
        {
            foreach (var o in WaitingFor.Values)
            {
                switch (o)
                {
                    case ServiceModeInterrogator smi:
                        smi.ResendMissing();
                        break;
                    case MessagePortInformationRequest msg:
                        device.SendMessage(msg);
                        break;
                }
            }
        }
    }
    
}
