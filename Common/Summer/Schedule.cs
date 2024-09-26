/*using Serilog;
using Summer;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Kirara;

namespace Summer
{
    
    public class Schedule : Singleton<Schedule>
    {

        private List<Task> tasks = new List<Task>();
        private Thread thread;
        private int fps = 100; // 每秒帧数

        public Schedule()
        {
            
        }

        

        public Schedule Start()
        {
            if(thread == null)
            {
                thread = new Thread(Run);
            }
            thread?.Start();
            return this;
        }   

        public Schedule Stop()
        {
            thread?.Abort();
            return this;
        }

        private void Run()
        {
            RunLoop();
        }


        public void AddTask(Action taskMethod, float seconds, int repeatCount = 0)
        {
            this.AddTask(taskMethod, (int)(seconds * 1000), TimeUnit.Milliseconds, repeatCount);
        }

        public void AddTask(Action taskMethod, int timeValue, TimeUnit timeUnit, int repeatCount = 0)
        {
            int interval = GetInterval(timeValue, timeUnit);
            long startTime = GetCurrentTime() + interval;
            Task task = new Task(taskMethod, startTime, interval, repeatCount);
            tasks.Add(task);
        }

        public void RemoveTask(Action taskMethod)
        {
            Task taskToRemove = tasks.Find(task => task.TaskMethod == taskMethod);
            if (taskToRemove != null)
            {
                tasks.Remove(taskToRemove);
            }
        }


        /// <summary>
        /// 计时器主循环
        /// </summary>
        private void RunLoop()
        {
            // tick间隔
            int interval = 1000 / fps; 
            // 开始循环
            while (true)
            {
                Time.Tick();
                long startTime = GetCurrentTime();
                // 把完毕的任务移除
                List<Task> tasksToRemove = tasks.FindAll(task => task.Completed);
                foreach (Task task in tasksToRemove)
                {
                    tasks.Remove(task);
                }
                // 执行任务
                foreach (Task task in tasks)
                {
                    if (task.ShouldRun())
                    {
                        task.Run();
                    }
                }
                // 控制周期
                long endTime = GetCurrentTime();
                int msTime = (int)(interval - (endTime - startTime));
                if(msTime > 0)
                {
                    Thread.Sleep(msTime); // Sleep for millisecond
                }
                
            }
        }

        public static long GetCurrentTime()
        {
            // 获取从1970年1月1日午夜（也称为UNIX纪元）到现在的毫秒数
            return DateTimeOffset.Now.ToUnixTimeMilliseconds();
        }

        private int GetInterval(int timeValue, TimeUnit timeUnit)
        {
            switch (timeUnit)
            {
                case TimeUnit.Milliseconds:
                    return timeValue;
                case TimeUnit.Seconds:
                    return timeValue * 1000;
                case TimeUnit.Minutes:
                    return timeValue * 1000 * 60;
                case TimeUnit.Hours:
                    return timeValue * 1000 * 60 * 60;
                case TimeUnit.Days:
                    return timeValue * 1000 * 60 * 60 * 24;
                default:
                    throw new ArgumentException("Invalid time unit.");
            }
        }

        private class Task
        {
            public Action TaskMethod { get; }
            public long StartTime { get; }
            public long Interval { get; }
            public int RepeatCount { get; }

            private int currentCount;

            private long lastTick = 0; //上一次执行开始的时间

            public bool Completed = false; //是否已经执行完毕

            public Task(Action taskMethod, long startTime, long interval, int repeatCount)
            {
                TaskMethod = taskMethod;
                StartTime = startTime;
                Interval = interval;
                RepeatCount = repeatCount;
                currentCount = 0;
            }

            public bool ShouldRun()
            {
                if (currentCount == RepeatCount && RepeatCount != 0)
                {
                    Log.Information("RepeatCount={0}", RepeatCount);
                    return false;
                }

                long now = GetCurrentTime();
                if (now >= StartTime && (now - lastTick) >= Interval)
                {
                    return true;
                }

                return false;
            }

            public void Run()
            {
                lastTick = GetCurrentTime();
                try
                {
                    TaskMethod.Invoke();
                }catch (Exception ex)
                {
                    Log.Error("Schedule has Error:{0}",ex.Message);
                    return;
                }
                

                currentCount++;

                if (currentCount == RepeatCount && RepeatCount != 0)
                {
                    Console.WriteLine("Task completed.");
                    Completed = true;
                }
            }
        }
    }

    public enum TimeUnit
    {
        Milliseconds,
        Seconds,
        Minutes,
        Hours,
        Days
    }

    public class Time
    {
        /// <summary>
        /// 获取上一帧运行所用的时间
        /// </summary>
        public static float deltaTime { get; private set; }

        // 记录最后一次tick的时间
        private static long lastTick = 0;

        /// <summary>
        /// 由Schedule调用，请不要自行调用，除非你知道自己在做什么！！！
        /// </summary>
        public static void Tick()
        {
            long now = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            if (lastTick == 0) lastTick = now;
            deltaTime = (now - lastTick) * 0.001f;
            lastTick = now;
        }
    }
}*/