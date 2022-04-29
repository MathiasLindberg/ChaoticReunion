/*!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
  !!    THIS FILE IS AUTO-GENERATED - please do not edit!             !!
  !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!*/

using System;
using BinaryReader = System.IO.BinaryReader;
using BinaryWriter = System.IO.BinaryWriter;
using MemoryStream= System.IO.MemoryStream;

#pragma warning disable 0109 // "hides inherited member - new: nothing to hide"
namespace dk.lego.devicesdk.bluetooth.V3.messages {
public class MessagePortInformationAllowedCombinations : MessagePortInformation {
    public readonly byte[] allowedCombinations;

    public MessagePortInformationAllowedCombinations(UInt32 hubID, UInt32 portID, byte[] allowedCombinations)
        : base(hubID, portID, PortInformationType.ALLOWED_MODE_COMBINATIONS)
    {
        this.allowedCombinations = allowedCombinations;
    }

    override
    protected int totalLengthInBytes(int __totalLengthOfRemainder) {
        int __totalLength = __totalLengthOfRemainder;
        __totalLength += 0; // Fixed-length fields
        __totalLength += (allowedCombinations).Length;
        return base.totalLengthInBytes(__totalLength);
    }

    override
    internal void unparse(BinaryWriter __buffer, int __totalLength) {
        base.unparse(__buffer, __totalLength);

        __buffer.Write(allowedCombinations);
    }

    internal static new MessagePortInformationAllowedCombinations parse(BinaryReader __buffer, Byte hubID, Byte portID) {
        // Extract fields:
        #pragma warning disable 0219 // Ignore unused variables
        byte[] allowedCombinations = parseRest(__buffer);
        #pragma warning restore 0219

        return new MessagePortInformationAllowedCombinations(hubID, portID, allowedCombinations);
    }

    override
    public void visitWith<T>(MessagePortInformation_Visitor<T> visitor, T arg) {
        visitor.handle_MessagePortInformationAllowedCombinations(this, arg);
    }

}
 }
