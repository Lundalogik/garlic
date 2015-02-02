using System;
using System.IO;

namespace Garlic
{
    public static class Logger
    {
        private static TextWriter _out = Console.Out;

        public static TextWriter Out
        {
            get { return _out; }
            set { _out = value; }
        }
        public static void ResetOut()
        {
            _out = Console.Out;
        }

        public static void Info(string message)
        {
            _out.WriteLine(message);
        }

        public static void Info(string message, params object[] f)
        {
            Info(string.Format(message, f));
        }

        public static void Error(string message)
        {
            _out.WriteLine(message);
        }

        public static void Error(string message, params object[] f)
        {
            Info(string.Format(message, f));
        }

        public static void Debug(string message)
        {
            _out.WriteLine(message);
        }

        public static void Debug(string message, params object[] f)
        {
            Info(string.Format(message, f));
        }

        public static void Error(Exception message)
        {
            Error(message.Message + Environment.NewLine + message.StackTrace);
        }
    }
}

