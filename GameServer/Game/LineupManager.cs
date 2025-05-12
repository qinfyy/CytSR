using NLog;
using Proto;
using Proto.Server;
using GameServer.Network;

namespace GameServer.Game
{
    public class LineupManager
    {
        public static readonly Logger Log = LogManager.GetCurrentClassLogger();

        private static readonly uint[] DEFAULT_LINEUP = new uint[4] { 8001, 1001, 1310, 0 };

        private readonly Player _Player;

        public LineupManager(Player playerInfo)
        {
            _Player = playerInfo;
        }

        // 初始化默认阵容
        public void InitDefaults()
        {
            if (_Player.Data.LineupCompData == null)
                _Player.Data.LineupCompData = new PlayerLineupData();

            var avatarComp = _Player.Data.AvatarCompData;
            var lineupComp = _Player.Data.LineupCompData;
            lineupComp.CurLineupIndex = 0;
            lineupComp.Mp = 5;
            lineupComp.MpMax = 5;


            var lineupData = new LineupData();
            for (int i = 0; i < DEFAULT_LINEUP.Length; i++)
            {
                var id = DEFAULT_LINEUP[i];
                if (id != 0)
                {
                    var avatarBin = CreateLineupAvatarData(id, (uint)i, avatarComp);
                    lineupData.AvatarLists.Add(avatarBin);
                }
            }

            lineupComp.LineupLists.Add(lineupData);
            _Player.Save();
        }

        // 设置当前阵容的领袖
        public void SetCurLineupLeader(uint leaderSlot)
        {
            var lineupComp = _Player.Data.LineupCompData;

            var lineup = lineupComp.LineupLists
                .FirstOrDefault(l => l.Index == lineupComp.CurLineupIndex);

            lineup.LeaderSlot = leaderSlot;
            _Player.Save();
        }

        // 加入阵容
        public void JoinLineup(uint index, uint slot, uint baseAvatarId)
        {
            var lineupComp = _Player.Data.LineupCompData;
            var lineupBin = lineupComp.LineupLists
                .FirstOrDefault(l => l.Index == index);

            lineupBin.AvatarLists.Add(CreateLineupAvatarData(baseAvatarId, slot, _Player.Data.AvatarCompData));
            _Player.Save();
        }

        // 退出阵容
        public void QuitLineup(uint index, uint baseAvatarId)
        {
            var lineupComp = _Player.Data.LineupCompData;
            var lineupBin = lineupComp.LineupLists
                .FirstOrDefault(l => l.Index == index);

            var avatarToRemove = lineupBin.AvatarLists
                .FirstOrDefault(a => a.AvatarId == baseAvatarId);

            lineupBin.AvatarLists.Remove(avatarToRemove);
            _Player.Save();
        }

        // 替换阵容
        public void ReplaceLineup(uint index, uint leaderSlot, List<LineupSlotData> Slot)
        {
            var lineupBin = _Player.Data.LineupCompData.LineupLists
                .FirstOrDefault(l => l.Index == index);

            lineupBin.AvatarLists.Clear();
            for (int i = 0; i < Slot.Count; i++)
            {
                lineupBin.AvatarLists.Add(CreateLineupAvatarData(Slot[i].Id, Slot[i].Slot, _Player.Data.AvatarCompData));
            }

            lineupBin.LeaderSlot = leaderSlot;
            _Player.Save();
        }

        // 同步阵容
        public void SyncCurLineup(GameSession session)
        {
            var sln = new SyncLineupNotify();
            sln.Lineup = GetCurLineupProto();
            sln.ReasonLists.Add(SyncLineupReason.SyncReasonNone);

            session.Send(NetPacket.Create(CmdId.CmdSyncLineupNotify, sln));
        }

        // 获取当前阵容的数据
        public LineupInfo GetCurLineupProto() =>
            GetLineupProto(_Player.Data.LineupCompData.CurLineupIndex);

        // 获取指定index的阵容数据
        public LineupInfo GetLineupProto(uint index)
        {
            var lineupComp = _Player.Data.LineupCompData;
            LineupData lineupData = lineupComp.LineupLists.FirstOrDefault(lineup => lineup.Index == index);

            if (lineupData == null)
                Log.Error("未找到 LineupData");

            var li = new LineupInfo
            {
                Index = lineupData.Index,
                Name = lineupData.Name,
                IsVirtual = lineupData.IsVirtual,
                PlaneId = lineupData.PlaneId,
                Mp = lineupComp.Mp,
                MaxMp = lineupComp.MpMax,
                LeaderSlot = lineupData.LeaderSlot,
                ExtraLineupType = (ExtraLineupType)lineupData.ExtraLineupType
            };

            foreach (var avatar in lineupData.AvatarLists)
            {
                li.AvatarLists.Add(new LineupAvatar
                {
                    Id = avatar.AvatarId,
                    AvatarType = (AvatarType)avatar.AvatarType,
                    Slot = avatar.Slot,
                    Hp = avatar.Hp,
                    SpBar = new SpBarInfo { SpCur = 10000, SpMax = 10000 }
                });
            }

            return li;
        }

        // 创建阵容角色
        private static Proto.Server.LineupAvatarData CreateLineupAvatarData(uint id, uint slot, PlayerAvatarData avatarComp)
        {
            var avatarData = new AvatarData();
            foreach (var avatar in avatarComp.AvatarLists)
            {
                if (avatar.AvatarId == id)
                {
                    avatarData = avatar;
                    break;
                }
            }

            return new Proto.Server.LineupAvatarData
            {
                AvatarId = id,
                Slot = slot,
                Hp = 10000,
                Sp = 10000,
                AvatarType = avatarData.AvatarType
            };
        }

        public List<Proto.LineupInfo> GetAllLineupProto()
        {
            var playerInfo = _Player.Data;
            var lineupComp = playerInfo.LineupCompData;

            List<Proto.LineupInfo> result = new List<Proto.LineupInfo>();
            for (int i = 0; i < lineupComp.LineupLists.Count; i++)
            {
                var lineupInfo = GetLineupProto((uint)i);
                if (lineupInfo != null)
                    result.Add(lineupInfo);
            }
            return result;
        }
    }
}