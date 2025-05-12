using Proto;
using GameServer.Network;

namespace GameServer.Handlers
{
    [PacketHandler(CmdId.CmdPlayerHeartBeatCsReq)]
    internal class HandlerPlayerHeartBeatCsReq : IPacketHandler
    {
        public void Handle(GameSession session, NetPacket packet)
        {
            var req = NetPacket.GetProto<PlayerHeartBeatCsReq>(packet);

            var rsp = new PlayerHeartBeatScRsp
            {
                ServerTimeMs = (ulong)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                Retcode = 0,
                ClientTimeMs = req.ClientTimeMs,
                DownloadData = new ClientDownloadData()
                {
                    Version = 51,
                    Time = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                    Data = Convert.FromBase64String("bG9jYWwgb2JqID0gQ1MuVW5pdHlFbmdpbmUuR2FtZU9iamVjdC5GaW5kKCIvVUlSb290L0Fib3ZlRGlhbG9nL0JldGFIaW50RGlhbG9nKENsb25lKS9Db250ZW50cy9WZXJzaW9uVGV4dCIpCmlmIG9iaiB0aGVuCiAgICBsb2NhbCB0ZXh0Q29tcG9uZW50ID0gb2JqOkdldENvbXBvbmVudEluQ2hpbGRyZW4odHlwZW9mKENTLlJQRy5DbGllbnQuTG9jYWxpemVkVGV4dCkpCiAgICBpZiB0ZXh0Q29tcG9uZW50IHRoZW4KICAgICAgICBsb2NhbCByZXN1bHQgPSBzdHJpbmcubWF0Y2godGV4dENvbXBvbmVudC50ZXh0LCAiVUlEOiVkKyIpCiAgICAgICAgbG9jYWwgY3VyVWlkID0gcmVzdWx0IG9yICJVbmtub3duIFVpZCIKICAgICAgIHRleHRDb21wb25lbnQudGV4dCA9ICI8Y29sb3I9I0ZGOEMwMD4gQ3l0U1IgIiAuLiBjdXJVaWQgLi4gIjwvY29sb3I+IgogICAgZW5kCmVuZA==")
                }
            };
            
            session.Send(NetPacket.Create(CmdId.CmdPlayerHeartBeatScRsp, rsp));
        }
    }
}
