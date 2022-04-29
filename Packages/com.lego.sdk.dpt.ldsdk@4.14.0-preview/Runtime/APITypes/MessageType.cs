using System;
namespace LEGODeviceUnitySDK
{
    public enum MessageType
    {
        /** Unknown */
        LEMessageTypeHubUnknown = 0xFF,
        /** Properties */
        LEMessageTypeHubProperties = 0x01,
        /** Hub Actions */
        LEMessageTypeHubActions = 0x02,
        /** Hub Alerts */
        LEMessageTypeHubAlerts = 0x03,
        /** Attached I/O */
        LEMessageTypeHubAttachedIO = 0x04,
        /** Errors */
        LEMessageTypeError = 0x05,
        /** Restart in boot mode */
        LEMessageTypeFirmwareUpdateBootMode = 0x10,
        /** Port Information Request */
        LEMessageTypePortInformationRequest = 0x21,
        /** Mode Information Request */
        LEMessageTypePortModeInformationRequest = 0x22,
        /** Input Format (Single) */
        LEMessageTypePortInputFormatSetupSingle = 0x41,
        /** Input Format (Combined) */
        LEMessageTypePortInputFormatSetupCombined = 0x42,
        /** Port Information */
        LEMessageTypePortInformation = 0x43,
        /** Port Mode Information */
        LEMessageTypePortModeInformation = 0x44,
        /** Port Value (Single) */
        LEMessageTypePortValueSingle = 0x45,
        /** Port Value (Combined) */
        LEMessageTypePortValueCombined = 0x46,
        /** Port Input Format (Single) */
        LEMessageTypePortInputFormatSingle = 0x47,
        /** Port Input Format (Combined) */
        LEMessageTypePortInputFormatCombined = 0x48,
        /** Virtual Port Setup */
        LEMessageTypeVirtualPortSetup = 0x61,
        /** Port Output Command */
        LEMessageTypePortOutputCommand = 0x81,
        /** Port Output Command Feedback */
        LEMessageTypePortOutputCommandFeedback = 0x82,
    }
}
