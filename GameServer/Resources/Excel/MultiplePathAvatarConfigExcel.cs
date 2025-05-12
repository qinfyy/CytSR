using GameServer.Resources.Enums;

namespace GameServer.Resources.Excel
{
    [ResourceType("MultiplePathAvatarConfig.json")]
    public class MultiplePathAvatarConfigExcel : ExcelBase
    {
        public List<Condition>? UnlockConditions = new();
        public string ChangeConfigPath { get; set; } = "";
        public string Gender { get; set; } = "";
        public uint AvatarID { get; set; }
        public uint BaseAvatarID { get; set; }

        public override uint GetId()
        {
            return AvatarID;
        }
    }

    public class Condition
    {
        public string Param { get; set; } = "";
        public ConditionTypeEnum Type { get; set; }
    }
}