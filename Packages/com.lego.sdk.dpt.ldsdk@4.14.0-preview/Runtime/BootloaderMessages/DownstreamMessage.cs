/*!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
  !!    THIS FILE IS AUTO-GENERATED - please do not edit!             !!
  !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!*/

using System;
using BinaryReader = System.IO.BinaryReader;
using BinaryWriter = System.IO.BinaryWriter;
using MemoryStream= System.IO.MemoryStream;

#pragma warning disable 0109 // "hides inherited member - new: nothing to hide"
namespace dk.lego.devicesdk.bluetooth.V3bootloader.messages {
public abstract class DownstreamMessage {
    public readonly BootloaderMessageType msgType;

    public DownstreamMessage(BootloaderMessageType msgType)
        : base()
    {
        this.msgType = msgType;
    }

virtual
    protected int totalLengthInBytes(int __totalLengthOfRemainder) {
        int __totalLength = __totalLengthOfRemainder;
        __totalLength += 1; // Fixed-length fields
        return __totalLength;
    }

virtual
    internal void unparse(BinaryWriter __buffer, int __totalLength) {
        __buffer.Write((Byte)((Byte)(msgType)));
    }

    internal static DownstreamMessage parse(BinaryReader __buffer) {
        // Extract fields:
        #pragma warning disable 0219 // Ignore unused variables
        BootloaderMessageType msgType = (BootloaderMessageType)(__buffer.ReadByte());
        #pragma warning restore 0219

        // Determine message type:
        switch (msgType) {
        case BootloaderMessageType.ERASE_FLASH:
            return EraseFlashReq.parse(__buffer);
        case BootloaderMessageType.PROGRAM_FLASH:
            return ProgramFlashData.parse(__buffer);
        case BootloaderMessageType.START_APP:
            return StartAppReq.parse(__buffer);
        case BootloaderMessageType.INITIATE_LOADER:
            return InitiateLoaderReq.parse(__buffer);
        case BootloaderMessageType.GET_INFO:
            return GetInfoReq.parse(__buffer);
        case BootloaderMessageType.GET_CHECKSUM:
            return GetChecksumReq.parse(__buffer);
        case BootloaderMessageType.GET_FLASH_STATE:
            return GetFlashStateReq.parse(__buffer);
        case BootloaderMessageType.DISCONNECT:
            return Disconnect.parse(__buffer);
        default: throw new ArgumentException("Invalid value of msgType: "+msgType);
        } // switch
    }

    public abstract void visitWith<T>(DownstreamMessage_Visitor<T> visitor, T arg);

    #region Message base class utilites
    protected static readonly System.Text.Encoding __asciiCharset = System.Text.Encoding.ASCII;
    protected static readonly System.Text.Encoding __utf8Charset  = System.Text.Encoding.UTF8;

    public int totalLengthInBytes() {
        return totalLengthInBytes(0);
    }

    public static DownstreamMessage parse(byte[] data) {
        BinaryReader buffer = new BinaryReader(new MemoryStream(data));
        DownstreamMessage result = parse(buffer);
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
