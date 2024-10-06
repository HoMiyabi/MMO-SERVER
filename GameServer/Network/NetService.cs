using System;
using System.Collections.Concurrent;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using GameServer.Database;
using GameServer.Manager;
using GameServer.Model;
using Kirara;
using Proto;
using Serilog;

namespace GameServer.Network
{
    public class NetService
    {
        private Server server;

        // 记录conn最后一次心跳包的时间
        private ConcurrentDictionary<Connection, DateTime> connToLastHeartBeatTime = new();

        private CancellationTokenSource cts = new();

        private readonly byte[] heartBeatResponseSendBytes;

        public NetService()
        {
            server = new Server("0.0.0.0", 32510);
            server.Connected += OnClientConnected;
            server.Disconnected += OnDisconnected;

            heartBeatResponseSendBytes = Connection.GetSendBytes(new HeartBeatResponse());
        }

        public void Start()
        {
            server.Start();

            MessageRouter.Instance.Subscribe<HeartBeatRequest>(OnHeartBeatRequest);

            Task.Run(() => CleanConnectionAsync(cts.Token));
        }

        private async Task CleanConnectionAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(5.0), token);
                }
                catch (TaskCanceledException)
                {
                    break;
                }

                var now = DateTime.UtcNow;
                foreach (var (conn, lastTime) in connToLastHeartBeatTime)
                {
                    if ((now - lastTime).TotalSeconds > 10)
                    {
                        conn.Close();
                        if (!connToLastHeartBeatTime.TryRemove(conn, out _))
                        {
                            Log.Warning($"删除失败 {conn.Get<DbPlayer>().Id} {lastTime}");
                        }
                    }
                }
            }
        }

        private void OnHeartBeatRequest(Connection conn, HeartBeatRequest message)
        {
            connToLastHeartBeatTime[conn] = DateTime.UtcNow;
            conn.SendBytes(heartBeatResponseSendBytes);
        }

        private void OnClientConnected(Connection conn)
        {
            connToLastHeartBeatTime[conn] = DateTime.UtcNow;

            IPEndPoint iPEndPoint = conn.socket.RemoteEndPoint as IPEndPoint;
            Log.Information($"客户端连接 {iPEndPoint?.Address}:{iPEndPoint?.Port}");
        }

        private void OnDisconnected(Connection conn)
        {
            if (!connToLastHeartBeatTime.TryRemove(conn, out var lastTime))
            {
                Log.Warning($"删除失败 {conn.Get<DbPlayer>().Id} {lastTime.NameValue()}");
            }
            // IPEndPoint iPEndPoint = conn.Socket.RemoteEndPoint as IPEndPoint;
            // Log.Information($"客户端断开 IP:{iPEndPoint?.Address} Port:{iPEndPoint?.Port}");
            Log.Information("客户端断开");

            var character = conn.Get<Character>();
            if (character != null)
            {
                var space = character.space;
                if (space != null)
                {
                    space.CharacterLeave(conn, character);
                }
                CharacterManager.Instance.RemoveCharacter(character.id);
            }
        }

        public void Close()
        {
            cts.Cancel();
            server.Stop();
        }
    }
}