/*!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
  !!    THIS FILE IS AUTO-GENERATED - please do not edit!             !!
  !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!*/

using System;
using BinaryReader = System.IO.BinaryReader;
using BinaryWriter = System.IO.BinaryWriter;
using MemoryStream= System.IO.MemoryStream;

#pragma warning disable 0109 // "hides inherited member - new: nothing to hide"
namespace dk.lego.devicesdk.bluetooth.V3.messages {
public class MessagePortInformationRequest : MessagePortMetadataRelated {
    public readonly PortInformationType information;

    public MessagePortInformationRequest(UInt32 hubID, UInt32 portID, PortInformationType information)
        : base(hubID, LegoMessageType.PORT_INFORMATION_REQUEST, portID)
    {
        this.information = information;
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

        __buffer.Write((Byte)((Byte)(information)));
    }

    internal static new MessagePortInformationRequest parse(BinaryReader __buffer, Byte hubID, Byte portID) {
        // Extract fields:
        #pragma warning disable 0219 // Ignore unused variables
        PortInformationType information = (PortInformationType)(__buffer.ReadByte());
        #pragma warning restore 0219

        return new MessagePortInformationRequest(hubID, portID, information);
    }

    override
    public void visitWith<T>(MessagePortMetadataRelated_Visitor<T> visitor, T arg) {
        visitor.handle_MessagePortInformationRequest(this, arg);
    }

}
 }
