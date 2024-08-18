using System.Net.Sockets;
using System.Net;
using System.Text;
using Google.Protobuf;
using Summer.Network;
using Serilog;

namespace NetClient
{
    internal class Program
    {
        static private Connection conn;
        static private void Main(string[] args)
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

            conn = new(socket);

            Proto.Package package = new()
            {
                Request = new()
                {
                    UserLogin = new()
                    {
                        Username = String.Empty,
                        Password = String.Empty,
                    }
                }
            };
            package.Request.UserLogin.Username = "kirara";
            package.Request.UserLogin.Password = "pwd";
            conn.Send(package);


            Console.ReadKey();
            conn.Close();
        }

        static private void SendRequest(Google.Protobuf.IMessage message)
        {
            Proto.Package package = new()
            {
                Request = new(),
            };

            foreach (var p in typeof(Proto.Request).GetProperties())
            {
                if (p.PropertyType == message.GetType())
                {
                    p.SetValue(package, message);
                    conn.Send(package);
                    return;
                }
            }
        }
    }
}
