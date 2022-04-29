/*!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
  !!    THIS FILE IS AUTO-GENERATED - please do not edit!             !!
  !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!*/

using System;
using BinaryReader = System.IO.BinaryReader;
using BinaryWriter = System.IO.BinaryWriter;
using MemoryStream= System.IO.MemoryStream;

#pragma warning disable 0109 // "hides inherited member - new: nothing to hide"
namespace dk.lego.devicesdk.bluetooth.V3.messages {
public class MessageHubAttachedIOVirtualAttached : MessageHubAttachedIOAbstractAttached {
    public readonly Byte portA;
    public readonly Byte portB;

    public MessageHubAttachedIOVirtualAttached(UInt32 hubID, UInt32 portID, dk.lego.devicesdk.device.IOType ioType, UInt32 portA, UInt32 portB)
        : base(hubID, portID, 0x2, ioType)
    {
        this.portA = (Byte)portA;
        this.portB = (Byte)portB;
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

        __buffer.Write((Byte)(portA));
        __buffer.Write((Byte)(portB));
    }

    internal static new MessageHubAttachedIOVirtualAttached parse(BinaryReader __buffer, Byte hubID, Byte portID, dk.lego.devicesdk.device.IOType ioType) {
        // Extract fields:
        #pragma warning disable 0219 // Ignore unused variables
        Byte portA = __buffer.ReadByte();
        Byte portB = __buffer.ReadByte();
        #pragma warning restore 0219

        return new MessageHubAttachedIOVirtualAttached(hubID, portID, ioType, portA, portB);
    }

    override
    public void visitWith<T>(MessagePortConnectivityRelated_Visitor<T> visitor, T arg) {
        visitor.handle_MessageHubAttachedIOVirtualAttached(this, arg);
    }

}
 }
