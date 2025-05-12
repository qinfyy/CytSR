using GameServer.Network;
using Proto;

namespace GameServer.Handlers
{
    [PacketHandler(CmdId.CmdJoinLineupCsReq)]
    internal class HandlerJoinLineupCsReq : IPacketHandler
    {
        public void Handle(GameSession session, NetPacket packet)
        {
            var req = NetPacket.GetProto<JoinLineupCsReq>(packet);
            session._Player.LineupMgr.JoinLineup(req.Index, req.Slot, req.BaseAvatarId);
            session._Player.LineupMgr.SyncCurLineup(session);
            session.Send(NetPacket.Create(CmdId.CmdJoinLineupScRsp));
        }
    }
}
