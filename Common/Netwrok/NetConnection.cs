using Network;
using Proto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Common.Network
{
    /// <summary>
    /// 客户端网络连接
    /// 职责：发送消息，接收消息回调，关闭连接，断开通知回调
    /// </summary>
    public class NetConnection
    {
        public delegate void DataReceivedCallback(NetConnection sender, byte[] data);
        public delegate void DisconnectedCallback(NetConnection sender);

        public Socket socket;
        private DataReceivedCallback dataReceivedCallback;
        private DisconnectedCallback disconnectedCallback;

        public NetConnection(Socket socket, DataReceivedCallback dataReceivedCallback, DisconnectedCallback disconnectedCallback)
        {
            this.socket = socket;
            this.dataReceivedCallback = dataReceivedCallback;
            this.disconnectedCallback = disconnectedCallback;

            var lfd = new LengthFieldDecoder(socket, 64 * 1024, 0, 4, 0, 4);
            lfd.DataReceived += OnDataReceived;
            lfd.Disconnected += OnDisconnected;
            lfd.Start();
        }

        public void Close()
        {
            try
            {
                socket.Shutdown(SocketShutdown.Both);
            } catch { }
            socket.Close();
            disconnectedCallback(this);
        }

        private void OnDataReceived(byte[] buffer)
        {
            dataReceivedCallback(this, buffer);
        }

        private void OnDisconnected(Socket _)
        {
            disconnectedCallback(this);
        }
    }
}
