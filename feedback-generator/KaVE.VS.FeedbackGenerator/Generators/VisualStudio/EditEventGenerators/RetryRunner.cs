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
    internal class RetryRunner<TResult>
    {
        private readonly Func<TResult> _tryComputeResult;
        private readonly TimeSpan _retryInterval;
        private readonly int _numberOfTries;
        private TResult _result;

        public RetryRunner(Func<TResult> tryComputeResult,
            TimeSpan retryInterval,
            int numberOfTries,
            Action<TResult> onCompleted)
        {
            _numberOfTries = numberOfTries;
            _retryInterval = retryInterval;
            _tryComputeResult = tryComputeResult;

            var backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += delegate { Run(); };
            backgroundWorker.RunWorkerCompleted += delegate { onCompleted(_result); };

            backgroundWorker.RunWorkerAsync();
        }

        private void Run()
        {
            for (var tries = 0; tries < _numberOfTries; tries++)
            {
                try
                {
                    _result = _tryComputeResult();
                    return;
                }
                catch
                {
                    Thread.Sleep(_retryInterval);
                }
            }
        }
    }

    [ShellComponent]
    public class RetryRunner : IRetryRunner
    {
        /// <summary>
        ///     Repeats <paramref name="onTry" /> (starting instantly)
        ///     with the interval <paramref name="retryInterval" />
        ///     for <paramref name="numberOfTries" /> times OR until <paramref name="onTry" /> does not throw an exception.
        ///     <para>Calls <paramref name="onDone" /> when finished</para>
        /// </summary>
        public void Try<TResult>(Func<TResult> onTry,
            TimeSpan retryInterval,
            int numberOfTries,
            Action<TResult> onDone)
        {
            // ReSharper disable once ObjectCreationAsStatement
            new RetryRunner<TResult>(onTry, retryInterval, numberOfTries, onDone);
        }
    }

    public interface IRetryRunner {
        /// <summary>
        ///     Repeats <paramref name="onTry" /> (starting instantly)
        ///     with the interval <paramref name="retryInterval" />
        ///     for <paramref name="numberOfTries" /> times OR until <paramref name="onTry" /> does not throw an exception.
        ///     <para>Calls <paramref name="onDone" /> when finished</para>
        /// </summary>
        void Try<TResult>(Func<TResult> onTry,
            TimeSpan retryInterval,
            int numberOfTries,
            Action<TResult> onDone);
    }
}