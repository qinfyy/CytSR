using Proto;
using GameServer.Network;

namespace GameServer.Handlers
{
    [PacketHandler(CmdId.CmdDressAvatarCsReq)]
    internal class HandlerDressAvatarCsReq : IPacketHandler
    {
        public void Handle(GameSession session, NetPacket packet)
        {
            var req = NetPacket.GetProto<DressAvatarCsReq>(packet);
            session._Player.AvatarMgr.DressEquipment(req.AvatarId, req.EquipmentUniqueId);
            session._Player.SyncClient();

            session.Send(NetPacket.Create(CmdId.CmdDressAvatarScRsp));
        }
    }
}
