using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using dk.lego.devicesdk.bluetooth.V3.messages;
using LEGO.Logger;
using System.Globalization;

namespace LEGODeviceUnitySDK
{
    internal class ServiceInfoCache : CacheableMetadata, MessagePortModeInformation_Visitor<Void>
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(ServiceInfoCache));

        private readonly AttachedIOIdentificationKey key;

        public ServiceInfoCache(AttachedIOIdentificationKey key, string filePath) : base(filePath)
        {
            if (key == null) throw new NullReferenceException("service revision ID");
            this.key = key;
        }
        
        #region File format
        private HashSet<string> StringKeys = new HashSet<string> {
            "NAME", "SYMBOL", "ALLOWED_COMBINATIONS"
        };
        private HashSet<string> FloatKeys = new HashSet<string> {
            "MIN_RAW", "MAX_RAW",
            "MIN_PCT", "MAX_PCT",
            "MIN_SI", "MAX_SI"
        };
        private static readonly char[] Separators = new char[] {'\n', '\r'};

        public override void PopulateFromFileData(string data)
        {
            serviceData.Clear();
            portModeData.Clear();

            if (data != null) { 
                var lines = data.Split(Separators, StringSplitOptions.RemoveEmptyEntries);
                foreach (var line in lines) {
                    try {
                        PopulateWithLine(line);
                    } catch (Exception e) {
                        logger.Warn("Skipping line: "+line+": "+e);
                    }
                }
            }
            logger.Info("Read cache for service "+this.key.getIdentifier()+": sizes="+serviceData.Count+","+portModeData.Count);
        }

        private void PopulateWithLine(string line) {
            var colonPos = line.IndexOf(':');
            if (colonPos < 0) return;
            var key = line.Substring(0, colonPos);
            var valueStr = line.Substring(colonPos+1);

            var slashPos = key.IndexOf('/');
            if (slashPos >= 0) {
                // A mode-level property:
                var mode = Convert.ToInt32(key.Substring(0, slashPos), CultureInfo.InvariantCulture);
                var keyInMode = key.Substring(slashPos+1);

                if (!portModeData.ContainsKey(mode))
                    portModeData.Add(mode, new ValueDictionary());

                var value = ConvertValue(keyInMode, valueStr);
                portModeData[mode][keyInMode] = value;
            } else {
                // A service-level property:
                var value = ConvertValue(key, valueStr);
                serviceData[key] = value;
            }
        }
        private object ConvertValue(string key, string valueStr) {
            return StringKeys.Contains(key) ? (object)valueStr
                    : FloatKeys.Contains(key) ? (object)Convert.ToSingle(valueStr)
                    : (object)Convert.ToInt32(valueStr, CultureInfo.InvariantCulture);
        }

        public override string ToFileData()
        {
            var buf = new StringBuilder();
            foreach (var item in serviceData) {
                buf.Append(item.Key);
                buf.Append(":");
                buf.Append(Convert.ToString((IConvertible)item.Value, CultureInfo.InvariantCulture));
                buf.Append("\n");
            }
            foreach (var modeItem in this.portModeData) {
                var mode = modeItem.Key;
                foreach (var item in modeItem.Value) {
                    buf.Append(mode);
                    buf.Append("/");
                    buf.Append(item.Key);
                    buf.Append(":");
                    buf.Append(Convert.ToString((IConvertible)item.Value, CultureInfo.InvariantCulture));
                    buf.Append("\n");
                }
            }
            return buf.ToString();
        }
        #endregion

        #region Population
        private ValueDictionary serviceData = new ValueDictionary();
        private Dictionary<int,ValueDictionary> portModeData = new Dictionary<int, ValueDictionary>();

        public void PopulateWith(MessagePortInformationModeInfo msg)
        {
            serviceData["CAPABILITIES"] = msg.capabilities;
            serviceData["MODE_COUNT"]   = msg.modeCount;
            serviceData["INPUT_MODES"]  = msg.inputModes;
            serviceData["OUTPUT_MODES"] = msg.outputModes;
            MarkAsChanged();
        }

        public void PopulateWith(MessagePortInformationAllowedCombinations msg)
        {
            serviceData["ALLOWED_COMBINATIONS"] = Convert.ToBase64String(msg.allowedCombinations);
            MarkAsChanged();
        }

        public void PopulateWith(MessagePortModeInformation msg)
        {
            if (!portModeData.ContainsKey(msg.mode))
                portModeData.Add(msg.mode, new ValueDictionary());
                
            msg.visitWith(this, Void.Instance);
            MarkAsChanged();
        }

        void MessagePortModeInformation_Visitor<Void>.handle_MessagePortModeInformationName(MessagePortModeInformationName msg, Void arg)
        {
            portModeData[msg.mode]["NAME"] = msg.name;
        }

        void MessagePortModeInformation_Visitor<Void>.handle_MessagePortModeInformationRaw(MessagePortModeInformationRaw msg, Void arg)
        {
            portModeData[msg.mode]["MIN_RAW"] = msg.minRaw;
            portModeData[msg.mode]["MAX_RAW"] = msg.maxRaw;
        }

        void MessagePortModeInformation_Visitor<Void>.handle_MessagePortModeInformationPct(MessagePortModeInformationPct msg, Void arg)
        {
            portModeData[msg.mode]["MIN_PCT"] = msg.minPct;
            portModeData[msg.mode]["MAX_PCT"] = msg.maxPct;
        }

        void MessagePortModeInformation_Visitor<Void>.handle_MessagePortModeInformationSI(MessagePortModeInformationSI msg, Void arg)
        {
            portModeData[msg.mode]["MIN_SI"] = msg.minSI;
            portModeData[msg.mode]["MAX_SI"] = msg.maxSI;
        }

        void MessagePortModeInformation_Visitor<Void>.handle_MessagePortModeInformationSymbol(MessagePortModeInformationSymbol msg, Void arg)
        {
            portModeData[msg.mode]["SYMBOL"] = msg.symbol;
        }

        void MessagePortModeInformation_Visitor<Void>.handle_MessagePortModeInformationMapping(MessagePortModeInformationMapping msg, Void arg)
        {
            portModeData[msg.mode]["MAPPING_INPUT"] = msg.mappingInput;
            portModeData[msg.mode]["MAPPING_OUTPUT"] = msg.mappingOutput;
        }

        void MessagePortModeInformation_Visitor<Void>.handle_MessagePortModeInformationValueFormat(MessagePortModeInformationValueFormat msg, Void arg)
        {
            portModeData[msg.mode]["VALUE_FORMAT_COUNT"] = msg.valueFormatCount;
            portModeData[msg.mode]["VALUE_FORMAT_DECIMALS"] = msg.valueFormatDecimals;
            portModeData[msg.mode]["VALUE_FORMAT_FIGURES"] = msg.valueFormatFigures;
            portModeData[msg.mode]["VALUE_FORMAT_TYPE"] = (int)msg.valueFormatType;
        }
        #endregion

        #region Answering queries
        public MessagePortInformation LookupInfo(MessagePortInformationRequest request) {
            switch (request.information) {
            case PortInformationType.MODE_INFO:
                Byte capabilities;
                Byte modeCount;
                UInt16 inputModes;
                UInt16 outputModes;

                if (serviceData.TryGetValue("CAPABILITIES", out capabilities) &&
                    serviceData.TryGetValue("MODE_COUNT", out modeCount) &&
                    serviceData.TryGetValue("INPUT_MODES", out inputModes) &&
                    serviceData.TryGetValue("OUTPUT_MODES", out outputModes))
                return new MessagePortInformationModeInfo(request.hubID, request.portID,
                        capabilities, modeCount, inputModes, outputModes);
                else break;
            case PortInformationType.ALLOWED_MODE_COMBINATIONS:
                string acBase64;
                if (serviceData.TryGetValue("ALLOWED_COMBINATIONS", out acBase64))
                    try {
                    return new MessagePortInformationAllowedCombinations(request.hubID, request.portID,
                        Convert.FromBase64String(acBase64));
                } catch (FormatException) { break; }
                else break;
            }
            return null;
        }

        public MessagePortModeInformation LookupInfo(MessagePortModeInformationRequest request) {
            ValueDictionary dict;
            if (!portModeData.TryGetValue(request.mode, out dict)) {
                return null; // Nothing found for that mode.
            }

            switch (request.information) {
            case PortModeInformationType.NAME:
                string name;
                if (dict.TryGetValue("NAME", out name))
                    return new MessagePortModeInformationName(request.hubID, request.portID, request.mode,
                        name);
                else break;

            case PortModeInformationType.SYMBOL:
                string symbol;
                if (dict.TryGetValue("SYMBOL", out symbol))
                    return new MessagePortModeInformationSymbol(request.hubID, request.portID, request.mode,
                        symbol);
                else break;

            case PortModeInformationType.RAW: {
                    float min, max;
                    if (dict.TryGetValue("MIN_RAW", out min) &&
                        dict.TryGetValue("MAX_RAW", out max))
                        return new MessagePortModeInformationRaw(request.hubID, request.portID, request.mode,
                            min, max);
                    else break;
                }

            case PortModeInformationType.PCT: {
                    float min, max;
                    if (dict.TryGetValue("MIN_PCT", out min) &&
                        dict.TryGetValue("MAX_PCT", out max))
                        return new MessagePortModeInformationPct(request.hubID, request.portID, request.mode,
                            min, max);
                    else break;
                }

            case PortModeInformationType.SI: {
                    float min, max;
                    if (dict.TryGetValue("MIN_SI", out min) &&
                        dict.TryGetValue("MAX_SI", out max))
                        return new MessagePortModeInformationSI(request.hubID, request.portID, request.mode,
                            min, max);
                    else break;
                }

            case PortModeInformationType.MAPPING: {
                    uint mappingInput, mappingOutput;
                    if (dict.TryGetValue("MAPPING_INPUT", out mappingInput) &&
                        dict.TryGetValue("MAPPING_OUTPUT", out mappingOutput))
                        return new MessagePortModeInformationMapping(request.hubID, request.portID, request.mode,
                            mappingInput, mappingOutput);
                    else break;
                }

            case PortModeInformationType.VALUE_FORMAT: {
                    uint count, type, figures, decimals;
                    if (dict.TryGetValue("VALUE_FORMAT_COUNT", out count) &&
                        dict.TryGetValue("VALUE_FORMAT_TYPE", out type) &&
                        dict.TryGetValue("VALUE_FORMAT_FIGURES", out figures) &&
                        dict.TryGetValue("VALUE_FORMAT_DECIMALS", out decimals))
                        return new MessagePortModeInformationValueFormat(request.hubID, request.portID, request.mode,
                            count, (ModeInformationValueFormatType)type, figures, decimals);
                    else break;
                }

            }
            return null;
        }
        #endregion


        private class ValueDictionary : IEnumerable<KeyValuePair<string,object>> {
            // Allowed value types: int, float, string.
            // (Note that uint32 is also converted to int32.)
            private Dictionary<string, object> values = new Dictionary<string, object>();

            public object this[string key] {
                set {
                    if (value is byte || value is sbyte || value is UInt16 || value is Int16 || value is UInt32)
                        value = Convert.ToInt32(value, CultureInfo.InvariantCulture);
                    else if (value is float || value is double)
                        value = Convert.ToSingle(value, CultureInfo.InvariantCulture);

                    values[key] = value;
                }
            }

            public bool TryGetValue(string key, out string value) {
                object v;
                bool res = values.TryGetValue(key, out v) && v is string;
                value = res ? (string)v : null;
                return res;
            }

            public bool TryGetValue(string key, out byte value) {
                object v;
                bool res = values.TryGetValue(key, out v) && v is int;
                value = res ? (byte)(int)v : (byte)0;
                return res;
            }

            public bool TryGetValue(string key, out UInt16 value) {
                object v;
                bool res = values.TryGetValue(key, out v) && v is int;
                value = res ? (UInt16)(int)v : (UInt16)0;
                return res;
            }

            public bool TryGetValue(string key, out UInt32 value) {
                object v;
                bool res = values.TryGetValue(key, out v) && v is int;
                value = res ? (UInt32)(int)v : (UInt32)0;
                return res;
            }

            public bool TryGetValue(string key, out float value) {
                object v;
                bool res = values.TryGetValue(key, out v) && v is float;
                value = res ? (float)v : 0.0f;
                return res;
            }

            public void Clear() { values.Clear(); }
            public int Count { get { return values.Count; } }

            #region IEnumerable
            public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
            {
                return values.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return values.GetEnumerator();
            }
            #endregion
        }
    }

}

