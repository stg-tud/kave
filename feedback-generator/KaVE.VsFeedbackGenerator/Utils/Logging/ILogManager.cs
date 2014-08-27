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
 */

using System;
using System.Collections.Generic;
using KaVE.Model.Events;

namespace KaVE.VsFeedbackGenerator.Utils.Logging
{
    public delegate void LogEventHandler(ILog log);

    public delegate void LogEntryEventHandler(IDEEvent entry);

    public delegate void LogEntriesEventHandler(IEnumerable<IDEEvent> entries);

    public interface ILog
    {
        event LogEntryEventHandler EntryAppended;
        event LogEntriesEventHandler EntriesRemoved;
        event LogEventHandler Deleted;

        DateTime Date { get; }
        long SizeInBytes { get; }
        bool IsEmpty();
        IEnumerable<IDEEvent> ReadAll();

        void Append(IDEEvent entry);
        void RemoveRange(IEnumerable<IDEEvent> entries);
        void RemoveEntriesOlderThan(DateTime time);
        void Delete();
    }

    public interface ILogManager
    {
        event LogEventHandler LogAdded;

        event EventHandler LogsChanged;

        string BaseLocation { get; }
        IEnumerable<ILog> Logs { get; }
        ILog CurrentLog { get; }
        string FormatedLogsSize { get; }

        void DeleteLogsOlderThan(DateTime time);
        void DeleteAllLogs();
    }
}