/*!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
  !!    THIS FILE IS AUTO-GENERATED - please do not edit!             !!
  !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!*/

using System;
using BinaryReader = System.IO.BinaryReader;
using BinaryWriter = System.IO.BinaryWriter;
using MemoryStream= System.IO.MemoryStream;

#pragma warning disable 0109 // "hides inherited member - new: nothing to hide"
namespace dk.lego.devicesdk.bluetooth.V3.messages {
public class MessagePortInputFormatCombined : MessagePortDynamicsRelated {
    public readonly Byte combinedControlByte;
    public readonly UInt16 dataSetPointer;

    public MessagePortInputFormatCombined(UInt32 hubID, UInt32 portID, UInt32 combinedControlByte, UInt32 dataSetPointer)
        : base(hubID, LegoMessageType.PORT_INPUT_FORMAT_COMBINED, portID)
    {
        this.combinedControlByte = (Byte)combinedControlByte;
        this.dataSetPointer = (UInt16)dataSetPointer;
    }

    override
    protected int totalLengthInBytes(int __totalLengthOfRemainder) {
        int __totalLength = __totalLengthOfRemainder;
        __totalLength += 3; // Fixed-length fields
        return base.totalLengthInBytes(__totalLength);
    }

    override
    internal void unparse(BinaryWriter __buffer, int __totalLength) {
        base.unparse(__buffer, __totalLength);

        __buffer.Write((Byte)(combinedControlByte));
        __buffer.Write((UInt16)(dataSetPointer));
    }

    internal static new MessagePortInputFormatCombined parse(BinaryReader __buffer, Byte hubID, Byte portID) {
        // Extract fields:
        #pragma warning disable 0219 // Ignore unused variables
        Byte combinedControlByte = __buffer.ReadByte();
        UInt16 dataSetPointer = __buffer.ReadUInt16();
        #pragma warning restore 0219

        return new MessagePortInputFormatCombined(hubID, portID, combinedControlByte, dataSetPointer);
    }

    override
    public void visitWith<T>(MessagePortDynamicsRelated_Visitor<T> visitor, T arg) {
        visitor.handle_MessagePortInputFormatCombined(this, arg);
    }

}
 }
