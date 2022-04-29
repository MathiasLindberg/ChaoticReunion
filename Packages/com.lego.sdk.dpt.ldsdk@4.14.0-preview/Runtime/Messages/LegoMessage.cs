/*!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
  !!    THIS FILE IS AUTO-GENERATED - please do not edit!             !!
  !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!*/

using System;
using BinaryReader = System.IO.BinaryReader;
using BinaryWriter = System.IO.BinaryWriter;
using MemoryStream= System.IO.MemoryStream;

#pragma warning disable 0109 // "hides inherited member - new: nothing to hide"
namespace dk.lego.devicesdk.bluetooth.V3.messages {
public abstract class LegoMessage {
    public readonly Byte hubID;
    public readonly LegoMessageType msgType;

    public LegoMessage(UInt32 hubID, LegoMessageType msgType)
        : base()
    {
        this.hubID = (Byte)hubID;
        this.msgType = msgType;
    }

virtual
    protected int totalLengthInBytes(int __totalLengthOfRemainder) {
        int __totalLength = __totalLengthOfRemainder;
        __totalLength += 3; // Fixed-length fields
        return __totalLength;
    }

virtual
    internal void unparse(BinaryWriter __buffer, int __totalLength) {
        __buffer.Write((Byte)(__totalLength - (int)__buffer.BaseStream.Position));
        __buffer.Write((Byte)(hubID));
        __buffer.Write((Byte)((Byte)(msgType)));
    }

    internal static LegoMessage parse(BinaryReader __buffer) {
        // Extract fields:
        #pragma warning disable 0219 // Ignore unused variables
        Byte totalLength = __buffer.ReadByte();
        Byte hubID = __buffer.ReadByte();
        LegoMessageType msgType = (LegoMessageType)(__buffer.ReadByte());
        #pragma warning restore 0219

        // Determine message type:
        switch (msgType) {
        case LegoMessageType.PROPERTY:
            return MessageHubProperty.parse(__buffer, hubID);
        case LegoMessageType.ACTION:
            return MessageHubAction.parse(__buffer, hubID);
        case LegoMessageType.ALERT:
            return MessageHubAlert.parse(__buffer, hubID);
        case LegoMessageType.ATTACHED_IO:
            return MessagePortRelated.parse(__buffer, hubID, msgType);
        case LegoMessageType.ERROR:
            return MessageError.parse(__buffer, hubID);
        case LegoMessageType.FIRMWARE_BOOT_MODE:
            return MessageFirmwareBootMode.parse(__buffer, hubID);
        case LegoMessageType.PORT_INFORMATION_REQUEST:
            return MessagePortRelated.parse(__buffer, hubID, msgType);
        case LegoMessageType.PORT_MODE_INFORMATION_REQUEST:
            return MessagePortRelated.parse(__buffer, hubID, msgType);
        case LegoMessageType.PORT_INPUT_FORMAT_SETUP_SINGLE:
            return MessagePortRelated.parse(__buffer, hubID, msgType);
        case LegoMessageType.PORT_INPUT_FORMAT_SETUP_COMBINED:
            return MessagePortRelated.parse(__buffer, hubID, msgType);
        case LegoMessageType.PORT_INFORMATION:
            return MessagePortRelated.parse(__buffer, hubID, msgType);
        case LegoMessageType.PORT_MODE_INFORMATION:
            return MessagePortRelated.parse(__buffer, hubID, msgType);
        case LegoMessageType.PORT_VALUE_SINGLE:
            return MessagePortValueSingle.parse(__buffer, hubID);
        case LegoMessageType.PORT_VALUE_COMBINED:
            return MessagePortRelated.parse(__buffer, hubID, msgType);
        case LegoMessageType.PORT_INPUT_FORMAT_SINGLE:
            return MessagePortRelated.parse(__buffer, hubID, msgType);
        case LegoMessageType.PORT_INPUT_FORMAT_COMBINED:
            return MessagePortRelated.parse(__buffer, hubID, msgType);
        case LegoMessageType.VIRTUAL_PORT_SETUP:
            return MessageVirtualPortSetup.parse(__buffer, hubID);
        case LegoMessageType.PORT_OUTPUT_COMMAND:
            return MessagePortRelated.parse(__buffer, hubID, msgType);
        case LegoMessageType.PORT_OUTPUT_COMMAND_FEEDBACK:
            return MessagePortOutputCommandFeedback.parse(__buffer, hubID);
        default: throw new ArgumentException("Invalid value of msgType: "+msgType);
        } // switch
    }

