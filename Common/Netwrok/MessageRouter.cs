﻿using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Common.Network
{
    internal class MessageUnit
    {
        public NetConnection sender;
        public Google.Protobuf.IMessage message;
    }

    /// <summary>
    /// 消息分发器
    /// </summary>
    public class MessageRouter : Singleton<MessageRouter>
    {
        int threadCount = 1; // 工作线程数
        int workerCount = 0; // 正在工作的线程数
        bool running = false; // 是否在运行

        AutoResetEvent threadEvent = new(true); // 通过Set每次可以唤醒1个线程

        /// <summary>
        /// 消息队列
        /// </summary>
        private Queue<MessageUnit> messageQueue = new();

        public delegate void MessageHandler<T>(NetConnection sender, T message);

        /// <summary>
        /// 消息频道
        /// </summary>
        private Dictionary<string, Delegate> delegateMap = new();

        /// <summary>
        /// 订阅
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="handler"></param>
        public void On<T>(MessageHandler<T> handler) where T : Google.Protobuf.IMessage
        {
            string key = typeof(T).FullName;
            delegateMap.TryAdd(key, null);
            delegateMap[key] = (delegateMap[key] as MessageHandler<T>) + handler;
            Console.WriteLine("[订阅]" + key + ": " + delegateMap[key].GetInvocationList().Length);
        }

        /// <summary>
        /// 订阅
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="handler"></param>
        public void Off<T>(MessageHandler<T> handler) where T : Google.Protobuf.IMessage
        {
            string key = typeof(T).FullName;
            delegateMap.TryAdd(key, null);
            delegateMap[key] = (delegateMap[key] as MessageHandler<T>) - handler;
        }

        /// <summary>
        /// 触发
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sender"></param>
        private void Fire<T>(NetConnection sender, T msg)
        {
            string key = typeof(T).FullName;
            // CollectionsMarshal.GetValueRefOrNullRef(delegateMap, key);
            if (delegateMap.ContainsKey(key))
            {
                MessageHandler<T> handler = delegateMap[key] as MessageHandler<T>;
                try
                {
                    handler?.Invoke(sender, msg);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("MessageRouter.Fire error: " + ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// 退订
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        public void AddMessage(NetConnection sender, Google.Protobuf.IMessage message)
        {
            messageQueue.Enqueue(new()
            {
                sender = sender,
                message = message,
            });
            threadEvent.Set(); // 唤醒1个worker
        }

        /// <summary>
        /// Clamp to 1 ~ 200
        /// </summary>
        /// <param name="threadCount"></param>
        public void Start(int threadCount)
        {
            running = true;
            threadCount = Math.Clamp(threadCount, 1, 200);
            this.threadCount = threadCount;
            for (int i = 0; i < threadCount; i++)
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback((MessageWork)));
            }
            while (workerCount < threadCount)
            {
                Thread.Sleep(100);
            }
        }

        public void Stop()
        {
            running = false;
            messageQueue.Clear();
            while (workerCount > 0)
            {
                threadEvent.Set();
            }
            Thread.Sleep(100);
        }

        private void MessageWork(object state)
        {
            Console.WriteLine("worker thread start");

            try
            {
                Interlocked.Increment(ref workerCount);
                while (running)
                {
                    if (messageQueue.Count == 0)
                    {
                        threadEvent.WaitOne(); // 可以通过Set()唤醒
                        continue;
                    }

                    MessageUnit msg = messageQueue.Dequeue();
                    Google.Protobuf.IMessage package = msg.message;


                    if (package != null)
                    {
                        ExecuteMessage(msg.sender, package);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
            finally
            {
                Interlocked.Decrement(ref workerCount);
                Console.WriteLine("worker thread end");
            }
        }

        /// <summary>
        /// 递归处理消息
        /// </summary>
        /// <param name="message"></param>
        private void ExecuteMessage(NetConnection sender, Google.Protobuf.IMessage message)
        {
            GetType()
                .GetMethod("Fire", BindingFlags.NonPublic | BindingFlags.Instance)
                .MakeGenericMethod(message.GetType())
                .Invoke(this, new object[] { sender, message });

            Type t = message.GetType();
            foreach (var p in t.GetProperties())
            {
                if (p.Name == "Parser" || p.Name == "Descriptor")
                {
                    continue;
                }

                var value = p.GetValue(message);
                if (value != null)
                {
                    if (value.GetType().IsAssignableTo(typeof(Google.Protobuf.IMessage)))
                    {
                        //Console.WriteLine("发现消息，触发订阅，需要递归");
                        ExecuteMessage(sender, value as Google.Protobuf.IMessage);
                    }
                }
            }
        }

    }
}