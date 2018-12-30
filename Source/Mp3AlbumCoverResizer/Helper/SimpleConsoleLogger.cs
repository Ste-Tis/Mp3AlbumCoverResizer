using System;

namespace Mp3AlbumCoverResizer.Helper
{
    /// <summary>
    ///     Simple logging class
    /// </summary>
    public class SimpleConsoleLogger
    {
        /// <summary>
        ///     Available logging level
        /// </summary>
        public enum LogLevels
        {
            DEBUG = 0,
            INFO = 1,
            WARNING = 2,
            ERROR = 3,
            CRITICAL = 4,
            SILENT = 100
        }

        /// <summary>
        ///     Only log messages equal or above this level
        /// </summary>
        protected LogLevels LogLevel { get; }

        /// <summary>
        ///     Initialize logger
        /// </summary>
        /// <param name="level">Only log messages equal or above this level</param>
        public SimpleConsoleLogger(LogLevels level = LogLevels.INFO)
        {
            LogLevel = level;
        }

        /// <summary>
        ///     Create new log message
        /// </summary>
        /// <param name="level">Level of message</param>
        /// <param name="message">Message to write into log</param>
        public void Log(LogLevels level, string message)
        {
            if (level >= LogLevel && LogLevel != LogLevels.SILENT)
            {
                Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} [{level}] {message}");
            }
        }

        /// <summary>
        ///     Create new debugging message
        /// </summary>
        /// <param name="message">Message to write into log</param>
        public void LogDebug(string message)
        {
            Log(LogLevels.DEBUG, message);
        }

        /// <summary>
        ///     Create new info message
        /// </summary>
        /// <param name="message">Message to write into log</param>
        public void LogInfo(string message)
        {
            Log(LogLevels.INFO, message);
        }

        /// <summary>
        ///     Create new warning message
        /// </summary>
        /// <param name="message">Message to write into log</param>
        public void LogWarning(string message)
        {
            Log(LogLevels.WARNING, message);
        }

        /// <summary>
        ///     Create new error message
        /// </summary>
        /// <param name="message">Message to write into log</param>
        public void LogError(string message)
        {
            Log(LogLevels.ERROR, message);
        }

        /// <summary>
        ///     Create new critical error message
        /// </summary>
        /// <param name="message">Message to write into log</param>
        public void LogCritical(string message)
        {
            Log(LogLevels.CRITICAL, message);
        }
    }
}
