using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace SleepyCommon
{
    public static class Log
    {
        private enum LogLevel
        {
            Trace,
            Debug,
            Info,
            Warning,
            Error
        }

        private static readonly object LogLock;

        private static string LogFilePath;

        private static Stopwatch s_stopWatch;

        static Log()
        {
            LogLock = new object();
            LogFilePath = "";
            s_stopWatch = Stopwatch.StartNew();
            try
            {
                lock (LogLock)
                {
                    if (string.IsNullOrEmpty(LogFilePath))
                    {
                        string path = ((Application.platform == RuntimePlatform.OSXPlayer) ? Path.Combine(Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Library"), "Logs"), "Unity") : Application.dataPath);
                        LogFilePath = Path.Combine(path, Assembly.GetExecutingAssembly().GetName().Name + ".log");
                    }
                    if (File.Exists(LogFilePath))
                    {
                        File.Delete(LogFilePath);
                    }
                }
            }
            catch (Exception exception)
            {
                UnityEngine.Debug.LogException(exception);
                LogFilePath = "";
            }
        }

        public static void Info(string sText)
        {
            LogToFile(sText, LogLevel.Info);
        }

        public static void Error(string sText)
        {
            LogToFile(sText, LogLevel.Error);
        }

        public static void Error(Exception ex)
        {
            string text = ex.ToString();
            if (ex.InnerException != null)
            {
                text += $"\r\n{ex.InnerException}";
            }
            LogToFile(text, LogLevel.Error);
        }

        public static void Error(string sText, Exception ex)
        {
            LogToFile(sText, LogLevel.Error);
            Error(ex);
        }

        public static void Warning(string sText)
        {
            LogToFile(sText, LogLevel.Warning);
        }

        public static void Debug(string sText)
        {
            LogToFile(sText, LogLevel.Debug);
        }

        public static void Trace(string sText)
        {
            LogToFile(sText, LogLevel.Trace);
        }

        public static void Separator()
        {
            LogToFile("---------------------------------------------------------------", LogLevel.Info);
        }

        private static void LogToFile(string log, LogLevel level)
        {
            lock (LogLock)
            {
                using StreamWriter streamWriter = File.AppendText(LogFilePath);
                long num = s_stopWatch.ElapsedTicks / Stopwatch.Frequency;
                long num2 = s_stopWatch.ElapsedTicks % Stopwatch.Frequency;
                streamWriter.WriteLine(level.ToString() + " " + $"{num:n0}.{num2:D7} | " + log);
                if (level == LogLevel.Warning || level == LogLevel.Error)
                {
                    streamWriter.WriteLine(new StackTrace(fNeedFileInfo: true).ToString());
                    streamWriter.WriteLine();
                }
            }
        }
    }
}
