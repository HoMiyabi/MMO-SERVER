using Summer.Network;
using GameServer.Network;
using Network;
using Proto;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Serilog;

namespace GameServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File("logs/server-log.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            NetService netService = new();
            netService.Start();
            Log.Debug("服务器启动完成");

            MessageRouter.Instance.On<Proto.UserLoginRequest>(OnUserLoginRequest);

            while (true)
            {
                Thread.Sleep(100);
            }
        }

        private static void OnUserLoginRequest(Connection sender, UserLoginRequest message)
        {
            Log.Information($"[用户登录] {message}");
        }
    }
}