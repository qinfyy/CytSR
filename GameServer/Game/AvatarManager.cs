using GameServer.Network;
using GameServer.Resources;
using NLog;
using Proto;
using Proto.Server;

namespace GameServer.Game
{
    public class AvatarManager
    {
        public static readonly Logger Log = LogManager.GetCurrentClassLogger();
        private static readonly uint[] FilterIds = { 7205, 7212, 7213, 7005, 7006, 6022, 6023, 8901, 8902, 1224, 8002, 8003, 8004, 8005, 8006, 8007, 8008 };
        private static readonly uint[] MulitPathIds = { 1001, 1224, 8001, 8002, 8003, 8004, 8005, 8006, 8007, 8008 };
        
        private readonly Player _Player;
        private readonly WeakReference<ItemManager> _ItemMgr;

        public AvatarManager(Player playerInfo)
        {
            _Player = playerInfo;
            _ItemMgr = new WeakReference<ItemManager>(playerInfo.ItemMgr);
        }

        public void InitDefaults()
        {
            if (_Player.Data.AvatarCompData == null)
                _Player.Data.AvatarCompData = new PlayerAvatarData();

            var avatarComp = _Player.Data.AvatarCompData;
            UnlockAvatar(8001);
            UnlockAvatar(1001);
            UnlockAllAvatars();
            UnlockAllAvatarPath();
            avatarComp.Mar7thType = (uint)MultiPathAvatarType.Mar7thKnightType;
            avatarComp.BasicType = (uint)MultiPathAvatarType.GirlShamanType;
            avatarComp.TrailblazerGender = (uint)Gender.GenderWoman;
            avatarComp.UnlockSkins = new uint[] { 1100101 };

            _Player.Save();
        }

        public bool HasAvatar(uint avatarId) =>
            _Player.Data.AvatarCompData != null && _Player.Data.AvatarCompData.AvatarLists.Any(a => a.AvatarId == avatarId);

        public void DressEquipment(uint avatarId, uint equipmentUid)
        {
            bool mulitId = false;
            if (MulitPathIds.Contains(avatarId))
                mulitId = true;

            if (!mulitId && !HasAvatar(avatarId))
            {
                Log.Error("角色不存在");
                return;
            }

            var itemMgr = _ItemMgr.TryGetTarget(out var itemManager) ? itemManager : null;
            if (itemMgr == null)
            {
                Log.Error("ItemManager不可用");
                return;
            }

            itemMgr.AssignEquipment(equipmentUid, avatarId);

            var prevEquipmentUid = SetEquipment(avatarId, equipmentUid, mulitId);
            if (prevEquipmentUid != 0)
            {
                itemMgr.AssignEquipment(prevEquipmentUid, 0);
            }
        }

        public void DressRelic(uint avatarId, uint relicUid, uint relicSlot)
        {
            bool mulitId = false;
            if (MulitPathIds.Contains(avatarId))
                mulitId = true;

            if (!mulitId && !HasAvatar(avatarId))
            {
                Log.Error("角色不存在");
                return;
            }

            var itemMgr = _ItemMgr.TryGetTarget(out var itemManager) ? itemManager : null;
            if (itemMgr == null)
            {
                Log.Error("ItemManager不可用");
                return;
            }

            itemMgr.AssignRelic(relicUid, avatarId);

            var prevRelicUid = SetRelicUid(avatarId, relicUid, relicSlot);
            if (prevRelicUid != 0)
            {
                itemMgr.AssignRelic(prevRelicUid, 0);
            }
        }

        public void TakeOffRelic(uint avatarId, uint relicSlot)
        {
            var relicUid = SetRelicUid(avatarId, 0, relicSlot);

            var itemMgr = _ItemMgr.TryGetTarget(out var itemManager) ? itemManager : null;
            if (itemMgr != null)
                itemMgr.AssignRelic(relicUid, 0);
            else
                Log.Error("ItemManager不可用");
        }

        private uint SetEquipment(uint avatarId, uint equipmentUid, bool mulitAvatar)
        {
            var avatarComp = _Player.Data.AvatarCompData;
            uint prevEquipmentUid = 0;

            if (mulitAvatar)
            {
                var ad = avatarComp.AvatarLists
                    .FirstOrDefault(a => a.PathInfoes.ContainsKey(avatarId)); 
                if (ad != null && ad.PathInfoes.TryGetValue(avatarId, out var pi))
                {
                    pi.EquipmentUniqueId = equipmentUid;
                }
                else
                {
                    Log.Error("Avatar Path不存在");
                    return 0;
                }
            } 
            else
            {
                var avatar = avatarComp?.AvatarLists.FirstOrDefault(a => a.AvatarId == avatarId);

                if (avatar == null)
                {
                    Log.Error("角色不存在");
                    return 0;
                }

                prevEquipmentUid = avatar.EquipmentUniqueId;
                avatar.EquipmentUniqueId = equipmentUid;
            }
            
            return prevEquipmentUid;
        }

