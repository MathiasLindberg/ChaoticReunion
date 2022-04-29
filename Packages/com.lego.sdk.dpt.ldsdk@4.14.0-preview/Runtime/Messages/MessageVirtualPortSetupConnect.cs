/*!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
  !!    THIS FILE IS AUTO-GENERATED - please do not edit!             !!
  !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!*/

using System;
using BinaryReader = System.IO.BinaryReader;
using BinaryWriter = System.IO.BinaryWriter;
using MemoryStream= System.IO.MemoryStream;

#pragma warning disable 0109 // "hides inherited member - new: nothing to hide"
namespace dk.lego.devicesdk.bluetooth.V3.messages {
public class MessageVirtualPortSetupConnect : MessageVirtualPortSetup {
    public readonly Byte portA;
    public readonly Byte portB;

    public MessageVirtualPortSetupConnect(UInt32 hubID, UInt32 portA, UInt32 portB)
        : base(hubID, 0x1)
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

    internal static new MessageVirtualPortSetupConnect parse(BinaryReader __buffer, Byte hubID) {
        // Extract fields:
        #pragma warning disable 0219 // Ignore unused variables
        Byte portA = __buffer.ReadByte();
        Byte portB = __buffer.ReadByte();
        #pragma warning restore 0219

        return new MessageVirtualPortSetupConnect(hubID, portA, portB);
    }

    override
    public void visitWith<T>(LegoMessage_Visitor<T> visitor, T arg) {
        visitor.handle_MessageVirtualPortSetupConnect(this, arg);
    }

}
 }
