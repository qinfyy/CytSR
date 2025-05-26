using Proto;
using GameServer.Network;

namespace GameServer.Handlers
{
    [PacketHandler(CmdId.CmdSetClientPausedCsReq)]
    internal class HandlerSetClientPausedCsReq : IPacketHandler
    {
        public void Handle(GameSession session, NetPacket packet)
        {
            var req = NetPacket.GetProto<SetClientPausedCsReq>(packet);

            var rsp = new SetClientPausedCsReq()
            {
                IsPaused = req.IsPaused
            };

            session.Send(NetPacket.Create(CmdId.CmdSetClientPausedScRsp, rsp));
        }
    }
}
