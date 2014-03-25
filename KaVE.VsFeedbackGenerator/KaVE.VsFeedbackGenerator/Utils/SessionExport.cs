using System;
using System.Collections.Generic;
using System.IO;

namespace KaVE.VsFeedbackGenerator.Utils
{
    public interface ISessionExport {
        ExportResult<IList<T>> Export<T>(IList<T> events, Func<string, ILogWriter<T>> writerFactory);
    }

    public class SessionExport : ISessionExport
    {
        private readonly ISessionPublisher _publisher;

        public SessionExport(ISessionPublisher publisher)
        {
            _publisher = publisher;
        }

        internal string ExportToTemporaryFile<T>(IList<T> events, Func<string, ILogWriter<T>> writer)
        {
            var tempFileName = Path.GetTempFileName();
            using (var logWriter = writer(tempFileName))
            {
                foreach (var e in events)
                {
                    logWriter.Write(e);
                }
            }
            return tempFileName;
        }

        public ExportResult<IList<T>> Export<T>(IList<T> events, Func<string, ILogWriter<T>> writerFactory)
        {
            var filename = ExportToTemporaryFile(events, writerFactory);
            return ExportResult<IList<T>>.CloneWithData(_publisher.Publish(filename), events);
        }
    }
}