using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Log
    {
        public const int DEBUG = 0; // 调试
        public const int INFO  = 1; // 普通
        public const int WARN  = 2; // 警告 
        public const int ERROR = 3; // 错误

        public static int level = INFO; // 日志级别

        public delegate void PrintAction(string text);
        public static event PrintAction Print;

        public static void Debug(string text)
        {
            Print?.Invoke(text);
        }

        public static void Info(string text)
        {
            if (level <= INFO)
            {
                Print?.Invoke(text);
            }
        }

        public static void Warn(string text)
        {
            if (level <= WARN)
            {
                Print?.Invoke(text);
            }
        }

        public static void Error(string text)
        {
            if (level <= ERROR)
            {
                Print?.Invoke(text);
            }
        }
    }
}
