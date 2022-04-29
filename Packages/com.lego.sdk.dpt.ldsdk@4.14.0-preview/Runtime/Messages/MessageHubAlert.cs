/*!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
  !!    THIS FILE IS AUTO-GENERATED - please do not edit!             !!
  !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!*/

using System;
using BinaryReader = System.IO.BinaryReader;
using BinaryWriter = System.IO.BinaryWriter;
using MemoryStream= System.IO.MemoryStream;

#pragma warning disable 0109 // "hides inherited member - new: nothing to hide"
namespace dk.lego.devicesdk.bluetooth.V3.messages {
public class MessageHubAlert : LegoMessage {
    public readonly HubAlert alert;
    public readonly HubAlertOperation operation;
    public readonly byte[] payload;

    public MessageHubAlert(UInt32 hubID, HubAlert alert, HubAlertOperation operation, byte[] payload)
        : base(hubID, LegoMessageType.ALERT)
    {
        this.alert = alert;
        this.operation = operation;
        this.payload = payload;
    }

    override
    protected int totalLengthInBytes(int __totalLengthOfRemainder) {
        int __totalLength = __totalLengthOfRemainder;
        __totalLength += 2; // Fixed-length fields
        __totalLength += (payload).Length;
        return base.totalLengthInBytes(__totalLength);
    }

    override
    internal void unparse(BinaryWriter __buffer, int __totalLength) {
        base.unparse(__buffer, __totalLength);

        __buffer.Write((Byte)((Byte)(alert)));
        __buffer.Write((Byte)((Byte)(operation)));
        __buffer.Write(payload);
    }

    internal static new MessageHubAlert parse(BinaryReader __buffer, Byte hubID) {
        // Extract fields:
        #pragma warning disable 0219 // Ignore unused variables
        HubAlert alert = (HubAlert)(__buffer.ReadByte());
        HubAlertOperation operation = (HubAlertOperation)(__buffer.ReadByte());
        byte[] payload = parseRest(__buffer);
        #pragma warning restore 0219

        return new MessageHubAlert(hubID, alert, operation, payload);
    }

    override
    public void visitWith<T>(LegoMessage_Visitor<T> visitor, T arg) {
        visitor.handle_MessageHubAlert(this, arg);
    }

}
 }
