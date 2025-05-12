using Proto;
using GameServer.Network;

namespace GameServer.Handlers
{
    [PacketHandler(CmdId.CmdChangeLineupLeaderCsReq)]
    internal class HandlerChangeLineupLeaderCsReq : IPacketHandler
    {
        public void Handle(GameSession session, NetPacket packet)
        {
            var req = NetPacket.GetProto<ChangeLineupLeaderCsReq>(packet);

            session._Player.LineupMgr.SetCurLineupLeader(req.Slot);
            var rsp = new ChangeLineupLeaderScRsp()
            {
                Slot = req.Slot,
                Retcode = 0
            };

            session.Send(NetPacket.Create(CmdId.CmdChangeLineupLeaderScRsp, rsp));
        }
    }
}
