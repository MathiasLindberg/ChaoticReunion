/*!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
  !!    THIS FILE IS AUTO-GENERATED - please do not edit!             !!
  !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!*/

using System;
using BinaryReader = System.IO.BinaryReader;
using BinaryWriter = System.IO.BinaryWriter;
using MemoryStream= System.IO.MemoryStream;

#pragma warning disable 0109 // "hides inherited member - new: nothing to hide"
namespace dk.lego.devicesdk.bluetooth.V3bootloader.messages {
public class GetFlashStateResp : UpstreamMessage {
    public readonly Byte flashProtectionLevel;

    public GetFlashStateResp(UInt32 flashProtectionLevel)
        : base(BootloaderMessageType.GET_FLASH_STATE)
    {
        this.flashProtectionLevel = (Byte)flashProtectionLevel;
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

        __buffer.Write((Byte)(flashProtectionLevel));
    }

    internal static new GetFlashStateResp parse(BinaryReader __buffer) {
        // Extract fields:
        #pragma warning disable 0219 // Ignore unused variables
        Byte flashProtectionLevel = __buffer.ReadByte();
        #pragma warning restore 0219

        return new GetFlashStateResp(flashProtectionLevel);
    }

    override
    public void visitWith<T>(UpstreamMessage_Visitor<T> visitor, T arg) {
        visitor.handle_GetFlashStateResp(this, arg);
    }

}
 }
