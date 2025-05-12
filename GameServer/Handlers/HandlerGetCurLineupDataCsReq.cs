using Proto;
using GameServer.Network;

namespace GameServer.Handlers
{
    [PacketHandler(CmdId.CmdGetCurLineupDataCsReq)]
    internal class HandlerGetCurLineup : IPacketHandler
    {
        public uint[] CurrLineup = new uint[4]
        {
            1317, 1222, 1221, 1306
        };

        public void Handle(GameSession session, NetPacket packet)
        {
            var rsp = new GetCurLineupDataScRsp
            {
                Retcode = 0,
                Lineup = session._Player.LineupMgr.GetCurLineupProto()
            };

            

            session.Send(NetPacket.Create(CmdId.CmdGetCurLineupDataScRsp, rsp));
        }
    }
}