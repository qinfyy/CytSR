using Proto;

namespace GameServer.Network
{
    [AttributeUsage(AttributeTargets.Class)]
    public class PacketHandlerAttribute : Attribute
    {
        public CmdId Id { get; }

        public PacketHandlerAttribute(CmdId id)
        {
            Id = id;
        }
    }
}
