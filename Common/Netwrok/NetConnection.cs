using Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf;

namespace Summer.Network
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

        #region 发送网络数据包

        private Proto.Package _package = null;

        public Proto.Request Request
        {
            get
            {
                if (_package == null)
                {
                    _package = new();
                }
                if (_package.Request == null)
                {
                    _package.Request = new();
                }
                return _package.Request;
            }
        }

        public Proto.Response Response
        {
            get
            {
                if (_package == null)
                {
                    _package = new();
                }
                if (_package.Response == null)
                {
                    _package.Response = new();
                }
                return _package.Response;
            }
        }

        public void Send()
        {
            if (_package != null)
            {
                Send(_package);
                _package = null;
            }
        }

        public void Send(Proto.Package package)
        {
            // 我的
            int size = package.CalculateSize();
            byte[] data = new byte[4 + size];

            var ms = new MemoryStream(data);
            ms.Write(BitConverter.GetBytes(size));
            package.WriteTo(ms);
            Send(data, 0, data.Length);

            // 视频
            //byte[] data = null;
            //using (var ms = new MemoryStream())
            //{
            //    package.WriteTo(ms);
            //    data = new byte[4 + ms.Length];
            //    Buffer.BlockCopy(BitConverter.GetBytes(ms.Length), 0, data, 0, 4);
            //    Buffer.BlockCopy(ms.GetBuffer(), 0, data, 4, (int)ms.Length);
            //}
            //Send(data, 0, data.Length);
        }

        public void Send(byte[] data, int offset, int count)
        {
            lock (this)
            {
                if (socket.Connected)
                {
                    socket.BeginSend(data, offset, count, SocketFlags.None, SendCallback, socket);
                }
            }
        }

        private void SendCallback(IAsyncResult ar)
        {
            // 发送的字节数
            Socket client = (Socket)ar.AsyncState;
            int len = client.EndSend(ar);
        }

        #endregion
    }
}
