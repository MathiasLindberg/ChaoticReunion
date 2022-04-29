namespace CoreUnityBleBridge
{
    public static class Constants
    {
        public static class LDSDK
        {
            public const string V3_SERVICE = "00001623-1212-efde-1623-785feabcd123";
            public const string V3_CHARACTERISTIC = "00001624-1212-efde-1623-785feabcd123";
            public const string V3_BOOTLOADER_SERVICE = "00001625-1212-efde-1623-785feabcd123";
            public const string V3_BOOTLOADER_CHARACTERISTIC = "00001626-1212-efde-1623-785feabcd123";
        }

        public static class OAD
        {
            public const string SERVICE = "F000FFC0-0451-4000-B000-000000000000";

            public static class Characteristics
            {
                public const string IMAGE_NOTIFY = "F000FFC1-0451-4000-B000-000000000000";
                public const string IMAGE_BLOCK_REQUEST = "F000FFC2-0451-4000-B000-000000000000";
                public const string COUNT = "F000FFC3-0451-4000-B000-000000000000";
                public const string STATUS = "F000FFC4-0451-4000-B000-000000000000";
                public const string CONTROL = "F000FFC5-0451-4000-B000-000000000000";
            }
        }
    }
}