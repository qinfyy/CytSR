using Proto;
using GameServer.Network;

namespace GameServer.Handlers
{
    [PacketHandler(CmdId.CmdGetAvatarDataCsReq)]
    internal class HandlerGetAvatarDataCsReq : IPacketHandler
    {
        public void Handle(GameSession session, NetPacket packet)
        {
            var req = NetPacket.GetProto<GetAvatarDataCsReq>(packet);

            var rsp = new GetAvatarDataScRsp()
            {
                Retcode = 0,
                IsAll = req.IsGetAll,
            };

            var al = session._Player.AvatarMgr.GetAvatarListProto();
            foreach (var a in al)
            {
                rsp.AvatarLists.Add(a);
            }

            foreach (var entry in session._Player.AvatarMgr.GetAvatarPathProto())
            {
                rsp.MultiPathAvatarInfoLists.Add(entry);
            }

            foreach (var cap in session._Player.AvatarMgr.GetCurAvatarPathProto())
            {
                rsp.CurMultiPathAvatarTypeMaps.Add(cap.Key, cap.Value);
            }

            rsp.SkinLists = session._Player.Data.AvatarCompData.UnlockSkins;

            session.Send(NetPacket.Create(CmdId.CmdGetAvatarDataScRsp, rsp));
        }
    }
}
