using GameServer.Network;
using Proto;

namespace GameServer.Handlers
{
    [PacketHandler(CmdId.CmdDressAvatarSkinCsReq)]
    internal class HandlerDressAvatarSkinCsReq : IPacketHandler
    {
        public void Handle(GameSession session, NetPacket packet)
        {
            var req = NetPacket.GetProto<DressAvatarSkinCsReq>(packet);

            session._Player.AvatarMgr.DressAvatarSkin(req.AvatarId, req.SkinId);

            session.Send(NetPacket.Create(CmdId.CmdDressAvatarSkinScRsp));
        }
    }
}
