/*!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
  !!    THIS FILE IS AUTO-GENERATED - please do not edit!             !!
  !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!*/

using System;
using BinaryReader = System.IO.BinaryReader;
using BinaryWriter = System.IO.BinaryWriter;
using MemoryStream= System.IO.MemoryStream;

#pragma warning disable 0109 // "hides inherited member - new: nothing to hide"
namespace dk.lego.devicesdk.bluetooth.V3.messages {
public class MessageFirmwareBootMode : LegoMessage {
    public readonly byte[] rawInputValues;

    public MessageFirmwareBootMode(UInt32 hubID, byte[] rawInputValues)
        : base(hubID, LegoMessageType.FIRMWARE_BOOT_MODE)
    {
        this.rawInputValues = rawInputValues;
    }

    override
    protected int totalLengthInBytes(int __totalLengthOfRemainder) {
        int __totalLength = __totalLengthOfRemainder;
        __totalLength += 0; // Fixed-length fields
        __totalLength += (rawInputValues).Length;
        return base.totalLengthInBytes(__totalLength);
    }

    override
    internal void unparse(BinaryWriter __buffer, int __totalLength) {
        base.unparse(__buffer, __totalLength);

        __buffer.Write(rawInputValues);
    }

    internal static new MessageFirmwareBootMode parse(BinaryReader __buffer, Byte hubID) {
        // Extract fields:
        #pragma warning disable 0219 // Ignore unused variables
        byte[] rawInputValues = parseRest(__buffer);
        #pragma warning restore 0219

        return new MessageFirmwareBootMode(hubID, rawInputValues);
    }

    override
    public void visitWith<T>(LegoMessage_Visitor<T> visitor, T arg) {
        visitor.handle_MessageFirmwareBootMode(this, arg);
    }

}
 }
