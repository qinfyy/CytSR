using System.Net;
using System.Net.Sockets;
using NLog;

namespace GameServer.Network.KCP
{
    public class KCPClient : IDisposable
    {
        public static readonly Logger Log = LogManager.GetCurrentClassLogger();
        public bool Closed { get { return _Closed; } }

        protected UdpClient udpSock;
        private bool _Closed = false;
        protected MhyKcpBase server;
        protected EndPoint remoteAddress;

        public KCPClient(EndPoint ipEp)
        {
            if (ipEp is IPEndPoint ipEndPoint)
            {
                udpSock = new UdpClient();
                udpSock.Connect(ipEndPoint);
                server = new MhyKcpBase();

                remoteAddress = ipEp;

                server.Timeout = 10000;
                server.OutputCallback = new UdpKcpCallback(udpSock);

                Task.Run(BackgroundUpdate);
            }
            else
            {
                throw new ArgumentException("EndPoint must be IPEndPoint", nameof(ipEp));
            }
        }

        /// <summary>
        /// Invoke when server requested disconnect. uints are Conv/Token.
        /// </summary>
        public event Action<uint, uint, uint>? StartDisconnected;

        public MhyKcpBase.ConnectionState State { get => server.State; }

        protected virtual async Task BackgroundUpdate()
        {
            while (!_Closed)
            {
                try
                {
                    IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
                    byte[] receivedBytes = udpSock.Receive(ref remoteEP);
                    var packet = new UdpReceiveResult(receivedBytes, remoteEP);
                    
                    if (packet.RemoteEndPoint.ToString() == remoteAddress.ToString())
                    {
                        //Log.Debug($"Client packet (ip {remoteAddress}), buf = {Convert.ToHexString(packet)}");
                        server.Input(packet.Buffer);
                    }
                    else
                    {
                        // Log.Warn($"Bad packet sent to client: {fromip}, buf = {Convert.ToHexString(packet)}");
                    }
                    if (server.State == MhyKcpBase.ConnectionState.CLOSED)
                    {
                        if (packet.Buffer.Length == 20)
                        {
                            try
                            {
                                Handshake disconnpkt = new();
                                disconnpkt.Decode(packet.Buffer);
                                StartDisconnected?.Invoke(disconnpkt.Conv, disconnpkt.Token, disconnpkt.Data);
                            }
                            catch {  }
                        }
                    }
                    if (server.State != MhyKcpBase.ConnectionState.CONNECTED)
                    {
                        _Closed = true;
                        server.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("Update fail: " + ex);
                    _Closed = true;
                    server.Dispose();
                }
                
                // 添加短暂延迟以避免CPU占用过高
                await Task.Delay(1);
            }
        }

        public async Task ConnectAsync()
        {
            server.AcceptNonblock();
            await server.ConnectAsync();
        }

        public async Task SendAsync(byte[] data)
        {
            await server.SendAsync(data);
        }

        public void Send(ReadOnlySpan<byte> data)
        {
            server.Send(data);
        }

        public void Close()
        {
            _Closed = true;
            udpSock.Close();
        }

        public void Dispose()
        {
            _Closed = true;
            udpSock.Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>null if disconnected</returns>
        public byte[]? Receive()
        {
            try
            {
                return server.Receive();
            }
            catch
            {
                if (server.State == MhyKcpBase.ConnectionState.CONNECTED) throw;
                else return null;
            }
        }

        public void Disconnect(uint conv = 0, uint token = 0, uint data = 1)
        {
            server.Disconnect(conv, token, data);
        }
    }
}
