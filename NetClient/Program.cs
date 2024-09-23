using System.Diagnostics;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using Google.Protobuf;
using Kirara;
using Proto;
using Serilog;

namespace NetClient
{
    internal class Program
    {
        private static Connection conn;
        private static Stopwatch stopwatch = new();

        private static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File("logs/client-log.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            Thread.Sleep(1000);

            string host = "127.0.0.1";
            int port = 32510;

            var ipEndPort = new IPEndPoint(IPAddress.Parse(host), port);

            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(ipEndPort);

            Log.Information("成功连接到服务器");

            MessageRouter.Instance.Subscribe<HeartBeatResponse>(Handler);
            MessageRouter.Instance.Start(4);

            conn = new(socket);
            conn.Received += MessageRouter.Instance.AddMessage;

            var message = new HeartBeatRequest() { };

            for (int i = 0; i < 10; i++)
            {
                stopwatch.Restart();
                conn.Send(message);
                Log.Information("Send");
                Thread.Sleep(100);
            }

            conn.Close();
            MessageRouter.Instance.Stop();
        }

        private static void Handler(Connection connection, HeartBeatResponse message)
        {
            Log.Information($"Receive {stopwatch.ElapsedMilliseconds}");
        }
    }
}
