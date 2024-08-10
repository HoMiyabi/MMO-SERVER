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
            new NetConnection(socket,
                OnDataReceived,
                OnDisconnected);
        }

        private void OnDisconnected(NetConnection sender)
        {
            Console.WriteLine("断开连接");
        }

        private void OnDataReceived(NetConnection sender, byte[] data)
        {
            var package = Proto.Package.Parser.ParseFrom(data);
            MessageRouter.Instance.AddMessage(sender, package);
        }
    }
}