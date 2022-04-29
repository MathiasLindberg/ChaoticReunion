/*!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
  !!    THIS FILE IS AUTO-GENERATED - please do not edit!             !!
  !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!*/

using System;
using BinaryReader = System.IO.BinaryReader;
using BinaryWriter = System.IO.BinaryWriter;
using MemoryStream= System.IO.MemoryStream;

#pragma warning disable 0109 // "hides inherited member - new: nothing to hide"
namespace dk.lego.devicesdk.bluetooth.V3.messages {
public class MessageHubAttachedIOAttached : MessageHubAttachedIOAbstractAttached {
    public readonly LEGODeviceUnitySDK.LEGORevision hardwareRevision;
    public readonly LEGODeviceUnitySDK.LEGORevision firmwareRevision;

    public MessageHubAttachedIOAttached(UInt32 hubID, UInt32 portID, dk.lego.devicesdk.device.IOType ioType, LEGODeviceUnitySDK.LEGORevision hardwareRevision, LEGODeviceUnitySDK.LEGORevision firmwareRevision)
        : base(hubID, portID, 0x1, ioType)
    {
        this.hardwareRevision = hardwareRevision;
        this.firmwareRevision = firmwareRevision;
    }

    override
    protected int totalLengthInBytes(int __totalLengthOfRemainder) {
        int __totalLength = __totalLengthOfRemainder;
        __totalLength += 0; // Fixed-length fields
        __totalLength += 4;
        __totalLength += 4;
        return base.totalLengthInBytes(__totalLength);
    }

    override
    internal void unparse(BinaryWriter __buffer, int __totalLength) {
        base.unparse(__buffer, __totalLength);

        __buffer.Write(hardwareRevision.unparse());
        __buffer.Write(firmwareRevision.unparse());
    }

    internal static new MessageHubAttachedIOAttached parse(BinaryReader __buffer, Byte hubID, Byte portID, dk.lego.devicesdk.device.IOType ioType) {
        // Extract fields:
        #pragma warning disable 0219 // Ignore unused variables
        LEGODeviceUnitySDK.LEGORevision hardwareRevision = LEGODeviceUnitySDK.LEGORevision.parse(parseByteString(__buffer, 4));
        LEGODeviceUnitySDK.LEGORevision firmwareRevision = LEGODeviceUnitySDK.LEGORevision.parse(parseByteString(__buffer, 4));
        #pragma warning restore 0219

        return new MessageHubAttachedIOAttached(hubID, portID, ioType, hardwareRevision, firmwareRevision);
    }

    override
    public void visitWith<T>(MessagePortConnectivityRelated_Visitor<T> visitor, T arg) {
        visitor.handle_MessageHubAttachedIOAttached(this, arg);
    }

}
 }
