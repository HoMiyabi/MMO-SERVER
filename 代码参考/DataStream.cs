using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Summer.Core
{
    /// <summary>
    /// 数据流
    /// 采用大端模式存储
    /// </summary>
    public class DataStream : MemoryStream
    {
        private static Queue<DataStream> pool = new Queue<DataStream>();
        public static int PoolMaxCount = 200;

        public static DataStream Allocate()
        {
            lock (pool)
            {
                if (pool.Count > 0)
                {
                    return pool.Dequeue();
                }
            }
            return new DataStream();
        }

        public static DataStream Allocate(byte[] bytes)
        {
            DataStream stream = Allocate();
            stream.Write(bytes, 0, bytes.Length);
            stream.Position = 0;
            return stream;
        }


        public ushort ReadUShort()
        {
            byte[] bytes = new byte[2];
            this.Read(bytes, 0, 2);
            if(BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            return BitConverter.ToUInt16(bytes);
        }

        public ushort ReadUShortBE()
        {
            //位运算获取大端字节序
            return (ushort)((ReadByte() << 8) | ReadByte());
        }

        protected override void Dispose(bool disposing)
        {
            Log.Information("DataStream自动释放");
            lock (pool)
            {
                if (pool.Count < PoolMaxCount)
                {
                    this.Position = 0;
                    this.SetLength(0);
                    pool.Enqueue(this);
                    Console.WriteLine("DataStream池子长度：" + pool.Count);
                }
                else
                {
                    this.Dispose(disposing);
                    this.Close();
                }
            }

        }
    }
}
