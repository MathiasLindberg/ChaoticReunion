using System;
using System.Collections.Generic;
using UnityEngine;

namespace LEGODeviceUnitySDK
{

    public struct LEGOValueRange 
    {
        private readonly float min, max;
        public float Max { get {return max; } }
        public float Min { get {return min; } }

        public LEGOValueRange(float min, float max) { this.min = min; this.max = max; }
    }


    public class LEGOValue
    {

        public LEGOValue(int mode, string modeName, float[] rawValues, float[] pctValues, float[] siValues,
            byte[] rawData)
        {
            this._mode = mode;
            this._modeName = modeName;
            this._rawValues = rawValues;
            this._pctValues = pctValues;
            this._siValues = siValues;
            this._rawData = rawData;
        }

        private readonly int _mode;
        public int Mode { get { return _mode; } }

        private readonly string _modeName;
        public string ModeName { get { return _modeName; } }

        private readonly float[] _rawValues, _pctValues, _siValues;
        public float[] RawValues { get { return _rawValues; } }
        public float[] PctValues { get { return _pctValues; } }
        public float[] SIValues { get { return _siValues; } }

        private byte[] _rawData;
        public byte[] RawData { get {return _rawData; } }

        public override string ToString()
        {
            return string.Format("[LEGOValue: Mode:{0}, ModeName={1}, RawValues={2}, PctValues={3}, SIValues={4}]", Mode, ModeName, RawValues, PctValues, SIValues);
        }

        public string RawValuesToString()
        {
            var strings = new List<string>();
            foreach (var rawValue in RawValues)
            {
                strings.Add(rawValue.ToString());
            }
            var joined = string.Join(", ", strings.ToArray());
            return joined;
        }
    }
}

