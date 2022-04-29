/*!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
  !!    THIS FILE IS AUTO-GENERATED - please do not edit!             !!
  !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!*/

using System;
using BinaryReader = System.IO.BinaryReader;
using BinaryWriter = System.IO.BinaryWriter;
using MemoryStream= System.IO.MemoryStream;

#pragma warning disable 0109 // "hides inherited member - new: nothing to hide"
namespace dk.lego.devicesdk.bluetooth.V3.messages {
public class MessagePortModeInformationName : MessagePortModeInformation {
    public readonly String name;

    public MessagePortModeInformationName(UInt32 hubID, UInt32 portID, UInt32 mode, String name)
        : base(hubID, portID, mode, PortModeInformationType.NAME)
    {
        this.name = name;
    }

    override
    protected int totalLengthInBytes(int __totalLengthOfRemainder) {
        int __totalLength = __totalLengthOfRemainder;
        __totalLength += 0; // Fixed-length fields
        __totalLength += name.Length;
        return base.totalLengthInBytes(__totalLength);
    }

    override
    internal void unparse(BinaryWriter __buffer, int __totalLength) {
        base.unparse(__buffer, __totalLength);

        __buffer.Write(__asciiCharset.GetBytes(name));
    }

    internal static new MessagePortModeInformationName parse(BinaryReader __buffer, Byte hubID, Byte portID, Byte mode) {
        // Extract fields:
        #pragma warning disable 0219 // Ignore unused variables
        String name = trimNuls(__asciiCharset.GetString(parseRest(__buffer)));
        #pragma warning restore 0219

        return new MessagePortModeInformationName(hubID, portID, mode, name);
    }

    override
    public void visitWith<T>(MessagePortModeInformation_Visitor<T> visitor, T arg) {
        visitor.handle_MessagePortModeInformationName(this, arg);
    }

}
 }
