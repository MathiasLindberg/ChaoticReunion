/*!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
  !!    THIS FILE IS AUTO-GENERATED - please do not edit!             !!
  !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!*/

using System;
using BinaryReader = System.IO.BinaryReader;
using BinaryWriter = System.IO.BinaryWriter;
using MemoryStream= System.IO.MemoryStream;

#pragma warning disable 0109 // "hides inherited member - new: nothing to hide"
namespace dk.lego.devicesdk.bluetooth.V3.messages {
public class MessagePortValueCombined : MessagePortDynamicsRelated {
    public readonly UInt16 datasetBitmask;
    public readonly byte[] rawInputValues;

    public MessagePortValueCombined(UInt32 hubID, UInt32 portID, UInt32 datasetBitmask, byte[] rawInputValues)
        : base(hubID, LegoMessageType.PORT_VALUE_COMBINED, portID)
    {
        this.datasetBitmask = (UInt16)datasetBitmask;
        this.rawInputValues = rawInputValues;
    }

    override
    protected int totalLengthInBytes(int __totalLengthOfRemainder) {
        int __totalLength = __totalLengthOfRemainder;
        __totalLength += 2; // Fixed-length fields
        __totalLength += (rawInputValues).Length;
        return base.totalLengthInBytes(__totalLength);
    }

    override
    internal void unparse(BinaryWriter __buffer, int __totalLength) {
        base.unparse(__buffer, __totalLength);

        __buffer.Write((UInt16)(datasetBitmask));
        __buffer.Write(rawInputValues);
    }

    internal static new MessagePortValueCombined parse(BinaryReader __buffer, Byte hubID, Byte portID) {
        // Extract fields:
        #pragma warning disable 0219 // Ignore unused variables
        UInt16 datasetBitmask = __buffer.ReadUInt16();
        byte[] rawInputValues = parseRest(__buffer);
        #pragma warning restore 0219

        return new MessagePortValueCombined(hubID, portID, datasetBitmask, rawInputValues);
    }

    override
    public void visitWith<T>(MessagePortDynamicsRelated_Visitor<T> visitor, T arg) {
        visitor.handle_MessagePortValueCombined(this, arg);
    }

}
 }
