using Proto;
using GameServer.Network;

namespace GameServer.Handlers
{
    [PacketHandler(CmdId.CmdReplaceLineupCsReq)]
    internal class HandlerReplaceLineupCsReq : IPacketHandler
    {
        public void Handle(GameSession session, NetPacket packet)
        {
            var req = NetPacket.GetProto<ReplaceLineupCsReq>(packet);
            session._Player.LineupMgr.ReplaceLineup(req.Index, req.LeaderSlot, req.Slots);

            // 同步当前阵容
            session._Player.LineupMgr.SyncCurLineup(session);

            //session.Send(NetPacket.Create(CmdId.CmdSyncLineupNotify, sln));
            session.Send(NetPacket.Create(CmdId.CmdReplaceLineupScRsp));
        }
    }
}
