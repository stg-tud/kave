using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using KaVE.EventGenerator.ReSharper8.Utils.Json;
using KaVE.Model.Events;

#if !DEBUG
using System.IO.Compression;
#endif

namespace KaVE.EventGenerator.ReSharper8.SessionManager
{
    public class Session
    {
        private readonly string _logFileName;

        private DateTime _sessionStartTime;

        public Session(string logFileName)
        {
            _logFileName = logFileName;
        }

        public DateTime Date
        { 
            // TODO include date in log file name and extract it here
            get
            {
                if (_sessionStartTime == default(DateTime))
                {
                    _sessionStartTime = Events.First().StartTime;
                }
                return _sessionStartTime;
            }
        }

        public IEnumerable<SessionEvent> Events
        {
            get
            {
                var logReader = NewLogReader(_logFileName);
                return logReader.GetEnumeration<IDEEvent>().Select(evt => new SessionEvent(evt));
            }
        }

        private static JsonLogReader NewLogReader(string logFilePath)
        {
            Stream logStream = new FileStream(logFilePath, FileMode.Open);
            try
            {
#if !DEBUG
                logStream = new GZipStream(logStream, CompressionMode.Decompress);
#endif
                return new JsonLogReader(logStream);
            }
            catch (Exception)
            {
                logStream.Close();
                throw;
            }
        }
    }
}