using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using GameServer.Database;
using GameServer.Resources.Enums;

namespace GameServer.Resources.Excel
{
    [ResourceType("AvatarConfig.json,AvatarConfigTrial.json", true)]
    public class AvatarConfigExcel : ExcelBase
    {
        [JsonIgnore]
        public List<AvatarSkillTreeConfigExcel> DefaultSkillTree { get; set; } = new();

        [JsonIgnore]
        public List<AvatarSkillTreeConfigExcel> SkillTree { get; set; } = new();

        [JsonPropertyName("AvatarID")]
        public uint AvatarId { get; set; } = 0;

        public string AvatarName { get; set; } = "";

        public int ExpGroup { get; set; } = 0;

        public int MaxPromotion { get; set; } = 0;

        public int MaxRank { get; set; } = 0;

        public List<int> RankIDList { get; set; } = new();

        public string? JsonPath { get; set; } = "";

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public RarityEnum Rarity { get; set; } = 0;

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public DamageTypeEnum DamageType { get; set; } = 0;

        [JsonIgnore]
        public uint RankUpItemId { get; set; }

        [JsonIgnore]
        public string NameKey { get; set; } = "";

        public override uint GetId()
        {
            return AvatarId;
        }

        public override void OnLoad()
        {
            if (!GameData.AvatarConfigData.ContainsKey(GetId()))
            {
                GameData.AvatarConfigData.Add(GetId(), this);
            }

            RankUpItemId = AvatarId + 10000;

            var regex = new Regex(@"(?<=Avatar_)(.*?)(?=_Config)");
            var match = regex.Match(JsonPath ?? "");
            if (match.Success)
            {
                NameKey = match.Value;
            }

            JsonPath = null;
        }
    }
}
