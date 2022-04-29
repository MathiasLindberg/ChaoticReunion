/*!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
  !!    THIS FILE IS AUTO-GENERATED - please do not edit!             !!
  !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!*/

using System;
using BinaryReader = System.IO.BinaryReader;
using BinaryWriter = System.IO.BinaryWriter;
using MemoryStream= System.IO.MemoryStream;

#pragma warning disable 0109 // "hides inherited member - new: nothing to hide"
namespace dk.lego.devicesdk.bluetooth.V3.messages {
public class MessagePortModeInformationRaw : MessagePortModeInformation {
    public readonly float minRaw;
    public readonly float maxRaw;

    public MessagePortModeInformationRaw(UInt32 hubID, UInt32 portID, UInt32 mode, float minRaw, float maxRaw)
        : base(hubID, portID, mode, PortModeInformationType.RAW)
    {
        this.minRaw = minRaw;
        this.maxRaw = maxRaw;
    }

    override
    protected int totalLengthInBytes(int __totalLengthOfRemainder) {
        int __totalLength = __totalLengthOfRemainder;
        __totalLength += 8; // Fixed-length fields
        return base.totalLengthInBytes(__totalLength);
    }

    override
    internal void unparse(BinaryWriter __buffer, int __totalLength) {
        base.unparse(__buffer, __totalLength);

        __buffer.Write((float)minRaw);
        __buffer.Write((float)maxRaw);
    }

    internal static new MessagePortModeInformationRaw parse(BinaryReader __buffer, Byte hubID, Byte portID, Byte mode) {
        // Extract fields:
        #pragma warning disable 0219 // Ignore unused variables
        float minRaw = __buffer.ReadSingle();
        float maxRaw = __buffer.ReadSingle();
        #pragma warning restore 0219

        return new MessagePortModeInformationRaw(hubID, portID, mode, minRaw, maxRaw);
    }

    override
    public void visitWith<T>(MessagePortModeInformation_Visitor<T> visitor, T arg) {
        visitor.handle_MessagePortModeInformationRaw(this, arg);
    }

}
 }
