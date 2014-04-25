using System;
using System.Collections.Generic;

namespace KaVE.VsFeedbackGenerator.Utils.Logging
{
    public interface ILog<TLogEntry> where TLogEntry : class
    {
        DateTime Date { get; }
        ILogReader<TLogEntry> NewLogReader();
        ILogWriter<TLogEntry> NewLogWriter();
        void Remove(TLogEntry entry);
        void RemoveRange(IEnumerable<TLogEntry> entries);
        void Delete();
    }

    public interface ILogManager<TLogEntry> where TLogEntry : class
    {
        string BaseLocation { get; }
        IEnumerable<ILog<TLogEntry>> GetLogs();
        ILog<TLogEntry> CurrentLog { get; }
        void DeleteLogsOlderThan(DateTime time);

    }
}