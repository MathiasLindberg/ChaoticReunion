/*!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
  !!    THIS FILE IS AUTO-GENERATED - please do not edit!             !!
  !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!*/

using System;
using BinaryReader = System.IO.BinaryReader;
using BinaryWriter = System.IO.BinaryWriter;
using MemoryStream= System.IO.MemoryStream;

#pragma warning disable 0109 // "hides inherited member - new: nothing to hide"
namespace dk.lego.devicesdk.bluetooth.V3.messages {
public class MessagePortModeInformationValueFormat : MessagePortModeInformation {
    public readonly Byte valueFormatCount;
    public readonly ModeInformationValueFormatType valueFormatType;
    public readonly Byte valueFormatFigures;
    public readonly Byte valueFormatDecimals;

    public MessagePortModeInformationValueFormat(UInt32 hubID, UInt32 portID, UInt32 mode, UInt32 valueFormatCount, ModeInformationValueFormatType valueFormatType, UInt32 valueFormatFigures, UInt32 valueFormatDecimals)
        : base(hubID, portID, mode, PortModeInformationType.VALUE_FORMAT)
    {
        this.valueFormatCount = (Byte)valueFormatCount;
        this.valueFormatType = valueFormatType;
        this.valueFormatFigures = (Byte)valueFormatFigures;
        this.valueFormatDecimals = (Byte)valueFormatDecimals;
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

        __buffer.Write((Byte)(valueFormatCount));
        __buffer.Write((Byte)((Byte)(valueFormatType)));
        __buffer.Write((Byte)(valueFormatFigures));
        __buffer.Write((Byte)(valueFormatDecimals));
    }

    internal static new MessagePortModeInformationValueFormat parse(BinaryReader __buffer, Byte hubID, Byte portID, Byte mode) {
        // Extract fields:
        #pragma warning disable 0219 // Ignore unused variables
        Byte valueFormatCount = __buffer.ReadByte();
        ModeInformationValueFormatType valueFormatType = (ModeInformationValueFormatType)(__buffer.ReadByte());
        Byte valueFormatFigures = __buffer.ReadByte();
        Byte valueFormatDecimals = __buffer.ReadByte();
        #pragma warning restore 0219

        return new MessagePortModeInformationValueFormat(hubID, portID, mode, valueFormatCount, valueFormatType, valueFormatFigures, valueFormatDecimals);
    }

    override
    public void visitWith<T>(MessagePortModeInformation_Visitor<T> visitor, T arg) {
        visitor.handle_MessagePortModeInformationValueFormat(this, arg);
    }

}
 }
