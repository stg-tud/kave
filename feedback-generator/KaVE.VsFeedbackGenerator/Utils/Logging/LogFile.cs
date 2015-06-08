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
using System.Threading;
using JetBrains.Util;
using KaVE.Commons.Model.Events;
using KaVE.Commons.Utils;
using KaVE.Commons.Utils.Assertion;
using KaVE.Commons.Utils.IO;
using KaVE.Commons.Utils.Logging.Json;
using ILogger = KaVE.Commons.Utils.Exceptions.ILogger;
using KaVE.Commons.Utils.Logging;

namespace KaVE.VsFeedbackGenerator.Utils.Logging
{
    public class LogFile : ILog
    {
        private readonly IIoUtils _ioUtils;

        public event LogEntryEventHandler EntryAppended = delegate { };

        public event LogEntriesEventHandler EntriesRemoved = delegate { };

        public event LogEventHandler Deleted = delegate { };

        public LogFile(string path)
        {
            _ioUtils = Registry.GetComponent<IIoUtils>();
            Path = path;
        }

        public string Path { get; private set; }

        public DateTime Date
        {
            get
            {
                var fileName = System.IO.Path.GetFileName(Path);
                Asserts.NotNull(fileName, "illegal log path: '{0}'", Path);
                var dateString = fileName.Substring(LogFileManager.LogDirectoryPrefix.Length);
                var date = DateTime.Parse(dateString);
                return date;
            }
        }

        public IEnumerable<IDEEvent> ReadAll()
        {
            using (var reader = NewLogReader())
            {
                return reader.ReadAll().ToList();
            }
        }

        private ILogReader<IDEEvent> NewLogReader()
        {
            var logStream = TryOpenLogFile();
            return new JsonLogReader<IDEEvent>(logStream, Registry.GetComponent<ILogger>());
        }

        // TODO @Sven: pull reader/writer handling into logfile and solve this by synchronization?
        private Stream TryOpenLogFile()
        {
            Stream result = null;
            Exception exception = null;
            for (var retries = 3; retries > 0 && result == null; retries--)
            {
                try
                {
                    result = _ioUtils.OpenFile(Path, FileMode.OpenOrCreate, FileAccess.Read);
                    exception = null;
                }
                catch (IOException e)
                {
                    if (IsConcurrentAccessException(e))
                    {
                        exception = e;
                        Thread.Sleep(100);
                        continue;
                    }
                    throw;
                }
            }
            if (exception != null)
            {
                throw new IOException(exception.Message, exception);
            }

            return result;
        }

        private bool IsConcurrentAccessException(IOException e)
        {
            return e.Message.Equals(
                string.Format(
                    "The process cannot access the file '{0}' because it is being used by another process.",
                    Path)) || e.Message.Equals(
                string.Format(
                    "Der Prozess kann nicht auf die Datei '{0}' zugreifen, da sie von einem anderen Prozess verwendet wird.",
                    Path));
        }

        public bool IsEmpty()
        {
            using (var reader = NewLogReader())
            {
                return reader.ReadNext() == null;
            }
        }

        public void Append(IDEEvent entry)
        {
            using (var writer = NewLogWriter())
            {
                writer.Write(entry);
                EntryAppended(entry);
            }
        }

        private ILogWriter<IDEEvent> NewLogWriter()
        {
            _ioUtils.CreateDirectory(Directory.GetParent(Path).FullName);
            var logStream = _ioUtils.OpenFile(Path, FileMode.Append, FileAccess.Write);
            return new JsonLogWriter<IDEEvent>(logStream);
        }

        public long SizeInBytes
        {
            get { return _ioUtils.GetFileSize(Path); }
        }

        public void RemoveRange(IEnumerable<IDEEvent> entries)
        {
            RemoveEntries(entries.Contains);
        }

        public void RemoveEntriesOlderThan(DateTime time)
        {
            RemoveEntries(ideEvent => ideEvent.TriggeredAt <= time);
        }

        private void RemoveEntries(Func<IDEEvent, bool> removeCondition)
        {
            var tempFileName = _ioUtils.GetTempFileName();
            var removed = new List<IDEEvent>();
            using (var stream = _ioUtils.OpenFile(tempFileName, FileMode.Append, FileAccess.Write))
            {
                using (var writer = new JsonLogWriter<IDEEvent>(stream))
                {
                    using (var reader = NewLogReader())
                    {
                        reader.ReadAll().ForEach(
                            entry =>
                            {
                                if (!removeCondition(entry))
                                {
                                    writer.Write(entry);
                                }
                                else
                                {
                                    removed.Add(entry);
                                }
                            });
                    }
                }
            }

            _ioUtils.DeleteFile(Path);
            _ioUtils.MoveFile(tempFileName, Path);
            EntriesRemoved(removed);
        }

        public void Delete()
        {
            _ioUtils.DeleteFile(Path);
            Deleted(this);
        }

        protected bool Equals(LogFile other)
        {
            return string.Equals(Path, other.Path);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj, Equals);
        }

        public override int GetHashCode()
        {
            return (Path != null ? Path.GetHashCode() : 0);
        }

        public override string ToString()
        {
            return "[LogFile " + Path + "]";
        }
    }
}