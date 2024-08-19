using System.Collections;
using System.Collections.Generic;
using Google.Protobuf;
using System.IO;
using System;
using System.Reflection;
using Google.Protobuf.Reflection;

namespace Summer
{
    /// <summary>
    /// Protobuf序列化与反序列化
    /// </summary>
    public class ProtoHelper
    {
        /// <summary>
        /// 序列化protobuf
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static byte[] Serialize(IMessage msg)
        {
            using (MemoryStream rawOutput = new MemoryStream())
            {
                msg.WriteTo(rawOutput);
                byte[] result = rawOutput.ToArray();
                return result;
            }
        }
        /// <summary>
        /// 解析
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataBytes"></param>
        /// <returns></returns>
        public static T Parse<T>(byte[] dataBytes) where T : IMessage, new()
        {
            T msg = new T();
            msg = (T)msg.Descriptor.Parser.ParseFrom(dataBytes);
            return msg;
        }

        private static Dictionary<string, Type> _registry = new();

        static ProtoHelper()
        {
            foreach (var t in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (typeof(Google.Protobuf.IMessage).IsAssignableFrom(t))
                {
                    var desc = t.GetProperty("Descriptor").GetValue(t) as MessageDescriptor;
                    _registry.Add(desc.FullName, t);
                }
            }
        }

        public static Proto.Package Pack(IMessage message)
        {
            Proto.Package package = new()
            {
                Fullname = message.Descriptor.FullName,
                Data = message.ToByteString(),
            };
            return package;
        }

        public static IMessage Unpack(Proto.Package package)
        {
            string fullname = package.Fullname;
            if (!String.IsNullOrEmpty(fullname) && _registry.TryGetValue(fullname, out Type t))
            {
                var desc = t.GetProperty("Descriptor").GetValue(t) as MessageDescriptor;
                return desc.Parser.ParseFrom(package.Data);
            }
            return null;
        }
    }
}
