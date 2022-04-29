/*!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
  !!    THIS FILE IS AUTO-GENERATED - please do not edit!             !!
  !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!*/

using System;
using BinaryReader = System.IO.BinaryReader;
using BinaryWriter = System.IO.BinaryWriter;
using MemoryStream= System.IO.MemoryStream;

#pragma warning disable 0109 // "hides inherited member - new: nothing to hide"
namespace dk.lego.devicesdk.bluetooth.V3.messages {
public class MessagePortInputFormatSetupCombined : MessagePortDynamicsRelated {
    public readonly PortInputFormatSetupCombinedSubCommandTypeEnum subCommand;
    public readonly byte[] rawInputValue;

    public MessagePortInputFormatSetupCombined(UInt32 hubID, UInt32 portID, PortInputFormatSetupCombinedSubCommandTypeEnum subCommand, byte[] rawInputValue)
        : base(hubID, LegoMessageType.PORT_INPUT_FORMAT_SETUP_COMBINED, portID)
    {
        this.subCommand = subCommand;
        this.rawInputValue = rawInputValue;
    }

    override
    protected int totalLengthInBytes(int __totalLengthOfRemainder) {
        int __totalLength = __totalLengthOfRemainder;
        __totalLength += 1; // Fixed-length fields
        __totalLength += (rawInputValue).Length;
        return base.totalLengthInBytes(__totalLength);
    }

    override
    internal void unparse(BinaryWriter __buffer, int __totalLength) {
        base.unparse(__buffer, __totalLength);

        __buffer.Write((Byte)((Byte)(subCommand)));
        __buffer.Write(rawInputValue);
    }

    internal static new MessagePortInputFormatSetupCombined parse(BinaryReader __buffer, Byte hubID, Byte portID) {
        // Extract fields:
        #pragma warning disable 0219 // Ignore unused variables
        PortInputFormatSetupCombinedSubCommandTypeEnum subCommand = (PortInputFormatSetupCombinedSubCommandTypeEnum)(__buffer.ReadByte());
        byte[] rawInputValue = parseRest(__buffer);
        #pragma warning restore 0219

        return new MessagePortInputFormatSetupCombined(hubID, portID, subCommand, rawInputValue);
    }

    override
    public void visitWith<T>(MessagePortDynamicsRelated_Visitor<T> visitor, T arg) {
        visitor.handle_MessagePortInputFormatSetupCombined(this, arg);
    }

}
 }
