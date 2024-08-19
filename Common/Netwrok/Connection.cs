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
using System.Buffers.Binary;

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

        private static Proto.Package Protocol_AddRoute(IMessage message)
        {
            return ProtoHelper.Pack(message);
        }

        private static byte[] Protocol_AddHeader(Proto.Package package)
        {
            int size = package.CalculateSize();
            byte[] sizeBytes = BitConverter.GetBytes(size);

            if (BitConverter.IsLittleEndian) // 转大端
            {
                Array.Reverse(sizeBytes);
            }

            byte[] buffer = new byte[4 + size];
            var ms = new MemoryStream(buffer);

            ms.Write(sizeBytes);
            package.WriteTo(ms);

            return buffer;
        }

        private static byte[] Protocol_Pack(IMessage message)
        {
            Proto.Package package = Protocol_AddRoute(message);
            byte[] buffer = Protocol_AddHeader(package);
            return buffer;
        }

        public void Send(Google.Protobuf.IMessage message)
        {
            byte[] buffer = Protocol_Pack(message);
            SocketSend(buffer, 0, buffer.Length);
        }

        public void SocketSend(byte[] buffer, int offset, int size)
        {
            lock (this)
            {
                if (Socket.Connected)
                {
                    Socket.BeginSend(buffer, offset, size, SocketFlags.None, SendCallback, Socket);
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
