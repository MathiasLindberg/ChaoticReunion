/*!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
  !!    THIS FILE IS AUTO-GENERATED - please do not edit!             !!
  !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!*/

using System;
using BinaryReader = System.IO.BinaryReader;
using BinaryWriter = System.IO.BinaryWriter;
using MemoryStream= System.IO.MemoryStream;

#pragma warning disable 0109 // "hides inherited member - new: nothing to hide"
namespace dk.lego.devicesdk.bluetooth.V3.messages {
public class MessagePortOutputCommand : MessagePortDynamicsRelated {
    public readonly Byte startupAndCompletion;
    public readonly PortOutputCommandSubCommandTypeEnum subCommand;
    public readonly byte[] payload;

    public MessagePortOutputCommand(UInt32 hubID, UInt32 portID, UInt32 startupAndCompletion, PortOutputCommandSubCommandTypeEnum subCommand, byte[] payload)
        : base(hubID, LegoMessageType.PORT_OUTPUT_COMMAND, portID)
    {
        this.startupAndCompletion = (Byte)startupAndCompletion;
        this.subCommand = subCommand;
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

        __buffer.Write((Byte)(startupAndCompletion));
        __buffer.Write((Byte)((Byte)(subCommand)));
        __buffer.Write(payload);
    }

    internal static new MessagePortOutputCommand parse(BinaryReader __buffer, Byte hubID, Byte portID) {
        // Extract fields:
        #pragma warning disable 0219 // Ignore unused variables
        Byte startupAndCompletion = __buffer.ReadByte();
        PortOutputCommandSubCommandTypeEnum subCommand = (PortOutputCommandSubCommandTypeEnum)(__buffer.ReadByte());
        byte[] payload = parseRest(__buffer);
        #pragma warning restore 0219

        return new MessagePortOutputCommand(hubID, portID, startupAndCompletion, subCommand, payload);
    }

    override
    public void visitWith<T>(MessagePortDynamicsRelated_Visitor<T> visitor, T arg) {
        visitor.handle_MessagePortOutputCommand(this, arg);
    }

}
 }
