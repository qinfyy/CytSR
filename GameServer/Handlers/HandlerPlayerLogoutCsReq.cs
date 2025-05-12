using GameServer.Network;
using Proto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Handlers
{
    [PacketHandler(CmdId.CmdPlayerLogoutCsReq)]
    internal class HandlerPlayerLogoutCsReq : IPacketHandler
    {
        public void Handle(GameSession session, NetPacket packet)
        {
            session.Send(NetPacket.Create(CmdId.CmdPlayerLogoutScRsp));
            session._Player.OnPlayerLogout();
            session.Close();
        }
    }
}
