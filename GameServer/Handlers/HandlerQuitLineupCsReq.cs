using GameServer.Network;
using Proto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Handlers
{
    [PacketHandler(CmdId.CmdQuitLineupCsReq)]
    internal class HandlerQuitLineupCsReq : IPacketHandler
    {
        public void Handle(GameSession session, NetPacket packet)
        {
            var req = NetPacket.GetProto<QuitLineupCsReq>(packet);
            session._Player.LineupMgr.QuitLineup(req.Index, req.BaseAvatarId);
            session.Send(NetPacket.Create(CmdId.CmdQuitLineupScRsp));
        }
    }
}
