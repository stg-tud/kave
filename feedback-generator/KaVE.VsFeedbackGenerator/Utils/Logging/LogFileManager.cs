/*
 * Copyright 2014 Technische Universität Darmstadt
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *    http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 * 
 * Contributors:
 *    - Sven Amann
 *    - Uli Fahrer
 */

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using KaVE.Commons.Utils.IO;
using KaVE.JetBrains.Annotations;

namespace KaVE.VsFeedbackGenerator.Utils.Logging
{
    public class LogFileManager : ILogManager
    {
        internal const string LogDirectoryPrefix = "Log_";

        public event LogEventHandler LogCreated = delegate { };

        private readonly IIoUtils _ioUtils;
        private IDictionary<string, ILog> _logs;

        public LogFileManager([NotNull] string baseLocation)
        {
            BaseLocation = baseLocation;
            _ioUtils = Registry.GetComponent<IIoUtils>();
            _logs = new Dictionary<string, ILog>();
        }

        public string BaseLocation { get; private set; }

        public IEnumerable<ILog> Logs
        {
            get
            {
                var logs = new Dictionary<string, ILog>();
                var logPaths = _ioUtils.GetFiles(BaseLocation, LogDirectoryPrefix + "*");
                foreach (var logPath in logPaths)
                {
                    logs[logPath] = GetOrCreateLog(logPath);
                }
                // copy list to ensure the result is not modified
                var result = logs.Values.ToList();
                _logs = logs;
                return result;
            }
        }

        public ILog CurrentLog
        {
            get
            {
                var fileName = LogDirectoryPrefix + DateTime.Today.ToString("yyyy-MM-dd");
                var logPath = Path.Combine(BaseLocation, fileName);
                var log = GetOrCreateLog(logPath);
                if (!_ioUtils.FileExists(logPath))
                {
                    _ioUtils.CreateFile(logPath);
                    _logs[logPath] = log;
                    LogCreated(log);
                }
                return log;
            }
        }

        private ILog GetOrCreateLog(string logPath)
        {
            return _logs.ContainsKey(logPath) ? _logs[logPath] : new LogFile(logPath);
        }

        public long LogsSize
        {
            get { return Logs.Select(log => log.SizeInBytes).Sum(); }
        }

        public void DeleteLogsOlderThan(DateTime time)
        {
            foreach (var log in Logs)
            {
                if (log.Date < time.Date)
                {
                    log.Delete();
                }
                else if (log.Date == time.Date)
                {
                    log.RemoveEntriesOlderThan(time);
                }
            }
        }

        public void DeleteAllLogs()
        {
            foreach (var log in Logs)
            {
                log.Delete();
            }
            _ioUtils.DeleteDirectoryWithContent(BaseLocation);
        }

        public override string ToString()
        {
            return "LogFileManager[" + BaseLocation + "]";
        }
    }
}