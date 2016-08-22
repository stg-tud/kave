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

namespace KaVE.FeedbackProcessor.Preprocessing.Logging
{
    public interface IMultiThreadedPreprocessingLogger
    {
        void ReadingIds(int numUnindexedZips);
        void StartWorkerReadIds(int taskId);
        void ReadIds(int taskId, string zip);
        void StopWorkerReadIds(int taskId);

        void GroupZipsByIds();

        void MergeGroups(int numGroups);
        void StartWorkerMergeGroup(int taskId);
        void MergeGroup(int taskId, int count);
        void StopWorkerMergeGroup(int taskId);

        void Cleaning(int numUnclean);
        void StartWorkerCleanZip(int taskId);
        void CleanZip(int taskId, string zip);
        void StopWorkerCleanZip(int taskId);
    }

    public class MultiThreadedPreprocessingLogger : IMultiThreadedPreprocessingLogger
    {
        private readonly IPrepocessingLogger _log;

        public MultiThreadedPreprocessingLogger(IPrepocessingLogger log)
        {
            _log = log;
        }

        public void ReadingIds(int numUnindexedZips) {}

        public void StartWorkerReadIds(int taskId) {}

        public void ReadIds(int taskId, string zip) {}

        public void StopWorkerReadIds(int taskId) {}

        public void GroupZipsByIds() {}

        public void MergeGroups(int numGroups) {}

        public void StartWorkerMergeGroup(int taskId) {}

        public void MergeGroup(int taskId, int count) {}

        public void StopWorkerMergeGroup(int taskId) {}

        public void Cleaning(int numUnclean) {}

        public void StartWorkerCleanZip(int taskId) {}

        public void CleanZip(int taskId, string zip) {}

        public void StopWorkerCleanZip(int taskId) {}

        // TODO add lock!!
    }
}