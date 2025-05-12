using Proto;
using GameServer.Network;

namespace GameServer.Handlers
{
    [PacketHandler(CmdId.CmdGetTutorialCsReq)]
    internal class HandlerGetTutorialCsReq : IPacketHandler
    {
        public void Handle(GameSession session, NetPacket packet)
        {
            //var rsp = new GetTutorialScRsp
            //{
            //    Retcode = 0
            //};

            //foreach (var guideId in GameData.TutorialDataData.Values)
            //{
            //    rsp.TutorialLists.Add(new Tutorial
            //    {
            //        Id = guideId.TutorialID,
            //        Status = TutorialStatus.TutorialFinish
            //    });
            //}

            //session.Send(NetPacket.Create(CmdId.CmdGetTutorialScRsp, rsp));

        }
    }
}
