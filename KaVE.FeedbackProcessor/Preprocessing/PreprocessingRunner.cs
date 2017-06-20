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

using System.Collections.Generic;
using System.IO;
using KaVE.Commons.Utils;
using KaVE.Commons.Utils.Assertion;
using KaVE.FeedbackProcessor.Preprocessing.Filters;
using KaVE.FeedbackProcessor.Preprocessing.Logging;
using KaVE.FeedbackProcessor.Preprocessing.Model;

namespace KaVE.FeedbackProcessor.Preprocessing
{
    internal class PreprocessingRunner
    {
        private readonly int _numWorkers;
        private readonly string _dirLogs;

        private readonly PreprocessingIo _io;
        private readonly IDictionary<int, AppendingFileLogger> _loggers;

        public PreprocessingRunner(string dirIn, string dirTmp, string dirOut, int numWorkers = 1)
        {
            _numWorkers = numWorkers;
            Asserts.That(Directory.Exists(dirIn));
            Asserts.That(Directory.Exists(dirOut));

            Asserts.That(Directory.Exists(dirTmp));
            _dirLogs = dirTmp + @"Preprocessing-Logs\";
            var dirMerged = dirTmp + @"Preprocessing-Merged\";

            Asserts.Not(Directory.Exists(_dirLogs));
            Directory.CreateDirectory(_dirLogs);
            Asserts.Not(Directory.Exists(dirMerged));
            Directory.CreateDirectory(dirMerged);

            _loggers = new Dictionary<int, AppendingFileLogger>();


            _io = new PreprocessingIo(dirIn, dirMerged, dirOut);
        }

        public void Run()
        {
            new MultiThreadedPreprocessing(
                _io,
                //new MultiThreadedPreprocessingLogger(new ConsoleLogger(new DateUtils())),
                new MultiThreadedPreprocessingLogger(CreateWorkerLogger(-1)),
                _numWorkers,
                CreateIdReader,
                new Grouper(new GrouperLogger(CreateWorkerLogger(-2))),
                CreateGroupMerger,
                CreateCleaner).Run();


            foreach (var log in _loggers.Values)
            {
                log.Dispose();
            }
        }

        private IdReader CreateIdReader(int taskId)
        {
            return new IdReader(new IdReaderLogger(CreateWorkerLogger(taskId)));
        }

        private GroupMerger CreateGroupMerger(int taskId)
        {
            return new GroupMerger(_io, new GroupMergerLogger(CreateWorkerLogger(taskId)));
        }

        private Cleaner CreateCleaner(int taskId)
        {
            return new Cleaner(_io, new CleanerLogger(CreateWorkerLogger(taskId)))
            {
                Filters =
                {
                    new VersionFilter(1000),
                    new ErrorAndInfoEventFilter(),
                    new NoSessionIdFilter(),
                    new NoTimeFilter(),
                    new InvalidCompletionEventFilter()
                }
            };
        }

        private AppendingFileLogger CreateWorkerLogger(int taskId)
        {
            if (_loggers.ContainsKey(taskId))
            {
                return _loggers[taskId];
            }

            var relFile = taskId == -1 ? "main.log" : taskId == -2 ? "grouper.log" : @"worker{0}.log".FormatEx(taskId);
            var logFile = Path.Combine(_dirLogs + relFile);
            var logger = new AppendingFileLogger(logFile, new DateUtils());

            _loggers[taskId] = logger;
            return logger;
        }
    }
}