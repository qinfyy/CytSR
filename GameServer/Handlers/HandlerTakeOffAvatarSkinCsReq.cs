using GameServer.Network;
using Proto;

namespace GameServer.Handlers
{
    [PacketHandler(CmdId.CmdTakeOffAvatarSkinCsReq)]
    internal class HandlerTakeOffAvatarSkinCsReq : IPacketHandler
    {
        public void Handle(GameSession session, NetPacket packet)
        {
            var req = NetPacket.GetProto<TakeOffAvatarSkinCsReq>(packet);

            session._Player.AvatarMgr.DressAvatarSkin(req.AvatarId, 0);

            session.Send(NetPacket.Create(CmdId.CmdTakeOffAvatarSkinScRsp));
        }
    }
}
