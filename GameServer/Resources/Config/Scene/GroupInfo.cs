using System.Text.Json.Serialization;
using GameServer.Resources.Enums;

namespace GameServer.Resources.Config.Scene
{

    public class GroupInfo
    {
        public uint Id { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public GroupLoadSideEnum LoadSide { get; set; }

        public bool LoadOnInitial { get; set; }
        public string GroupName { get; set; } = "";

        public uint OwnerMainMissionID { get; set; }
        public List<AnchorInfo> AnchorList { get; set; } = new List<AnchorInfo>();
        public List<MonsterInfo> MonsterList { get; set; } = new List<MonsterInfo>();
        public List<PropInfo> PropList { get; set; } = new List<PropInfo>();
        public List<NpcInfo> NPCList { get; set; } = new List<NpcInfo>();

        public void Load()
        {
            foreach (var prop in PropList)
            {
                prop.Load(this);
            }
        }
    }
}