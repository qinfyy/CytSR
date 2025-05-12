namespace GameServer.Resources.Excel;

[ResourceType("AvatarSkillTreeConfig.json")]
public class AvatarSkillTreeConfigExcel : ExcelBase
{
    public uint PointID { get; set; }
    public uint Level { get; set; }
    public uint AvatarID { get; set; }
    public bool DefaultUnlock { get; set; }
    public uint MaxLevel { get; set; }

    public override uint GetId()
    {
        return PointID * 10 + Level;
    }

    public override void AfterAllDone()
    {
        GameData.AvatarConfigData.TryGetValue(AvatarID, out var excel);
        if (excel != null && DefaultUnlock)
        {
            excel.DefaultSkillTree.Add(this);
        }
        if (excel != null) 
        { 
            excel.SkillTree.Add(this); 
        }
        GameData.AvatarSkillTreeConfigData.Add(GetId(), this);
    }
}