using GameServer.Resources.Enums;
using GameServer.Resources;
using GameServer.Resources.Config.Scene;
using Proto;
using Proto.Server;

namespace GameServer.Game;

public class SceneManager
{
    public Player _Player { get; set; }

    public uint GameModeType { get; set; }
    public uint TeleportId { get; set; }

    public List<SceneEntityGroupInfo> EntityGroupList { get; set; } = new();
    public Dictionary<uint, SceneEntityInfo> Entities { get; set; } = new();

    public SceneManager(Player player)
    {
        _Player = player;
    }

    public void InitDefaults()
    {
        if (_Player.Data.SceneCompData == null)
            _Player.Data.SceneCompData = new PlayerSceneData();

        _Player.Data.SceneCompData.FloorId = 20001001;
        _Player.Data.SceneCompData.PlaneId = 20001;
        _Player.Data.SceneCompData.EntryId = 2000101;

        _Player.Data.SceneCompData.Pos = new VectorData()
        {
            X = 99, Y = 62, Z = -4800
        };
        _Player.Data.SceneCompData.Rot = new VectorData()
        {
            X = 0, Y = 0, Z = 0
        }; ;

        _Player.Save();
    }

    public static SceneEntityInfo Actor(uint avatarId, MotionInfo playerPos)
    {
        return new SceneEntityInfo
        {
            EntityId = avatarId,
            Motion = playerPos,
            Actor = new SceneActorInfo
            {
                BaseAvatarId = avatarId,
                AvatarType = AvatarType.AvatarFormalType
            }
        };
    }

