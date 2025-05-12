using Proto;
using GameServer.Network;

namespace GameServer.Handlers
{
    [PacketHandler(CmdId.CmdInteractPropCsReq)]
    internal class HandlerInteractPropCsReq : IPacketHandler
    {
        public void Handle(GameSession session, NetPacket packet)
        {
            var req = NetPacket.GetProto<InteractPropCsReq>(packet);
            var rsp = new InteractPropScRsp()
            {
                Retcode = 0,
                PropEntityId = req.PropEntityId,
                PropState = 1
            };
            session.Send(NetPacket.Create(CmdId.CmdInteractPropScRsp, rsp));
        }
    }
}
