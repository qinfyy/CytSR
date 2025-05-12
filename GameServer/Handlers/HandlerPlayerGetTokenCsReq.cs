using GameServer.Game;
using GameServer.Database;
using GameServer.Network;
using Proto;

namespace GameServer.Handlers
{
    [PacketHandler(CmdId.CmdPlayerGetTokenCsReq)]
    internal class HandlerPlayerGetTokenCsReq : IPacketHandler
    {
        public void Handle(GameSession session, NetPacket packet)
        {
            var req = NetPacket.GetProto<PlayerGetTokenCsReq>(packet);
            var login = Storage.GetPlayerDataByAccountUid(req.AccountUid);
            session._Player = new Player(session, login.Item1, login.Item2);
            var rsp = new PlayerGetTokenScRsp()
            {
                Retcode = 0,
                Msg = "OK",
                Uid = login.Item1,
            };

            session.Send(NetPacket.Create(CmdId.CmdPlayerGetTokenScRsp, rsp));
        }
    }
}
