using GameServer.Resources;
using GameServer.Database;
using Microsoft.Extensions.Configuration;
using GameServer.Network;
using NLog;
using NLog.Config;
using System.Xml;

namespace GameServer
{
    internal class Program
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        public static AppSettings Config { get; private set; }

        public static void Main(string[] args)
        {
            using var stringReader = new StringReader(File.ReadAllText("./NLog.config"));
            using var xmlReader = XmlReader.Create(stringReader);
            LogManager.Configuration = new XmlLoggingConfiguration(xmlReader, null);

            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            Config = config.GetSection("AppSettings").Get<AppSettings>();

            logger.Info("Cyt SR - Game Server");

            ResourceLoader.LoadGameData();
            Storage.Initialize();
            PacketFactory.LoadPacketHandlers();
            Network.GameServer.Start();

            Thread.Sleep(-1);
        }
    }
}