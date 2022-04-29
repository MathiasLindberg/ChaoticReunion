/*!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
  !!    THIS FILE IS AUTO-GENERATED - please do not edit!             !!
  !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!*/

using System;
using BinaryReader = System.IO.BinaryReader;
using BinaryWriter = System.IO.BinaryWriter;
using MemoryStream= System.IO.MemoryStream;

#pragma warning disable 0109 // "hides inherited member - new: nothing to hide"
namespace dk.lego.devicesdk.bluetooth.V3bootloader.messages {
public class GetChecksumReq : DownstreamMessage {
    public GetChecksumReq()
        : base(BootloaderMessageType.GET_CHECKSUM)
    {
    }

    override
    protected int totalLengthInBytes(int __totalLengthOfRemainder) {
        int __totalLength = __totalLengthOfRemainder;
        __totalLength += 0; // Fixed-length fields
        return base.totalLengthInBytes(__totalLength);
    }

    override
    internal void unparse(BinaryWriter __buffer, int __totalLength) {
        base.unparse(__buffer, __totalLength);

    }

    internal static new GetChecksumReq parse(BinaryReader __buffer) {
        // Extract fields:
        #pragma warning disable 0219 // Ignore unused variables
        #pragma warning restore 0219

        return new GetChecksumReq();
    }

    override
    public void visitWith<T>(DownstreamMessage_Visitor<T> visitor, T arg) {
        visitor.handle_GetChecksumReq(this, arg);
    }

}
 }
