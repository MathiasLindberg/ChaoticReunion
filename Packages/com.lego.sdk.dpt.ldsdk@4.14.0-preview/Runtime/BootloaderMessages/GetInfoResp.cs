/*!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
  !!    THIS FILE IS AUTO-GENERATED - please do not edit!             !!
  !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!*/

using System;
using BinaryReader = System.IO.BinaryReader;
using BinaryWriter = System.IO.BinaryWriter;
using MemoryStream= System.IO.MemoryStream;

#pragma warning disable 0109 // "hides inherited member - new: nothing to hide"
namespace dk.lego.devicesdk.bluetooth.V3bootloader.messages {
public class GetInfoResp : UpstreamMessage {
    public readonly LEGODeviceUnitySDK.LEGORevision flashLoaderVersion;
    public readonly UInt32 flashStartAddress;
    public readonly UInt32 flashEndAddress;
    public readonly Byte systemTypeID;

    public GetInfoResp(LEGODeviceUnitySDK.LEGORevision flashLoaderVersion, UInt32 flashStartAddress, UInt32 flashEndAddress, UInt32 systemTypeID)
        : base(BootloaderMessageType.GET_INFO)
    {
        this.flashLoaderVersion = flashLoaderVersion;
        this.flashStartAddress = flashStartAddress;
        this.flashEndAddress = flashEndAddress;
        this.systemTypeID = (Byte)systemTypeID;
    }

    override
    protected int totalLengthInBytes(int __totalLengthOfRemainder) {
        int __totalLength = __totalLengthOfRemainder;
        __totalLength += 9; // Fixed-length fields
        __totalLength += 4;
        return base.totalLengthInBytes(__totalLength);
    }

    override
    internal void unparse(BinaryWriter __buffer, int __totalLength) {
        base.unparse(__buffer, __totalLength);

        __buffer.Write(flashLoaderVersion.unparse());
        __buffer.Write((UInt32)(flashStartAddress));
        __buffer.Write((UInt32)(flashEndAddress));
        __buffer.Write((Byte)(systemTypeID));
    }

    internal static new GetInfoResp parse(BinaryReader __buffer) {
        // Extract fields:
        #pragma warning disable 0219 // Ignore unused variables
        LEGODeviceUnitySDK.LEGORevision flashLoaderVersion = LEGODeviceUnitySDK.LEGORevision.parse(parseByteString(__buffer, 4));
        UInt32 flashStartAddress = __buffer.ReadUInt32();
        UInt32 flashEndAddress = __buffer.ReadUInt32();
        Byte systemTypeID = __buffer.ReadByte();
        #pragma warning restore 0219

        return new GetInfoResp(flashLoaderVersion, flashStartAddress, flashEndAddress, systemTypeID);
    }

    override
    public void visitWith<T>(UpstreamMessage_Visitor<T> visitor, T arg) {
        visitor.handle_GetInfoResp(this, arg);
    }

}
 }
