namespace GameServer
{
    public class AppSettings
    {
        public string Host { get; set; } = "";
        public int Port { get; set; }
        public string DatabasePath { get; set; } = "";
        public string ResourcePath { get; set; } = "";
        public bool LogPacket { get; set; }
    }
}
