using Common;
using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Common.Network
{
    internal class MessageUnit
    {
        public NetConnection sender;
        public IMessage message;
    }

    /// <summary>
    /// 消息分发器
    /// </summary>
    public class MessageRouter : Singleton<MessageRouter>
    {
        int threadCount = 1;
        int workerCount = 0;

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
        public void On<T>(MessageHandler<T> handler) where T : IMessage
        {
            string key = typeof(T).Name;
            delegateMap.TryAdd(key, null);
            delegateMap[key] = (delegateMap[key] as MessageHandler<T>) + handler;
            Console.WriteLine(key + ": " + delegateMap[key].GetInvocationList().Length);
        }

        /// <summary>
        /// 订阅
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="handler"></param>
        public void Off<T>(MessageHandler<T> handler) where T : IMessage
        {
            string key = typeof(T).Name;
            delegateMap.TryAdd(key, null);
            delegateMap[key] = (delegateMap[key] as MessageHandler<T>) - handler;
        }

        /// <summary>
        /// 退订
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        public void AddMessage(NetConnection sender, IMessage message)
        {
            messageQueue.Enqueue(new()
            {
                sender = sender,
                message = message,
            });
        }

        /// <summary>
        /// Clamp to 1 ~ 200
        /// </summary>
        /// <param name="threadCount"></param>
        public void Start(int threadCount)
        {
            this.threadCount = Math.Clamp(threadCount, 1, 200);
            for (int i = 0; i < this.threadCount; i++)
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback((MessageWork)));
            }
            while (workerCount < this.threadCount)
            {
                Thread.Sleep(100);
            }
        }

        private void MessageWork(object state)
        {
            Console.WriteLine("worker thread start");

            try
            {
                Interlocked.Increment(ref workerCount);
                while (true)
                {

                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                Interlocked.Decrement(ref workerCount);
                Console.WriteLine("worker thread end");
            }   
        }
    }
}
