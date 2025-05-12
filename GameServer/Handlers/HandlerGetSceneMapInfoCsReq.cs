using Proto;
using GameServer.Resources;
using GameServer.Resources.Excel;
using GameServer.Resources.Enums;
using GameServer.Network;

namespace GameServer.Handlers
{
    [PacketHandler(CmdId.CmdGetSceneMapInfoCsReq)]
    internal class HandlerGetSceneMapInfoCsReq : IPacketHandler
    {
        public void Handle(GameSession session, NetPacket packet)
        {
            var req = NetPacket.GetProto<GetSceneMapInfoCsReq>(packet);

            var rsp = new GetSceneMapInfoScRsp
            {
                Retcode = 0
            };

            foreach (var fid in req.FloorIdLists)
            {
                var mazeMap = new SceneMapInfo
                {
                    //EntryId = entry,
                    FloorId = fid,
                };

                List<MapEntranceExcel> mecs = new List<MapEntranceExcel>();
                foreach (var entry in GameData.MapEntranceData)
                {
                    if (entry.Value.FloorId == fid)
                    {
                        mecs.Add(entry.Value);
                    }
                }
                if (mecs.Count == 0)
                {
                    rsp.SceneMapInfoes.Add(mazeMap);
                    continue;
                }
                foreach (var excel in mecs)
                {
                    GameData.GetFloorInfo(excel.PlaneId, excel.FloorId, out var floorInfo);

                    if (floorInfo == null)
                    {
                        rsp.SceneMapInfoes.Add(mazeMap);
                        continue;
                    }

                    mazeMap.ChestLists.Add(new ChestInfo
                    {
                        ExistNum = 1,
                        ChestType = ChestType.MapInfoChestTypeNormal
                    });

                    mazeMap.ChestLists.Add(new ChestInfo
                    {
                        ExistNum = 1,
                        ChestType = ChestType.MapInfoChestTypePuzzle
                    });

                    mazeMap.ChestLists.Add(new ChestInfo
                    {
                        ExistNum = 1,
                        ChestType = ChestType.MapInfoChestTypeChallenge
                    });

                    foreach (var groupInfo in floorInfo.Groups.Values) // all the icons on the map
                    {
                        var mazeGroup = new MazeGroup
                        {
                            GroupId = groupInfo.Id
                        };
                        mazeMap.MazeGroupLists.Add(mazeGroup);
                    }

                    var utl = new List<uint>();
                    foreach (var teleport in floorInfo.CachedTeleports.Values)
                        utl.Add(teleport.MappingInfoID);
                    mazeMap.UnlockTeleportLists = utl.ToArray();

                    foreach (var prop in floorInfo.UnlockedCheckpoints)
                    {
                        var mazeTeleport = new Ofcaigdhpoh
                        {
                            GroupId = prop.AnchorGroupID,
                            ConfigId = prop.ID,
                            State = (uint)PropStateEnum.CheckPointEnable

                        };

                        mazeMap.Lmngahfnaons.Add(mazeTeleport);
                    }

                    foreach (var prop in floorInfo.UnlockedCheckpoints)
                    {
                        var mazeProp = new MazePropState
                        {
                            GroupId = prop.AnchorGroupID,
                            ConfigId = prop.ID,
                            State = (uint)PropStateEnum.CheckPointEnable
                        };
                        mazeMap.MazePropLists.Add(mazeProp);
                    }
                }

                var lsl = new List<uint>();
                foreach (uint[] range in new uint[][] { new uint[] { 0, 101 }, new uint[] { 10000, 10099 }, new uint[] { 20000, 20099 }, new uint[] { 30000, 30099 } })
                {
                    for (uint i = range[0]; i < range[1]; i++)
                    {
                        lsl.Add(i);
                    }
                }
                mazeMap.LightenSectionLists = lsl.ToArray();

                rsp.SceneMapInfoes.Add(mazeMap);
            }

            session.Send(NetPacket.Create(CmdId.CmdGetSceneMapInfoScRsp, rsp));
        }
    }
}
