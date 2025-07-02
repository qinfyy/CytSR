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
                IsGetAll = req.IsGetAll,
            };

            var al = session._Player.AvatarMgr.GetAvatarListProto();
            foreach (var a in al)
            {
                rsp.AvatarLists.Add(a);
            }

            foreach (var entry in session._Player.AvatarMgr.GetAvatarPathProto())
            {
                rsp.MultiPathAvatarTypeInfoLists.Add(entry);
            }

            foreach (var cap in session._Player.AvatarMgr.GetCurAvatarPathProto())
            {
                rsp.CurrentMultiPathAvatarIds.Add(cap.Key, cap.Value);
            }

            rsp.OwnedSkinIdLists = session._Player.Data.AvatarCompData.UnlockSkins;

            session.Send(NetPacket.Create(CmdId.CmdGetAvatarDataScRsp, rsp));
        }
    }
}