        private uint SetRelicUid(uint avatarId, uint relicUid, uint relicSlot)
        {
            avatarId = avatarId;

            var avatarComp = _Player.Data.AvatarCompData;
            var avatar = avatarComp.AvatarLists.FirstOrDefault(a => a.AvatarId == avatarId);

            var prevRelicUid = avatar.RelicMaps.ContainsKey(relicSlot) ? avatar.RelicMaps[relicSlot] : 0;

            avatar.RelicMaps[relicSlot] = relicUid;
            return prevRelicUid;
        }

        public void TakeOffEquipment(uint avatarId)
        {
            bool mulitId = false;
            if (MulitPathIds.Contains(avatarId))
                mulitId = true;

            if (!mulitId && !HasAvatar(avatarId))
            {
                Log.Error("角色不存在");
                return;
            }

            var equipmentUid = SetEquipment(avatarId, 0, mulitId);

            var itemMgr = _ItemMgr.TryGetTarget(out var itemManager) ? itemManager : null;
            if (itemMgr != null)
                itemMgr.AssignEquipment(equipmentUid, 0);
            else
                Log.Error("ItemManager不可用");
        }

        public void UnlockAllAvatars()
        {
            foreach (var entry in GameData.AvatarConfigData)
            {
                UnlockAvatar(entry.Key);
            }
        }

        public void UnlockAvatar(uint avatarId)
        {
            if (HasAvatar(avatarId)) return;
            var avatarComp = _Player.Data.AvatarCompData;
            bool avatarExists = GameData.AvatarConfigData
                .Any(entry => entry.Value.AvatarId == avatarId && !FilterIds.Contains(entry.Value.AvatarId));

            if (!avatarExists) return;
            var newAvatarData = new AvatarData
            {
                AvatarType = (int)AvatarType.AvatarFormalType,
                AvatarId = avatarId,
                Level = 80,
                Exp = 3,
                Promotion = 6,
                Rank = 6,
                //SkillTreeLists
                EquipmentUniqueId = 0,
                //RelicMaps
                FirstMetTimeStamp = (ulong)DateTimeOffset.Now.ToUnixTimeSeconds(),
            };

            foreach (var entry in GameData.AvatarSkillTreeConfigData)
            {
                if (entry.Value.AvatarID == avatarId && entry.Value.DefaultUnlock)
                {
                    newAvatarData.SkillTreeLists.Add(new AvatarSkillTreeData
                    {
                        PointId = entry.Value.PointID,
                        Level = entry.Value.Level
                    });
                }
            }
            avatarComp.AvatarLists.Add(newAvatarData);

            _Player.Save();
        }

        public void UnlockAvatarPath(uint avatarId, uint baseAvatarID)
        {
            var avatarComp = _Player.Data.AvatarCompData;
            if (baseAvatarID != 1001 && baseAvatarID != 8001) return;

            var npi = new PathInfo
            {
                PathId = avatarId,
                Rank = 6,
                EquipmentUniqueId = 0,
                //RelicMaps
                //SkillTreeLists
            };

            foreach (var skillTree in GameData.AvatarSkillTreeConfigData)
            {
                if (skillTree.Value.AvatarID == avatarId && skillTree.Value.DefaultUnlock)
                {
                    npi.SkillTreeLists.Add(new AvatarSkillTreeData
                    {
                        PointId = skillTree.Value.PointID,
                        Level = skillTree.Value.Level
                    });
                }
            }

            int ali = avatarComp.AvatarLists.FindIndex(a => a.AvatarId == baseAvatarID);

            if (ali != -1)
            {
                avatarComp.AvatarLists[ali].PathInfoes[avatarId] = npi;
                _Player.Save();
            }
        }

        public void UnlockAllAvatarPath()
        {
            foreach (var entry in GameData.MultiplePathAvatarConfigData)
            {
                UnlockAvatarPath(entry.Key, entry.Value.BaseAvatarID);
            }
        }

        public List<Proto.Avatar> GetAvatarListProto()
        {
            var avatarComp = _Player.Data.AvatarCompData;
            var avatarList = new List<Proto.Avatar>();

            if (avatarComp?.AvatarLists != null)
            {
                foreach (var a in avatarComp.AvatarLists)
                {
                    var avatar = new Proto.Avatar
                    {
                        BaseAvatarId = a.AvatarId,
                        Exp = a.Exp,
                        Level = a.Level,
                        Promotion = a.Promotion,
                        Rank = a.Rank,
                        EquipmentUniqueId = a.EquipmentUniqueId,
                        FirstMetTimeStamp = a.FirstMetTimeStamp,
                        DressedSkinId = a.Skin
                    };

                    for (int i = 0; i < a.SkillTreeLists.Count; i++)
                    {
                        avatar.SkilltreeLists.Add(new AvatarSkillTree
                        {
                            PointId = a.SkillTreeLists[i].PointId,
                            Level = a.SkillTreeLists[i].Level
                        });
                    }

                    foreach (var r in a.RelicMaps)
                    {
                        avatar.EquipRelicLists.Add(new EquipRelic
                        {
                            Type = r.Key,
                            RelicUniqueId = r.Value
                        });
                    }

                    if (avatar.BaseAvatarId == 1001)
                        avatar.DressedSkinId = 1100101;

                    avatarList.Add(avatar);
                }
            }

            return avatarList.Count > 0 ? avatarList : new List<Proto.Avatar>();

        }

