using System.Collections.Concurrent;
using System.Text.Json.Serialization;
using GameServer.Database;
using GameServer.Resources.Enums;

namespace GameServer.Resources.Config.Scene;

public class FloorInfo
{
    public uint FloorID { get; set; }
    public uint StartGroupIndex { get; set; }
    public uint StartGroupID { get; set; }
    public uint StartAnchorID { get; set; }

    public List<FloorGroupInfo> GroupInstanceList { get; set; } = new List<FloorGroupInfo>();

    public List<ExtraDataInfo> SavedValues { get; set; } = new List<ExtraDataInfo>();

    public List<FloorDimensionInfo> DimensionList { get; set; } = [];

    [JsonIgnore]
    public bool Loaded { get; private set; } = false;

    [JsonIgnore]
    public List<GroupInfo> GroupList { get; set; } = new List<GroupInfo>();

    [JsonIgnore]
    public Dictionary<uint, GroupInfo> Groups { get; set; } = new Dictionary<uint, GroupInfo>();

    [JsonIgnore]
    public Dictionary<uint, PropInfo> CachedTeleports { get; set; } = new Dictionary<uint, PropInfo>();

    [JsonIgnore]
    public List<PropInfo> UnlockedCheckpoints { get; set; } = new List<PropInfo>();

    public GroupInfo GetGroupInfoByIndex(uint groupIndex)
    {
        return GroupList[(int)groupIndex];
    }

    public List<ExtraDataInfo> GetExtraDatas()
    {
        if (SavedValues == null)
        {
            SavedValues = new List<ExtraDataInfo>();
        }

        return SavedValues;
    }

    public AnchorInfo GetStartAnchorInfo()
    {
        GroupInfo group = GetGroupInfoByIndex(StartGroupIndex);
        if (group == null) return null;

        return GetAnchorInfo(group, StartAnchorID);
    }

    public AnchorInfo? GetAnchorInfo(uint groupId, uint anchorId)
    {
        Groups.TryGetValue(groupId, out GroupInfo? group);
        if (group == null) return null;
        return GetAnchorInfo(group, anchorId);
    }

    private AnchorInfo? GetAnchorInfo(GroupInfo group, uint anchorId)
    {
        return group.AnchorList.Find(info => info.ID == anchorId);
    }

    public void OnLoad()
    {
        if (Loaded) return;

        SavedValues = GetExtraDatas();

        foreach (var dimension in DimensionList) 
        { 
            dimension.OnLoad(this); 
        }

        // Cache anchors
        foreach (var group in Groups.Values)
        {
            foreach (var prop in group.PropList)
            {
                if (prop.AnchorID > 0)
                {
                    CachedTeleports.TryAdd(prop.MappingInfoID, prop);
                    UnlockedCheckpoints.Add(prop);

                    prop.State = PropStateEnum.CheckPointEnable;
                }
                else if (!string.IsNullOrEmpty(prop.InitLevelGraph))
                {
                    string json = prop.InitLevelGraph;

                    if (json.Contains("Maze_GroupProp_OpenTreasure_WhenMonsterDie"))
                    {
                        // prop.Trigger = new TriggerOpenTreasureWhenMonsterDie(group.Id);
                    }
                    else if (json.Contains("Common_Console"))
                    {
                        // prop.CommonConsole = true;
                    }

                    prop.ValueSource = null;
                    prop.InitLevelGraph = null;
                }
            }
        }

        Loaded = true;
    }
}

public class FloorGroupInfo
{
    public string GroupPath { get; set; } = "";
    public bool IsDelete { get; set; }
    public uint ID { get; set; }
}

public class ExtraDataInfo
{
    public uint ID { get; set; }
    public string Name { get; set; } = "";
    public int DefaultValue { get; set; }
    public List<int> AllowedValues { get; set; } = new List<int>();
    public int MaxValue { get; set; }
    public int? MinValue { get; set; }
}

public class FloorDimensionInfo
{
    public int ID { get; set; }
    public List<ExtraDataInfo> SavedValues { get; set; } = [];
    public List<uint> GroupIndexList { get; set; } = [];

    [JsonIgnore] public List<uint> GroupIDList { get; set; } = [];

    public void OnLoad(FloorInfo floor)
    {
        foreach (var data in SavedValues)
            floor.SavedValues.Add(new ExtraDataInfo
            {
                ID = data.ID,
                Name = data.Name,
                DefaultValue = data.MaxValue
            });

        foreach (var index in GroupIndexList) GroupIDList.Add(floor.GroupInstanceList[(int)index].ID);
    }
}