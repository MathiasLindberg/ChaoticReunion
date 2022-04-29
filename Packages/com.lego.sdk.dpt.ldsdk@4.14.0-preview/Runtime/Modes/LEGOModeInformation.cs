using System;
using System.Collections.Generic;

namespace LEGODeviceUnitySDK
{
    public enum LEGOModeInformationValueFormatType
    {
        /** 8 BIT */
        LEModeInformationValueFormatType8BIT = 0x00,
        /** 16 BIT */
        LEModeInformationValueFormatType16BIT = 0x01,
        /** 32 BIT */
        LEModeInformationValueFormatType32BIT = 0x02,
        /** Float */
        LEModeInformationValueFormatTypeFloat = 0x03
    }

    /// <summary>
    /// All information associated with a given mode
    /// </summary>
    public class LEGOModeInformation
    {
        public String Name { get; private set; }
        public int Number { get; private set; }
        public float MinRaw { get; private set; }
        public float MaxRaw { get; private set; }
        public float MinPct { get; private set; }
        public float MaxPct { get; private set; }
        public float MinSI { get; private set; }
        public float MaxSI { get; private set; }
        public String Symbol { get; private set; }
        public uint ValueFormatDataSetCount { get; private set; }
        public LEGOModeInformationValueFormatType ValueFormatDataSetType { get; private set; }
        public uint ValueFormatFigures { get; private set; }
        public uint ValueFormatDecimals { get; private set; }
        public uint ValueDataLength { get; private set; }
        /// Size of each value element.
        public uint ValueDataSetSize { get; private set; }

        public LEGOModeInformation(string name, int number, float minRaw, float maxRaw, float minPct, float maxPct, float minSI, float maxSI, string symbol, uint valueFormatDataSetCount, LEGOModeInformationValueFormatType valueFormatDataSetType, uint valueFormatFigures, uint valueFormatDecimals, uint valueDataLength, uint valueDataSetSize)
        {
            this.Name = name;
            this.Number = number;
            this.MinRaw = minRaw;
            this.MaxRaw = maxRaw;
            this.MinPct = minPct;
            this.MaxPct = maxPct;
            this.MinSI = minSI;
            this.MaxSI = maxSI;
            this.Symbol = symbol;
            this.ValueFormatDataSetCount = valueFormatDataSetCount;
            this.ValueFormatDataSetType = valueFormatDataSetType;
            this.ValueFormatFigures = valueFormatFigures;
            this.ValueFormatDecimals = valueFormatDecimals;
            this.ValueDataLength = valueDataLength;
            this.ValueDataSetSize = valueDataSetSize;
        }

        #region Value conversion
        internal float[] ConvertRawToPct(float[] rawValues) {
            return ConvertRaw(rawValues, MinPct, MaxPct);
        }

        internal float[] ConvertRawToSI(float[] rawValues) {
            return ConvertRaw(rawValues, MinSI, MaxSI);
        }

        private float[] ConvertRaw(float[] rawValues, float minTarget, float maxTarget) {
            if (rawValues==null) return null;

            var srcRange = MaxRaw - MinRaw;
            var targetRange = maxTarget - minTarget;

            if (srcRange==0.0) return null; // Don't bother translating.

            var res = new float[rawValues.Length];
            for (int i=0; i<res.Length; i++) {
                var converted = minTarget + (rawValues[i] - MinRaw) * targetRange / srcRange;
                res[i] = UnityEngine.Mathf.Clamp(converted, minTarget, maxTarget);
            }

            return res;
        }
        #endregion

        public override string ToString()
        {
            return string.Format("[LEGOModeInformation: Name={0}, Number={1}, Symbol={2}, ValueDataLength={3}, ValueDataSetSize={4}, " +
            "MinRaw={5}, MaxRaw={6}, MinPct={7}, MaxPct={8}, MinSI={9}, MaxSI={10}, ValueFormatDataSetCount={11}, ValueFormatDataSetType={12}, ValueFormatFigures={13}, ValueDataSetSize={14}, ValueFormatDecimals={15}]",
            Name, Number, Symbol, ValueDataLength, ValueDataSetSize, MinRaw, MaxRaw, MinPct, MaxPct, MinSI, MaxSI, ValueFormatDataSetCount, ValueFormatDataSetType, ValueFormatFigures, ValueDataSetSize, ValueFormatDecimals);
        }
 
        public class Builder {
            public String Name { get; set; }
            public int Number { get; set; }
            public float MinRaw { get; set; }
            public float MaxRaw { get; set; }
            public float MinPct { get; set; }
            public float MaxPct { get; set; }
            public float MinSI { get; set; }
            public float MaxSI { get; set; }
            public String Symbol { get; set; }
            public uint ValueFormatDataSetCount { get; set; }
            public LEGOModeInformationValueFormatType ValueFormatDataSetType { get; set; }
            public uint ValueFormatFigures { get; set; }
            public uint ValueFormatDecimals { get; set; }
            public uint ValueDataLength { get; set; }
            public uint ValueDataSetSize { get; set; }

            public Builder(int number)
            {
                this.Number = number;
            }

            public LEGOModeInformation Build() {
                return new LEGOModeInformation(
                    name:Name,
                    number:Number,
                    minRaw:MinRaw, maxRaw:MaxRaw,
                    minPct:MinPct, maxPct:MaxPct,
                    minSI:MinSI, maxSI:MaxSI,
                    symbol:Symbol,
                    valueFormatDataSetCount:ValueFormatDataSetCount,
                    valueFormatDataSetType:ValueFormatDataSetType,
                    valueFormatFigures:ValueFormatFigures,
                    valueFormatDecimals:ValueFormatDecimals,
                    valueDataLength:ValueDataLength,
                    valueDataSetSize:ValueDataSetSize);
            }
        }
        
    }
}