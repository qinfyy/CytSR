using System.Reflection;
using NLog;
using Proto;

namespace GameServer.Network
{
    public static class PacketFactory
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        public static readonly Dictionary<CmdId, IPacketHandler> Handlers = new Dictionary<CmdId, IPacketHandler>();

        public static void LoadPacketHandlers()
        {
            Log.Info("加载包处理器...");

            var packetHandlers = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.Namespace== "GameServer.Handlers" && t.GetCustomAttribute<PacketHandlerAttribute>() != null)
                .Select(t => new
                {
                    Type = t,
                    Attribute = t.GetCustomAttribute<PacketHandlerAttribute>()
                })
                .Where(x => !Handlers.ContainsKey(x.Attribute.Id))
                .ToList();

            ushort loadHandlerCount = 0;
            foreach (var packetHandler in packetHandlers)
            {
                Handlers.Add(packetHandler.Attribute.Id, (IPacketHandler)Activator.CreateInstance(packetHandler.Type));
#if DEBUG
                Log.Debug($"为 CmdId { packetHandler.Attribute.Id} 加载包处理器 { packetHandler.Type.Name}");
#endif
                loadHandlerCount++;
            }

            Log.Info($"已加载 {loadHandlerCount} 个包处理器");
        }

        public static IPacketHandler GetPacketHandler(CmdId cmdId)
        {
            return Handlers.TryGetValue(cmdId, out IPacketHandler handler) ? handler : null;
        }
    }
}
