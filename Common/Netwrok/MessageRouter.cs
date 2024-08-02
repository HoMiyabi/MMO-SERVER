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
        /// 退订
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="handler"></param>
        public void Off<T>(MessageHandler<T> handler) where T : IMessage
        {
            string key = typeof(T).Name;
            delegateMap.TryAdd(key, null);
            delegateMap[key] = (delegateMap[key] as MessageHandler<T>) - handler;
        }

        public void AddMessage(NetConnection sender, IMessage message)
        {
            messageQueue.Enqueue(new()
            {
                sender = sender,
                message = message,
            });
        }
    }
}
