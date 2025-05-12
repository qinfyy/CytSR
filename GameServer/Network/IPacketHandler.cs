namespace GameServer.Network
{
    public interface IPacketHandler
    {
        void Handle(GameSession session, NetPacket packet);
    }
}
