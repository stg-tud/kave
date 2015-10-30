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
using KaVE.Commons.Model.Events;

namespace KaVE.VS.FeedbackGenerator.Utils.Logging
{
    public delegate void LogEventHandler(ILog log);

    public delegate void LogEntryEventHandler(IDEEvent entry);

    public delegate void LogEntriesEventHandler(IEnumerable<IDEEvent> entries);

    /// <summary>
    ///     A log file represents a series of log entries for a specific day.
    /// </summary>
    public interface ILog
    {
        /// <summary>
        ///     Fires when <see cref="Append" /> is invoked on this log.
        /// </summary>
        event LogEntryEventHandler EntryAppended;

        /// <summary>
        ///     Fires when either <see cref="RemoveRange" /> or <see cref="RemoveEntriesOlderThan" /> is invoked on this log and at
        ///     least one entry is actually removed.
        /// </summary>
        event LogEntriesEventHandler EntriesRemoved;

        /// <summary>
        ///     Fires when <see cref="Delete" /> is invoked on this log.
        /// </summary>
        event LogEventHandler Deleted;

        /// <summary>
        ///     The day this log represents.
        /// </summary>
        DateTime Date { get; }

        long SizeInBytes { get; }

        int ApproximateNumberOfEvents { get; }

        bool IsEmpty();

        IEnumerable<IDEEvent> ReadAll();

        void Append(IDEEvent entry);

        void RemoveRange(IEnumerable<IDEEvent> entries);

        void RemoveEntriesOlderThan(DateTime time);

        void Delete();
    }

    /// <summary>
    ///     A log manager series a set of log files, where each file represents one day.
    /// </summary>
    public interface ILogManager
    {
        /// <summary>
        ///     Fires when <see cref="CurrentLog" /> is requested and there is no log for the current day.
        /// </summary>
        event LogEventHandler LogCreated;

        IEnumerable<ILog> Logs { get; }

        /// <summary>
        ///     Returns the log for today. If there is none, it will be created.
        /// </summary>
        /// TODO Make this a method, because it has a side effect.
        ILog CurrentLog { get; }

        long LogsSizeInBytes { get; }

        void DeleteLogsOlderThan(DateTime time);

        void DeleteAllLogs();
    }
}