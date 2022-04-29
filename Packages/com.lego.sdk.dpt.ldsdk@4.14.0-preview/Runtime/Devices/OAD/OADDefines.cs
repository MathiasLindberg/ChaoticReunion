/**
 *
 * -- Ported Code --
 * Ported C# version of SimpleLink BLE OAD iOS from Texas Instruments.
 * 
 * Ported SimpleLink Version: 1.0.1
 * 
 * Source URL: http://git.ti.com/simplelink-ble-oad-ios/simplelink-ble-oad-ios/trees/master/ti_oad/Classes
 *
 */

namespace LEGODeviceUnitySDK
{
    public static class OADDefines
    {
        public const string OAD_SERVICE                             = "f000ffc0-0451-4000-b000-000000000000";
        public const string OAD_IMAGE_NOTIFY_CHARACTERISTIC         = "f000ffc1-0451-4000-b000-000000000000";
        public const string OAD_IMAGE_BLOCK_REQUEST_CHARACTERISTIC  = "f000ffc2-0451-4000-b000-000000000000";
        public const string OAD_COUNT_CHARACTERISTIC                = "f000ffc3-0451-4000-b000-000000000000";
        public const string OAD_STATUS_CHARACTERISTIC               = "f000ffc4-0451-4000-b000-000000000000";
        public const string OAD_CONTROL_CHARACTERISTIC              = "f000ffc5-0451-4000-b000-000000000000";

        public const byte TOAD_CONTROL_CMD_GET_BLOCK_SIZE                  = 0x01;
        public const byte TOAD_CONTROL_CMD_START_OAD_PROCESS               = 0x03;
        public const byte TOAD_CONTROL_CMD_ENABLE_OAD_IMAGE_CMD            = 0x04;
        public const byte TOAD_CONTROL_CMD_CANCEL_OAD                      = 0x05;
        public const byte TOAD_CONTROL_CMD_GET_DEVICE_TYPE_CMD             = 0x10;
        public const byte TOAD_CONTROL_CMD_IMAGE_BLOCK_WRITE_CHAR_RESPONSE = 0x12;
        
        public const byte EOAD_STATUS_CODE_SUCCESS                         = 0x00;
        public const byte EOAD_STATUS_CODE_CRC_ERR                         = 0x01;
        public const byte EOAD_STATUS_CODE_FLASH_ERR                       = 0x02;
        public const byte EOAD_STATUS_CODE_BUFFER_OFL                      = 0x03;
        public const byte EOAD_STATUS_CODE_ALREADY_STARTED                 = 0x04;
        public const byte EOAD_STATUS_CODE_NOT_STARTED                     = 0x05;
        public const byte EOAD_STATUS_CODE_DL_NOT_COMPLETE                 = 0x06;
        public const byte EOAD_STATUS_CODE_NO_RESOURCES                    = 0x07;
        public const byte EOAD_STATUS_CODE_IMAGE_TOO_BIG                   = 0x08;
        public const byte EOAD_STATUS_CODE_INCOMPATIBLE_IMAGE              = 0x09;
        public const byte EOAD_STATUS_CODE_INVALID_FILE                    = 0x0A;
        public const byte EOAD_STATUS_CODE_INCOMPATIBLE_FILE               = 0x0B;
        public const byte EOAD_STATUS_CODE_AUTH_FAIL                       = 0x0C;
        public const byte EOAD_STATUS_CODE_EXT_NOT_SUPPORTED               = 0x0D;
        public const byte EOAD_STATUS_CODE_DL_COMPLETE                     = 0x0E;
        public const byte EOAD_STATUS_CODE_CCCD_NOT_ENABLED                = 0x0F;
        public const byte EOAD_STATUS_CODE_IMG_ID_TIMEOUT                  = 0x10;

        public const uint SEGMENT_TYPE_OFF_WIRELESS_STD = 1;
        public const uint SEGMENT_TYPE_OFF_PAYLOAD_LEN = 4;

        public const byte SEGMENT_TYPE_BOUNDARY_INFO    = 0x00;
        public const byte SEGMENT_TYPE_CONTIGUOUS_INFO  = 0x01;
        public const byte SEGMENT_TYPE_SECURITY_INFO    = 0x03;

