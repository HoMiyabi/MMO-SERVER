using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Summer
{
    public class Varint
    {

        public static byte[] VarintEncode(ulong value)
        {
            var list = new List<byte>();
            while (value > 0)
            {
                byte b = (byte)(value & 0x7f);
                value >>= 7;
                if (value > 0)
                {
                    b |= 0x80;
                }
                list.Add(b);
            }
            return list.ToArray();
        }

        //varint 解析
        public static ulong VarintDecode(byte[] buffer)
        {
            ulong value = 0;
            int shift = 0;
            int len = buffer.Length;
            for (int i = 0; i < len; i++)
            {
                byte b = buffer[i];
                value |= (ulong)(b & 0x7F) << shift;
                if ((b & 0x80) == 0)
                {
                    break;
                }
                shift += 7;
            }
            return value;
        }

        public static int VarintSize(ulong value)
        {
            //位置7位，如果前面都为0，说明只有一个有效字节
            if ((value & (0xFFFFFFFF << 7)) == 0)
            {
                return 1;
            }

            if ((value & (0xFFFFFFFF << 14)) == 0)
            {
                return 2;
            }

            if ((value & (0xFFFFFFFF << 21)) == 0)
            {
                return 3;
            }

            if ((value & (0xFFFFFFFF << 28)) == 0)
            {
                return 4;
            }
            return 5;

        }



    }
}
