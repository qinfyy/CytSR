using GameServer.Network;
using Proto;

namespace GameServer.Handlers
{
    [PacketHandler(CmdId.CmdSetMultipleAvatarPathsCsReq)]
    internal class HandlerSetMultipleAvatarPathsCsReq : IPacketHandler
    {
        public void Handle(GameSession session, NetPacket packet)
        {
            var req = NetPacket.GetProto<SetMultipleAvatarPathsCsReq>(packet);

            foreach(var avatarId in req.AvatarIdLists) //AvatarIdLists
            {
                session._Player.AvatarMgr.SetAvatarPath(avatarId);
            }

            session.Send(NetPacket.Create(CmdId.CmdSetMultipleAvatarPathsScRsp));
        }
    }
}
