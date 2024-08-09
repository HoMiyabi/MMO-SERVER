using Common.Network;
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

            MessageRouter.Instance.Start(4);

            MessageRouter.Instance.On<Proto.UserLoginRequest>(OnUserLoginRequest);

            Console.ReadKey();
        }

        private static void OnUserLoginRequest(NetConnection sender, UserLoginRequest message)
        {
            Console.WriteLine($"发现用户登录请求:{message.Username},{message.Password}");
        }
    }
}