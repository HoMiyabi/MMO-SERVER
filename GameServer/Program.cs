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
                .WriteTo.Async(c => c.Console())
                .WriteTo.Async(c
                    =>c.File("logs/server-log.txt", rollingInterval: RollingInterval.Day))
                .CreateLogger();



            NetService netService = new();
            netService.Start();
            Log.Debug("网络服务启动完成");

            UserService userService = UserService.Instance;
            userService.Start();
            Log.Debug("玩家服务启动完成");

            SpaceService spaceService = SpaceService.Instance;
            spaceService.Start();
            Log.Debug("地图服务启动完成");

            Console.ReadKey();
        }
    }
}