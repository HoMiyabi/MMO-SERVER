using System;
using GameServer.Network;
using GameServer.Manager;
using Kirara;
using Serilog;

namespace GameServer
{
    internal static class Program
    {
        private static void InitLogger()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Async(c => c.Console())
                .WriteTo.Async(c =>
                    c.File("logs/server-log.txt", rollingInterval: RollingInterval.Day))
                .CreateLogger();
        }

        private static void Main(string[] args)
        {
            InitLogger();

            DefineManager.Instance.Init();

            var netService = new NetService();
            netService.Start();
            Log.Debug("网络服务启动完成");

            var userService = UserService.Instance;
            userService.Start();
            Log.Debug("玩家服务启动完成");

            var spaceService = SpaceService.Instance;
            spaceService.Start();
            Log.Debug("地图服务启动完成");

            var space = SpaceManager.Instance.GetSpace(2);
            var monster = space.monsterManager.CreateMonster(
                1001, 3, new Float3(79.9143f, 22.4004f, 33.5548f), new Float3(0, 0, 0), "鸡哥");
            space.AddMonster(monster);

            Console.ReadLine();

            netService.Close();
            CharacterManager.Instance.Close();

            Log.Information("Bye bye!");
        }
    }
}