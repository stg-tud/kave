﻿/*
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
    public interface ILog<TLogEntry> where TLogEntry : IDEEvent
    {
        DateTime Date { get; }
        ILogReader<TLogEntry> NewLogReader();
        ILogWriter<TLogEntry> NewLogWriter();
        double GetFileSize();
        void RemoveRange(IEnumerable<TLogEntry> entries);
        void RemoveEntriesOlderThan(DateTime time);
        void Delete();
        
    }

    public interface ILogManager<TLogEntry> where TLogEntry : IDEEvent
    {
        event EventHandler LogsChanged;

        string BaseLocation { get; }
        IEnumerable<ILog<TLogEntry>> GetLogs();
        ILog<TLogEntry> CurrentLog { get; }
        void DeleteLogsOlderThan(DateTime time);
        void DeleteLogFileDirectory();
        double GetAccumulatedLogFileSize();
    }
}