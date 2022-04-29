using System;

namespace LEGODeviceUnitySDK
{
    public static class ModeInformationValueFormatTypeExtensions
    {
        public static uint ByteSize(this LEGOModeInformationValueFormatType type) {
            switch (type) {
            case LEGOModeInformationValueFormatType.LEModeInformationValueFormatType8BIT:
                return 1;
            case LEGOModeInformationValueFormatType.LEModeInformationValueFormatType16BIT:
                return 2;
            case LEGOModeInformationValueFormatType.LEModeInformationValueFormatType32BIT:
            case LEGOModeInformationValueFormatType.LEModeInformationValueFormatTypeFloat:
                return 4;
            default:
                return 0;
            }
        }
    }
}

