using Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Summer.Network;
using Common;

namespace GameServer.Network
{
    public class NetService
    {
        TcpServer tcpServer;
        public NetService()
        {
            tcpServer = new("0.0.0.0", 32510);
            tcpServer.Connected += OnClientConnected;
            tcpServer.Disconnected += OnDisconnected;
            tcpServer.DataReceived += OnDataReceived;
        }

        public void Start()
        {
            tcpServer.Start();
            MessageRouter.Instance.Start(10);
        }

        private void OnClientConnected(Connection conn)
        {
            IPEndPoint iPEndPoint = conn.Socket.RemoteEndPoint as IPEndPoint;
            Log.Info($"[客户端连接] IP:{iPEndPoint?.Address} Port:{iPEndPoint?.Port}");
        }

        private void OnDisconnected(Connection conn)
        {
            IPEndPoint iPEndPoint = conn.Socket.RemoteEndPoint as IPEndPoint;
            Log.Info($"[客户端断开] IP:{iPEndPoint?.Address} Port:{iPEndPoint?.Port}");
        }

        private void OnDataReceived(Connection conn, byte[] data)
        {
            var package = Proto.Package.Parser.ParseFrom(data);
            MessageRouter.Instance.AddMessage(conn, package);
        }
    }
}