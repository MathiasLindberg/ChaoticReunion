/*!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
  !!    THIS FILE IS AUTO-GENERATED - please do not edit!             !!
  !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!*/

using System;
using BinaryReader = System.IO.BinaryReader;
using BinaryWriter = System.IO.BinaryWriter;
using MemoryStream= System.IO.MemoryStream;

#pragma warning disable 0109 // "hides inherited member - new: nothing to hide"
namespace dk.lego.devicesdk.bluetooth.V3.messages {
public class MessageError : LegoMessage {
    public readonly Byte commandID;
    public readonly ErrorType errorType;
    public readonly byte[] payload;

    public MessageError(UInt32 hubID, UInt32 commandID, ErrorType errorType, byte[] payload)
        : base(hubID, LegoMessageType.ERROR)
    {
        this.commandID = (Byte)commandID;
        this.errorType = errorType;
        this.payload = payload;
    }

    override
    protected int totalLengthInBytes(int __totalLengthOfRemainder) {
        int __totalLength = __totalLengthOfRemainder;
        __totalLength += 2; // Fixed-length fields
        __totalLength += (payload).Length;
        return base.totalLengthInBytes(__totalLength);
    }

    override
    internal void unparse(BinaryWriter __buffer, int __totalLength) {
        base.unparse(__buffer, __totalLength);

        __buffer.Write((Byte)(commandID));
        __buffer.Write((Byte)((Byte)(errorType)));
        __buffer.Write(payload);
    }

    internal static new MessageError parse(BinaryReader __buffer, Byte hubID) {
        // Extract fields:
        #pragma warning disable 0219 // Ignore unused variables
        Byte commandID = __buffer.ReadByte();
        ErrorType errorType = (ErrorType)(__buffer.ReadByte());
        byte[] payload = parseRest(__buffer);
        #pragma warning restore 0219

        return new MessageError(hubID, commandID, errorType, payload);
    }

    override
    public void visitWith<T>(LegoMessage_Visitor<T> visitor, T arg) {
        visitor.handle_MessageError(this, arg);
    }

}
 }
