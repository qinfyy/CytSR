using GameServer.Resources.Excel;
using GameServer.Resources.Config.Scene;

namespace GameServer.Resources
{
    public static class GameData
    {
        public static Dictionary<uint, MapEntranceExcel> MapEntranceData { get; private set; } = new();
        public static Dictionary<uint, MazePlaneExcel> MazePlaneData { get; private set; } = new();
        public static Dictionary<uint, AvatarSkillTreeConfigExcel> AvatarSkillTreeConfigData { get; private set; } = new();
        public static Dictionary<uint, AvatarConfigExcel> AvatarConfigData { get; private set; } = new();
        public static Dictionary<uint, MultiplePathAvatarConfigExcel> MultiplePathAvatarConfigData { get; private set; } = new();
        public static Dictionary<uint, ContentPackageConfigExcel> ContentPackageConfigData { get; private set; } = new();
        public static Dictionary<uint, MappingInfoExcel> MappingInfoData { get; private set; } = new();
        public static Dictionary<uint, ItemConfigExcel> ItemConfigData { get; private set; } = new();
        public static Dictionary<uint, EquipmentConfigExcel> EquipmentConfigData { get; private set; } = new();
        public static Dictionary<uint, MainMissionScheduleExcel> MainMissionScheduleData { get; private set; } = new();

        public static Dictionary<string, FloorInfo> FloorInfoData { get; private set; } = new();

        public static bool GetFloorInfo(uint planeId, uint floorId, out FloorInfo floorInfo)
        {
            string key = $"P{planeId}_F{floorId}";
            if (FloorInfoData.TryGetValue(key, out floorInfo))
            {
                return true;
            }
            else
            {
                floorInfo = null;
                return false;
            }
        }

        public static int GetMinPromotionForLevel(int level)
        {
            return Math.Max(Math.Min((int)((level - 11) / 10D), 6), 0);
        }
    }
}
