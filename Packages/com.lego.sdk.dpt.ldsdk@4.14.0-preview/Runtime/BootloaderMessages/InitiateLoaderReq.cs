/*!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
  !!    THIS FILE IS AUTO-GENERATED - please do not edit!             !!
  !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!*/

using System;
using BinaryReader = System.IO.BinaryReader;
using BinaryWriter = System.IO.BinaryWriter;
using MemoryStream= System.IO.MemoryStream;

#pragma warning disable 0109 // "hides inherited member - new: nothing to hide"
namespace dk.lego.devicesdk.bluetooth.V3bootloader.messages {
public class InitiateLoaderReq : DownstreamMessage {
    public readonly UInt32 byteCount;

    public InitiateLoaderReq(UInt32 byteCount)
        : base(BootloaderMessageType.INITIATE_LOADER)
    {
        this.byteCount = byteCount;
    }

    override
    protected int totalLengthInBytes(int __totalLengthOfRemainder) {
        int __totalLength = __totalLengthOfRemainder;
        __totalLength += 4; // Fixed-length fields
        return base.totalLengthInBytes(__totalLength);
    }

    override
    internal void unparse(BinaryWriter __buffer, int __totalLength) {
        base.unparse(__buffer, __totalLength);

        __buffer.Write((UInt32)(byteCount));
    }

    internal static new InitiateLoaderReq parse(BinaryReader __buffer) {
        // Extract fields:
        #pragma warning disable 0219 // Ignore unused variables
        UInt32 byteCount = __buffer.ReadUInt32();
        #pragma warning restore 0219

        return new InitiateLoaderReq(byteCount);
    }

    override
    public void visitWith<T>(DownstreamMessage_Visitor<T> visitor, T arg) {
        visitor.handle_InitiateLoaderReq(this, arg);
    }

}
 }
