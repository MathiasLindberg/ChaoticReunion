/*!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
  !!    THIS FILE IS AUTO-GENERATED - please do not edit!             !!
  !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!*/

using System;
using BinaryReader = System.IO.BinaryReader;
using BinaryWriter = System.IO.BinaryWriter;
using MemoryStream= System.IO.MemoryStream;

#pragma warning disable 0109 // "hides inherited member - new: nothing to hide"
namespace dk.lego.devicesdk.bluetooth.V3.messages {
public abstract class MessageVirtualPortSetup : LegoMessage {
    public readonly Byte subCommand;

    public MessageVirtualPortSetup(UInt32 hubID, UInt32 subCommand)
        : base(hubID, LegoMessageType.VIRTUAL_PORT_SETUP)
    {
        this.subCommand = (Byte)subCommand;
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

        __buffer.Write((Byte)(subCommand));
    }

    internal static new MessageVirtualPortSetup parse(BinaryReader __buffer, Byte hubID) {
        // Extract fields:
        #pragma warning disable 0219 // Ignore unused variables
        Byte subCommand = __buffer.ReadByte();
        #pragma warning restore 0219

        // Determine message type:
        switch (subCommand) {
        case 0x0:
            return MessageVirtualPortSetupDisconnect.parse(__buffer, hubID);
        case 0x1:
            return MessageVirtualPortSetupConnect.parse(__buffer, hubID);
        default: throw new ArgumentException("Invalid value of subCommand: "+subCommand);
        } // switch
    }

}
 }
