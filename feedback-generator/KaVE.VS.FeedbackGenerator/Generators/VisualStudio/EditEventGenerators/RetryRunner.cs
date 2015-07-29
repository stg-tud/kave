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
using System.ComponentModel;
using System.Threading;
using JetBrains.Application;

namespace KaVE.VS.FeedbackGenerator.Generators.VisualStudio.EditEventGenerators
{
    public interface IRetryRunner
    {
        void Try(Func<bool> onTry);
    }

    [ShellComponent]
    public class RetryRunner : IRetryRunner
    {
        public const int NumberOfTries = 10;
        public static TimeSpan RetryInterval = TimeSpan.FromMilliseconds(500);

        public void Try(Func<bool> onTry)
        {
            new InternalRetryRunner().Run(onTry);
        }

        private class InternalRetryRunner
        {
            private Func<bool> _onTry;

            public void Run(Func<bool> onTry)
            {
                _onTry = onTry;

                var backgroundWorker = new BackgroundWorker();
                backgroundWorker.DoWork += delegate { Retry(); };
                backgroundWorker.RunWorkerAsync();
            }

            private void Retry()
            {
                for (var tries = 0; tries < NumberOfTries; tries++)
                {
                    if (_onTry())
                    {
                        return;
                    }

                    Thread.Sleep(RetryInterval);
                }
            }
        }
    }
}