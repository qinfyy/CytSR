using GameServer.Resources;
using System.Text.Json.Serialization;

namespace GameServer.Resources.Excel
{
    [ResourceType("MapEntrance.json", true)]
    public class MapEntranceExcel : ExcelBase
    {
        [JsonPropertyName("BeginMainMissionIDList")]
        public List<uint> BeginMainMissionIdList { get; set; } = new List<uint>();

        [JsonPropertyName("EntranceType")]
        public string EntranceType { get; set; } = "";

        [JsonPropertyName("FinishMainMissionIDList")]
        public List<uint> FinishMainMissionIdList { get; set; } = new List<uint>();

        [JsonPropertyName("FinishSubMissionIDList")]
        public List<uint> FinishSubMissionIdList { get; set; } = new List<uint>();

        [JsonPropertyName("FloorID")]
        public uint FloorId { get; set; }

        [JsonPropertyName("ID")]
        public uint Id { get; set; }

        [JsonPropertyName("PlaneID")]
        public uint PlaneId { get; set; }

        [JsonPropertyName("StartGroupID")]
        public uint StartGroupId { get; set; }

        [JsonPropertyName("StartAnchorID")]
        public uint StartAnchorId { get; set; }

        public override uint GetId()
        {
            return Id;
        }
    }
}
