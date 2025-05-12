using static GameServer.Resources.Excel.MazePlaneExcel;
using System.Text.Json.Serialization;
using GameServer.Resources.Enums;
using GameServer.Database;

namespace GameServer.Resources.Excel
{
    [ResourceType("ItemConfig.json,ItemConfigAvatar.json,ItemConfigAvatarPlayerIcon.json," +
                    "ItemConfigAvatarRank.json,ItemConfigBook.json,ItemConfigDisk.json," +
                    "ItemConfigEquipment.json,ItemConfigRelic.json,ItemPlayerCard.json", true)]
    public class ItemConfigExcel : ExcelBase
    {
        public uint ID { get; set; }
        public string ItemName { get; set; } = "";

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ItemMainTypeEnum ItemMainType { get; set; } = ItemMainTypeEnum.Unknown;

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ItemSubTypeEnum ItemSubType { get; set; } = ItemSubTypeEnum.Unknown;

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ItemRarityEnum Rarity { get; set; } = ItemRarityEnum.Unknown;

        public int PileLimit { get; set; }
        public int PurposeType { get; set; }

        public int UseDataID { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ItemUseMethodEnum UseMethod { get; set; }

        public List<MappingInfoItem> ReturnItemIDList { get; set; } = new();

        public override uint GetId()
        {
            return ID;
        }
    }
}