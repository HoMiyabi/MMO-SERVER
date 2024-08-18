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
        public enum Level
        {
            Debug, Info, Warning, Error
        }

        public static Level allowLevel = Level.Info; // 日志级别

        public delegate void PrintAction(string text);
        public static event PrintAction Print;

        static Log()
        {
            Print += Console.WriteLine;
        }

        private static void WriteLine(Level textLevel, string text)
        {
            if (textLevel >= allowLevel)
            {
                Print?.Invoke($"[{textLevel}] - {text}");
            }
        }

        public static void Debug(string text)
        {
            WriteLine(Level.Debug, text);
        }

        public static void Info(string text)
        {
            WriteLine(Level.Info, text);
        }

        public static void Warning(string text)
        {
            WriteLine(Level.Warning, text);
        }

        public static void Error(string text)
        {
            WriteLine(Level.Error, text);
        }
    }
}
