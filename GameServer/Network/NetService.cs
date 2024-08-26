using System.Net;
using GameServer.Model;
using Summer.Network;
using Serilog;

namespace GameServer.Network;

public class NetService
{
    private TcpServer tcpServer;

    // 记录conn最后一次心跳包的时间
    private Dictionary<Connection, DateTime> connToLastHeartBeatTime = new();

    public NetService()
    {
        tcpServer = new("0.0.0.0", 32510);
        tcpServer.Connected += OnClientConnected;
        tcpServer.Disconnected += OnDisconnected;
        //tcpServer.DataReceived += OnDataReceived;
    }

    public void Start()
    {
        tcpServer.Start();
        MessageRouter.Instance.Start(10);

        MessageRouter.Instance.Subscribe<Proto.HeartBeatRequest>(OnHeartBeatRequest);

        Task.Run(CleanConnectionAsync);
    }

    // todo)) 有线程安全问题
    private async Task CleanConnectionAsync()
    {
        while (true)
        {
            await Task.Delay(TimeSpan.FromSeconds(5.0));

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
        // Log.Information("收到心跳包: " + conn);
        connToLastHeartBeatTime[conn] = DateTime.UtcNow;

        Proto.HeartBeatResponse response = new();
        conn.Send(response);
    }

    private void OnClientConnected(Connection conn)
    {
        connToLastHeartBeatTime[conn] = DateTime.UtcNow;

        IPEndPoint iPEndPoint = conn.Socket.RemoteEndPoint as IPEndPoint;
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

    //private void OnDataReceived(Connection conn, IMessage message)
    //{
    //    //MessageRouter.Instance.AddMessage(conn, message);
    //}
}