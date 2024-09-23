using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using GameServer.Model;
using Kirara;
using Serilog;
// using Connection = Summer.Network.Connection;
// using MessageRouter = Summer.Network.MessageRouter;

namespace GameServer.Network
{
    public class NetService
    {
        // private TcpServer tcpServer;
        private Server server;

        // 记录conn最后一次心跳包的时间
        private Dictionary<Connection, DateTime> connToLastHeartBeatTime = new();

        private CancellationTokenSource cts = new();

        public NetService()
        {
            server = new Server("0.0.0.0", 32510);
            server.Connected += OnClientConnected;
            server.Disconnected += OnDisconnected;

            // tcpServer = new("0.0.0.0", 32510);
            // tcpServer.Connected += OnClientConnected;
            // tcpServer.Disconnected += OnDisconnected;
            //tcpServer.DataReceived += OnDataReceived;
        }

        public void Start()
        {
            // tcpServer.Start();
            server.Start();
            MessageRouter.Instance.Start(10);

            MessageRouter.Instance.Subscribe<Proto.HeartBeatRequest>(OnHeartBeatRequest);

            Task.Run(() => CleanConnectionAsync(cts.Token));
        }

        // todo)) 有线程安全问题
        private async Task CleanConnectionAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(5.0), token);
                if (token.IsCancellationRequested)
                {
                    break;
                }

                var now = DateTime.UtcNow;
                foreach (var (conn, lastTime) in connToLastHeartBeatTime)
                {
                    if ((now - lastTime).TotalSeconds > 10)
                    {
                        conn.Close();
                        connToLastHeartBeatTime.Remove(conn);
                    }
                }
            }
        }

        private void OnHeartBeatRequest(Connection conn, Proto.HeartBeatRequest message)
        {
            // Log.Debug("收到心跳包: " + conn);
            connToLastHeartBeatTime[conn] = DateTime.UtcNow;

            Proto.HeartBeatResponse response = new();
            conn.Send(response);
        }

        private void OnClientConnected(Connection conn)
        {
            connToLastHeartBeatTime[conn] = DateTime.UtcNow;

            IPEndPoint iPEndPoint = conn.socket.RemoteEndPoint as IPEndPoint;
            Log.Information($"客户端连接 IP:{iPEndPoint?.Address} Port:{iPEndPoint?.Port}");
        }

        private void OnDisconnected(Connection conn)
        {
            connToLastHeartBeatTime.Remove(conn);
            // IPEndPoint iPEndPoint = conn.Socket.RemoteEndPoint as IPEndPoint;
            // Log.Information($"客户端断开 IP:{iPEndPoint?.Address} Port:{iPEndPoint?.Port}");
            Log.Information($"客户端断开");

            var space = conn.Get<Space>();
            if (space != null)
            {
                var ch = conn.Get<Character>();
                space.CharacterLeave(conn, ch);
            }
        }

        public void Close()
        {
            cts.Cancel();
            server.Stop();
        }
    }
}