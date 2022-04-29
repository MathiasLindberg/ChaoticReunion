using System;
using System.Collections.Generic;
using LEGO.Logger;

namespace LEGODeviceUnitySDK
{
    /// <summary>
    /// Holds all mode information associated with a given attached I/O
    /// </summary>
    public class LEGOAttachedIOModeInformation
    {

        /// <summary>
        /// Input modes of the attached I/O
        /// </summary>
        public int[] InputModes { get; private set; }

        /// <summary>
        /// Outputmodes of the attached I/O
        /// </summary>
        public int[] OutputModes { get; private set; }

        /// <summary>
        /// Allowed combinations of the different modes associated with the attached I/O
        /// </summary>
        public int[][] AllowedModeCombinations { get; private set; }
        internal readonly uint[] AllowedModeCombinationMasks;

        /// <summary>
        /// The number of available modes
        /// </summary>
        public int ModeCount { get; private set; }

        public LEGOModeInformation[] AttachedIOModeInformation { get; private set; }

        private LEGOAttachedIOModeInformation(int[] inputModes, int[] outputModes,
            int[][] allowedModeCombinations, uint[] allowedModeCombinationMasks,
            int modeCount, LEGOModeInformation[] attachedIOModeInformation)
        {
            this.InputModes = inputModes;
            this.OutputModes = outputModes;
            this.AllowedModeCombinations = allowedModeCombinations;
            this.AllowedModeCombinationMasks = allowedModeCombinationMasks;
            this.ModeCount = modeCount;
            this.AttachedIOModeInformation = attachedIOModeInformation;
        }

        public override string ToString()
        {
            return string.Format("[LEGOAttachedIOModeInformation: InputModes={0}, OutputModes={1}, ModeCount={2}]", ArrayValuesToString(InputModes), ArrayValuesToString(OutputModes), ModeCount.ToString());
        }

        private static string ArrayValuesToString(int[] array)
        {
            if (array == null || array.Length == 0)
                return "NotExist/Empty";
            
            var strings = new List<string>();
            foreach (var i in array)
            {
                strings.Add(i.ToString());
            }
            var joined = string.Join(", ", strings.ToArray());
            return joined;
        }

        /// <summary>
        /// Retrieves the mode information associated with a given mode name
        /// </summary>
        /// <returns>LEGOModeInformation associated with provided mode name, if no association null is returned.</returns>
        /// <param name="name">The mode name to fetch information for</param>
        public LEGOModeInformation ModeInformationForName(string name)
        {
            if (AttachedIOModeInformation != null && AttachedIOModeInformation.Length > 0)
            {
                foreach (var modeInfo in AttachedIOModeInformation)
                {
                    if (modeInfo.Name == name)
                        return modeInfo;
                }
            }
            return null;
        }

        /// <summary>
        /// Retrieves the mode information associated with a given mode
        /// </summary>
        /// <returns>LEGOModeInformation associated with provided mode number, if no association null is returned.</returns>
        /// <param name="modeNumber">The mode to fetch information for</param>
        public LEGOModeInformation ModeInformationForModeNumber(int modeNumber)
        {
            if (AttachedIOModeInformation != null && AttachedIOModeInformation.Length > 0)
            {
                foreach (var modeInfo in AttachedIOModeInformation)
                {
                    if (modeInfo.Number == modeNumber)
                        return modeInfo;
                }
            }
            return null;
        }

        /*
        int[] JsonNodesToIntArray(JSONNode[] jsonNodes)
        {
            var intValues = new int[jsonNodes.Length];
            for (int i = 0; i < jsonNodes.Length; i++)
            {
                intValues[i] = jsonNodes[i].GetInt();
            }
            return intValues;
        }

        LEGOModeInformation[] JsonNodesToModeInformationArray(JSONNode[] jsonNodes)
        {
            var modeInformationValues = new LEGOModeInformation[jsonNodes.Length];
            for (int i = 0; i < jsonNodes.Length; i++)
            {
                var modeInformationJsonNode = jsonNodes[i];
                modeInformationValues[i] = new LEGOModeInformation(modeInformationJsonNode);
            }
            return modeInformationValues;
        }
        */

        internal class Builder {
            public int[] InputModes;
            public int[] OutputModes;
            public int[][] AllowedModeCombinations;
            public uint[] AllowedModeCombinationMasks;
            public int ModeCount;

            public LEGOModeInformation[] AttachedIOModeInformation;

            public LEGOAttachedIOModeInformation Build() {
                return new LEGOAttachedIOModeInformation(
                    inputModes:InputModes,
                    outputModes:OutputModes,
                    allowedModeCombinations:AllowedModeCombinations,
                    allowedModeCombinationMasks:AllowedModeCombinationMasks,
                    modeCount:ModeCount,
                    attachedIOModeInformation:AttachedIOModeInformation);
            }
        }

    }
}