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
    public class Runner
    {
        private readonly IPreprocessingIo _io;
        private readonly int _numProcs;
        private readonly Func<IPreprocessingIo, IPrepocessingLogger, IdReader> _idReaderFactory;
        private readonly Grouper _grouper;
        private readonly Func<IPreprocessingIo, IPrepocessingLogger, GroupMerger> _zipGroupMergerFactory;
        private readonly Func<IPreprocessingIo, IPrepocessingLogger, Cleaner> _zipCleanerFactory;
        private PreprocessingData _data;

        private IRunnerLogger _log;

        public Runner(IPreprocessingIo io,
            IRunnerLogger log,
            int numProcs,
            Func<IPreprocessingIo, IPrepocessingLogger, IdReader> idReaderFactory,
            Grouper grouper,
            Func<IPreprocessingIo, IPrepocessingLogger, GroupMerger> zipGroupMergerFactory,
            Func<IPreprocessingIo, IPrepocessingLogger, Cleaner> zipCleanerFactory)
        {
            _io = io;
            _log = log;
            _numProcs = numProcs;

            _idReaderFactory = idReaderFactory;
            _grouper = grouper;
            _zipGroupMergerFactory = zipGroupMergerFactory;
            _zipCleanerFactory = zipCleanerFactory;
        }

        public void Run()
        {
            FindZips();
            InParallel(ReadIds);
            GroupZipsByIds();
            InParallel(MergeZipGroups);
            InParallel(CleanZips);
        }

        private void FindZips()
        {
            var zips = _io.FindRelativeZipPaths();
            _data = new PreprocessingData(zips);
        }

        private void ReadIds(int taskId, IPrepocessingLogger log)
        {
            var idReader = _idReaderFactory(_io, log);

            string zip;
            while (_data.AcquireNextUnindexedZip(out zip))
            {
                var ids = idReader.Read(zip);
                _data.StoreIds(zip, ids);
            }
        }

        private void GroupZipsByIds()
        {
            var zipGroups = _grouper.GroupRelatedZips(_data.GetIdsByZip());
            _data.StoreZipGroups(zipGroups);
        }

        private void MergeZipGroups(int taskId, IPrepocessingLogger log)
        {
            var zipMerger = _zipGroupMergerFactory(_io, log);

            IKaVESet<string> zips;
            while (_data.AcquireNextUnmergedZipGroup(out zips))
            {
                zipMerger.Merge(zips);
            }
        }

        private void CleanZips(int taskId, IPrepocessingLogger log)
        {
            var cleaner = _zipCleanerFactory(_io, log);

            string zip;
            while (_data.AcquireNextUncleansedZip(out zip))
            {
                cleaner.Clean(zip);
            }
        }

        private void InParallel(Action<int, IPrepocessingLogger> task)
        {
            var tasks = new Task[_numProcs];
            for (var i = 0; i < _numProcs; i++)
            {
                var taskId = i;
                tasks[i] = Task.Factory.StartNew(
                    () =>
                    {
                        using (var logger = _io.CreateLoggerWorker(taskId))
                        {
                            task(taskId, logger);
                        }
                    });
            }
            Task.WaitAll(tasks);
        }
    }
}