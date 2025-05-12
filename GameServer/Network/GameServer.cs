using System.Collections.Concurrent;
using System.Net;
using GameServer.Network.KCP;
using NLog;

namespace GameServer.Network
{
    public static class GameServer
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private static int _sessionIdCounter = 0;
        private static KCPServer _kcpServer;
        private static CancellationTokenSource _cts = new();
        private static ConcurrentDictionary<int, GameSession> _sessions = new();

        public static void Start()
        {
            try
            {
                var host = Program.Config.Host;
                var port = Program.Config.Port;

                IPEndPoint ipep = new(IPAddress.Parse(host), port);
                _kcpServer = new KCPServer(ipep);
                log.Info($"游戏服务器正在监听 {ipep}");

                // 启动接收客户端连接的任务
                Task.Run(() => AcceptConnectionsAsync(_cts.Token), _cts.Token);
            }
            catch (Exception ex)
            {
                log.Error($"启动服务器失败: {ex}");
            }
        }

        private static async Task AcceptConnectionsAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    var result = await Task.Run(() => _kcpServer.Accept(), token);

                    if (result == null)
                        continue;

                    log.Info($"客户端 {result.RemoteEndpoint} 已连接");

                    var sessionId = Interlocked.Increment(ref _sessionIdCounter);
                    var session = new GameSession(result.Connection, sessionId);
                    _sessions[sessionId] = session;

                    // 启动会话处理线程
                    _ = Task.Run(() =>
                    {
                        session.ClientLoop();
                        _sessions.TryRemove(sessionId, out var removedSession);
                        log.Info($"客户端 {result.RemoteEndpoint} 已断开连接");
                    }, token);
                }
                catch (OperationCanceledException) when (token.IsCancellationRequested)
                {
                    log.Info("服务器停止接受新连接");
                }
                catch (Exception ex)
                {
                    log.Error($"客户端连接处理异常: {ex}");
                }
            }
        }

        public static void Stop()
        {
            _cts.Cancel();
            _kcpServer?.Close();
            log.Info("游戏服务器已停止");
        }
    }
}