    public SceneInfo Load()
    {
        uint entityId = 10000;

        GameData.MapEntranceData.TryGetValue(_Player.Data.SceneCompData.EntryId, out var entrance);
        GameData.MazePlaneData.TryGetValue(entrance.PlaneId, out var mazePlane);
        GameData.GetFloorInfo(entrance.PlaneId, entrance.FloorId, out var floorInfos);

        var proto = new SceneInfo
        {
            EntryId = entrance.Id,
            PlaneId = entrance.PlaneId,
            FloorId = entrance.FloorId,
            GameModeType = (uint)mazePlane.PlaneType
        };

        if (mazePlane.WorldID == 100)
            mazePlane.WorldID = 401;

        proto.WorldId = mazePlane.WorldID;

        foreach (var (groupId, group) in floorInfos.Groups)
        {
            if (group.LoadSide == GroupLoadSideEnum.Client || group.OwnerMainMissionID > 0)
                continue;

            var groupInfo = new SceneEntityGroupInfo { GroupId = groupId };

            foreach (PropInfo prop in group.PropList)
            {
                bool isEventProp = GameData.MainMissionScheduleData.ContainsKey(group.OwnerMainMissionID);

                if (!isEventProp && (prop.IsDelete || prop.IsClientOnly || !prop.LoadOnInitial))
                {
                    continue;
                }

                entityId++;
                var propEntity = new SceneEntityInfo
                {
                    InstId = prop.ID,
                    GroupId = groupId,
                    Motion = new MotionInfo()
                    {
                        Pos = prop.ToPositionProto().ToProto(),
                        Rot = prop.ToRotationProto().ToProto()
                    },
                    Prop = new ScenePropInfo
                    {
                        PropId = prop.PropID,
                        PropState = prop.MappingInfoID > 0 ? 8 : (prop.State == 0 ? 1 : (uint)prop.State)
                    },
                    EntityId = entityId
                };

                groupInfo.EntityLists.Add(propEntity);
            }

            foreach (var npc in group.NPCList)
            {

                bool isEventGroup = GameData.MainMissionScheduleData.ContainsKey(group.OwnerMainMissionID);
                if (!isEventGroup && (npc.IsDelete || npc.IsClientOnly))
                    continue;

                entityId++;
                var npcEntity = new SceneEntityInfo
                {
                    InstId = npc.ID,
                    GroupId = groupId,
                    EntityId = entityId,
                    Motion = new MotionInfo()
                    {
                        Pos = npc.ToPositionProto().ToProto(),
                        Rot = npc.ToPositionProto().ToProto(),
                    },
                    Npc = new SceneNpcInfo { NpcId = npc.NPCID }
                };
                groupInfo.EntityLists.Add(npcEntity);
            }

            foreach (var monster in group.MonsterList)
            {
                if (monster.IsClientOnly || monster.IsDelete)
                    continue;

                entityId++;
                var monsterEntity = new SceneEntityInfo
                {
                    InstId = monster.ID,
                    GroupId = groupId,
                    EntityId = entityId,
                    Motion = new MotionInfo()
                    {
                        Pos = monster.ToPositionProto().ToProto(),
                        Rot = monster.ToRotationProto().ToProto()
                    },
                    NpcMonster = new SceneNpcMonsterInfo
                    {
                        MonsterId = monster.NPCMonsterID,
                        EventId = monster.EventID,
                        WorldLevel = 6
                    }
                };
                groupInfo.EntityLists.Add(monsterEntity);
            }

            proto.EntityGroupLists.Add(groupInfo);
        }

        uint startGroup = entrance.StartGroupId;
        uint startAnchor = entrance.StartAnchorId;

        if (TeleportId != 0)
        {
            floorInfos.CachedTeleports.TryGetValue(TeleportId, out var teleport);
            if (teleport != null)
            {
                startGroup = teleport.AnchorGroupID;
                startAnchor = teleport.AnchorID;
            }

            AnchorInfo playerAnchor = floorInfos.GetAnchorInfo(startGroup, startAnchor);

            var pos = playerAnchor.ToPositionProto();
            var rot = playerAnchor.ToRotationProto();
            _Player.Data.SceneCompData.Pos = new Proto.Server.VectorData() { X = pos.X, Y = pos.Y, Z = pos.Z };
            _Player.Data.SceneCompData.Rot = new Proto.Server.VectorData() { X = rot.X, Y = rot.Y, Z = rot.Z };
        }

        var playerPos = new MotionInfo()
        {
            Pos = new Position(_Player.Data.SceneCompData.Pos).ToProto(),
            Rot = new Position(_Player.Data.SceneCompData.Rot).ToProto(),
        };

        var playerGroup = new SceneEntityGroupInfo { State = 0, GroupId = 0 };

        var curLineup = _Player.Data.LineupCompData.LineupLists
            .FirstOrDefault(l => l.Index == _Player.Data.LineupCompData.CurLineupIndex);
        foreach (var avatar in curLineup.AvatarLists)
        {
            playerGroup.EntityLists.Add(Actor(avatar.AvatarId, playerPos));
        }
        var leaderSlot = curLineup.LeaderSlot;
        var leaderAvatarId = curLineup.AvatarLists[(int)leaderSlot].AvatarId;
        proto.LeaderEntityId = leaderAvatarId;

        proto.EntityGroupLists.Add(playerGroup);

        var lsl = new List<uint>();
        foreach (uint[] range in new uint[][] { new uint[] { 0, 101 }, new uint[] { 10000, 10099 }, new uint[] { 20000, 20099 }, new uint[] { 30000, 30099 } })
        {
            for (uint i = range[0]; i < range[1]; i++)
            {
                lsl.Add(i);
            }
        }
        proto.LightenSectionLists = lsl.ToArray();

        foreach (var value in floorInfos.SavedValues)
                proto.FloorSavedDatas[value.Name] = 2;

        _Player.Data.SceneCompData.EntryId = _Player.Data.SceneCompData.EntryId;
        _Player.Data.SceneCompData.PlaneId = entrance.PlaneId;
        _Player.Data.SceneCompData.FloorId = entrance.FloorId;
        _Player.Data.SceneCompData.PlaneId = entrance.PlaneId;
        _Player.Data.SceneCompData.FloorId = entrance.FloorId;
        _Player.Save();

        return proto;
    }
}
