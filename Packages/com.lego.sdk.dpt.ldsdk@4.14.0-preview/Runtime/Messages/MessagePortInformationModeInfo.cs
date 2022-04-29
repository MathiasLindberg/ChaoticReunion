/*!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
  !!    THIS FILE IS AUTO-GENERATED - please do not edit!             !!
  !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!*/

using System;
using BinaryReader = System.IO.BinaryReader;
using BinaryWriter = System.IO.BinaryWriter;
using MemoryStream= System.IO.MemoryStream;

#pragma warning disable 0109 // "hides inherited member - new: nothing to hide"
namespace dk.lego.devicesdk.bluetooth.V3.messages {
public class MessagePortInformationModeInfo : MessagePortInformation {
    public readonly Byte capabilities;
    public readonly Byte modeCount;
    public readonly UInt16 inputModes;
    public readonly UInt16 outputModes;

    public MessagePortInformationModeInfo(UInt32 hubID, UInt32 portID, UInt32 capabilities, UInt32 modeCount, UInt32 inputModes, UInt32 outputModes)
        : base(hubID, portID, PortInformationType.MODE_INFO)
    {
        this.capabilities = (Byte)capabilities;
        this.modeCount = (Byte)modeCount;
        this.inputModes = (UInt16)inputModes;
        this.outputModes = (UInt16)outputModes;
    }

    override
    protected int totalLengthInBytes(int __totalLengthOfRemainder) {
        int __totalLength = __totalLengthOfRemainder;
        __totalLength += 6; // Fixed-length fields
        return base.totalLengthInBytes(__totalLength);
    }

    override
    internal void unparse(BinaryWriter __buffer, int __totalLength) {
        base.unparse(__buffer, __totalLength);

        __buffer.Write((Byte)(capabilities));
        __buffer.Write((Byte)(modeCount));
        __buffer.Write((UInt16)(inputModes));
        __buffer.Write((UInt16)(outputModes));
    }

    internal static new MessagePortInformationModeInfo parse(BinaryReader __buffer, Byte hubID, Byte portID) {
        // Extract fields:
        #pragma warning disable 0219 // Ignore unused variables
        Byte capabilities = __buffer.ReadByte();
        Byte modeCount = __buffer.ReadByte();
        UInt16 inputModes = __buffer.ReadUInt16();
        UInt16 outputModes = __buffer.ReadUInt16();
        #pragma warning restore 0219

        return new MessagePortInformationModeInfo(hubID, portID, capabilities, modeCount, inputModes, outputModes);
    }

    override
    public void visitWith<T>(MessagePortInformation_Visitor<T> visitor, T arg) {
        visitor.handle_MessagePortInformationModeInfo(this, arg);
    }

}
 }
