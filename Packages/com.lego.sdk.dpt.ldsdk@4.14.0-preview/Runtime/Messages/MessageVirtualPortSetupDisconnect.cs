/*!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
  !!    THIS FILE IS AUTO-GENERATED - please do not edit!             !!
  !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!*/

using System;
using BinaryReader = System.IO.BinaryReader;
using BinaryWriter = System.IO.BinaryWriter;
using MemoryStream= System.IO.MemoryStream;

#pragma warning disable 0109 // "hides inherited member - new: nothing to hide"
namespace dk.lego.devicesdk.bluetooth.V3.messages {
public class MessageVirtualPortSetupDisconnect : MessageVirtualPortSetup {
    public readonly Byte portID;

    public MessageVirtualPortSetupDisconnect(UInt32 hubID, UInt32 portID)
        : base(hubID, 0x0)
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

    internal static new MessageVirtualPortSetupDisconnect parse(BinaryReader __buffer, Byte hubID) {
        // Extract fields:
        #pragma warning disable 0219 // Ignore unused variables
        Byte portID = __buffer.ReadByte();
        #pragma warning restore 0219

        return new MessageVirtualPortSetupDisconnect(hubID, portID);
    }

    override
    public void visitWith<T>(LegoMessage_Visitor<T> visitor, T arg) {
        visitor.handle_MessageVirtualPortSetupDisconnect(this, arg);
    }

}
 }
