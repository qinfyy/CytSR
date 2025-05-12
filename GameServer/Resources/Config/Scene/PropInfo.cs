using GameServer.Database;
using GameServer.Resources.Config.Scene;
using GameServer.Resources.Enums;
using System.Text.Json.Serialization;

namespace GameServer.Resources.Config.Scene;

public class PropInfo : PositionInfo
{
    public uint MappingInfoID { get; set; }
    public uint AnchorGroupID { get; set; }
    public uint AnchorID { get; set; }
    public uint PropID { get; set; }
    public uint EventID { get; set; }
    public uint CocoonID { get; set; }
    public uint FarmElementID { get; set; }
    public bool IsClientOnly { get; set; }
    public bool LoadOnInitial { get; set; }

    public PropValueSource? ValueSource { get; set; }
    public string? InitLevelGraph { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public PropStateEnum State { get; set; } = PropStateEnum.Closed;

    [JsonIgnore]
    public Dictionary<int, List<int>> UnlockDoorID { get; set; } = new();

    public void Load(GroupInfo info)
    {
        if (ValueSource != null)
        {
            foreach (var v in ValueSource.Values)
            {
                try
                {
                    if (v.TryGetValue("Key", out var key) && v.TryGetValue("Value", out var value) && value != null && key != null)
                    {
                        if (key.ToString().Contains("Door") ||
                            key.ToString().Contains("Bridge") ||
                            key.ToString().Contains("UnlockTarget") ||
                            key.ToString().Contains("Rootcontamination") ||
                            key.ToString().Contains("Portal"))
                        {
                            try
                            {
                                var values = value.ToString()?.Split(",");
                                if (values != null && int.TryParse(values[0], out var mainKey) && int.TryParse(values[1], out var subKey))
                                {
                                    if (!UnlockDoorID.ContainsKey(mainKey))
                                        UnlockDoorID[mainKey] = new List<int>();
                                    UnlockDoorID[mainKey].Add(subKey);
                                }
                            }
                            catch
                            {
                                // Handle parsing errors if necessary
                            }
                        }
                    }
                }
                catch
                {
                    // Handle any unexpected errors in value processing
                }
            }
        }
    }
}

public class PropValueSource
{
    public List<Dictionary<string, object>> Values { get; set; } = new();
}