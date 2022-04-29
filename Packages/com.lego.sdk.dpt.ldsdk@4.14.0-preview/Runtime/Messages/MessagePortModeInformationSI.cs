/*!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
  !!    THIS FILE IS AUTO-GENERATED - please do not edit!             !!
  !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!*/

using System;
using BinaryReader = System.IO.BinaryReader;
using BinaryWriter = System.IO.BinaryWriter;
using MemoryStream= System.IO.MemoryStream;

#pragma warning disable 0109 // "hides inherited member - new: nothing to hide"
namespace dk.lego.devicesdk.bluetooth.V3.messages {
public class MessagePortModeInformationSI : MessagePortModeInformation {
    public readonly float minSI;
    public readonly float maxSI;

    public MessagePortModeInformationSI(UInt32 hubID, UInt32 portID, UInt32 mode, float minSI, float maxSI)
        : base(hubID, portID, mode, PortModeInformationType.SI)
    {
        this.minSI = minSI;
        this.maxSI = maxSI;
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

        __buffer.Write((float)minSI);
        __buffer.Write((float)maxSI);
    }

    internal static new MessagePortModeInformationSI parse(BinaryReader __buffer, Byte hubID, Byte portID, Byte mode) {
        // Extract fields:
        #pragma warning disable 0219 // Ignore unused variables
        float minSI = __buffer.ReadSingle();
        float maxSI = __buffer.ReadSingle();
        #pragma warning restore 0219

        return new MessagePortModeInformationSI(hubID, portID, mode, minSI, maxSI);
    }

    override
    public void visitWith<T>(MessagePortModeInformation_Visitor<T> visitor, T arg) {
        visitor.handle_MessagePortModeInformationSI(this, arg);
    }

}
 }
