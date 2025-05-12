using System.Text.Json.Serialization;
using GameServer.Resources.Enums;

namespace GameServer.Resources.Excel
{
    [ResourceType("EquipmentConfig.json")]
    public class EquipmentConfigExcel : ExcelBase
    {
        public uint EquipmentID { get; set; }
        public bool Release { get; set; }
        public int ExpType { get; set; }
        public int MaxPromotion { get; set; } = 0;
        public int MaxRank { get; set; } = 0;

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public RarityEnum Rarity { get; set; } = 0;

        public override uint GetId()
        {
            return EquipmentID;
        }

        public override void OnLoad()
        {
            if (Release == false) return;
            GameData.EquipmentConfigData.Add(GetId(), this);
        }
    }
}