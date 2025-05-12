using GameServer.Network;
using Proto;

namespace GameServer.Handlers
{
    [PacketHandler(CmdId.CmdGetBagCsReq)]
    internal class HandlerGetBagCsReq : IPacketHandler
    {
        public void Handle(GameSession session, NetPacket packet)
        {
            var rsp = new GetBagScRsp();
            var el = session._Player.ItemMgr.GetEquipmentListProto();
            foreach (var e in el)
            {
                rsp.EquipmentLists.Add(e);
            }

            var ml = session._Player.ItemMgr.GetMaterialListProto();
            foreach (var m in ml)
            {
                rsp.MaterialLists.Add(m);
            }

            var rl = session._Player.ItemMgr.GetRelicListProto();
            foreach (var r in rl)
            {
                rsp.RelicLists.Add(r);
            }

            session.Send(NetPacket.Create(CmdId.CmdGetBagScRsp, rsp));
        }
    }
}
