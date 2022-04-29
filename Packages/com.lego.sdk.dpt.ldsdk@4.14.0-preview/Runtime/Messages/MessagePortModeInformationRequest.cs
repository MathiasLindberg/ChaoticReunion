/*!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
  !!    THIS FILE IS AUTO-GENERATED - please do not edit!             !!
  !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!*/

using System;
using BinaryReader = System.IO.BinaryReader;
using BinaryWriter = System.IO.BinaryWriter;
using MemoryStream= System.IO.MemoryStream;

#pragma warning disable 0109 // "hides inherited member - new: nothing to hide"
namespace dk.lego.devicesdk.bluetooth.V3.messages {
public class MessagePortModeInformationRequest : MessagePortMetadataRelated {
    public readonly Byte mode;
    public readonly PortModeInformationType information;

    public MessagePortModeInformationRequest(UInt32 hubID, UInt32 portID, UInt32 mode, PortModeInformationType information)
        : base(hubID, LegoMessageType.PORT_MODE_INFORMATION_REQUEST, portID)
    {
        this.mode = (Byte)mode;
        this.information = information;
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

        __buffer.Write((Byte)(mode));
        __buffer.Write((Byte)((Byte)(information)));
    }

    internal static new MessagePortModeInformationRequest parse(BinaryReader __buffer, Byte hubID, Byte portID) {
        // Extract fields:
        #pragma warning disable 0219 // Ignore unused variables
        Byte mode = __buffer.ReadByte();
        PortModeInformationType information = (PortModeInformationType)(__buffer.ReadByte());
        #pragma warning restore 0219

        return new MessagePortModeInformationRequest(hubID, portID, mode, information);
    }

    override
    public void visitWith<T>(MessagePortMetadataRelated_Visitor<T> visitor, T arg) {
        visitor.handle_MessagePortModeInformationRequest(this, arg);
    }

}
 }
