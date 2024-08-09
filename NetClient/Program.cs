using System.Net.Sockets;
using System.Net;
using System.Text;
using Google.Protobuf;
using Common.Network;

namespace NetClient
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string host = "127.0.0.1";
            int port = 32510;

            var ipEndPort = new IPEndPoint(IPAddress.Parse(host), port);

            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(ipEndPort);

            Console.WriteLine("成功连接到服务器");

            Proto.Package package = new()
            {
                Request = new()
                {
                    UserLogin = new()
                    {
                        Username = "tyx",
                        Password = "123456",
                    }
                }
            };

            NetConnection conn = new(socket, null, null);
            conn.Send(package);
            conn.Send(package);
            conn.Send(package);

            Console.ReadKey();

            conn.Close();
        }

        //static void SendMessage(Socket socket, byte[] body)
        //{
        //    byte[] lenBytes = BitConverter.GetBytes(body.Length);
        //    socket.Send(lenBytes);
        //    socket.Send(body);
        //}
    }
}
