/*!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
  !!    THIS FILE IS AUTO-GENERATED - please do not edit!             !!
  !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!*/

using System;
using BinaryReader = System.IO.BinaryReader;
using BinaryWriter = System.IO.BinaryWriter;
using MemoryStream= System.IO.MemoryStream;

#pragma warning disable 0109 // "hides inherited member - new: nothing to hide"
namespace dk.lego.devicesdk.bluetooth.V3.messages {
public class MessagePortInputFormatSetupSingle : MessagePortDynamicsRelated {
    public readonly Byte mode;
    public readonly UInt32 deltaInterval;
    public readonly bool notificationEnabled;

    public MessagePortInputFormatSetupSingle(UInt32 hubID, UInt32 portID, UInt32 mode, UInt32 deltaInterval, bool notificationEnabled)
        : base(hubID, LegoMessageType.PORT_INPUT_FORMAT_SETUP_SINGLE, portID)
    {
        this.mode = (Byte)mode;
        this.deltaInterval = deltaInterval;
        this.notificationEnabled = notificationEnabled;
    }

    override
    protected int totalLengthInBytes(int __totalLengthOfRemainder) {
        int __totalLength = __totalLengthOfRemainder;
        __totalLength += 6; // Fixed-length fields
        return base.totalLengthInBytes(__totalLength);
    }

    override
    internal void unparse(BinaryWriter __buffer, int __totalLength) {
        base.unparse(__buffer, __totalLength);

        __buffer.Write((Byte)(mode));
        __buffer.Write((UInt32)(deltaInterval));
        __buffer.Write((notificationEnabled) ? (byte)1 : (byte)0);
    }

    internal static new MessagePortInputFormatSetupSingle parse(BinaryReader __buffer, Byte hubID, Byte portID) {
        // Extract fields:
        #pragma warning disable 0219 // Ignore unused variables
        Byte mode = __buffer.ReadByte();
        UInt32 deltaInterval = __buffer.ReadUInt32();
        bool notificationEnabled = (__buffer.ReadByte() != 0);
        #pragma warning restore 0219

        return new MessagePortInputFormatSetupSingle(hubID, portID, mode, deltaInterval, notificationEnabled);
    }

    override
    public void visitWith<T>(MessagePortDynamicsRelated_Visitor<T> visitor, T arg) {
        visitor.handle_MessagePortInputFormatSetupSingle(this, arg);
    }

}
 }
