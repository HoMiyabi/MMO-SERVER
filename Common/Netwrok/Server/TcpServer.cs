using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace Summer.Network
{
    /// <summary>
    /// 负责监听TCP网络端口，异步接收Socket连接
    /// Connected 有新的连接
    /// DataReceived 有新的消息
    /// Disconnected 有连接断开
    /// IsRunning 是否正在运行
    /// Stop() 关闭服务器
    /// Start() 启动服务器
    /// </summary>
    public class TcpServer
    {
        private IPEndPoint endPoint;
        private Socket serverSocket;    //服务端监听对象
        private int backlog = 100;

        public event EventHandler<Socket> SocketConnected; //客户端接入事件

        public delegate void ConnectedAction(Connection conn);
        public delegate void DataReceivedAction(Connection conn, byte[] data);
        public delegate void DisconnectedAction(Connection conn);

        public event ConnectedAction Connected;
        public event DataReceivedAction DataReceived;
        public event DisconnectedAction Disconnected;

        public TcpServer(string host, int port)
        {
            endPoint = new IPEndPoint(IPAddress.Parse(host), port);
        }

        public TcpServer(string host, int port, int backlog): this(host, port)
        {
            this.backlog = backlog;
        }

        public void Start()
        {
            if (!IsRunning)
            {
                serverSocket = new Socket(
                    AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                serverSocket.Bind(endPoint);
                serverSocket.Listen(backlog);
                Log.Info("开始监听端口：" + endPoint.Port);
                
                SocketAsyncEventArgs args = new();
                args.Completed += OnAccept; //当有人连入的时候
                serverSocket.AcceptAsync(args);
            }
            else
            {
                Log.Info("TcpServer is already running.");
            }
        }

        private void OnAccept(object sender, SocketAsyncEventArgs e)
        {
            Socket client = e.AcceptSocket; //连入的人

            //继续接收下一位
            e.AcceptSocket = null;
            serverSocket.AcceptAsync(e);

            // 真的有人连进来
            if (e.SocketError == SocketError.Success)
            {
                if (client != null)
                {
                    OnSocketConnected(client);
                }
            }
        }

        private void OnSocketConnected(Socket socket)
        {
            SocketConnected?.Invoke(this, socket);
            Connection conn = new(socket);
            conn.OnDataReceived += (conn, data) => DataReceived?.Invoke(conn, data);
            conn.OnDisconnected += conn => Disconnected?.Invoke(conn);
            Connected?.Invoke(conn);
        }

        public bool IsRunning
        {
            get { return serverSocket != null; }
        }

        public void Stop()
        {
            if (serverSocket == null)
                return;
            serverSocket.Close();
            serverSocket = null;
        }
    }
}
