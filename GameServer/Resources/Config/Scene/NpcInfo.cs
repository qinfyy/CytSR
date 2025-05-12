namespace GameServer.Resources.Config.Scene;

public class NpcInfo : PositionInfo
{
    public uint NPCID { get; set; }
    public bool IsClientOnly { get; set; }
}