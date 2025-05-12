using Proto;
using GameServer.Network;

namespace GameServer.Handlers
{
    [PacketHandler(CmdId.CmdReplaceLineupCsReq)]
    internal class HandlerReplaceLineupCsReq : IPacketHandler
    {
        public void Handle(GameSession session, NetPacket packet)
        {
            var req = NetPacket.GetProto<ReplaceLineupCsReq>(packet);
            session._Player.LineupMgr.ReplaceLineup(req.Index, req.LeaderSlot, req.LineupSlotLists);
            //var sln = new SyncLineupNotify();
            //var lineup = new LineupInfo()
            //{
            //    Mp = 5,
            //    MaxMp = 5,
            //    Name = "Lineup 1"
            //};

            //for (int i = 0; i < req.Slots.Count; i++)
            //{
            //    var avatar = new LineupAvatar()
            //    {
            //        Id = req.Slots[i].Id,
            //        Satiety = 0,
            //        Slot = (uint)i,
            //        AvatarType = AvatarType.AvatarFormalType,
            //        Hp = 10000,
            //        SpBar = new SpBarInfo()
            //        {
            //            CurSp = 10000,
            //            MaxSp = 10000,
            //        }
            //    };

            //    lineup.AvatarLists.Add(avatar);
            //}

            // 获取 scene_mgr 并刷新演员组
            //var sceneMgr = session.Context.SceneMgr;
            //var refreshInfo = sceneMgr.RefreshActorGroup();
            //if (refreshInfo != null)
            //{
            //    await session.Send(
            //        CMD_SCENE_GROUP_REFRESH_SC_NOTIFY,
            //        new SceneGroupRefreshScNotify
            //        {
            //            RefreshGroupList = new List<RefreshInfo> { refreshInfo }
            //        }
            //    );
            //}

            // 同步当前阵容
            session._Player.LineupMgr.SyncCurLineup(session);

            //session.Send(NetPacket.Create(CmdId.CmdSyncLineupNotify, sln));
            session.Send(NetPacket.Create(CmdId.CmdReplaceLineupScRsp));
        }
    }
}
