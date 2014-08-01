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
using System.IO;
using System.Linq;
using KaVE.JetBrains.Annotations;
using KaVE.Model.Events;

namespace KaVE.VsFeedbackGenerator.Utils.Logging
{
    public class LogFileManager<TLogEntry> : ILogManager<TLogEntry> where TLogEntry : IDEEvent
    {
        private readonly IIoUtils _ioUtils;
        internal const string LogDirectoryPrefix = "Log_";

        public event EventHandler LogsChanged = delegate {  };

        public LogFileManager([NotNull] string baseLocation)
        {
            BaseLocation = baseLocation;
            _ioUtils = Registry.GetComponent<IIoUtils>();
        }

        public string BaseLocation { get; private set; }

        public IEnumerable<ILog<TLogEntry>> Logs
        {
            get { return _ioUtils.GetFiles(BaseLocation, LogDirectoryPrefix + "*").Select(CreateLogFile); }
        }

        private static LogFile<TLogEntry> CreateLogFile(string logDirectoryPath)
        {
            return new LogFile<TLogEntry>(logDirectoryPath);
        }

        public double TotalLogsSizeInMB
        {
            get { return Logs.Select(log => log.SizeInMB).Sum(); }
        }

        public void DeleteLogsOlderThan(DateTime time)
        {
            foreach (var log in Logs)
            {
                if (log.Date < time.Date)
                {
                    log.Delete();
                }
                else
                {
                    log.RemoveEntriesOlderThan(time);
                }
            }
            LogsChanged.Invoke(this, new EventArgs());
        }

        public void DeleteAllLogs()
        {
            _ioUtils.DeleteDirectoryWithContent(BaseLocation);
        }

        public ILog<TLogEntry> CurrentLog
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