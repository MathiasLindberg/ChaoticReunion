/*!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
  !!    THIS FILE IS AUTO-GENERATED - please do not edit!             !!
  !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!*/

using System;
using BinaryReader = System.IO.BinaryReader;
using BinaryWriter = System.IO.BinaryWriter;
using MemoryStream= System.IO.MemoryStream;

#pragma warning disable 0109 // "hides inherited member - new: nothing to hide"
namespace dk.lego.devicesdk.bluetooth.V3.messages {
public class MessagePortModeInformationPct : MessagePortModeInformation {
    public readonly float minPct;
    public readonly float maxPct;

    public MessagePortModeInformationPct(UInt32 hubID, UInt32 portID, UInt32 mode, float minPct, float maxPct)
        : base(hubID, portID, mode, PortModeInformationType.PCT)
    {
        this.minPct = minPct;
        this.maxPct = maxPct;
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

        __buffer.Write((float)minPct);
        __buffer.Write((float)maxPct);
    }

    internal static new MessagePortModeInformationPct parse(BinaryReader __buffer, Byte hubID, Byte portID, Byte mode) {
        // Extract fields:
        #pragma warning disable 0219 // Ignore unused variables
        float minPct = __buffer.ReadSingle();
        float maxPct = __buffer.ReadSingle();
        #pragma warning restore 0219

        return new MessagePortModeInformationPct(hubID, portID, mode, minPct, maxPct);
    }

    override
    public void visitWith<T>(MessagePortModeInformation_Visitor<T> visitor, T arg) {
        visitor.handle_MessagePortModeInformationPct(this, arg);
    }

}
 }
