/*!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
  !!    THIS FILE IS AUTO-GENERATED - please do not edit!             !!
  !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!*/

using System;
using BinaryReader = System.IO.BinaryReader;
using BinaryWriter = System.IO.BinaryWriter;
using MemoryStream= System.IO.MemoryStream;

#pragma warning disable 0109 // "hides inherited member - new: nothing to hide"
namespace dk.lego.devicesdk.bluetooth.V3.messages {
public abstract class MessagePortInformation : MessagePortMetadataRelated {
    public readonly PortInformationType informationType;

    public MessagePortInformation(UInt32 hubID, UInt32 portID, PortInformationType informationType)
        : base(hubID, LegoMessageType.PORT_INFORMATION, portID)
    {
        this.informationType = informationType;
    }

    override
    protected int totalLengthInBytes(int __totalLengthOfRemainder) {
        int __totalLength = __totalLengthOfRemainder;
        __totalLength += 1; // Fixed-length fields
        return base.totalLengthInBytes(__totalLength);
    }

    override
    internal void unparse(BinaryWriter __buffer, int __totalLength) {
        base.unparse(__buffer, __totalLength);

        __buffer.Write((Byte)((Byte)(informationType)));
    }

    internal static new MessagePortInformation parse(BinaryReader __buffer, Byte hubID, Byte portID) {
        // Extract fields:
        #pragma warning disable 0219 // Ignore unused variables
        PortInformationType informationType = (PortInformationType)(__buffer.ReadByte());
        #pragma warning restore 0219

        // Determine message type:
        switch (informationType) {
        case PortInformationType.MODE_INFO:
            return MessagePortInformationModeInfo.parse(__buffer, hubID, portID);
        case PortInformationType.ALLOWED_MODE_COMBINATIONS:
            return MessagePortInformationAllowedCombinations.parse(__buffer, hubID, portID);
        default: throw new ArgumentException("Invalid value of informationType: "+informationType);
        } // switch
    }

    public abstract void visitWith<T>(MessagePortInformation_Visitor<T> visitor, T arg);

    override
    public void visitWith<T>(MessagePortMetadataRelated_Visitor<T> visitor, T arg) {
        visitor.handle_MessagePortInformation(this, arg);
    }

}
 }
