using Summer.Network;
using GameServer.Network;
using Network;
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

            UserService userService = new();
            userService.Start();
            Log.Debug("玩家服务启动完成");

            MessageRouter.Instance.Subscribe<Proto.UserLoginRequest>(OnUserLoginRequest);

            //while (true)
            //{
            //    Thread.Sleep(100);
            //}
            Console.ReadKey();
            // 修正提交测试
        }

        private static void OnUserLoginRequest(Connection sender, Proto.UserLoginRequest message)
        {
            Log.Information($"用户登录 {message}");
        }
    }
}