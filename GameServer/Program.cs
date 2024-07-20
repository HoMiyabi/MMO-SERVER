using GameServer.Network;
using Network;
using Proto;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace GameServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            NetService netService = new();
            netService.Init(32510);
            netService.Start();

            Console.ReadKey();
        }
    }
}