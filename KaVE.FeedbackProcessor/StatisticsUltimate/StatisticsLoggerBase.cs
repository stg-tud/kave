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

namespace KaVE.FeedbackProcessor.StatisticsUltimate
{
    public interface IStatisticsLogger
    {
        void SearchingZips(string root);

        void FoundZips(int totalZips);

        void StartingStatCreation(int taskId);

        void CreatingStats(int taskId, string zip);

        void FinishedStatCreation(int taskId);
    }

    public class StatisticsLoggerBase : IStatisticsLogger
    {
        protected readonly object Lock = new object();

        public void SearchingZips(string dirIn)
        {
            lock (Lock)
            {
                Log("Searching for .zip files in '{0}' ...", dirIn);
            }
        }

        private int _total;

        public void FoundZips(int count)
        {
            lock (Lock)
            {
                _total = count;
                Append(" found {0} files", count);
            }
        }

        public void StartingStatCreation(int taskId)
        {
            lock (Lock)
            {
                Log("({0}) StartingStatCreation", taskId);
            }
        }

        private int _current;

        public void CreatingStats(int taskId, string zip)
        {
            lock (Lock)
            {
                _current++;
                var perc = 100 * _current / (double) _total;
                Log("({0}) CreatingStats for {1} ({2}/{3} started -- {4:0.0}%)", taskId, zip, _current, _total, perc);
            }
        }

        public void FinishedStatCreation(int taskId)
        {
            lock (Lock)
            {
                Log("({0}) FinishedStatCreation", taskId);
            }
        }

        protected static void Append(string msg, params object[] args)
        {
            Console.Write(msg, args);
        }

        private bool _isFirstLine = true;

        protected void Log(string msg, params object[] args)
        {
            if (!_isFirstLine)
            {
                Console.WriteLine();
            }
            _isFirstLine = false;
            Console.Write(DateTime.Now);
            Console.Write('\t');
            Append(msg, args);
        }
    }
}