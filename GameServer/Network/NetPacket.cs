using NLog;
using Proto;
using ProtoBuf;
using System.Buffers;
using System.Buffers.Binary;

namespace GameServer.Network
{
    public class NetPacket
    {
        public static readonly Logger Log = LogManager.GetCurrentClassLogger();

        public const uint HEADER_CONST = 0x9D74C714;
        public const uint TAIL_CONST = 0xD7A152C8;

        public ushort CmdId { get; private set; }
        public byte[] Head { get; private set; }
        public byte[] Data { get; private set; }

        private NetPacket(ushort cmdId, byte[] data, byte[] head)
        {
            CmdId = cmdId;
            Head = head;
            Data = data;
        }

        public static NetPacket FromBytes(byte[] raw)
        {
            if (raw.Length < 16)
            {
                Log.Error("Packet is too short to be valid");
                return null;
            }

            uint headMagic = BinaryPrimitives.ReadUInt32BigEndian(raw.AsSpan(0));

            if (headMagic != HEADER_CONST)
            {
                Log.Error("Invalid head magic");
                return null;
            }

            var cmdId = BinaryPrimitives.ReadUInt16BigEndian(raw.AsSpan(4));
            ushort headerLength = BinaryPrimitives.ReadUInt16BigEndian(raw.AsSpan(6));
            uint dataLength = BinaryPrimitives.ReadUInt32BigEndian(raw.AsSpan(8));

            if (raw.Length != 16 + headerLength + dataLength)
            {
                Log.Error("Packet length does not match expected length");
                return null;
            }

            var head = raw.Skip(12).Take(headerLength).ToArray();
            var data = raw.Skip(12 + headerLength).Take((int)dataLength).ToArray();
            uint tailMagic = BinaryPrimitives.ReadUInt32BigEndian(raw.AsSpan(12 + (int)dataLength + headerLength));

            if (tailMagic != TAIL_CONST)
            {
                Log.Error("Invalid tail magic");
                return null;
            }

            return new NetPacket(cmdId, data, head);
        }

        public byte[] GetBytes()
        {
            ushort cmdId = CmdId;
            byte[] data = Data;
            byte[] head = Head;

            byte[] buf = new byte[16 + head.Length + data.Length];
            Array.Fill(buf, (byte)0);

            BinaryPrimitives.WriteUInt32BigEndian(buf, HEADER_CONST);
            BinaryPrimitives.WriteUInt16BigEndian(buf.AsSpan(4), cmdId);
            BinaryPrimitives.WriteUInt16BigEndian(buf.AsSpan(6), (ushort)head.Length);
            BinaryPrimitives.WriteUInt32BigEndian(buf.AsSpan(8), (uint)data.Length);
            head.CopyTo(buf.AsSpan(12));
            data.CopyTo(buf.AsSpan(12 + head.Length));
            BinaryPrimitives.WriteUInt32BigEndian(buf.AsSpan(12 + head.Length + data.Length), TAIL_CONST);

            return buf;
        }

        public static NetPacket Create<T>(CmdId cmdId, T proto)
        {
            MemoryStream stream = new MemoryStream();
            Serializer.Serialize(stream, proto);
            byte[] data = stream.ToArray();
            stream.Close();

            return Create(cmdId, data, Array.Empty<byte>());
        }

        public static NetPacket Create(CmdId cmdId, byte[]? data = null, byte[]? head = null) =>
            new NetPacket((ushort)cmdId, data ?? Array.Empty<byte>(), head ?? Array.Empty<byte>());

        public static T? GetProto<T>(NetPacket packet)
        {
            T? SerializedBody = default;
            MemoryStream ms = new MemoryStream(packet.Data);

            try
            {
                SerializedBody = Serializer.Deserialize<T>(ms);
            }
            catch (Exception ex)
            {
                string? packetName = Enum.GetName(typeof(CmdId), packet.CmdId);
                Log.Error($"Failed to deserialize {packetName ?? packet.CmdId.ToString()}!\n{ex}");
            }

            ms.Close();
            return SerializedBody;
        }
    }
}