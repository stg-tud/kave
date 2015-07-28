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
    [ShellComponent]
    public class RetryRunner : IRetryRunner
    {
        public void Try<TResult>(Func<TResult> onTry,
            TimeSpan retryInterval,
            int numberOfTries,
            Action<TResult> onSuccess,
            Action onFailure,
            Action<Exception> onError)
        {
            // ReSharper disable once ObjectCreationAsStatement
            new InternalRetryRunner<TResult>(onTry, retryInterval, numberOfTries, onSuccess, onFailure, onError);
        }

        private class InternalRetryRunner<TResult>
        {
            private readonly Func<TResult> _tryComputeResult;
            private readonly Action<TResult> _onSuccess;
            private readonly Action _onFailure;
            private readonly Action<Exception> _onError;
            private readonly TimeSpan _retryInterval;
            private readonly int _numberOfTries;
            private TResult _result;

            public InternalRetryRunner(Func<TResult> tryComputeResult,
                TimeSpan retryInterval,
                int numberOfTries,
                Action<TResult> onSuccess,
                Action onFailure,
                Action<Exception> onError)
            {
                _onFailure = onFailure;
                _onSuccess = onSuccess;
                _onError = onError ?? (e => { throw e; });
                _numberOfTries = numberOfTries;
                _retryInterval = retryInterval;
                _tryComputeResult = tryComputeResult;

                var backgroundWorker = new BackgroundWorker();
                backgroundWorker.DoWork += delegate { Run(); };

                backgroundWorker.RunWorkerAsync();
            }

            private void Run()
            {
                for (var tries = 0; tries < _numberOfTries; tries++)
                {
                    try
                    {
                        _result = _tryComputeResult();
                        _onSuccess(_result);
                        return;
                    }
                    catch (RetryException)
                    {
                        Thread.Sleep(_retryInterval);
                    }
                    catch (Exception exception)
                    {
                        _onError(exception);
                        return;
                    }
                }
                _onFailure();
            }
        }
    }

    public interface IRetryRunner
    {
        /// <summary>
        ///     Throw <code>RetryException</code> to cancel a try
        /// </summary>
        void Try<TResult>(Func<TResult> onTry,
            TimeSpan retryInterval,
            int numberOfTries,
            Action<TResult> onSuccess,
            Action onFailure,
            Action<Exception> onError = null);
    }

    public class RetryException : Exception { }
}