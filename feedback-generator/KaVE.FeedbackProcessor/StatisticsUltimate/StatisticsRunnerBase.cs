/*
 * Copyright 2017 Sebastian Proksch
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
using System.Linq;
using System.Threading.Tasks;
using KaVE.FeedbackProcessor.Preprocessing.Model;

namespace KaVE.FeedbackProcessor.StatisticsUltimate
{
    public class StatisticsRunnerBase
    {
        protected readonly object Lock = new object();

        private readonly IPreprocessingIo _io;
        private readonly IStatisticsLogger _log;
        private readonly int _numProcs;

        private ISet<string> _ids;

        protected StatisticsRunnerBase(IPreprocessingIo io, IStatisticsLogger log, int numProcs)
        {
            _io = io;
            _log = log;
            _numProcs = numProcs;
        }

        protected void FindZips()
        {
            _ids = new HashSet<string>();
            _log.SearchingZips(_io.GetFullPath_In(""));
            foreach (var zip in _io.FindRelativeZipPaths())
            {
                _ids.Add(zip);
            }
            _log.FoundZips(_ids.Count);
        }

        protected void InParallel(Action<int> task)
        {
            var tasks = new Task[_numProcs];
            for (var i = 0; i < _numProcs; i++)
            {
                var taskId = i;
                tasks[i] = Task.Factory.StartNew(() => { task(taskId); });
            }
            Task.WaitAll(tasks);
        }

        protected bool GetNextZip(out string zip)
        {
            lock (Lock)
            {
                if (_ids.Count > 0)
                {
                    zip = _ids.First();
                    _ids.Remove(zip);
                    return true;
                }
                zip = null;
                return false;
            }
        }
    }
}