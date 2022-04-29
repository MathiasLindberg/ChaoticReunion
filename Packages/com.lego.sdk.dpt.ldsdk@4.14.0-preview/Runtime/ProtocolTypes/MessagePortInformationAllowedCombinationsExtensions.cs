using System;
using dk.lego.devicesdk.bluetooth.V3.messages;
using System.Collections.Generic;

namespace LEGODeviceUnitySDK
{
    public static class MessagePortInformationAllowedCombinationsExtensions
    {
        public static int[][] ParseAllowedCombinations(this MessagePortInformationAllowedCombinations self) {
            var allowedCombinations = new List<int[]>();
            var rawData = self.allowedCombinations;
            for (int combiNr=0; 2*combiNr<rawData.Length; combiNr++) {
                var bitSet = BitConverter.ToUInt16(rawData, 2*combiNr);
                allowedCombinations.Add(ProtocolUtils.IntegerArrayFromBitSet(bitSet));
            }
            return allowedCombinations.ToArray();
        }

        public static uint[] ParseAllowedCombinationMasks(this MessagePortInformationAllowedCombinations self) {
            var bitmasks = new List<uint>();
            var rawData = self.allowedCombinations;
            for (int combiNr=0; 2*combiNr<rawData.Length; combiNr++) {
                var bitSet = BitConverter.ToUInt16(rawData, 2*combiNr);
                bitmasks.Add(bitSet);
            }
            return bitmasks.ToArray();
        }
            
    }
}

