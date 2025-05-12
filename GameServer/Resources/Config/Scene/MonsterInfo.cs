namespace GameServer.Resources.Config.Scene;

public class MonsterInfo : PositionInfo
{
    public uint NPCMonsterID { get; set; }
    public uint EventID { get; set; }
    public uint FarmElementID { get; set; }
    public bool IsClientOnly { get; set; }
}