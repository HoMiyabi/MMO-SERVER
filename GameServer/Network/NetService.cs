using Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using GameServer.Model;
using Summer.Network;
using Serilog;
using Google.Protobuf;

namespace GameServer.Network;

public class NetService
{
    TcpServer tcpServer;
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
    }

    private void OnClientConnected(Connection conn)
    {
        IPEndPoint iPEndPoint = conn.Socket.RemoteEndPoint as IPEndPoint;
        Log.Information($"客户端连接 IP:{iPEndPoint?.Address} Port:{iPEndPoint?.Port}");
    }

    private void OnDisconnected(Connection conn)
    {
        IPEndPoint iPEndPoint = conn.Socket.RemoteEndPoint as IPEndPoint;
        Log.Information($"客户端断开 IP:{iPEndPoint?.Address} Port:{iPEndPoint?.Port}");

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