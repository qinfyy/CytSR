using Proto;
using GameServer.Network;

namespace GameServer.Handlers
{
    [PacketHandler(CmdId.CmdGetMissionStatusCsReq)]
    internal class HandlerGetMissionStatusCsReq : IPacketHandler
    {
        public void Handle(GameSession session, NetPacket packet)
        {
            var req = NetPacket.GetProto<GetMissionStatusCsReq>(packet);

            var rsp = new GetMissionStatusScRsp
            {
                Retcode = 0,
                FinishedMainMissionIdLists = req.MainMissionIdLists
            };

            if (req.SubMissionIdLists != null)
            {
                foreach (uint id in req.SubMissionIdLists)
                {
                    rsp.SubMissionStatusLists.Add(new Mission()
                    {
                        Id = id,
                        Progress = 1,
                        Status = MissionStatus.MissionFinish
                    });
                }
            }

            if (req.MainMissionIdLists != null)
            {
                rsp.FinishedMainMissionIdLists = req.MainMissionIdLists;
            }

            session.Send(NetPacket.Create(CmdId.CmdGetMissionStatusScRsp, rsp));
        }
}
}
