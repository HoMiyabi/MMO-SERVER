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
        public delegate void DataReceivedAction(Connection sender, byte[] data);
        public delegate void DisconnectedAction(Connection sender);

        public Socket socket;

        /// <summary>
        /// [回调] 当接收到数据
        /// </summary>
        public event DataReceivedAction onDataReceived;

        /// <summary>
        /// [回调] 当断开连接
        /// </summary>
        public event DisconnectedAction onDisconnected;

        public Connection(Socket socket)
        {
            this.socket = socket;

            // 创建解码器
            var lfd = new LengthFieldDecoder(socket, 64 * 1024, 0, 4, 0, 4);
            lfd.DataReceived += data => onDataReceived?.Invoke(this, data);
            lfd.Disconnected += (_) => onDisconnected(this);
            lfd.Start(); // 启动解码器
        }

        /// <summary>
        /// 主动关闭连接
        /// </summary>
        public void Close()
        {
            try
            {
                socket.Shutdown(SocketShutdown.Both);
            }
            catch
            {

            }
            socket.Close();
            onDisconnected?.Invoke(this);
        }


        #region 发送网络数据包

        public void Send(Google.Protobuf.IMessage message)
        {
            // 我的
            int size = message.CalculateSize();
            byte[] data = new byte[4 + size];
            var ms = new MemoryStream(data);
            ms.Write(BitConverter.GetBytes(size));
            message.WriteTo(ms);
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
