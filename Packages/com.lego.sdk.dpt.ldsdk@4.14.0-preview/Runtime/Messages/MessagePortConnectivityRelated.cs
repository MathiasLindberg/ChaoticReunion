/*!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
  !!    THIS FILE IS AUTO-GENERATED - please do not edit!             !!
  !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!*/

using System;
using BinaryReader = System.IO.BinaryReader;
using BinaryWriter = System.IO.BinaryWriter;
using MemoryStream= System.IO.MemoryStream;

#pragma warning disable 0109 // "hides inherited member - new: nothing to hide"
namespace dk.lego.devicesdk.bluetooth.V3.messages {
public abstract class MessagePortConnectivityRelated : MessagePortRelated {
    public MessagePortConnectivityRelated(UInt32 hubID, LegoMessageType msgType, UInt32 portID)
        : base(hubID, msgType, portID)
    {
    }

    override
    protected int totalLengthInBytes(int __totalLengthOfRemainder) {
        int __totalLength = __totalLengthOfRemainder;
        __totalLength += 0; // Fixed-length fields
        return base.totalLengthInBytes(__totalLength);
    }

    override
    internal void unparse(BinaryWriter __buffer, int __totalLength) {
        base.unparse(__buffer, __totalLength);

    }

    internal static new MessagePortConnectivityRelated parse(BinaryReader __buffer, Byte hubID, LegoMessageType msgType, Byte portID) {
        // Extract fields:
        #pragma warning disable 0219 // Ignore unused variables
        #pragma warning restore 0219

        // Determine message type:
        switch (msgType) {
        case LegoMessageType.ATTACHED_IO:
            return MessageHubAttachedIO.parse(__buffer, hubID, portID);
        default: throw new ArgumentException("Invalid value of msgType: "+msgType);
        } // switch
    }

    public abstract void visitWith<T>(MessagePortConnectivityRelated_Visitor<T> visitor, T arg);

    override
    public void visitWith<T>(LegoMessage_Visitor<T> visitor, T arg) {
        visitor.handle_MessagePortConnectivityRelated(this, arg);
    }

    override
    public void visitWith<T>(MessagePortRelated_Visitor<T> visitor, T arg) {
        visitor.handle_MessagePortConnectivityRelated(this, arg);
    }

}
 }
