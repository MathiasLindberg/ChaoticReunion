using System;
using dk.lego.devicesdk.bluetooth.V3.messages;
using LEGO.Logger;

namespace LEGODeviceUnitySDK
{

    /// <summary>
    /// Enquiring a device about a specific service on a given port.
    /// </summary>
    internal class ServiceModeInterrogator : CheckListBase<PortModeInformationType, MessagePortModeInformationRequest, LEGOModeInformation>, MessagePortModeInformation_Visitor<Void> {
        private static readonly ILog logger = LogManager.GetLogger(typeof(ServiceModeInterrogator));

        private readonly LEGODevice device;
        private readonly byte portID;
        private readonly uint modeNr;
        private readonly LEGOModeInformation.Builder builder;
        private readonly ServiceInfoCache cacheItem;

        public ServiceModeInterrogator(LEGODevice device, byte portID, uint modeNr, ServiceInfoCache cacheItem, Action onProgress, Action<LEGOModeInformation> onCompletion)
            : base(onProgress, onCompletion)
        {
            this.device = device;
            this.portID = portID;
            this.modeNr = modeNr;
            this.builder = new LEGOModeInformation.Builder((int)modeNr);
            this.cacheItem = cacheItem;
        }

        protected override LEGOModeInformation Result()
        {
            return builder.Build();
        }
        
        public void Initialize() {
            Suspended = true; // Avoid premature completion
            RegisterAndSendMessage(new MessagePortModeInformationRequest(0, portID, modeNr, PortModeInformationType.NAME));
            RegisterAndSendMessage(new MessagePortModeInformationRequest(0, portID, modeNr, PortModeInformationType.RAW));
            RegisterAndSendMessage(new MessagePortModeInformationRequest(0, portID, modeNr, PortModeInformationType.PCT));
            RegisterAndSendMessage(new MessagePortModeInformationRequest(0, portID, modeNr, PortModeInformationType.SI));
            RegisterAndSendMessage(new MessagePortModeInformationRequest(0, portID, modeNr, PortModeInformationType.SYMBOL));
            RegisterAndSendMessage(new MessagePortModeInformationRequest(0, portID, modeNr, PortModeInformationType.MAPPING));
            RegisterAndSendMessage(new MessagePortModeInformationRequest(0, portID, modeNr, PortModeInformationType.VALUE_FORMAT));
            Suspended = false;
        }

        #region Interrogation events
        public void DidReceivePortModeInformation(MessagePortModeInformation msg) 
        {
            HandlePortModeInformation(msg);
            CheckOff(msg.information);
        }
        private void HandlePortModeInformation(MessagePortModeInformation msg) 
        {
            msg.visitWith(this, Void.Instance);
        }
            
        #region Message visitor
        void MessagePortModeInformation_Visitor<Void>.handle_MessagePortModeInformationName(MessagePortModeInformationName msg, Void arg)
        {
            logger.Debug("MessagePortModeInformationName portId:"+msg.portID+"/"+msg.information+" NAME:"+msg.name);
            builder.Name = msg.name;
        }

        void MessagePortModeInformation_Visitor<Void>.handle_MessagePortModeInformationRaw(MessagePortModeInformationRaw msg, Void arg)
        {
            builder.MinRaw = msg.minRaw;
            builder.MaxRaw = msg.maxRaw;
        }

        void MessagePortModeInformation_Visitor<Void>.handle_MessagePortModeInformationPct(MessagePortModeInformationPct msg, Void arg)
        {
            builder.MinPct = msg.minPct;
            builder.MaxPct = msg.maxPct;
        }

        void MessagePortModeInformation_Visitor<Void>.handle_MessagePortModeInformationSI(MessagePortModeInformationSI msg, Void arg)
        {
            builder.MinSI = msg.minSI;
            builder.MaxSI = msg.maxSI;
        }

        void MessagePortModeInformation_Visitor<Void>.handle_MessagePortModeInformationSymbol(MessagePortModeInformationSymbol msg, Void arg)
        {
            builder.Symbol = msg.symbol;
        }

        void MessagePortModeInformation_Visitor<Void>.handle_MessagePortModeInformationMapping(MessagePortModeInformationMapping msg, Void arg)
        {
            //TODO: No place to store this info at present.
        }

        void MessagePortModeInformation_Visitor<Void>.handle_MessagePortModeInformationValueFormat(MessagePortModeInformationValueFormat msg, Void arg)
        {
            builder.ValueFormatDataSetCount = msg.valueFormatCount;
            builder.ValueFormatDataSetType = (LEGOModeInformationValueFormatType)msg.valueFormatType;

            builder.ValueFormatFigures = msg.valueFormatFigures;
            builder.ValueFormatDecimals = msg.valueFormatDecimals;

            // Derived values:
            builder.ValueDataSetSize = builder.ValueFormatDataSetType.ByteSize();
            builder.ValueDataLength = builder.ValueDataSetSize * builder.ValueFormatDataSetCount;
        }
        #endregion
        #endregion

        #region Sending messages and registering the expectance of an answer.
        private void RegisterAndSendMessage(MessagePortModeInformationRequest msg)
        {
            MessagePortModeInformation cachedAnswer = cacheItem.LookupInfo(msg);
            logger.Debug("Using cached answer for "+msg.portID+"#"+msg.mode+"/"+msg.information+" ? "+(cachedAnswer != null));
            if (cachedAnswer != null) 
            {
                try 
                {
                    if (cachedAnswer.information != msg.information)
                        throw new Exception("Internal error: cache lookup information mismatch for "+msg.information);
                    HandlePortModeInformation(cachedAnswer);
                    return;
                } catch (Exception e) 
                {
                    logger.Warn("Error using cache lookup result: "+e);
                }
            }

            // Still here? Then the cache could not be used.
            MarkAsPending(msg.information, msg);
            device.SendMessage(msg);
        }

        public override void ResendMissing()
        {
            foreach (var msg in WaitingFor.Values)
            {
                device.SendMessage(msg);
            }
        }
        
        #endregion

    }
}