        public List<MultiPathAvatarInfo> GetAvatarPathProto()
        {
            var avatarComp = _Player.Data.AvatarCompData;
            var res = new List<MultiPathAvatarInfo>();

            foreach (var avatar in avatarComp.AvatarLists)
            {
                if (avatar.PathInfoes.Count == 0)
                    continue;

                foreach (var pi in avatar.PathInfoes)
                {
                    var proto = new MultiPathAvatarInfo
                    {
                        AvatarId = (MultiPathAvatarType)pi.Key,
                        Rank = pi.Value.Rank,
                        PathEquipmentId = pi.Value.EquipmentUniqueId,
                        DressedSkinId = pi.Value.Skin
                    };

                    foreach (var skill in pi.Value.SkillTreeLists)
                        proto.MultiPathSkillTrees.Add(new AvatarSkillTree
                        {
                            PointId = skill.PointId,
                            Level = skill.Level
                        });

                    foreach (var relic in pi.Value.RelicMaps)
                        proto.EquipRelicLists.Add(new EquipRelic
                        {
                            Type = relic.Key,
                            RelicUniqueId = relic.Value
                        });

                    res.Add(proto);
                }
            }

            return res;
        }

        public Dictionary<uint, MultiPathAvatarType> GetCurAvatarPathProto() =>
            new Dictionary<uint, MultiPathAvatarType>()
            {
                {8001, (MultiPathAvatarType)_Player.Data.AvatarCompData.BasicType },
                {1001, (MultiPathAvatarType)_Player.Data.AvatarCompData.Mar7thType }
            };

        public void SetAvatarPath(MultiPathAvatarType curId)
        {
            var acd = _Player.Data.AvatarCompData;
            uint baseId = 0;

            var avatarMappings = new Dictionary<MultiPathAvatarType, (uint id, Action<uint> setter)>
            {
                { MultiPathAvatarType.BoyWarriorType, (8001, id => acd.BasicType = id) },
                { MultiPathAvatarType.GirlWarriorType, (8002, id => acd.BasicType = id) },
                { MultiPathAvatarType.BoyKnightType, (8003, id => acd.BasicType = id) },
                { MultiPathAvatarType.GirlKnightType, (8004, id => acd.BasicType = id) },
                { MultiPathAvatarType.BoyShamanType, (8005, id => acd.BasicType = id) },
                { MultiPathAvatarType.GirlShamanType, (8006, id => acd.BasicType = id) },
                { MultiPathAvatarType.BoyMemoryType, (8007, id => acd.BasicType = id) },
                { MultiPathAvatarType.GirlMemoryType, (8008, id => acd.BasicType = id) },
                { MultiPathAvatarType.Mar7thKnightType, (1001, id => acd.Mar7thType = id) },
                { MultiPathAvatarType.Mar7thRogueType, (1224, id => acd.Mar7thType = id) },
            };

            if (avatarMappings.TryGetValue(curId, out var mapping))
            {
                baseId = mapping.id;
                mapping.setter(baseId);
            }

            _Player.Session.Send(NetPacket.Create(CmdId.CmdAvatarPathChangedNotify, new AvatarPathChangedNotify
            {
                CurMultiPathAvatarType = curId,
                BaseAvatarId = baseId
            }));

            _Player.SyncClient();
        }

        public void DressAvatarSkin(uint avatarId, uint skinId)
        {
            bool mulitId = false;
            if (MulitPathIds.Contains(avatarId))
                mulitId = true;

            if (mulitId)
            {
                var ad = _Player.Data.AvatarCompData.AvatarLists
                    .FirstOrDefault(a => a.PathInfoes.ContainsKey(avatarId));
                if (ad != null && ad.PathInfoes.TryGetValue(avatarId, out var pi))
                {
                    ad.Skin = skinId;
                    pi.Skin = skinId;
                }
                else
                {
                    Log.Error("Avatar Path不存在");
                    return;
                }
            }
            else
            {
                var ad = _Player.Data.AvatarCompData.AvatarLists.FirstOrDefault(a => a.AvatarId == avatarId);

                if (ad == null)
                {
                    Log.Error("角色不存在");
                    return;
                }
                ad.Skin = skinId;
            }

            _Player.Save();
            _Player.SyncClient();
        }
    }
}