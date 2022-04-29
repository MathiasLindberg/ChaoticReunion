using System;
namespace LEGODeviceUnitySDK
{
    public enum MessageErrorType
    {
        /** ACK */
        LEMessageErrorTypeACK = 0x01,
        /** NACK */
        LEMessageErrorTypeNACK = 0x02,
        /** Buffer Overflow */
        LEMessageErrorTypeBufferOverFlow = 0x03,
        /** Timeout */
        LEMessageErrorTypeTimeout = 0x04,
        /** Command not recognized */
        LEMessageErrorTypeCommandNotRecognized = 0x05,
        /** Invalid use (e.g. parameter errors) */
        LEMessageErrorTypeInvalidUse = 0x06
    }
}