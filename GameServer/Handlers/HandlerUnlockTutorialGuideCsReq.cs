using GameServer.Network;
using Proto;

namespace GameServer.Handlers
{
    [PacketHandler(CmdId.CmdUnlockTutorialGuideCsReq)]
    internal class HandlerUnlockTutorialGuideCsReq : IPacketHandler
    {
        public void Handle(GameSession session, NetPacket packet)
        {
            session.Send(NetPacket.Create(CmdId.CmdUnlockTutorialGuideScRsp));
        }

    }
}
