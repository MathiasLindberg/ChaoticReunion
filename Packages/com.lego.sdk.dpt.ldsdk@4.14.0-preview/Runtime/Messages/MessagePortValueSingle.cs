/*!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
  !!    THIS FILE IS AUTO-GENERATED - please do not edit!             !!
  !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!*/

using System;
using BinaryReader = System.IO.BinaryReader;
using BinaryWriter = System.IO.BinaryWriter;
using MemoryStream= System.IO.MemoryStream;

#pragma warning disable 0109 // "hides inherited member - new: nothing to hide"
namespace dk.lego.devicesdk.bluetooth.V3.messages {
public class MessagePortValueSingle : LegoMessage {
    public readonly Byte portID;
    public readonly byte[] rawInputValue;

    public MessagePortValueSingle(UInt32 hubID, UInt32 portID, byte[] rawInputValue)
        : base(hubID, LegoMessageType.PORT_VALUE_SINGLE)
    {
        this.portID = (Byte)portID;
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

        __buffer.Write((Byte)(portID));
        __buffer.Write(rawInputValue);
    }

    internal static new MessagePortValueSingle parse(BinaryReader __buffer, Byte hubID) {
        // Extract fields:
        #pragma warning disable 0219 // Ignore unused variables
        Byte portID = __buffer.ReadByte();
        byte[] rawInputValue = parseRest(__buffer);
        #pragma warning restore 0219

        return new MessagePortValueSingle(hubID, portID, rawInputValue);
    }

    override
    public void visitWith<T>(LegoMessage_Visitor<T> visitor, T arg) {
        visitor.handle_MessagePortValueSingle(this, arg);
    }

}
 }
