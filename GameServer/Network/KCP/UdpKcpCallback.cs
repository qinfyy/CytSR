#define KCP_PACKET_AUDIT

using System.Buffers;
using System.Net;
using System.Net.Sockets;
using System.Net.Sockets.Kcp;

namespace GameServer.Network.KCP
{
    public class UdpKcpCallback : IKcpCallback
    {
        private readonly UdpClient udpSock;
        private readonly IPEndPoint? ipEp;

        public UdpKcpCallback(UdpClient udpSock, IPEndPoint? ipEp = null)
        {
            this.udpSock = udpSock;
            this.ipEp = ipEp;
        }

        public void Output(IMemoryOwner<byte> buffer, int avalidLength, bool isKcpPacket = true)
        {
            byte[] data = new byte[avalidLength];
            buffer.Memory.Slice(0, avalidLength).CopyTo(data);
            
            if (ipEp != null)
            {
                udpSock.Send(data, data.Length, ipEp);
            }
            else
            {
                udpSock.Send(data, data.Length);
            }
            
            buffer.Dispose();
        }
    }

}
