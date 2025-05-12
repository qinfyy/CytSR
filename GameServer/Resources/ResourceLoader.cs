using System.Reflection;
using System.Text.Json;
using GameServer.Resources.Excel;
using GameServer.Resources.Config.Scene;
using NLog;

namespace GameServer.Resources
{
    public class ResourceLoader
    {
        public static readonly Logger Log = LogManager.GetCurrentClassLogger();

        private static readonly string ResourceRootPath = Program.Config.ResourcePath;

        private static readonly JsonSerializerOptions SerializerOptions;

        static ResourceLoader()
        {
            SerializerOptions = new JsonSerializerOptions();
            SerializerOptions.Converters.Add(new CustomJsonFactory());
        }

        public static void LoadGameData()
        {
            Log.Info("正在加载 Resource ...");
            LoadAllExcel();
            LoadFloorInfo();
            Log.Info("完成加载 Resource");
        }

        public static void LoadAllExcel()
        {
            var classes = Assembly.GetExecutingAssembly().GetTypes();
            List<ExcelBase> resList = new();

            foreach (var cla in classes.Where(x => x.IsSubclassOf(typeof(ExcelBase))))
            {
                var res = LoadExcel(cla);
                if (res != null) resList.AddRange(res);
            }

            foreach (var cla in resList) cla.AfterAllDone();
        }

