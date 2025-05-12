using Proto;
using GameServer.Network;

namespace GameServer.Handlers
{
    [PacketHandler(CmdId.CmdGetCurSceneInfoCsReq)]
    internal class HandlerGetCurSceneInfoCsReq : IPacketHandler
    {
        public void Handle(GameSession session, NetPacket packet)
        {
            var pos = session._Player.Data.SceneCompData.Pos;
            var rot = session._Player.Data.SceneCompData.Rot;

            var rsp = new GetCurSceneInfoScRsp()
            {
                Retcode = 0,
                Scene = session._Player.SceneMgr.Load()
            };

            session.Send(NetPacket.Create(CmdId.CmdGetCurSceneInfoScRsp, rsp));
        }
    }
}
