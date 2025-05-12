using GameServer.Network;
using Proto;

namespace GameServer.Handlers
{
    [PacketHandler(CmdId.CmdSetAvatarPathCsReq)]
    internal class HandlerSetAvatarPathCsReq : IPacketHandler
    {
        public void Handle(GameSession session, NetPacket packet)
        {
            var req = NetPacket.GetProto<SetAvatarPathCsReq>(packet);
            session._Player.AvatarMgr.SetAvatarPath(req.AvatarId);

            session.Send(NetPacket.Create(CmdId.CmdSetAvatarPathScRsp, new SetAvatarPathScRsp()
            {
                Retcode = 0,
                AvatarId = req.AvatarId
            }));
        }
    }
}
