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
 */
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
            CreateLogFileDirectory();
        }

        public string BaseLocation { get; private set; }

        public IEnumerable<ILog<TLogEntry>> GetLogs()
        {
            return Directory.GetFiles(BaseLocation, LogDirectoryPrefix + "*").Select(CreateLogFile);
        }

        private void CreateLogFileDirectory()
        {
            Directory.CreateDirectory(BaseLocation);
        }

        private static LogFile<TLogEntry> CreateLogFile(string logDirectoryPath)
        {
            return new LogFile<TLogEntry>(logDirectoryPath);
        }

        public void DeleteLogsOlderThan(DateTime time)
        {
            GetLogs().Where(log => log.Date < time).ForEach(log => log.Delete());
        }

        public virtual void DeleteLogFileDirectory()
        {
            Directory.Delete(BaseLocation, true);
            CreateLogFileDirectory();
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