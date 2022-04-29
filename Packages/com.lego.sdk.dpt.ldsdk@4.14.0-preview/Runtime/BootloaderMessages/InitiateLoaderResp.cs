/*!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
  !!    THIS FILE IS AUTO-GENERATED - please do not edit!             !!
  !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!*/

using System;
using BinaryReader = System.IO.BinaryReader;
using BinaryWriter = System.IO.BinaryWriter;
using MemoryStream= System.IO.MemoryStream;

#pragma warning disable 0109 // "hides inherited member - new: nothing to hide"
namespace dk.lego.devicesdk.bluetooth.V3bootloader.messages {
public class InitiateLoaderResp : UpstreamMessage {
    public readonly Byte errorCode;

    public InitiateLoaderResp(UInt32 errorCode)
        : base(BootloaderMessageType.INITIATE_LOADER)
    {
        this.errorCode = (Byte)errorCode;
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

        __buffer.Write((Byte)(errorCode));
    }

    internal static new InitiateLoaderResp parse(BinaryReader __buffer) {
        // Extract fields:
        #pragma warning disable 0219 // Ignore unused variables
        Byte errorCode = __buffer.ReadByte();
        #pragma warning restore 0219

        return new InitiateLoaderResp(errorCode);
    }

    override
    public void visitWith<T>(UpstreamMessage_Visitor<T> visitor, T arg) {
        visitor.handle_InitiateLoaderResp(this, arg);
    }

}
 }
