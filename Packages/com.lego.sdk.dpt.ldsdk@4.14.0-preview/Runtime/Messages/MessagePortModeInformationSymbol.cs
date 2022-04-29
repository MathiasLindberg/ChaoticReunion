/*!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
  !!    THIS FILE IS AUTO-GENERATED - please do not edit!             !!
  !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!*/

using System;
using BinaryReader = System.IO.BinaryReader;
using BinaryWriter = System.IO.BinaryWriter;
using MemoryStream= System.IO.MemoryStream;

#pragma warning disable 0109 // "hides inherited member - new: nothing to hide"
namespace dk.lego.devicesdk.bluetooth.V3.messages {
public class MessagePortModeInformationSymbol : MessagePortModeInformation {
    public readonly String symbol;

    public MessagePortModeInformationSymbol(UInt32 hubID, UInt32 portID, UInt32 mode, String symbol)
        : base(hubID, portID, mode, PortModeInformationType.SYMBOL)
    {
        this.symbol = symbol;
    }

    override
    protected int totalLengthInBytes(int __totalLengthOfRemainder) {
        int __totalLength = __totalLengthOfRemainder;
        __totalLength += 0; // Fixed-length fields
        __totalLength += symbol.Length;
        return base.totalLengthInBytes(__totalLength);
    }

    override
    internal void unparse(BinaryWriter __buffer, int __totalLength) {
        base.unparse(__buffer, __totalLength);

        __buffer.Write(__asciiCharset.GetBytes(symbol));
    }

    internal static new MessagePortModeInformationSymbol parse(BinaryReader __buffer, Byte hubID, Byte portID, Byte mode) {
        // Extract fields:
        #pragma warning disable 0219 // Ignore unused variables
        String symbol = trimNuls(__asciiCharset.GetString(parseRest(__buffer)));
        #pragma warning restore 0219

        return new MessagePortModeInformationSymbol(hubID, portID, mode, symbol);
    }

    override
    public void visitWith<T>(MessagePortModeInformation_Visitor<T> visitor, T arg) {
        visitor.handle_MessagePortModeInformationSymbol(this, arg);
    }

}
 }
