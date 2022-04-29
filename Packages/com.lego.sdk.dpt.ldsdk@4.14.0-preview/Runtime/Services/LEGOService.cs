using System.Collections.Generic;
using System;
using LEGO.Logger;
using dk.lego.devicesdk.bluetooth.V3.messages;
using System.IO;

namespace LEGODeviceUnitySDK
{
    public class LEGOService
        : ILEGOService, MessagePortDynamicsRelated_Visitor<Void>
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(LEGOService));

        #region Service dependent metadata

        public virtual string ServiceName
        {
            get { return "Undefined"; }
        }

        protected virtual int DefaultModeNumber
        {
            get { return 0; }
        }

        protected virtual int DefaultInputFormatDelta
        {
            get { return 1; }
        }

        protected virtual LEGOInputFormat.InputFormatUnit DefaultInputFormatUnit
        {
            get { return LEGOInputFormat.InputFormatUnit.LEInputFormatUnitSI; }
        }

        protected virtual bool DefaultInputFormatEnableNotifications
        {
            get { return true; }
        }

        #endregion

        #region State - static

        public LEGOConnectInfo ConnectInfo { get; private set; }
        public ILEGODevice Device { get; private set; }

        public bool IsInternalService
        {
            get { return ConnectInfo.IsInternal; }
        }

        public IOType ioType
        {
            get { return ConnectInfo.Type; }
        }

        public LEGOAttachedIOModeInformation ModeInformation { get; private set; }

        public LEGOInputFormat DefaultInputFormat
        {
            get
            {
                return new LEGOInputFormat(ConnectInfo.PortID, ConnectInfo.Type, DefaultModeNumber,
                    DefaultInputFormatDelta, DefaultInputFormatUnit,
                    DefaultInputFormatEnableNotifications);
            }
        }

        public LEGOModeInformation modeInformationForModeNo(int modeNumber)
        {
            return ModeInformation.ModeInformationForModeNumber(modeNumber);
        }

        public LEGOModeInformation modeInformationForModeName(string modeName)
        {
            return ModeInformation.ModeInformationForName(modeName);
        }

        #endregion

        #region Service attachment state

        private ServiceState _state = ServiceState.Disconnected;

        public ServiceState State
        {
            get { return _state; }
        }

        internal void DidChangeState(ServiceState newState)
        {
            if (_state != newState)
            {
                ServiceState oldValue = _state;
                _state = newState;
                ResetCommandQueue();

                _delegates.OfType<ILEGOGeneralServiceDelegate>().ForEach(legoServiceDelegate =>
                    legoServiceDelegate.DidChangeState(this, oldValue, _state));
            }
        }

        internal void ResetQuaDisconnect()
        {
            ResetCommandQueue();
        }

        private void ResetCommandQueue()
        {
            commandTracker.Clear();
        }

        #endregion


        #region Current mode and input format

        public LEGOInputFormat InputFormat { get; private set; }
        public LEGOInputFormatCombined InputFormatCombined { get; private set; }

        #region Combined modes configuration

        private CombinedModeBuilder combinedModeBuilder;
        private ModeDataSet[] pendingModeDatasets;

        public void ResetCombinedModesConfiguration()
        {
            SendResetCombinedMode();
            combinedModeBuilder = null;
        }

        private void SendResetCombinedMode()
        {
            var portID = (uint) ConnectInfo.PortID;
            var empty = new byte[0];
            SendMessage(new MessagePortInputFormatSetupCombined(0, portID,
                PortInputFormatSetupCombinedSubCommandTypeEnum.RESET, empty));
        }

        public void AddCombinedMode(int modeNumber, int deltaInterval)
        {
            if (combinedModeBuilder == null)
                combinedModeBuilder = new CombinedModeBuilder(this);
            // Apparently, the backward compatible behaviour is that the last added mode will become the update trigger.
            combinedModeBuilder.AddMode((uint) modeNumber, (uint) deltaInterval, true);
        }

        public void ActivateCombinedModes()
        {
            logger.Debug("Activating Combined Modes for DeviceID: " + Device.DeviceID + " ConnectInfo.Port: " +
                         ConnectInfo.PortID);
            if (combinedModeBuilder == null)
            {
                logger.Error("No combined modes have been built!");
                return;
            }

            var messages = combinedModeBuilder.BuildMessageSequence();
            if (messages != null)
            {
                foreach (var m in messages)
                    SendMessage(m);
            }

            pendingModeDatasets = combinedModeBuilder.DataSetOrder();
            combinedModeBuilder = null;
        }

        #endregion

        private readonly Dictionary<int, LEGOValue> valueByMode = new Dictionary<int, LEGOValue>();

        public LEGOValue ValueForMode(int mode)
        {
            LEGOValue value;
            return valueByMode.TryGetValue(mode, out value) ? value : null;
        }

        public void UpdateCurrentInputFormatWithNewMode(int mode)
        {
            if (mode < 0) throw new ArgumentException("Negative mode number");
            var oldFormat = InputFormat != null ? InputFormat : DefaultInputFormat;
            if (InputFormatCombined != null) SendResetCombinedMode();
            SendMessage(new MessagePortInputFormatSetupSingle(0, (uint) ConnectInfo.PortID,
                (uint) mode, (uint) oldFormat.DeltaInterval, oldFormat.NotificationsEnabled));
        }

        public void UpdateCurrentInputFormatWithNewDeltaInterval(int deltaInterval)
        {
            if (deltaInterval < 0) throw new ArgumentException("Negative delta interval");
            var oldFormat = InputFormat != null ? InputFormat : DefaultInputFormat;
            if (InputFormatCombined != null) SendResetCombinedMode();
            SendMessage(new MessagePortInputFormatSetupSingle(0, (uint) ConnectInfo.PortID,
                (uint) oldFormat.Mode, (uint) deltaInterval, oldFormat.NotificationsEnabled));
        }

        public void UpdateInputFormat(LEGOInputFormat inputFormat)
        {
            logger.Debug("Sending new input format for " + ConnectInfo.Type + " with connect ID: " +
                         ConnectInfo.PortID);
            if (inputFormat == null) throw new ArgumentException("InputFormat is null");
            if (inputFormat.Mode < 0) throw new ArgumentException("Negative mode number");
            if (inputFormat.DeltaInterval < 0) throw new ArgumentException("Negative delta interval");

            if (InputFormatCombined != null) SendResetCombinedMode();
            SendMessage(new MessagePortInputFormatSetupSingle(0, (uint) ConnectInfo.PortID,
                (uint) inputFormat.Mode, (uint) inputFormat.DeltaInterval, inputFormat.NotificationsEnabled));
        }

        internal void HandleMessage(MessagePortDynamicsRelated msg)
        {
            msg.visitWith(this, Void.Instance);
        }

        void MessagePortDynamicsRelated_Visitor<Void>.handle_MessagePortInputFormatSetupSingle(
            MessagePortInputFormatSetupSingle msg, Void arg)
        {
            LEGODevice.ProtocolErrorS(msg);
        }

        void MessagePortDynamicsRelated_Visitor<Void>.handle_MessagePortInputFormatSetupCombined(
            MessagePortInputFormatSetupCombined msg, Void arg)
        {
            LEGODevice.ProtocolErrorS(msg);
        }

        void MessagePortDynamicsRelated_Visitor<Void>.handle_MessagePortOutputCommand(MessagePortOutputCommand msg,
            Void arg)
        {
            LEGODevice.ProtocolErrorS(msg);
        }

        void MessagePortDynamicsRelated_Visitor<Void>.handle_MessagePortInputFormatSingle(
            MessagePortInputFormatSingle msg, Void arg)
        {
            // The old sensor values are no longer valid.
            valueByMode.Clear(); //TODO: Some may still be valid in case of mode overlap!
            InputFormatCombined = null;

            var oldInputFormat = InputFormat;
            InputFormat = new LEGOInputFormat(ConnectInfo.PortID, ConnectInfo.Type,
                msg.mode, (int) msg.deltaInterval, LEGOInputFormat.InputFormatUnit.LEInputFormatUnitRaw,
                msg.notificationEnabled);

            // Notify:
            _delegates.OfType<ILEGOGeneralServiceDelegate>().ForEach(
                serviceDelegate => serviceDelegate.DidUpdateInputFormat(this, oldInputFormat, InputFormat));
        }

        void MessagePortDynamicsRelated_Visitor<Void>.handle_MessagePortInputFormatCombined(
            MessagePortInputFormatCombined msg, Void arg)
        {
            // The old sensor values are no longer valid.
            valueByMode.Clear(); //TODO: Some may still be valid in case of mode overlap!
            InputFormat = null;

            var oldFormat = InputFormatCombined;
            var combinationIndex = msg.CombinationIndex();
            var modeSet = ModeInformation.AllowedModeCombinations[combinationIndex];
            if (msg.dataSetPointer == 0)
            {
                // Reset combined mode
                InputFormatCombined = null;
            }
            else
            {
                InputFormatCombined = new LEGOInputFormatCombined(msg.portID, (int) combinationIndex,
                    msg.MultiUpdateEnabled(), modeSet, pendingModeDatasets);
            }
            // Keep the latest pendingModeDatasets, as more than one combined-mode-setup may be in progress - cf LBLE-1037.

            // Notify:
            _delegates.OfType<ILEGOGeneralServiceDelegate>().ForEach(
                serviceDelegate => serviceDelegate.DidUpdateInputFormatCombined(this, oldFormat, InputFormatCombined));
        }

        #region Sensor values

        /// Returns whether the read size was established succesfully.
        /// Moves the reader position to the end of the data for this service.
        public bool DidUpdateValueData(BinaryReader reader)
        {
            // Find the mode info for the current mode. This can fail in a couple of ways.
            if (InputFormat == null)
            {
                logger.Error("Value update error: No mode is set for service " + ServiceName);
                return false;
            }

            var modeNo = InputFormat.Mode;
            var modeInfo = modeInformationForModeNo(modeNo);
            if (modeInfo == null)
            {
                logger.Error("Value update error: Mode not known: " + InputFormat.Mode + " for service " + ServiceName);
                return false;
            }

            var totalSize = modeInfo.ValueDataLength;

            var available = reader.BaseStream.Length - reader.BaseStream.Position;
            if (available >= totalSize)
            {
                var newValue = ReadAllDataSetsForMode(reader, modeInfo);
                if (newValue == null)
                    return false;
                
                UpdateValueAndNotify(modeNo, newValue);
            }
            else
            {
                logger.Error("Value update error: Got only " + available + " bytes of " + totalSize);
                return false;
                // Ignore the garbage.
            }

            return true;
        }

        void MessagePortDynamicsRelated_Visitor<Void>.handle_MessagePortValueCombined(MessagePortValueCombined msg,
            Void arg)
        {
            var datasetBitmask = msg.datasetBitmask;
            var rawValues = msg.rawInputValues;

            if (InputFormatCombined == null)
            {
                logger.Error("Received PortValueCombined on port " + ConnectInfo.PortID +
                             " while not in a combined mode.");
                return;
            }

            var format = InputFormatCombined;
            var modeDataSets = format.ModeDataSets;

            var stream = new MemoryStream(rawValues);
            var reader = new BinaryReader(stream);
            
            try {
                // Read the raw data and reset the position of stream
                var position = reader.BaseStream.Position;
                byte[] rawData = new byte[reader.BaseStream.Length - reader.BaseStream.Position];
                reader.Read(rawData, 0, rawData.Length);
                reader.BaseStream.Position = position;
                
                UInt16 bitmask = datasetBitmask;
                // For some reason, the protocol has this bitmask in big-endian:
                bitmask = SwapByteOrderUInt16(bitmask);
                float[][] newVectors = new float[ModeInformation.ModeCount][];
                var maxModeNo = 0;
                for (int ds = 0; bitmask != 0; ds++, bitmask >>= 1)
                {
                    if ((bitmask & 1) != 0)
                    {
                        var modeDataSet = modeDataSets[ds];
                        var modeNo = modeDataSet.mode;
                        var dataSet = modeDataSet.dataSet;
                        var modeInfo = modeInformationForModeNo(modeNo);
                        
                        var newValue = ReadSingleDataSet(reader, modeInfo.ValueFormatDataSetType);

                        var vector = newVectors[modeNo];
                        if (vector == null)
                        {
                            vector = newVectors[modeNo] = new float[modeInfo.ValueFormatDataSetCount];
                            var oldVector = ValueForMode(modeNo);
                            if (oldVector != null)
                            {
                                Array.Copy(oldVector.RawValues, vector, vector.Length);
                            }

                            if (modeNo > maxModeNo) maxModeNo = modeNo;
                        }

                        vector[dataSet] = newValue;
                    }
                }

                for (var modeNo = 0; modeNo <= maxModeNo; modeNo++)
                {
                    if (newVectors[modeNo] != null)
                    {
                        var modeInfo = modeInformationForModeNo(modeNo);
                        UpdateValueAndNotify(modeNo, MakeLEGOValue(newVectors[modeNo], modeInfo, rawData));
                    }
                }

                if (stream.Position != stream.Length)
                    throw new ArgumentException("Extra data in PortValueCombined message: " +
                                                (stream.Length - stream.Position) + " bytes");
            }
            catch (Exception e)
            {
                logger.Error("Invalid PortValueCombined data on port " + ConnectInfo.PortID + ": " + e + " on data: " +
                             BitConverter.ToString(rawValues) + " bitmask=" + datasetBitmask);
            }
        }

        private ushort SwapByteOrderUInt16(ushort bitmask)
        {
            return (ushort) ((bitmask << 8) | (bitmask >> 8));
        }

        private LEGOValue ReadAllDataSetsForMode(BinaryReader reader, LEGOModeInformation modeInfo)
        {
            var rawValues = new float[modeInfo.ValueFormatDataSetCount];

            // Read the raw data and reset the position of stream
            var position = reader.BaseStream.Position;
            byte[] rawData = new byte[reader.BaseStream.Length - reader.BaseStream.Position];
            reader.Read(rawData, 0, rawData.Length);
            reader.BaseStream.Position = position;
            
            for (int i=0; i<rawValues.Length; i++) {
                rawValues[i] = ReadSingleDataSet(reader, modeInfo.ValueFormatDataSetType);
            }

            return MakeLEGOValue(rawValues, modeInfo, rawData);
        }
        
        private LEGOValue MakeLEGOValue(float[] rawValues, LEGOModeInformation modeInfo, byte[] rawData) {
            // Derive percent and SI values:
            float[] pctValues = modeInfo.ConvertRawToPct(rawValues);
            float[] siValues = modeInfo.ConvertRawToSI(rawValues);

            return new LEGOValue(modeInfo.Number, modeInfo.Name, rawValues, pctValues, siValues, rawData);
        }

        private float ReadSingleDataSet(BinaryReader reader, LEGOModeInformationValueFormatType dataSetType)
        {
            switch (dataSetType)
            {
                case LEGOModeInformationValueFormatType.LEModeInformationValueFormatType8BIT:
                    return reader.ReadSByte();
                case LEGOModeInformationValueFormatType.LEModeInformationValueFormatType16BIT:
                    return reader.ReadInt16();
                case LEGOModeInformationValueFormatType.LEModeInformationValueFormatType32BIT:
                    return reader.ReadInt32();
                case LEGOModeInformationValueFormatType.LEModeInformationValueFormatTypeFloat:
                    return reader.ReadSingle();
                default:
                    logger.Error("Value update error: Unknown value type: " + dataSetType);
                    return float.NaN;
            }
        }

        private void UpdateValueAndNotify(int modeNo, LEGOValue newValue)
        {
            // Update:
            LEGOValue oldValue;
            valueByMode.TryGetValue(modeNo, out oldValue);
            valueByMode[modeNo] = newValue;

            // Notify:
            NotifyObservers(oldValue, newValue);
        }

        #endregion

        #endregion

        #region Public operations

        public void ActivateBehaviorNumber(int behavior)
        {
            //TODO - implement as SendCommand() of a new LEGOServiceCommand.
        }

        public void DeactivateActiveBehavior()
        {
            //TODO - implement as SendCommand() of a new LEGOServiceCommand.
        }

        public void SendReadValueRequest()
        {
            SendMessage(new MessagePortInformationRequest(0, (uint) ConnectInfo.PortID, PortInformationType.VALUE));
        }

        public void SendResetStateRequest()
        {
            SendCommand(new WriteDataDirectCommand {HexDataString = "4411AA"});
        }

        #endregion

        #region Sensor value callbacks

        private void NotifyObservers(LEGOValue oldValue, LEGOValue newValue)
        {
            _delegates.OfType<ILEGOGeneralServiceDelegate>().ForEach(serviceDelegate =>
                serviceDelegate.DidUpdateValueData(this, oldValue, newValue));
            NotifySpecificObservers(oldValue, newValue);
        }

        /// <summary>
        /// Override this if there are specific observers defined.
        /// </summary>
        protected virtual void NotifySpecificObservers(LEGOValue oldValue, LEGOValue newValue)
        {
        }

        protected List<ILEGOServiceDelegate> _delegates = new List<ILEGOServiceDelegate>();

        public void RegisterDelegate(ILEGOServiceDelegate legoServiceDelegate)
        {
            if (!_delegates.Contains(legoServiceDelegate))
                _delegates.Add(legoServiceDelegate);
        }

        public void UnregisterDelegate(ILEGOServiceDelegate legoServiceDelegate)
        {
            _delegates.Remove(legoServiceDelegate);
        }

        #endregion


        #region Sending Commands and Handling Feedback

        public event Action OnReadyForNextCommand;
        public event Action<int> OnSendBufferFull;

        public void SendCommand(LEGOServiceCommand command)
        {
            SendCommand(command, (completed) => { });
        }

        /* There are three queue-levels of commands:
         *  1) Those received by the device;
         *  2) Those sent by the app, currently underway;
         *  3) The queue (supporting packet replacement) in the CUBB.
         */
        public void SendCommand(LEGOServiceCommand command, Action<bool> completionAction)
        {
            if (completionAction == null)
                completionAction = (completed) => { };

            if (!command.ExecuteImmediately && !commandTracker.HasRoomForMoreQueued)
            {
                // Can't handle any more queued commands.
                logger.Error("Service command queue is full - will not send command");
                completionAction(false);
                if (OnSendBufferFull != null) OnSendBufferFull(0);
            }

            var packetDroppingFeedback = new ObservablePacketDropFeedback(); // To be hooked up to the command tracker.
            var packetDroppingInfo = new PacketDroppingInfo
            {
                Group = 100 + ConnectInfo.PortID, // Group number must be positive, so we start numbering from 100.
                Feedback = packetDroppingFeedback
            };

            SendMessage(command.AsMessage(0, (uint) ConnectInfo.PortID), packetDroppingInfo);
            if (!command.RequestFeedback)
            {
                //If no feedback is requested we assume the command completed successfully.
                completionAction(true);
                completionAction = (completed) => { };
            }

            commandTracker.CommandSent(completionAction, packetDroppingFeedback);
        }

        private void SendMessage(LegoMessage legoMessage, PacketDroppingInfo? packetDroppingInfo = null)
        {
            ((LEGODevice) Device).SendMessage(legoMessage, packetDroppingInfo);
        }

        private ServiceCommandTracker commandTracker;

        private void ServiceDidBecomeReadyForNextCommand()
        {
            // Alert clients about available queuing room:
            if (OnReadyForNextCommand != null) OnReadyForNextCommand();
        }

        internal void DidReceiveFeedback(byte feedback)
        {
            commandTracker.FeedbackReceived(feedback);
        }

        #endregion

        #region Generic service commands

        public class WriteDataDirectCommand : LEGOServiceCommand
        {
            public string HexDataString;

            public override ICollection<IOType> SupportedIOTypes
            {
                get { return IOTypes.AllEvenUnknown; }
            }

            protected override CommandPayload MakeCommandPayload()
            {
                return new CommandPayload(PortOutputCommandSubCommandTypeEnum.DIRECT_WRITE,
                    ProtocolUtils.HexstringToByteArray(HexDataString));
            }
        }

        public class WriteDataDirectForModeCommand : LEGOServiceCommand
        {
            public string HexDataString;
            public int Mode;

            public override ICollection<IOType> SupportedIOTypes
            {
                get { return IOTypes.AllEvenUnknown; }
            }

            protected override CommandPayload MakeCommandPayload()
            {
                var payload = ProtocolUtils.HexstringToByteArray(HexDataString, prepad: 1);
                payload[0] = (byte) Mode;
                return new CommandPayload(PortOutputCommandSubCommandTypeEnum.DIRECT_MODE_WRITE, payload);
            }
        }

        #endregion

        #region Construction

        internal LEGOService(Builder builder)
        {
            Device = builder.legoDevice;
            ConnectInfo = builder.connectInfo;
            ModeInformation = builder.modeInformationBuilder.Build();

            commandTracker = new ServiceCommandTracker(ServiceDidBecomeReadyForNextCommand);
        }

        public class Builder
        {
            private static readonly ILog logger = LogManager.GetLogger(typeof(Builder));

            internal readonly ILEGODevice legoDevice;
            internal readonly LEGOConnectInfo connectInfo;
            internal LEGOAttachedIOModeInformation.Builder modeInformationBuilder;

            public Builder(ILEGODevice legoDevice, LEGOConnectInfo connectInfo)
            {
                this.legoDevice = legoDevice;
                this.connectInfo = connectInfo;
                this.modeInformationBuilder = new LEGOAttachedIOModeInformation.Builder();
            }

            public void SetCoarseModeInfo(int modeCount, int[] inputModes, int[] outputModes)
            {
                modeInformationBuilder.ModeCount = modeCount;
                modeInformationBuilder.InputModes = inputModes;
                modeInformationBuilder.OutputModes = outputModes;
            }

            public void SetAllowedModeCombinations(int[][] combinations, uint[] combinationMasks)
            {
                modeInformationBuilder.AllowedModeCombinations = combinations;
                modeInformationBuilder.AllowedModeCombinationMasks = combinationMasks;
            }

            public void SetModeInfo(uint modeNr, LEGOModeInformation modeInfo)
            {
                if (modeInformationBuilder.AttachedIOModeInformation == null)
                {
                    modeInformationBuilder.AttachedIOModeInformation =
                        new LEGOModeInformation[modeInformationBuilder.ModeCount];
                }

                modeInformationBuilder.AttachedIOModeInformation[modeNr] = modeInfo;
            }

            public LEGOService Build()
            {
                LEGOService legoService = DoBuild();

                logger.Debug("Did create service: " + connectInfo.Type + "    service: " + legoService.GetType().Name);
                return legoService;
            }

            private LEGOService DoBuild()
            {
                switch (connectInfo.Type)
                {
                    case IOType.LEIOTypeMotor:
                    case IOType.LEIOTypeTrainMotor:
                    case IOType.LEIOTypeDTMotor:
                        return new LEGOMotor(this);
                    case IOType.LEIOTypeLight:
                        return new LEGOSingleColorLight(this);
                    case IOType.LEIOTypeRGBLight:
                        return new LEGORGBLight(this);
                    case IOType.LEIOTypeCurrent:
                        return new LEGOCurrentSensor(this);
                    case IOType.LEIOTypeTiltSensor:
                        return new LEGOTiltSensor(this);
                    case IOType.LEIOTypeMotionSensor:
                        return new LEGOMotionSensor(this);
                    case IOType.LEIOTypeVoltage:
                        return new LEGOVoltageSensor(this);
                    case IOType.LEIOTypePiezoTone:
                        return new LEGOPiezoTonePlayer(this);
                    case IOType.LEIOTypeInternalMotorWithTacho:
                    case IOType.LEIOTypeMotorWithTacho:
                    {
                        if (connectInfo.VirtualConnection)
                            return new LEGODualMotorWithTacho(this);
                        else
                            return new LEGOSingleMotorWithTacho(this);
                    }

                    case IOType.LEIOTypeVisionSensor:
                        return new LEGOVisionSensor(this);
                    case IOType.LEIOTypeInternalTiltSensorThreeAxis:
                        return new LEGOTiltSensorThreeAxis(this);
                    case IOType.LEIOTypeSoundPlayer:
                        return new LEGOSoundPlayer(this);
                    case IOType.LEIOTypeDuploTrainColorSensor:
                        return new LEGOColorSensor(this);
                    case IOType.LEIOTypeMoveSensor:
                        return new LEGOMoveSensor(this);

                    case IOType.LEIOTypeTechnic3AxisAccelerometer:
                        return new LEGOTechnic3AxisAccelerationSensor(this);
                    case IOType.LEIOTypeTechnic3AxisGyroSensor:
                        return new LEGOTechnic3AxisGyroSensor(this);
                    case IOType.LEIOTypeTechnic3AxisOrientationSensor:
                        return new LEGOTechnic3AxisOrientationSensor(this);
                    case IOType.LEIOTypeTechnicTemperatureSensor:
                        return new LEGOTechnicTemperatureSensor(this);
                    case IOType.LEIOTypeTechnicMotorL:
                    case IOType.LEIOTypeTechnicMotorXL:
                    case IOType.LEIOTypeTechnicAzureAngularMotorM:
                    case IOType.LEIOTypeTechnicAzureAngularMotorL: 
                    case IOType.LEIOTypeTechnicGreyAngularMotorM:
                    case IOType.LEIOTypeTechnicGreyAngularMotorL:
                    case IOType.LEIOTypeTechnicAzureAngularMotorS:
                        if (connectInfo.VirtualConnection)
                        {
                            return new LEGODualTechnicMotor(this);
                        }
                        else
                        {
                            return new LEGOSingleTechnicMotor(this);
                        }

                    case IOType.LEIOTypeLEAFGameEngine:
                    {
                        return new LEGOLEAFGameEngine(this);
                    }
                    
                    case IOType.LEIOTypeLEAFFriendship:
                    {
                        return new LEGOLEAFFriendship(this);
                    }

                    case IOType.LEIOTypeTechnicColorSensor:
                        return new LEGOTechnicColorSensor(this);
                    case IOType.LEIOTypeTechnicDistanceSensor:
                        return new LEGOTechnicDistanceSensor(this);
                    case IOType.LEIOTypeTechnicForceSensor:
                        return new LEGOTechnicForceSensor(this);


                    case IOType.LEIOTypeBoostVM:
                    {
                        return new LEGOBoostVM(this);
                    }

                    case IOType.LEIOTypeRemoteControlButtonSensor:
                        return new LEGOButtonSensor(this);

                    default:
                        return new LEGOService(this);
                }
            }
        }

        #endregion
    }
}