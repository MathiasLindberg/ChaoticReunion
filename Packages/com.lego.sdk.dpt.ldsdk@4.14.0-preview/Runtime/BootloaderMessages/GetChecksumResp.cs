/*!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
  !!    THIS FILE IS AUTO-GENERATED - please do not edit!             !!
  !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!*/

using System;
using BinaryReader = System.IO.BinaryReader;
using BinaryWriter = System.IO.BinaryWriter;
using MemoryStream= System.IO.MemoryStream;

#pragma warning disable 0109 // "hides inherited member - new: nothing to hide"
namespace dk.lego.devicesdk.bluetooth.V3bootloader.messages {
public class GetChecksumResp : UpstreamMessage {
    public readonly Byte checksum;

    public GetChecksumResp(UInt32 checksum)
        : base(BootloaderMessageType.GET_CHECKSUM)
    {
        this.checksum = (Byte)checksum;
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

        __buffer.Write((Byte)(checksum));
    }

    internal static new GetChecksumResp parse(BinaryReader __buffer) {
        // Extract fields:
        #pragma warning disable 0219 // Ignore unused variables
        Byte checksum = __buffer.ReadByte();
        #pragma warning restore 0219

        return new GetChecksumResp(checksum);
    }

    override
    public void visitWith<T>(UpstreamMessage_Visitor<T> visitor, T arg) {
        visitor.handle_GetChecksumResp(this, arg);
    }

}
 }
