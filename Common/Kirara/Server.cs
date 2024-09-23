using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Serilog;

namespace Kirara
{
    public class Server
    {
        public event Action<Connection> Connected;
        public event Action<Connection> Disconnected;

        private readonly IPEndPoint endPoint;
        private readonly Socket serverSocket;
        private readonly CancellationTokenSource cts = new();

        public Server(string host, int port)
        {
            endPoint = new IPEndPoint(IPAddress.Parse(host), port);
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void Start()
        {
            serverSocket.Bind(endPoint);
            serverSocket.Listen();
            Log.Information($"开始监听 {endPoint.Address}:{endPoint.Port}");

            Accept(cts.Token);
        }

        private async Task Accept(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    var client = await serverSocket.AcceptAsync(token);
                    var conn = new Connection(client);
                    Connected?.Invoke(conn);
                    conn.Received += MessageRouter.Instance.AddMessage;
                    conn.Disconnected += Disconnected;
                }
                catch (OperationCanceledException e)
                {
                }
            }
        }

        public void Stop()
        {
            MessageRouter.Instance.Stop();
            cts.Cancel();
            serverSocket.Close();
        }
    }
}