        public static List<ExcelBase>? LoadExcel(Type cla)
        {
            var attribute = (ResourceTypeAttribute?)Attribute.GetCustomAttribute(cla, typeof(ResourceTypeAttribute));
            if (attribute == null) return null;

            var resource = (ExcelBase)Activator.CreateInstance(cla)!;
            var count = 0;
            List<ExcelBase> resList = new List<ExcelBase>();

            MethodInfo onLoadMethod = cla.GetMethod("OnLoad", BindingFlags.Public | BindingFlags.Instance);
            MethodInfo afterAllDoneMethod = cla.GetMethod("AfterAllDone", BindingFlags.Public | BindingFlags.Instance);
            bool isDefaultOnLoad = onLoadMethod?.DeclaringType == typeof(ExcelBase);
            bool isDefaultAfterAllDone = afterAllDoneMethod?.DeclaringType == typeof(ExcelBase);
            bool autoLoad = isDefaultOnLoad && isDefaultAfterAllDone;

           string dictName = cla.Name.Replace("Excel", "Data");

            foreach (var fileName in attribute.FileName)
            {
                try
                {
                    var path = Path.Combine(ResourceRootPath, "ExcelOutput", fileName);
                    var file = new FileInfo(path);
                    if (!file.Exists)
                    {
                        Log.Error($"文件 {path} 不存在");
                        continue;
                    }

                    string? json = file.OpenText().ReadToEnd();
                    JsonDocument document = JsonDocument.Parse(json);

                    if (document.RootElement.ValueKind == JsonValueKind.Array)
                    {
                        foreach (JsonElement item in document.RootElement.EnumerateArray())
                        {
                            object? instance = JsonSerializer.Deserialize(item.GetRawText(), cla, SerializerOptions);
                            if (instance is ExcelBase excelInstance)
                            {
                                if (autoLoad)
                                {
                                    AddToGameData(excelInstance, dictName);
                                }
                                else
                                {
                                    excelInstance.OnLoad();
                                }
                                resList.Add(excelInstance);
                                count++;
                            }
                        }
                    }
                    else if (document.RootElement.ValueKind == JsonValueKind.Object)
                    {
                        foreach (JsonProperty item in document.RootElement.EnumerateObject())
                        {
                            int id = int.Parse(item.Name);
                            JsonElement obj = item.Value;

                            object? instance = JsonSerializer.Deserialize(obj.GetRawText(), cla, SerializerOptions);
                            if (instance is ExcelBase excelInstance)
                            {
                                if (excelInstance.GetId() == 0)
                                {
                                    if (obj.ValueKind == JsonValueKind.Object)
                                    {
                                        foreach (var nestedItem in obj.EnumerateObject())
                                        {
                                            object? nestedInstance = JsonSerializer.Deserialize(nestedItem.Value.GetRawText(), cla, SerializerOptions);
                                            if (nestedInstance is ExcelBase nestedExcel)
                                            {
                                                if (autoLoad)
                                                {
                                                    AddToGameData(nestedExcel, dictName);
                                                }
                                                else
                                                {
                                                    nestedExcel.OnLoad();
                                                }
                                                resList.Add(nestedExcel);
                                                count++;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (autoLoad)
                                    {
                                        AddToGameData(excelInstance, dictName);
                                    }
                                    else
                                    {
                                        excelInstance.OnLoad();
                                    }
                                    resList.Add(excelInstance);
                                }
                                count++;
                            }
                        }
                    }

                    resource.Finalized();
                }
                catch (Exception ex)
                {
                    Log.Error($"读取 {fileName} 时出错: {ex}");
                }
            }

            Log.Info($"已加载 {count} 个 {cla.Name}");

            return resList;
        }

        private static void AddToGameData(ExcelBase instance, string dictName)
        {
            PropertyInfo? prop = typeof(GameData).GetProperty(dictName, BindingFlags.Static | BindingFlags.Public);
            if (prop != null)
            {
                try
                {
                    object? dictInstance = prop.GetValue(null);

                    Type dictType = dictInstance.GetType();
                    uint id = instance.GetId();

                    PropertyInfo indexProperty = dictType.GetProperty("Item", BindingFlags.Instance | BindingFlags.Public);
                    MethodInfo setMethod = indexProperty.GetSetMethod();

                    setMethod.Invoke(dictInstance, new object[] { id, instance });
                } 
                catch (Exception ex)
                {
                    Log.Error($"无法写入字典 {dictName}: {ex}");
                }
            }
            else
            {
                Log.Error($"未找到 GameData 中名为 {dictName} 的属性");
            }
        }

        public static void LoadFloorInfo()
        {
            Log.Info("加载 floor 文件...");
            string folderPath = Path.Combine(ResourceRootPath, "Config", "LevelOutput", "RuntimeFloor");
            DirectoryInfo directory = new(folderPath);
            bool missingGroupInfos = false;

            if (!directory.Exists)
            {
                Log.Warn($"floor 文件丢失，请检查您的资源文件夹：/Config/LevelOutput/RuntimeFloor。传送和世界怪物可能不工作！");
                return;
            }

            foreach (FileInfo file in directory.GetFiles())
            {
                try
                {
                    using var reader = file.OpenRead();
                    using StreamReader reader2 = new(reader);
                    var text = reader2.ReadToEnd();

                    FloorInfo? info = JsonSerializer.Deserialize<FloorInfo>(text);
                    var name = file.Name[..file.Name.IndexOf('.')];
                    GameData.FloorInfoData.Add(name, info!);
                }
                catch (Exception ex)
                {
                    Log.Error($"读取 {file.Name} 出错: {ex}");
                }
            }

            foreach (var info in GameData.FloorInfoData.Values)
            {
                foreach (var groupInfo in info.GroupInstanceList)
                {
                    if (groupInfo.IsDelete)
                    {
                        continue;
                    }
                    FileInfo file = new(Path.Combine(ResourceRootPath, groupInfo.GroupPath)); ;
                    if (!file.Exists)
                    {
                        continue;
                    }

                    try
                    {
                        using var reader = file.OpenRead();
                        using StreamReader reader2 = new(reader);
                        var text = reader2.ReadToEnd();

                        GroupInfo? group = JsonSerializer.Deserialize<GroupInfo>(text);
                        if (group != null)
                        {
                            group.Id = groupInfo.ID;
                            if (!info.Groups.ContainsKey(groupInfo.ID))
                            {
                                info.Groups.Add(groupInfo.ID, group);
                                group.Load();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"读取 {file.Name} 出错: {ex}");
                    }
                    if (info.Groups.Count == 0)
                    {
                        missingGroupInfos = true;
                    }
                }
                info.OnLoad();
            }

            if (missingGroupInfos)
                Log.Warn($"Group 文件丢失，请检查您的资源文件夹：/Config/LevelOutput/Group。传送，怪物战斗，和世界怪物可能无法工作！");

            Log.Info("已加载 " + GameData.FloorInfoData.Count + " 个 floor 信息");
        }
    }
} 
