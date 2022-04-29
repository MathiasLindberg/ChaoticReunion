/*!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
  !!    THIS FILE IS AUTO-GENERATED - please do not edit!             !!
  !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!*/

using System;
using BinaryReader = System.IO.BinaryReader;
using BinaryWriter = System.IO.BinaryWriter;
using MemoryStream= System.IO.MemoryStream;

#pragma warning disable 0109 // "hides inherited member - new: nothing to hide"
namespace dk.lego.devicesdk.bluetooth.V3.messages {
public abstract class MessageHubAttachedIOAbstractAttached : MessageHubAttachedIO {
    public readonly dk.lego.devicesdk.device.IOType ioType;

    public MessageHubAttachedIOAbstractAttached(UInt32 hubID, UInt32 portID, UInt32 eventType, dk.lego.devicesdk.device.IOType ioType)
        : base(hubID, portID, eventType)
    {
        this.ioType = ioType;
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

        __buffer.Write((UInt16)((UInt16)(ioType)));
    }

    internal static new MessageHubAttachedIOAbstractAttached parse(BinaryReader __buffer, Byte hubID, Byte portID, Byte eventType) {
        // Extract fields:
        #pragma warning disable 0219 // Ignore unused variables
        dk.lego.devicesdk.device.IOType ioType = (dk.lego.devicesdk.device.IOType)(__buffer.ReadUInt16());
        #pragma warning restore 0219

        // Determine message type:
        switch (eventType) {
        case 0x1:
            return MessageHubAttachedIOAttached.parse(__buffer, hubID, portID, ioType);
        case 0x2:
            return MessageHubAttachedIOVirtualAttached.parse(__buffer, hubID, portID, ioType);
        default: throw new ArgumentException("Invalid value of eventType: "+eventType);
        } // switch
    }

}
 }
