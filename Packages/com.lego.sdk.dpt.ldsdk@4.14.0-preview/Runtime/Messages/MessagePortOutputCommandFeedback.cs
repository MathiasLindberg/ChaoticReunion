/*!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
  !!    THIS FILE IS AUTO-GENERATED - please do not edit!             !!
  !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!*/

using System;
using BinaryReader = System.IO.BinaryReader;
using BinaryWriter = System.IO.BinaryWriter;
using MemoryStream= System.IO.MemoryStream;

#pragma warning disable 0109 // "hides inherited member - new: nothing to hide"
namespace dk.lego.devicesdk.bluetooth.V3.messages {
public class MessagePortOutputCommandFeedback : LegoMessage {
    public readonly Byte portID;
    public readonly Byte feedback;
    public readonly byte[] optionalFeedback;

    public MessagePortOutputCommandFeedback(UInt32 hubID, UInt32 portID, UInt32 feedback, byte[] optionalFeedback)
        : base(hubID, LegoMessageType.PORT_OUTPUT_COMMAND_FEEDBACK)
    {
        this.portID = (Byte)portID;
        this.feedback = (Byte)feedback;
        this.optionalFeedback = optionalFeedback;
    }

    override
    protected int totalLengthInBytes(int __totalLengthOfRemainder) {
        int __totalLength = __totalLengthOfRemainder;
        __totalLength += 2; // Fixed-length fields
        __totalLength += (optionalFeedback).Length;
        return base.totalLengthInBytes(__totalLength);
    }

    override
    internal void unparse(BinaryWriter __buffer, int __totalLength) {
        base.unparse(__buffer, __totalLength);

        __buffer.Write((Byte)(portID));
        __buffer.Write((Byte)(feedback));
        __buffer.Write(optionalFeedback);
    }

    internal static new MessagePortOutputCommandFeedback parse(BinaryReader __buffer, Byte hubID) {
        // Extract fields:
        #pragma warning disable 0219 // Ignore unused variables
        Byte portID = __buffer.ReadByte();
        Byte feedback = __buffer.ReadByte();
        byte[] optionalFeedback = parseRest(__buffer);
        #pragma warning restore 0219

        return new MessagePortOutputCommandFeedback(hubID, portID, feedback, optionalFeedback);
    }

    override
    public void visitWith<T>(LegoMessage_Visitor<T> visitor, T arg) {
        visitor.handle_MessagePortOutputCommandFeedback(this, arg);
    }

}
 }
