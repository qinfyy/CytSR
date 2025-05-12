using NLog;
using GameServer.Game;
using GameServer.Network.KCP;
using ProtoBuf;
using System.Text.Json;
using Proto;

namespace GameServer.Network
{
    public class GameSession
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();
        public MhyKcpBase Connection { get; }
        public int Id { get; }
        public MhyKcpBase.ConnectionState State => Connection.State;
        public Player _Player { get; set;  } = null;

        public GameSession(MhyKcpBase connection, int id)
        {
            Connection = connection;
            Id = id;
        }

        public void ClientLoop()
        {
            while (Connection.State == MhyKcpBase.ConnectionState.CONNECTED)
            {
                try
                {
                    byte[]? bytes = Connection.Receive();
                    if (bytes == null || bytes.Length < 0) break;
                    var packet = NetPacket.FromBytes(bytes);
                    if (packet == null) break;
                    HandlePacket(packet);
                }
                catch (Exception ex)
                {
                    log.Error($"会话 {Id} 处理包出现异常: {ex}");
                    break;
                }
            }
        }

        private void HandlePacket(NetPacket packet)
        {
            string? packetName = Enum.GetName(typeof(CmdId), packet.CmdId);
            CmdId cmdId = (CmdId)Enum.ToObject(typeof(CmdId), packet.CmdId);
            IPacketHandler handler = PacketFactory.GetPacketHandler(cmdId);

            if (handler == null)
            {
                if (packetName != null)
                {
                    if (Program.Config.LogPacket)
                    {
                        log.Warn($"{packetName} 未处理");
                    }
                    DummyPacket.Handler(this, packet.CmdId);
                }
                else if (Program.Config.LogPacket)
                {
                    log.Warn($"未知 CmdId: {packet.CmdId}");
                }
                return;
            }
            if (Program.Config.LogPacket)
            {
                LogPacket("接收", (ushort)cmdId, packet.Data);
            }
            handler.Handle(this, packet);
        }

        public void Send(NetPacket packet)
        {
            if (Program.Config.LogPacket)
            {
                LogPacket("发送", packet.CmdId, packet.Data);
            }
            Send(packet.GetBytes());
        }

        public void Send(byte[] bytes)
        {
            try
            {
                Connection.Send(bytes);
            }
            catch (Exception ex)
            {
                log.Warn($"会话 {Id} 发包出现异常: {ex}");
            }
        }

        public void Close()
        {
            Connection.Close();
        }

        public void LogPacket(string type, ushort cmdId, byte[] bytes)
        {
            string packetName = Enum.GetName((CmdId)cmdId);

            if (packetName == "CmdPlayerHeartBeatCsReq" ||
                packetName == "CmdPlayerHeartBeatScRsp" ||
                packetName == "CmdSceneEntityMoveCsReq" ||
                packetName == "CmdSceneEntityMoveScRsp"
                )
            {
                return;
            }
            if (packetName == null)
            {
                log.Debug($"[{type}] CMD: {cmdId} - {BitConverter.ToString(bytes).Replace("-", "")}");
                return;
            }
            if (bytes == null || bytes.Length == 0)
            {
                log.Debug($"[{type}] CMD: {packetName}");
                return;
            }
            try
            {
                Type packetType = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(x => x.GetTypes())
                    .FirstOrDefault(t => t.IsClass && t.Namespace == "Proto" && t.Name == packetName.Substring(3));

                MemoryStream memoryStream = new MemoryStream(bytes);
                object obj = Serializer.Deserialize(packetType, memoryStream);
                log.Debug($"[{type}] CMD: {packetName}\n{JsonSerializer.Serialize(obj)}");
                memoryStream.Close();
            }
            catch (Exception ex)
            {
                log.Debug($"[{type}] CMD: {packetName}\n{BitConverter.ToString(bytes).Replace("-", "")}");
            }
        }
    }
}
