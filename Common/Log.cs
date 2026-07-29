using System.IO;
using System;
using UnityEngine;
using System.Diagnostics;
using SleepyCommon;
using System.Reflection;

namespace SleepyCommon
{
    // Code based on TMPE Log.cs, for thread safe logging
    public static class Log
    {
        // -------------------------------------------------------------------------------------------
        private static readonly object LogLock = new object();
        private static string LogFilePath = "";
        private static Stopwatch s_stopWatch = Stopwatch.StartNew();

        // -------------------------------------------------------------------------------------------
        private enum LogLevel
        {
            Trace,
            Debug,
            Info,
            Warning,
            Error,
        }

        // -------------------------------------------------------------------------------------------
        static Log()
        {
            try
            {
                lock (LogLock)
                {
                    if (string.IsNullOrEmpty(LogFilePath))
                    {
                        string dir;

                        if (Application.platform != RuntimePlatform.OSXPlayer)
                        {
                            dir = Application.dataPath;
                        }
                        else
                        {
                            dir = Path.Combine(
                                Path.Combine(
                                    Path.Combine(
                                        Environment.GetFolderPath(Environment.SpecialFolder.Personal),
                                        "Library"),
                                    "Logs"),
                                "Unity");
                        }

                        LogFilePath = Path.Combine(dir, $"{Assembly.GetExecutingAssembly().GetName().Name}.log");
                    }

                    // Delete old version if found
                    if (File.Exists(LogFilePath))
                    {
                        File.Delete(LogFilePath);
                    }
                }
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogException(e);
                LogFilePath = "";
            }
        }

        // -------------------------------------------------------------------------------------------
        public static void Info(string sText)
        {
            LogToFile(sText, LogLevel.Info);
        }

        // -------------------------------------------------------------------------------------------
        public static void Error(string sText)
        {
            LogToFile(sText, LogLevel.Error);
        }

        // -------------------------------------------------------------------------------------------
        public static void Error(Exception ex)
        {
            string sException = ex.ToString();
            if (ex.InnerException is not null)
            {
                sException += $"\r\n{ex.InnerException}";
            }

            LogToFile(sException, LogLevel.Error);
        }

        // -------------------------------------------------------------------------------------------
        public static void Error(string sText, Exception ex)
        {
            LogToFile(sText, LogLevel.Error);
            Error(ex);
        }
        
        // -------------------------------------------------------------------------------------------
        public static void Warning(string sText)
        {
            LogToFile(sText, LogLevel.Warning);
        }

        // -------------------------------------------------------------------------------------------
        public static void Debug(string sText)
        {
            LogToFile(sText, LogLevel.Debug);
        }

        // -------------------------------------------------------------------------------------------
        public static void Trace(string sText)
        {
            LogToFile(sText, LogLevel.Trace);
        }

        // -------------------------------------------------------------------------------------------
        public static void Separator()
        {
            LogToFile("---------------------------------------------------------------", LogLevel.Info);
        }

        // -------------------------------------------------------------------------------------------
        private static void LogToFile(string log, LogLevel level)
        {
            lock (LogLock)
            {
                using (StreamWriter w = File.AppendText(LogFilePath))
                {
                    long secs = s_stopWatch.ElapsedTicks / Stopwatch.Frequency;
                    long fraction = s_stopWatch.ElapsedTicks % Stopwatch.Frequency;
                    w.WriteLine(
                        $"{level.ToString()} " +
                        $"{secs:n0}.{fraction:D7} | " +
                        $"{log}");

                    if (level == LogLevel.Warning || level == LogLevel.Error)
                    {
                        w.WriteLine((new System.Diagnostics.StackTrace(true)).ToString());
                        w.WriteLine();
                    }
                }
            }
        }
    }
}