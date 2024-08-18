using Summer.Network;
using GameServer.Network;
using Network;
using Proto;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Common;

namespace GameServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            NetService netService = new();
            netService.Start();

            MessageRouter.Instance.On<Proto.UserLoginRequest>(OnUserLoginRequest);

            while (true)
            {
                Thread.Sleep(100);
            }
        }

        private static void OnUserLoginRequest(Connection sender, UserLoginRequest message)
        {
            Log.Info($"[用户登录] {message}");
        }
    }
}