    public abstract void visitWith<T>(LegoMessage_Visitor<T> visitor, T arg);

    #region Message base class utilites
    protected static readonly System.Text.Encoding __asciiCharset = System.Text.Encoding.ASCII;
    protected static readonly System.Text.Encoding __utf8Charset  = System.Text.Encoding.UTF8;

    public int totalLengthInBytes() {
        return totalLengthInBytes(0);
    }

    public static LegoMessage parse(byte[] data) {
        BinaryReader buffer = new BinaryReader(new MemoryStream(data));
        LegoMessage result = parse(buffer);
        int remaining = RemainingBytesInBuffer(buffer);
        if (remaining  != 0) throw new ArgumentException("Extra bytes after message: "+remaining);
        return result;
    }

    public byte[] unparse() {
        int totalLength  = totalLengthInBytes();
        byte[] bytes = new byte[totalLength];
        BinaryWriter buffer = new BinaryWriter(new MemoryStream(bytes));
        unparse(buffer, totalLength);
        return bytes;
    }

    protected static int varIntSize(int value, int maxBytes) {
        if (maxBytes <= 1 || value < (1<< 7)) return 1;
        if (maxBytes <= 2 || value < (1<<14)) return 2;
        if (maxBytes <= 3 || value < (1<<21)) return 3;
        return 4;
    }

    protected static int parseVarInt(BinaryReader buffer, int maxBytes) {
        int v = buffer.ReadByte();

        if (maxBytes <= 1 || v < (1 << 7)) return v; else v &=~ (1 << 7);
        int d = buffer.ReadByte();
        v |= (d << 7);

        if (maxBytes <= 2 || v < (1 << 14)) return v; else v &=~ (1 << 14);
        d = buffer.ReadByte();
        v |= (d << 14);

        if (maxBytes <= 3 || v < (1 << 21)) return v; else v &=~ (1 << 21);
        d = buffer.ReadByte();
        v |= (d << 21);

        return v;
    }

    protected static void unparseVarInt(int value, BinaryWriter buffer, int maxBytes) {
        int d = value;
        if (maxBytes <= 1 || d == (d & 0x7F)) { buffer.Write((Byte)d); return; }
        buffer.Write((byte)(d | 0x80));

        d = (value >> 7) & 0xFF;
        if (maxBytes <= 2 || d == (d & 0x7F)) { buffer.Write((Byte)d); return; }
        buffer.Write((byte)(d | 0x80));

        d = (value >> 14) & 0xFF;
        if (maxBytes <= 3 || d == (d & 0x7F)) { buffer.Write((Byte)d); return; }
        buffer.Write((byte)(d | 0x80));

        d = (value >> 21) & 0xFF;
        buffer.Write((Byte)d);
    }
    protected static Byte[] parseRest(BinaryReader buffer) {
      int len = RemainingBytesInBuffer(buffer);
      return parseByteString(buffer, len);
    }

    protected static Byte[] parseByteString(BinaryReader buffer, int len) {
      return buffer.ReadBytes(len);
    }

    protected static String trimNuls(String s) {
      int pos = s.IndexOf('\0');
      return pos>=0 ? s.Substring(0,pos) : s;
    }

protected static int RemainingBytesInBuffer(BinaryReader buffer) {
    return (int)(buffer.BaseStream.Length - buffer.BaseStream.Position);
}
        #endregion Message base class utilites
}
 }
