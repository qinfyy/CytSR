namespace SDKServer
{
    public class SDKSettings
    {
        public string DatabaseConnectString { get; set; } = "";
        public bool AutoRegister { get; set; }
        public List<DispatchServers> DispatchRegionList { get; set; } = new List<DispatchServers>();
        public GateServerConfig GateServer { get; set; } = new GateServerConfig();
    }

    public class DispatchServers
    {
        public string Name { get; set; } = "";
        public string Tiele { get; set; } = "";
        public string DispatchUrl { get; set; } = "";
        public string Env { get; set; } = "";
    }

    public class GateServerConfig
    {
        public string Ip { get; set; } = "";
        public ushort Port { get; set; }
        public string RegionName { get; set; } = "";
    }
}