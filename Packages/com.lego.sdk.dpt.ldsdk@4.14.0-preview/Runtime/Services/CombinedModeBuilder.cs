using System;
using System.Collections.Generic;
using LEGO.Logger;
using dk.lego.devicesdk.bluetooth.V3.messages;
using System.Linq;

namespace LEGODeviceUnitySDK
{
    public class CombinedModeBuilder
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(CombinedModeBuilder));

        private const int MAXIMUM_MODE_DATA_SET_CONFIGURATIONS = 16;
        private const int MAXIMUM_SUPPORTED_MODE_NUMBER = 15;
        private const int MAXIMUM_SUPPORTED_DATA_SET_NUMBER = 15;

        private readonly ILEGOService service;
        private readonly List<CombinedModeConfiguration> dataSets = new List<CombinedModeConfiguration>();
        private bool enableCombinedModeMultiUpdate = false;

        private uint includedModesMask = 0;

        public CombinedModeBuilder(ILEGOService service)
        {
            this.service = service;
        }
        

        public void AddMode(uint modeNo, uint deltaInterval, bool updateTrigger) {
            var modeInformation = service.modeInformationForModeNo((int)modeNo);
            if (modeInformation == null) {
                logger.Error(String.Format("No Mode Information found for service {0} mode number {1}", service.ServiceName, modeNo));
                return;
            }

            if (modeNo > MAXIMUM_SUPPORTED_MODE_NUMBER) {
                logger.Error(String.Format("Cannot include mode number {0} in a combined mode - mode number is too high", modeNo));
                return;
            }

            var dataSetCount = modeInformation.ValueFormatDataSetCount;
            var modeBit = (1u << (int)modeNo);
            if ((includedModesMask & modeBit) != 0) {
                logger.Warn(String.Format("Mode/DataSet configuration for mode {0} already present. Overwritten. Consider performing reset if new setup should be initiated.", modeNo));
                dataSets.RemoveAll(x => x.modeNo == modeNo);
            }

            for (var dataSetNr = 0u; dataSetNr < dataSetCount && dataSetNr <= MAXIMUM_SUPPORTED_DATA_SET_NUMBER; dataSetNr++) {
                // Check for capacity:
                if (dataSets.Count >= MAXIMUM_MODE_DATA_SET_CONFIGURATIONS) {
                    logger.Error(String.Format("Maximum of {0} Mode/DataSet combinations reached! No more configurations allowed. Perform reset to initiate new setup.", MAXIMUM_MODE_DATA_SET_CONFIGURATIONS));
                    return;
                }

                // Update non-list state & emit warnings:
                var dataSetIsUpdateTrigger = updateTrigger && dataSetNr==0;
                if (dataSetNr==0) {
                    includedModesMask |= modeBit;
                }
                if (dataSetIsUpdateTrigger) {
                    if (enableCombinedModeMultiUpdate)
                        logger.Warn(String.Format("Mode {0} has been marked as an update trigger, but one is already present.", modeNo));
                    else 
                        enableCombinedModeMultiUpdate = true;
                }

                // Insert into list:
                var dataSetConfig = new CombinedModeConfiguration(modeNo, dataSetNr, deltaInterval);
                if (dataSetIsUpdateTrigger)
                    dataSets.Insert(0, dataSetConfig);
                else
                    dataSets.Add(dataSetConfig);
            }
        }

        public uint? FindCombinedModeIndex() {
            var allowedMasks = service.ModeInformation.AllowedModeCombinationMasks;

            if (allowedMasks == null) {
                logger.Error(String.Format("Could not combine modes for service {0} since no combinations could be found", service.ServiceName));
                return null;
            }

            for (var index = 0u; index < allowedMasks.Length; index++) {
                var notInThisCombi = includedModesMask & ~allowedMasks[index];
                if (notInThisCombi == 0)
                    return index;
            }


            logger.Error(String.Format("No valid allowed combination of modes matches the configured one (mask={0:X}) for the service {1}.", includedModesMask, service.ServiceName));

            return null;
        }

        public List<LegoMessage> BuildMessageSequence() {
            var indexOpt = FindCombinedModeIndex();
            if (!indexOpt.HasValue) {
                return null;
            }

            var index = indexOpt.Value;

            var messages = new List<LegoMessage>();
            var portID = (uint)service.ConnectInfo.PortID;
            var empty = new byte[0];
            messages.Add(new MessagePortInputFormatSetupCombined(0, portID, PortInputFormatSetupCombinedSubCommandTypeEnum.LOCK, empty));
            foreach (var cfg in dataSets) {
                messages.Add(new MessagePortInputFormatSetupSingle(0, portID,
                    cfg.modeNo, cfg.deltaInterval, true));
            }

            { // Construct SET_MODE_AND_DATA:
                var payload = new byte[1+dataSets.Count];
                payload[0] = (byte)index;
                var pos = 1;
                foreach (var x in dataSets)
                    payload[pos++] = (byte)((x.modeNo << 4) | (x.dataSetIndex));

                messages.Add(new MessagePortInputFormatSetupCombined(0, portID, PortInputFormatSetupCombinedSubCommandTypeEnum.SET_MODE_AND_DATA,
                    payload));
            }

            var commandType = enableCombinedModeMultiUpdate
                ? PortInputFormatSetupCombinedSubCommandTypeEnum.UNLOCK_AND_START_MULTI_UPDATE_ENABLED
                : PortInputFormatSetupCombinedSubCommandTypeEnum.UNLOCK_AND_START_MULTI_UPDATE_DISABLED;
            messages.Add(new MessagePortInputFormatSetupCombined(0, portID, commandType, empty));

            return messages;
        }

        internal ModeDataSet[] DataSetOrder() {
            var dataSetOrder = new ModeDataSet[dataSets.Count];
            int pos = 0;
            foreach (var x in dataSets)
                dataSetOrder[pos++] = new ModeDataSet {
                mode = (byte)x.modeNo,
                dataSet = (byte)x.dataSetIndex
            };
            return dataSetOrder;
        }
    }

    public class CombinedModeConfiguration {
        public readonly uint modeNo;
        public readonly uint dataSetIndex;
        public readonly uint deltaInterval;

        public CombinedModeConfiguration(uint modeNo, uint dataSetIndex, uint deltaInterval)
        {
            this.modeNo = modeNo;
            this.dataSetIndex = dataSetIndex;
            this.deltaInterval = deltaInterval;
        }
        
    }
}

