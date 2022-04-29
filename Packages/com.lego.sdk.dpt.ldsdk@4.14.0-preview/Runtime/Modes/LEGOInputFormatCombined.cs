using System.Collections.Generic;
using System;

namespace LEGODeviceUnitySDK
{
    /// <summary>
    /// This class represent a Combined Mode configuration
    /// </summary>
    public class LEGOInputFormatCombined
    {
        /// <summary>
        /// The "Allowed Combination Index" to enable - specifies which mode that can be combined.
        /// </summary>
        public int CombinationIndex { get; private set; }

        [Obsolete("Use ConnectInfo.PortID instead.")]
        /// <summary>
        /// The portID of the corresponding service, see LEGOService.LEGOConnectionInfo
        /// </summary>
        public int PortID { get; private set; }

        /// <summary>
        /// Whether Combined Mode setup should be initiated with multi update or not
        /// </summary>
        public bool MultiUpdateEnabled { get; private set; }

        /// <summary>
        /// Array of of mode and associated data set combinations
        /// </summary>
        public int[] ModeDataSetCombinations { get; private set; }

        internal ModeDataSet[] ModeDataSets {get; private set; }

        internal LEGOInputFormatCombined(int portID, int combinationIndex, bool multiUpdateEnabled, int[] modeDataSetCombinations, ModeDataSet[] modeDataSets)
        {
            #pragma warning disable 0618 // Obsolete properties
            this.PortID = portID;
            #pragma warning restore 0618 // Obsolete properties
            this.CombinationIndex = combinationIndex;
            this.MultiUpdateEnabled = multiUpdateEnabled;
            this.ModeDataSetCombinations = modeDataSetCombinations;
            this.ModeDataSets = modeDataSets;
        }
        
        public string ArrayValuesToString(int[] array)
        {
            var strings = new List<string>();
            foreach (var i in array)
            {
                strings.Add(i.ToString());
            }

            var joined = string.Join(", ", strings.ToArray());
            return joined;
        }

        public override string ToString()
        {
            #pragma warning disable 0618 // Obsolete properties
            return string.Format("[LEGOInputFormatCombined: CombinationIndex={0}, PortID={1}, MultiUpdateEnabled={2}, ModeDataSetCombinations={3}]", CombinationIndex, PortID, MultiUpdateEnabled, ArrayValuesToString(ModeDataSetCombinations));
            #pragma warning restore 0618 // Obsolete properties
        }
    }

    internal struct ModeDataSet {
        public byte mode;
        public byte dataSet;
    }
}