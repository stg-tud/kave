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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Util;
using KaVE.Commons.Model.Events;
using KaVE.VS.FeedbackGenerator.Utils.Logging;

namespace KaVE.VS.FeedbackGenerator.Tests.Utils.Logging
{
    internal class InMemoryLogManager : ILogManager, IEnumerable<ILog>
    {
        private IDictionary<DateTime, ILog> _logs = new Dictionary<DateTime, ILog>();

        public event LogEventHandler LogCreated = delegate { };

        public IEnumerable<ILog> Logs
        {
            get { return _logs.Values.ToList(); }
        }

        public void Add(DateTime logDate, params IDEEvent[] logEntries)
        {
            Add(logDate, logEntries.ToList());
        }

        public void Add(DateTime logDate, IEnumerable<IDEEvent> logEntries)
        {
            var log = new InMemoryLog { Date = logDate };
            logEntries.ForEach(log.Append);
            _logs.Add(logDate, log);
        }

        public ILog CurrentLog
        {
            get
            {
                var today = DateTime.Today;
                if (!_logs.ContainsKey(today))
                {
                    _logs[today] = new InMemoryLog {Date = today};
                    LogCreated(_logs[today]);
                }
                return _logs[today];
            }
        }

        public long LogsSizeInBytes
        {
            get { throw new NotImplementedException(); }
        }

        public void DeleteLogsOlderThan(DateTime time)
        {
            _logs.Where(entry => entry.Key < time).ForEach(entry => entry.Value.Delete());
            _logs = _logs.Where(entry => entry.Key >= time).ToDictionary(e => e.Key, e => e.Value);
        }

        public void DeleteAllLogs()
        {
            _logs.Values.ForEach(log => log.Delete());
            _logs.Clear();
        }

        public IEnumerator<ILog> GetEnumerator()
        {
            return _logs.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public class InMemoryLog : ILog
        {
            public event LogEntryEventHandler EntryAppended;
            public event LogEntriesEventHandler EntriesRemoved;
            public event LogEventHandler Deleted = delegate { };

            public DateTime Date { get; internal set; }

            private readonly IList<IDEEvent> _entries = new List<IDEEvent>();

            public long SizeInBytes
            {
                get { throw new NotImplementedException(); }
            }

            public bool IsEmpty()
            {
                throw new NotImplementedException();
            }

            public IEnumerable<IDEEvent> ReadAll()
            {
                return _entries;
            }

            public void Append(IDEEvent entry)
            {
                _entries.Add(entry);
            }

            public void RemoveRange(IEnumerable<IDEEvent> entries)
            {
                throw new NotImplementedException();
            }

            public void RemoveEntriesOlderThan(DateTime time)
            {
                throw new NotImplementedException();
            }

            public void Delete()
            {
                Deleted(this);
            }
        }
    }
}