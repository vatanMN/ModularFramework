using System;
using System.IO;
using UnityEngine;

namespace ModularFW.Core.Logging
{
    public enum LogLevel { Verbose = 0, Info = 1, Warning = 2, Error = 3 }

    /// <summary>
    /// Centralized logger. Wraps Unity's Debug.Log with severity levels, consistent
    /// [Level][Tag] formatting, and optional persistent file output for post-ship debugging.
    /// File logging is enabled in Editor and development builds; disabled in release.
    /// </summary>
    public static class GameLogger
    {
        public static LogLevel MinimumLevel = LogLevel.Verbose;

        public static bool FileLoggingEnabled =
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            true;
#else
            false;
#endif

        private static StreamWriter _writer;
        private static readonly object _fileLock = new object();

        public static void Log(string tag, string message, LogLevel level = LogLevel.Info)
        {
            if (level < MinimumLevel) return;
            var line = $"[{level}][{tag}] {message}";
            switch (level)
            {
                case LogLevel.Warning: Debug.LogWarning(line); break;
                case LogLevel.Error:   Debug.LogError(line);   break;
                default:               Debug.Log(line);        break;
            }
            if (FileLoggingEnabled) WriteToFile(line);
        }

        public static void LogVerbose(string tag, string message) => Log(tag, message, LogLevel.Verbose);
        public static void LogWarning(string tag, string message) => Log(tag, message, LogLevel.Warning);
        public static void LogError(string tag, string message)   => Log(tag, message, LogLevel.Error);

        public static void Close()
        {
            lock (_fileLock) { _writer?.Close(); _writer = null; }
        }

        private static void WriteToFile(string line)
        {
            lock (_fileLock)
            {
                try
                {
                    if (_writer == null)
                    {
                        var path = Path.Combine(Application.persistentDataPath, "game_log.txt");
                        _writer = new StreamWriter(path, append: true) { AutoFlush = true };
                    }
                    _writer.WriteLine($"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss.fff} {line}");
                }
                catch { /* file logging must never crash the game */ }
            }
        }
    }
}
