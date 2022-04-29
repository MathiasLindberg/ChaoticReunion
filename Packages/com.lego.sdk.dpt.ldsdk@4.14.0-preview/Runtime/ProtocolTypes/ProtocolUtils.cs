using System;
using System.Collections.Generic;

namespace LEGODeviceUnitySDK
{
    public static class ProtocolUtils
    {
        public static int[] IntegerArrayFromBitSet(UInt32 bitSet) {
            return IntegerListFromBitSet(bitSet).ToArray();
        }

        public static List<int> IntegerListFromBitSet(UInt32 bitSet) {
            var list = new List<int>();

            for (int i = 0; i <= 31; ++i) {
                int bit = (1 << i);
                if ((bitSet & bit) != 0) {
                    list.Add(i);
                }
            }
            return list;
        }

        // Based on https://stackoverflow.com/questions/321370/how-can-i-convert-a-hex-string-to-a-byte-array :
        public static byte[] HexstringToByteArray(string hex, int prepad=0, int postpad=0) {
            if (hex.Length % 2 == 1)
                throw new Exception("The hexstring must have an even number of digits");

            var len = (hex.Length >> 1) + prepad + postpad;
            byte[] arr = new byte[len];

            for (int i = 0; i < hex.Length >> 1; ++i)
            {
                arr[prepad+i] = (byte)((GetHexVal(hex[i << 1]) << 4) + (GetHexVal(hex[(i << 1) + 1])));
            }

            return arr;
        }

        public static int GetHexVal(char hex) {
            int val = (int)hex;
            if (val >= '0' && val < '0'+10) return val-'0';
            if (val >= 'A' && val < 'A'+6) return val-'A'+10;
            if (val >= 'a' && val < 'a'+6) return val-'a'+10;
            throw new ArgumentException("Bad hex character: '"+hex+"'");
        }

    }
}

