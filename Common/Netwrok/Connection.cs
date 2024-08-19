using Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf;
using System.IO;

namespace Summer.Network
{
    /// <summary>
    /// 通用网络连接
    /// 职责：发送消息，接收消息回调，关闭连接，断开通知回调
    /// </summary>
    public class Connection
    {
        public delegate void DataReceivedAction(Connection sender, IMessage data);
        public delegate void DisconnectedAction(Connection sender);

        public Socket Socket { get; private set; }

        /// <summary>
        /// [回调] 当接收到数据
        /// </summary>
        public event DataReceivedAction OnDataReceived;

        /// <summary>
        /// [回调] 当断开连接
        /// </summary>
        public event DisconnectedAction OnDisconnected;

        public Connection(Socket socket)
        {
            this.Socket = socket;

            // 创建解码器
            var lfd = new LengthFieldDecoder(socket, 64 * 1024, 0, 4, 0, 4);
            lfd.DataReceived += Received;
            lfd.Disconnected += (_) => OnDisconnected?.Invoke(this);
            lfd.Start(); // 启动解码器
        }

        /// <summary>
        /// 主动关闭连接
        /// </summary>
        public void Close()
        {
            try
            {
                Socket.Shutdown(SocketShutdown.Both);
            }
            catch
            {

            }
            Socket.Close();
            OnDisconnected?.Invoke(this);
        }


        #region 发送网络数据包

        public void Send(Google.Protobuf.IMessage message)
        {
            Proto.Package package = ProtoHelper.Pack(message);
            int size = package.CalculateSize();
            byte[] sizeBytes = BitConverter.GetBytes(size);

            // 转大端
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(sizeBytes);
            }

            byte[] data = new byte[4 + size];
            var ms = new MemoryStream(data);

            ms.Write(sizeBytes);
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
                if (Socket.Connected)
                {
                    Socket.BeginSend(data, offset, count, SocketFlags.None, SendCallback, Socket);
                }
            }
        }

        private void SendCallback(IAsyncResult ar)
        {
            // 发送的字节数
            Socket client = (Socket)ar.AsyncState;
            int len = client.EndSend(ar);
        }

        private void Received(byte[] data)
        {
            var package = Proto.Package.Parser.ParseFrom(data);
            var message = ProtoHelper.Unpack(package);

            OnDataReceived?.Invoke(this, message);
        }

        #endregion
    }
}