        public const byte TOAD_WIRELESS_STD_BLE                 = 0x01;
        public const byte TOAD_WIRELESS_STD_802_15_4_SUB_ONE    = 0x02;
        public const byte TOAD_WIRELESS_STD_802_15_4_2_POINT_4  = 0x04;
        public const byte TOAD_WIRELESS_STD_ZIGBEE              = 0x08;
        public const byte TOAD_WIRELESS_STD_RF4CE               = 0x10;
        public const byte TOAD_WIRELESS_STD_THREAD              = 0x20;
        public const byte TOAD_WIRELESS_STD_EASY_LINK           = 0x40;

        public const byte TOAD_IMAGE_COPY_STATUS_NO_ACTION_NEEDED   = 0xFF;
        public const byte TOAD_IMAGE_COPY_STATUS_IMAGE_TO_BE_COPIED = 0xFE;
        public const byte TOAD_IMAGE_COPY_STATUS_IMAGE_COPIED       = 0xFC;

        public const byte TOAD_IMAGE_VERIFICATION_STATUS_STD        = 0xFF;
        public const byte TOAD_IMAGE_VERIFICATION_STATUS_FAILED     = 0xFC;
        public const byte TOAD_IMAGE_VERIFICATION_STATUS_SUCCESS    = 0xFE;

        public const byte TOAD_IMAGE_CRC_STATUS_INVALID             = 0x00;
        public const byte TOAD_IMAGE_CRC_STATUS_VALID               = 0x02;
        public const byte TOAD_IMAGE_CRC_STATUS_NOT_CALCULATED_YET  = 0x03;

        public const byte TOAD_IMAGE_TYPE_PERSISTENT_APP    = 0x00;
        public const byte TOAD_IMAGE_TYPE_APP               = 0x01;
        public const byte TOAD_IMAGE_TYPE_STACK             = 0x02;
        public const byte TOAD_IMAGE_TYPE_APP_STACK_MERGED  = 0x03;
        public const byte TOAD_IMAGE_TYPE_NETWORK_PROC      = 0x04;
        public const byte TOAD_IMAGE_TYPE_BLE_FACTORY_IMAGE = 0x05;
        public const byte TOAD_IMAGE_TYPE_BIM               = 0x06;

        public static string[] EOAD_STATUS_STRINGS =
        {
            "OAD Succeeded",
            "The downloaded image’s CRC doesn’t match the one expected from the metadata",
            "Flash function failure such as flashOpen/flashRead/flash write/flash erase",
            "The block number of the received packet doesn’t match the one requested, an overflow has occurred.",
            "OAD start command received, while OAD is already is progress",
            "OAD data block received with OAD start process",
            "OAD enable command received without complete OAD image download",
            "Memory allocation fails/ used only for backward compatibility",
            "Image is too big",
            "Stack and flash boundary mismatch, program entry mismatch",
            "Invalid image ID received",
            "BIM/image header/firmware version mismatch",
            "Start OAD process / Image Identify message/image payload authentication/validation fail",
            "Data length extension or OAD control point characteristic not supported",
            "OAD image payload download complete",
            "Internal (target side) error code used to halt the process if a CCCD has not been enabled",
            "OAD Image ID has been tried too many times and has timed out. Device will disconnect."
        };

        public static string[] EOAD_STATE_STRINGS =
        {
            "Initializing",
            "Peripheral not connected anymore",
            "OAD Service is missing on peripheral",
            "Ready",
            "Get device type command sent",
            "Got device type response received",
            "OAD block size request sent",
            "OAD block size response received",
            "Header sent",
            "Header OK received",
            "Header FAIL received",
            "OAD Start download process sent",
            "Image transfer in progress",
            "Image transfer failed",
            "Image transfer completed",
            "OAD enable new image command sent",
            "Feedback complete OK",
            "Feedback complete FAILED",
            "Disconnect during download !",
            "Device disconnect, OAD complete !",
            "RSSI getting too low to program !"
        };

