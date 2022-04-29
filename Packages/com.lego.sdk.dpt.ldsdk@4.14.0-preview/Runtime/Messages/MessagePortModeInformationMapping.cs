/*!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
  !!    THIS FILE IS AUTO-GENERATED - please do not edit!             !!
  !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!*/

using System;
using BinaryReader = System.IO.BinaryReader;
using BinaryWriter = System.IO.BinaryWriter;
using MemoryStream= System.IO.MemoryStream;

#pragma warning disable 0109 // "hides inherited member - new: nothing to hide"
namespace dk.lego.devicesdk.bluetooth.V3.messages {
public class MessagePortModeInformationMapping : MessagePortModeInformation {
    public readonly Byte mappingOutput;
    public readonly Byte mappingInput;

    public MessagePortModeInformationMapping(UInt32 hubID, UInt32 portID, UInt32 mode, UInt32 mappingOutput, UInt32 mappingInput)
        : base(hubID, portID, mode, PortModeInformationType.MAPPING)
    {
        this.mappingOutput = (Byte)mappingOutput;
        this.mappingInput = (Byte)mappingInput;
    }

    override
    protected int totalLengthInBytes(int __totalLengthOfRemainder) {
        int __totalLength = __totalLengthOfRemainder;
        __totalLength += 2; // Fixed-length fields
        return base.totalLengthInBytes(__totalLength);
    }

    override
    internal void unparse(BinaryWriter __buffer, int __totalLength) {
        base.unparse(__buffer, __totalLength);

        __buffer.Write((Byte)(mappingOutput));
        __buffer.Write((Byte)(mappingInput));
    }

    internal static new MessagePortModeInformationMapping parse(BinaryReader __buffer, Byte hubID, Byte portID, Byte mode) {
        // Extract fields:
        #pragma warning disable 0219 // Ignore unused variables
        Byte mappingOutput = __buffer.ReadByte();
        Byte mappingInput = __buffer.ReadByte();
        #pragma warning restore 0219

        return new MessagePortModeInformationMapping(hubID, portID, mode, mappingOutput, mappingInput);
    }

    override
    public void visitWith<T>(MessagePortModeInformation_Visitor<T> visitor, T arg) {
        visitor.handle_MessagePortModeInformationMapping(this, arg);
    }

}
 }
