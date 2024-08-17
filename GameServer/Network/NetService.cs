using Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Summer.Network;

namespace GameServer.Network
{
    public class NetService
    {
        private TcpSocketListener listener = null;

        public NetService() { }

        public void Init(int port)
        {
            listener = new TcpSocketListener("0.0.0.0", port);
            listener.SocketConnected += OnClientConnected;
        }

        public void Start()
        {
            listener.Start();
        }

        private void OnClientConnected(object sender, Socket socket)
        {
            IPEndPoint iPEndPoint = socket.RemoteEndPoint as IPEndPoint;
            Console.WriteLine($"[客户端连接] IP:{iPEndPoint?.Address} Port:{iPEndPoint?.Port}");

            var conn = new Connection(socket);
            conn.OnDataReceived += OnDataReceived;
            conn.OnDisconnected += OnDisconnected;
        }

        private void OnDisconnected(Connection sender)
        {
            IPEndPoint iPEndPoint = sender.socket.RemoteEndPoint as IPEndPoint;
            Console.WriteLine($"[客户端断开] IP:{iPEndPoint?.Address} Port:{iPEndPoint?.Port}");
        }

        private void OnDataReceived(Connection sender, byte[] data)
        {
            var package = Proto.Package.Parser.ParseFrom(data);
            MessageRouter.Instance.AddMessage(sender, package);
        }
    }
}