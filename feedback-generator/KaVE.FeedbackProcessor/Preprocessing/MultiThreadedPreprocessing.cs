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
using System.Threading.Tasks;
using KaVE.Commons.Utils.Collections;
using KaVE.FeedbackProcessor.Preprocessing.Logging;
using KaVE.FeedbackProcessor.Preprocessing.Model;

namespace KaVE.FeedbackProcessor.Preprocessing
{
    public class MultiThreadedPreprocessing
    {
        private readonly IPreprocessingIo _io;
        private readonly int _numProcs;
        private readonly Func<int, IIdReader> _idReaderFactory;
        private readonly IGrouper _grouper;
        private readonly Func<int, IGroupMerger> _groupMergerFactory;
        private readonly Func<int, ICleaner> _cleanerFactory;
        private readonly IMultiThreadedPreprocessingLogger _log;

        private PreprocessingData _data;

        public MultiThreadedPreprocessing(IPreprocessingIo io,
            IMultiThreadedPreprocessingLogger log,
            int numProcs,
            Func<int, IIdReader> idReaderFactory,
            IGrouper grouper,
            Func<int, IGroupMerger> groupMergerFactory,
            Func<int, ICleaner> cleanerFactory)
        {
            _io = io;
            _log = log;
            _numProcs = numProcs;

            _idReaderFactory = idReaderFactory;
            _grouper = grouper;
            _groupMergerFactory = groupMergerFactory;
            _cleanerFactory = cleanerFactory;
        }

        public void Run()
        {
            Initialize();

            InParallel(ReadIds);
            GroupZipsByIds();
            InParallel(MergeZipGroups);
            InParallel(CleanZips);
        }

        private void Initialize()
        {
            var zips = _io.FindRelativeZipPaths();
            _data = new PreprocessingData(zips);
        }

        private void InParallel(Action<int> task)
        {
            var tasks = new Task[_numProcs];
            for (var i = 0; i < _numProcs; i++)
            {
                var taskId = i;
                tasks[i] = Task.Factory.StartNew(() => { task(taskId); });
            }
            Task.WaitAll(tasks);
        }

        private void ReadIds(int taskId)
        {
            var idReader = _idReaderFactory(taskId);

            string zip;
            while (_data.AcquireNextUnindexedZip(out zip))
            {
                var ids = idReader.Read(zip);
                _data.StoreIds(zip, ids);
            }
        }

        private void GroupZipsByIds()
        {
            var idsByZip = _data.GetIdsByZip();
            var zipGroups = _grouper.GroupRelatedZips(idsByZip);
            _data.StoreZipGroups(zipGroups);
        }

        private void MergeZipGroups(int taskId)
        {
            var zipMerger = _groupMergerFactory(taskId);

            IKaVESet<string> zips;
            while (_data.AcquireNextUnmergedZipGroup(out zips))
            {
                var relZipOut = zipMerger.Merge(zips);
                _data.StoreMergedZip(relZipOut);
            }
        }

        private void CleanZips(int taskId)
        {
            using (var cleaner = _cleanerFactory(taskId))
            {
                string zip;
                while (_data.AcquireNextUncleansedZip(out zip))
                {
                    cleaner.Clean(zip);
                }
            }
        }
    }
}