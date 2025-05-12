using GameServer.Network;
using Proto;

namespace GameServer.Handlers
{
    [PacketHandler(CmdId.CmdGetAllLineupDataCsReq)]
    internal class HandlerGetAllLineupDataCsReq : IPacketHandler
    {
        public void Handle(GameSession session, NetPacket packet)
        {
            var rsp = new GetAllLineupDataScRsp()
            {
                Retcode = 0,
                CurIndex = session._Player.Data.LineupCompData.CurLineupIndex
                //LineupLists
            };

            foreach (var lineup in session._Player.LineupMgr.GetAllLineupProto())
            {
                rsp.LineupLists.Add(lineup);
            }

            session.Send(NetPacket.Create(CmdId.CmdGetAllLineupDataScRsp, rsp));
        }
    }
}
