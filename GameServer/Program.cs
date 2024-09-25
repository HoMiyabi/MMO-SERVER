using System;
using System.Collections.Generic;
using System.Threading;
using GameServer.Network;
using GameServer.Manager;
using Kirara;
using Serilog;
using Summer;

namespace GameServer
{
    internal class Program
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

            NetService netService = new();
            netService.Start();
            Log.Debug("网络服务启动完成");

            UserService userService = UserService.Instance;
            userService.Start();
            Log.Debug("玩家服务启动完成");

            SpaceService spaceService = SpaceService.Instance;
            spaceService.Start();
            Log.Debug("地图服务启动完成");

            // Schedule.Instance.Start();
            // Log.Debug("定时器启动完成");

            Console.ReadLine();
            netService.Close();
            CharacterManager.Instance.Close();
        }
    }
}