        public const uint EOAD_IMAGE_IDENTIFY_PACKAGE_LEN    = 22;

        public const string EOAD_IMAGE_INFO_CC2640R2         = "OAD IMG ";
        public const string EOAD_IMAGE_INFO_CC26x2R1         = "CC26x2R1";
        public const string EOAD_IMAGE_INFO_CC13x2R1         = "CC13x2R1";

        enum HwRevision
        {
            HWREV_Unknown = -1, //!< -1 means that the chip's HW revision is unknown.
            HWREV_1_0 = 10,     //!< 10 means that the chip's HW revision is 1.0
            HWREV_1_1 = 11,     //!< 11 means that the chip's HW revision is 1.1
            HWREV_2_0 = 20,     //!< 20 means that the chip's HW revision is 2.0
            HWREV_2_1 = 21,     //!< 21 means that the chip's HW revision is 2.1
            HWREV_2_2 = 22,     //!< 22 means that the chip's HW revision is 2.2
            HWREV_2_3 = 23,     //!< 23 means that the chip's HW revision is 2.3
            HWREV_2_4 = 24      //!< 24 means that the chip's HW revision is 2.4
        }

        enum ChipFamily
        {
            FAMILY_Unknown = -1,        //!< -1 means that the chip's family member is unknown.
            FAMILY_CC26x0 = 0,          //!<  0 means that the chip is a CC26x0 family member.
            FAMILY_CC13x0 = 1,          //!<  1 means that the chip is a CC13x0 family member.
            FAMILY_CC26x1 = 2,          //!<  2 means that the chip is a CC26x1 family member.
            FAMILY_CC26x0R2 = 3,        //!<  3 means that the chip is a CC26x0R2 family (new ROM contents).
            FAMILY_CC13x2_CC26x2 = 4    //!<  4 means that the chip is a CC13x2, CC26x2 family member.
        }

        enum ChipType
        {
            CHIP_TYPE_Unknown = -1, //!< -1 means that the chip type is unknown.
            CHIP_TYPE_CC1310 = 0,   //!<  0 means that this is a CC1310 chip.
            CHIP_TYPE_CC1350 = 1,   //!<  1 means that this is a CC1350 chip.
            CHIP_TYPE_CC2620 = 2,   //!<  2 means that this is a CC2620 chip.
            CHIP_TYPE_CC2630 = 3,   //!<  3 means that this is a CC2630 chip.
            CHIP_TYPE_CC2640 = 4,   //!<  4 means that this is a CC2640 chip.
            CHIP_TYPE_CC2650 = 5,   //!<  5 means that this is a CC2650 chip.
            CHIP_TYPE_CUSTOM_0 = 6, //!<  6 means that this is a CUSTOM_0 chip.
            CHIP_TYPE_CUSTOM_1 = 7, //!<  7 means that this is a CUSTOM_1 chip.
            CHIP_TYPE_CC2640R2 = 8, //!<  8 means that this is a CC2640R2 chip.
            CHIP_TYPE_CC2642 = 9,   //!<  9 means that this is a CC2642 chip.
            CHIP_TYPE_CC2644 = 10,  //!< 10 means that this is a CC2644 chip.
            CHIP_TYPE_CC2652 = 11,  //!< 11 means that this is a CC2652 chip.
            CHIP_TYPE_CC1312 = 12,  //!< 12 means that this is a CC1312 chip.
            CHIP_TYPE_CC1352 = 13,  //!< 13 means that this is a CC1352 chip.
            CHIP_TYPE_CC1352P = 14  //!< 14 means that this is a CC1354 chip.
        }

        public static bool IsCharacteristicForOADness(string characteristic)
        {
            switch (characteristic)
            {
                case OAD_IMAGE_NOTIFY_CHARACTERISTIC:
                case OAD_IMAGE_BLOCK_REQUEST_CHARACTERISTIC:
                case OAD_COUNT_CHARACTERISTIC:
                case OAD_STATUS_CHARACTERISTIC:
                    return true;
                default:
                    return characteristic == OAD_CONTROL_CHARACTERISTIC;
            }
        }
    }
}