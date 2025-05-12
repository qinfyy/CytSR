using System;
using System.Buffers.Binary;

namespace GameServer.Network.KCP
{
    public class Handshake
    {
        public static readonly uint[] MAGIC_CONNECT = { 0xFF, 0xFFFFFFFF };
        public static readonly uint[] MAGIC_SEND_BACK_CONV = { 0x145, 0x14514545 };
        public static readonly uint[] MAGIC_DISCONNECT = { 0x194, 0x19419494 };
        public const int LEN = 20;

        public uint Magic1;
        public uint Conv;
        public uint Token;
        public uint Data;
        public uint Magic2;

        public Handshake() { }

        public Handshake(uint[] magic, uint conv = 0, uint token = 0, uint data = 0)
        {
            Magic1 = magic[0];
            Conv = conv;
            Token = token;
            Data = data;
            Magic2 = magic[1];
        }

        public void Encode(byte[] buffer)
        {
            if (buffer.Length < LEN)
                throw new ArgumentException("Buffer too small", nameof(buffer));

            BinaryPrimitives.WriteUInt32BigEndian(buffer.AsSpan(0, 4), Magic1);
            BinaryPrimitives.WriteUInt32BigEndian(buffer.AsSpan(4, 4), Conv);
            BinaryPrimitives.WriteUInt32BigEndian(buffer.AsSpan(8, 4), Token);
            BinaryPrimitives.WriteUInt32BigEndian(buffer.AsSpan(12, 4), Data);
            BinaryPrimitives.WriteUInt32BigEndian(buffer.AsSpan(16, 4), Magic2);
        }

        public void Decode(byte[] buffer, uint[]? verifyMagic = null)
        {
            if (buffer.Length < LEN)
                throw new ArgumentException("Handshake packet too small", nameof(buffer));

            var span = buffer.AsSpan();

            Magic1 = BinaryPrimitives.ReadUInt32BigEndian(span.Slice(0, 4));
            Conv = BinaryPrimitives.ReadUInt32BigEndian(span.Slice(4, 4));
            Token = BinaryPrimitives.ReadUInt32BigEndian(span.Slice(8, 4));
            Data = BinaryPrimitives.ReadUInt32BigEndian(span.Slice(12, 4));
            Magic2 = BinaryPrimitives.ReadUInt32BigEndian(span.Slice(16, 4));

            if (verifyMagic != null && (Magic1 != verifyMagic[0] || Magic2 != verifyMagic[1]))
                throw new ArgumentException("Invalid handshake packet", nameof(buffer));
        }

        public void Decode(Memory<byte> buffer, uint[]? verifyMagic = null)
        {
            if (buffer.Length < LEN)
                throw new ArgumentException("Handshake packet too small", nameof(buffer));

            var span = buffer.Span;

            Magic1 = BinaryPrimitives.ReadUInt32BigEndian(span.Slice(0, 4));
            Conv = BinaryPrimitives.ReadUInt32BigEndian(span.Slice(4, 4));
            Token = BinaryPrimitives.ReadUInt32BigEndian(span.Slice(8, 4));
            Data = BinaryPrimitives.ReadUInt32BigEndian(span.Slice(12, 4));
            Magic2 = BinaryPrimitives.ReadUInt32BigEndian(span.Slice(16, 4));

            if (verifyMagic != null && (Magic1 != verifyMagic[0] || Magic2 != verifyMagic[1]))
                throw new ArgumentException("Invalid handshake packet", nameof(buffer));
        }

        public static Handshake Parse(byte[] buffer)
        {
            var hs = new Handshake();
            hs.Decode(buffer); // 修复原代码使用了 Encode 错误
            return hs;
        }

        public byte[] AsBytes()
        {
            var ret = new byte[LEN];
            Encode(ret);
            return ret;
        }
    }
}
