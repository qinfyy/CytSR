using GameServer.Network;
using Proto;

namespace GameServer.Handlers
{
    [PacketHandler(CmdId.CmdGetMultiPathAvatarInfoCsReq)]
    internal class HandlerGetMultiPathAvatarInfoCsReq : IPacketHandler
    {
        public void Handle(GameSession session, NetPacket packet)
        {
            var rsp = new GetMultiPathAvatarInfoScRsp();
            var btil = new List<uint>();
            foreach (var entry in session._Player.AvatarMgr.GetAvatarPathProto())
            {
                rsp.MultiPathAvatarInfoLists.Add(entry);
                btil.Add((uint)entry.AvatarId);
            }

            rsp.BasicTypeIdLists = btil.ToArray();

            foreach (var cap in session._Player.AvatarMgr.GetCurAvatarPathProto())
                rsp.CurMultiPathAvatarTypeMaps.Add(cap.Key, cap.Value);

            session.Send(NetPacket.Create(CmdId.CmdGetMultiPathAvatarInfoScRsp, rsp));
        }
    }
}
