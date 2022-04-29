/*!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
  !!    THIS FILE IS AUTO-GENERATED - please do not edit!             !!
  !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!*/

using System;
using BinaryReader = System.IO.BinaryReader;
using BinaryWriter = System.IO.BinaryWriter;
using MemoryStream= System.IO.MemoryStream;

#pragma warning disable 0109 // "hides inherited member - new: nothing to hide"
namespace dk.lego.devicesdk.bluetooth.V3bootloader.messages {
public class ProgramFlashDone : UpstreamMessage {
    public readonly Byte checkSum;
    public readonly UInt32 byteCount;

    public ProgramFlashDone(UInt32 checkSum, UInt32 byteCount)
        : base(BootloaderMessageType.PROGRAM_FLASH)
    {
        this.checkSum = (Byte)checkSum;
        this.byteCount = byteCount;
    }

    override
    protected int totalLengthInBytes(int __totalLengthOfRemainder) {
        int __totalLength = __totalLengthOfRemainder;
        __totalLength += 5; // Fixed-length fields
        return base.totalLengthInBytes(__totalLength);
    }

    override
    internal void unparse(BinaryWriter __buffer, int __totalLength) {
        base.unparse(__buffer, __totalLength);

        __buffer.Write((Byte)(checkSum));
        __buffer.Write((UInt32)(byteCount));
    }

    internal static new ProgramFlashDone parse(BinaryReader __buffer) {
        // Extract fields:
        #pragma warning disable 0219 // Ignore unused variables
        Byte checkSum = __buffer.ReadByte();
        UInt32 byteCount = __buffer.ReadUInt32();
        #pragma warning restore 0219

        return new ProgramFlashDone(checkSum, byteCount);
    }

    override
    public void visitWith<T>(UpstreamMessage_Visitor<T> visitor, T arg) {
        visitor.handle_ProgramFlashDone(this, arg);
    }

}
 }
