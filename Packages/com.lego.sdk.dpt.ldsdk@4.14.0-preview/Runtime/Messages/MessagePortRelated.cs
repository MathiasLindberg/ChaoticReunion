/*!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
  !!    THIS FILE IS AUTO-GENERATED - please do not edit!             !!
  !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!*/

using System;
using BinaryReader = System.IO.BinaryReader;
using BinaryWriter = System.IO.BinaryWriter;
using MemoryStream= System.IO.MemoryStream;

#pragma warning disable 0109 // "hides inherited member - new: nothing to hide"
namespace dk.lego.devicesdk.bluetooth.V3.messages {
public abstract class MessagePortRelated : LegoMessage {
    public readonly Byte portID;

    public MessagePortRelated(UInt32 hubID, LegoMessageType msgType, UInt32 portID)
        : base(hubID, msgType)
    {
        this.portID = (Byte)portID;
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

        __buffer.Write((Byte)(portID));
    }

    internal static new MessagePortRelated parse(BinaryReader __buffer, Byte hubID, LegoMessageType msgType) {
        // Extract fields:
        #pragma warning disable 0219 // Ignore unused variables
        Byte portID = __buffer.ReadByte();
        #pragma warning restore 0219

        // Determine message type:
        switch (msgType) {
        case LegoMessageType.ATTACHED_IO:
            return MessagePortConnectivityRelated.parse(__buffer, hubID, msgType, portID);
        case LegoMessageType.PORT_INFORMATION_REQUEST:
            return MessagePortMetadataRelated.parse(__buffer, hubID, msgType, portID);
        case LegoMessageType.PORT_MODE_INFORMATION_REQUEST:
            return MessagePortMetadataRelated.parse(__buffer, hubID, msgType, portID);
        case LegoMessageType.PORT_INPUT_FORMAT_SETUP_SINGLE:
            return MessagePortDynamicsRelated.parse(__buffer, hubID, msgType, portID);
        case LegoMessageType.PORT_INPUT_FORMAT_SETUP_COMBINED:
            return MessagePortDynamicsRelated.parse(__buffer, hubID, msgType, portID);
        case LegoMessageType.PORT_INFORMATION:
            return MessagePortMetadataRelated.parse(__buffer, hubID, msgType, portID);
        case LegoMessageType.PORT_MODE_INFORMATION:
            return MessagePortMetadataRelated.parse(__buffer, hubID, msgType, portID);
        case LegoMessageType.PORT_VALUE_COMBINED:
            return MessagePortDynamicsRelated.parse(__buffer, hubID, msgType, portID);
        case LegoMessageType.PORT_INPUT_FORMAT_SINGLE:
            return MessagePortDynamicsRelated.parse(__buffer, hubID, msgType, portID);
        case LegoMessageType.PORT_INPUT_FORMAT_COMBINED:
            return MessagePortDynamicsRelated.parse(__buffer, hubID, msgType, portID);
        case LegoMessageType.PORT_OUTPUT_COMMAND:
            return MessagePortDynamicsRelated.parse(__buffer, hubID, msgType, portID);
        default: throw new ArgumentException("Invalid value of msgType: "+msgType);
        } // switch
    }

    public abstract void visitWith<T>(MessagePortRelated_Visitor<T> visitor, T arg);

}
 }
