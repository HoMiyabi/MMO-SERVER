using System.Net.Sockets;
using System.Net;
using System.Text;
using Proto;
using Google.Protobuf;

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

            Vector3 v = new() { X = 100, Y = 200, Z = 300 };

            SendMessage(socket, v.ToByteArray());

            Console.ReadKey();
        }

        static void SendMessage(Socket socket, byte[] body)
        {
            byte[] lenBytes = BitConverter.GetBytes(body.Length);
            socket.Send(lenBytes);
            socket.Send(body);
        }
    }
}
