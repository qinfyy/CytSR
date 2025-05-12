using System.Text.Json.Serialization;
using GameServer.Resources.Enums;

namespace GameServer.Resources.Excel
{
    [ResourceType("MazePlane.json", true)]
    public class MazePlaneExcel : ExcelBase
    {
        public uint PlaneID { get; set; }
        public uint WorldID { get; set; }
        public uint StartFloorID { get; set; }
        public string PlaneName { get; set; } = "";

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public PlaneTypeEnum PlaneType { get; set; }

        public override uint GetId()
        {
            return PlaneID;
        }
    }
}
