/*!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
  !!    THIS FILE IS AUTO-GENERATED - please do not edit!             !!
  !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!*/

using System;
using BinaryReader = System.IO.BinaryReader;
using BinaryWriter = System.IO.BinaryWriter;
using MemoryStream= System.IO.MemoryStream;

#pragma warning disable 0109 // "hides inherited member - new: nothing to hide"
namespace dk.lego.devicesdk.bluetooth.V3bootloader.messages {
public class ProgramFlashData : DownstreamMessage {
    public readonly UInt32 address;
    public readonly byte[] payload;

    public ProgramFlashData(UInt32 address, byte[] payload)
        : base(BootloaderMessageType.PROGRAM_FLASH)
    {
        this.address = address;
        this.payload = payload;
    }

    override
    protected int totalLengthInBytes(int __totalLengthOfRemainder) {
        int __totalLength = __totalLengthOfRemainder;
        __totalLength += 5; // Fixed-length fields
        __totalLength += (payload).Length;
        return base.totalLengthInBytes(__totalLength);
    }

    override
    internal void unparse(BinaryWriter __buffer, int __totalLength) {
        base.unparse(__buffer, __totalLength);

        __buffer.Write((Byte)(__totalLength - (int)__buffer.BaseStream.Position));
        __buffer.Write((UInt32)(address));
        __buffer.Write(payload);
    }

    internal static new ProgramFlashData parse(BinaryReader __buffer) {
        // Extract fields:
        #pragma warning disable 0219 // Ignore unused variables
        Byte length = __buffer.ReadByte();
        UInt32 address = __buffer.ReadUInt32();
        byte[] payload = parseRest(__buffer);
        #pragma warning restore 0219

        return new ProgramFlashData(address, payload);
    }

    override
    public void visitWith<T>(DownstreamMessage_Visitor<T> visitor, T arg) {
        visitor.handle_ProgramFlashData(this, arg);
    }

}
 }
