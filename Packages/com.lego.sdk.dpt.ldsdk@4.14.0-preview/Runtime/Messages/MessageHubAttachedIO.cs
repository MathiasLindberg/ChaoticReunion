/*!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
  !!    THIS FILE IS AUTO-GENERATED - please do not edit!             !!
  !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!*/

using System;
using BinaryReader = System.IO.BinaryReader;
using BinaryWriter = System.IO.BinaryWriter;
using MemoryStream= System.IO.MemoryStream;

#pragma warning disable 0109 // "hides inherited member - new: nothing to hide"
namespace dk.lego.devicesdk.bluetooth.V3.messages {
public abstract class MessageHubAttachedIO : MessagePortConnectivityRelated {
    public readonly Byte eventType;

    public MessageHubAttachedIO(UInt32 hubID, UInt32 portID, UInt32 eventType)
        : base(hubID, LegoMessageType.ATTACHED_IO, portID)
    {
        this.eventType = (Byte)eventType;
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

        __buffer.Write((Byte)(eventType));
    }

    internal static new MessageHubAttachedIO parse(BinaryReader __buffer, Byte hubID, Byte portID) {
        // Extract fields:
        #pragma warning disable 0219 // Ignore unused variables
        Byte eventType = __buffer.ReadByte();
        #pragma warning restore 0219

        // Determine message type:
        switch (eventType) {
        case 0x0:
            return MessageHubAttachedIODetached.parse(__buffer, hubID, portID);
        case 0x1:
            return MessageHubAttachedIOAbstractAttached.parse(__buffer, hubID, portID, eventType);
        case 0x2:
            return MessageHubAttachedIOAbstractAttached.parse(__buffer, hubID, portID, eventType);
        default: throw new ArgumentException("Invalid value of eventType: "+eventType);
        } // switch
    }

}
 }
