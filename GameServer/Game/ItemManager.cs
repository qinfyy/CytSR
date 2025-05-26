using GameServer.Resources;
using GameServer.Resources.Excel;
using NLog;
using Proto.Server;
using Proto;

namespace GameServer.Game
{
    public class ItemManager
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        private readonly Player _Player;

        public ItemManager(Player player)
        {
            _Player = player;
        }

        public void InitDefaults()
        {
            if (_Player.Data.ItemCompData == null)
                _Player.Data.ItemCompData = new PlayerItemData();

            GiveAllEquipment();
            AddMaterial(1, 1000000);
        }

        public void AddMaterial(uint materialId, uint amount)
        {
            var itemComp = _Player.Data.ItemCompData;
            if (itemComp == null)
            {
                 Log.Error("ItemBin未初始化。");
                return;
            }
            var material = itemComp.MaterialLists.FirstOrDefault(m => m.Tid == materialId);

            if (material != null)
            {
                material.Num += amount;
            }
            else
            {
                bool materialExists = false;
                foreach (var item in GameData.ItemConfigData)
                {
                    if (item.Value.ID == materialId)
                    {
                        materialExists = true;
                        break;
                    }
                }

                if (!materialExists)
                {
                    Log.Error($"id为{materialId}的物品不存在");
                    return;
                }

                itemComp.MaterialLists.Add(new MaterialData { Tid = materialId, Num = amount });
            }

            _Player.Save();
        }

        public void AssignEquipment(uint uniqueId, uint avatarId)
        {
            var itemComp = _Player.Data.ItemCompData;
            if (itemComp == null)
            {
                Log.Error("ItemBin未初始化。");
                return;
            }

            EquipmentData equipment = null;
            foreach (var e in itemComp.EquipmentLists)
            {
                if (e.UniqueId == uniqueId)
                {
                    equipment = e;
                    break;
                }
            }

            if (equipment == null)
            {
                Log.Error("武器不存在");
                return;
            }

            equipment.AvatarId = avatarId;

            _Player.Save();
        }

        public void AssignRelic(uint uniqueId, uint avatarId)
        {
            var itemComp = _Player.Data.ItemCompData;
            if (itemComp == null)
            {
                Log.Error("ItemBin未初始化。");
                return;
            }
            var relic = itemComp.RelicLists.FirstOrDefault(r => r.UniqueId == uniqueId);
            if (relic == null)
            {
                Log.Error("遗器不存在");
                return;
            }

            relic.AvatarId = avatarId;

            _Player.Save();
        }

        public void GiveAllEquipment()
        {
            foreach (var config in GameData.EquipmentConfigData)
            {
                if (config.Value.Release)
                {
                    GiveEquipment((uint)config.Value.EquipmentID);
                }
            }
        }

        public void GiveRelic(uint id, uint level, uint affixId, List<(uint, uint)> subAffixList)
        {
            uint uniqueId = NextUniqueId();
            var itemComp = _Player.Data.ItemCompData ?? throw new InvalidOperationException("ItemBin未初始化。");

            var rd = new RelicData
            {
                UniqueId = uniqueId,
                Tid = id,
                Level = level,
                MainAffixId = affixId,
            };
            foreach (var sal in subAffixList)
            {
                RelicAffixData data = new RelicAffixData
                {
                    AffixId = sal.Item1,
                    Cnt = sal.Item2,
                    Step = sal.Item2 * 2
                };

                rd.SubAffixLists.Add(data);
            }
            itemComp.RelicLists.Add(rd);
            _Player.Save();
        }

        public void GiveEquipment(uint equipmentId)
        {
            EquipmentConfigExcel config = GameData.EquipmentConfigData
                .FirstOrDefault(e => e.Value.EquipmentID == equipmentId).Value;

            if (config == null)
            {
                Log.Error($"id为{equipmentId}的武器不存在");
                return;
            }

            uint uniqueId = NextUniqueId();
            var itemComp = _Player.Data.ItemCompData ?? throw new InvalidOperationException("ItemBin未初始化。");

            itemComp.EquipmentLists.Add(new EquipmentData
            {
                Tid = (uint)config.EquipmentID,
                UniqueId = uniqueId,
                Level = 80,
                Rank = 5,
                Promotion = 6
            });

            _Player.Save();
        }

        public List<Equipment> GetEquipmentListProto()
        {
            var itemComp = _Player.Data.ItemCompData ?? throw new InvalidOperationException("ItemBin未初始化。");

            return itemComp.EquipmentLists.Select(e => new Equipment
            {
                Id = e.Tid,
                UniqueId = e.UniqueId,
                Level = e.Level,
                Exp = e.Exp,
                Promotion = e.Promotion,
                Rank = e.Rank,
                IsProtected = e.IsProtected,
                CharId = e.AvatarId,
                //BaseAvatarId = e.AvatarId > 8000 ? 8001 : e.AvatarId
            }).ToList();
        }

        public List<Material> GetMaterialListProto()
        {
            var itemComp = _Player.Data.ItemCompData;

            return itemComp.MaterialLists.Select(m => new Material
            {
                Id = m.Tid,
                Num = m.Num
            }).ToList();
        }

        public List<Relic> GetRelicListProto()
        {
            var itemComp = _Player.Data.ItemCompData;

            List<Relic> relicList = new List<Relic>();

            foreach (var r in itemComp.RelicLists)
            {
                Relic relic = new Relic
                {
                    MainAffixId = r.MainAffixId,
                    CharId = r.AvatarId,
                    UniqueId = r.UniqueId,
                    IsProtected = r.IsProtected,
                    Level = r.Level,
                    Exp = r.Exp,
                    Id = r.Tid
                };

                foreach (var s in r.SubAffixLists)
                {
                    RelicAffix affix = new RelicAffix
                    {
                        AffixId = s.AffixId,
                        Cnt = s.Cnt,
                        Step = s.Step
                    };
                    relic.SubAffixLists.Add(affix);
                }

                relicList.Add(relic);
            }

            return relicList;
        }

        private uint NextUniqueId()
        {
            var itemComp = _Player.Data.ItemCompData;
            itemComp.UidCounter++;
            return itemComp.UidCounter;
        }
    }

}
