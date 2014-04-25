using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Util;
using KaVE.JetBrains.Annotations;

namespace KaVE.VsFeedbackGenerator.Utils.Logging
{
    public class LogFileManager<TLogEntry> : ILogManager<TLogEntry> where TLogEntry : class
    {
        internal const string LogDirectoryPrefix = "Log_";

        public LogFileManager([NotNull] string baseLocation)
        {
            BaseLocation = baseLocation;
            Directory.CreateDirectory(BaseLocation);
        }

        public string BaseLocation { get; private set; }

        public IEnumerable<ILog<TLogEntry>> GetLogs()
        {
            return Directory.GetFiles(BaseLocation, LogDirectoryPrefix + "*").Select(CreateLogFile);
        }

        private static LogFile<TLogEntry> CreateLogFile(string logDirectoryPath)
        {
            return new LogFile<TLogEntry>(logDirectoryPath);
        }

        public void DeleteLogsOlderThan(DateTime time)
        {
            GetLogs().Where(log => log.Date < time).ForEach(log => log.Delete());
        }

        public ILog<TLogEntry> TodaysLog
        {
            get
            {
                var fileName = LogDirectoryPrefix + DateTime.Today.ToString("yyyy-MM-dd");
                var logPath = Path.Combine(BaseLocation, fileName);
                return CreateLogFile(logPath);
            }
        }

        public override string ToString()
        {
            return "LogFileManager[" + BaseLocation + "]";
        }
    }
}