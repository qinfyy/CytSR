
using System.Net;

namespace GameServer.Network.KCP
{
    public class UdpReceiveResult
    {
        public byte[] Buffer { get; set; }

        public int ReceivedBytes { get; set; }

        public IPEndPoint RemoteEndPoint { get; set; }

        public UdpReceiveResult(byte[] buffer, IPEndPoint remoteEndPoint)
        {
            Buffer = buffer;
            ReceivedBytes = buffer.Length;
            RemoteEndPoint = remoteEndPoint;
        }
    }
}