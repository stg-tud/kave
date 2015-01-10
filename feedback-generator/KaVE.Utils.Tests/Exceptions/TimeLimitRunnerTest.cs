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
 * 
 * Contributors:
 *    - Sebastian Proksch
 */

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using KaVE.Utils.Exceptions;
using NUnit.Framework;

namespace KaVE.Utils.Tests.Exceptions
{
    public class TimeLimitRunnerTest
    {
        private Stopwatch _stopWatch;
        private CancellationTokenSource _tokenSource;

        private bool _onSuccessCalled;
        private bool _onTimeoutCalled;
        private bool _onErrorCalled;
        private string _actualResult;
        private bool _wasTimeOut;
        private Exception _actualException;

        private long ActualDuration
        {
            get { return _stopWatch.ElapsedMilliseconds; }
        }

        [SetUp]
        public void Setup()
        {
            _tokenSource = new CancellationTokenSource();
            _stopWatch = new Stopwatch();

            _onSuccessCalled = false;
            _onTimeoutCalled = false;
            _onErrorCalled = false;

            _actualResult = null;
            _wasTimeOut = false;
            _actualException = null;
        }

        [Test]
        public void SuccessfulExecution()
        {
            Run(Ok("X", 100), 1000);

            Assert.True(_onSuccessCalled);
            Assert.False(_onTimeoutCalled);
            Assert.False(_onErrorCalled);

            Assert.AreEqual("X", _actualResult);
            Assert.Less(ActualDuration, 150);
        }

        [Test]
        public void SuccessfulExecutionWithCancellation()
        {
            _tokenSource.Cancel();
            Run(Ok("X", 100), 1000);

            Assert.False(_onSuccessCalled);
            Assert.False(_onTimeoutCalled);
            Assert.False(_onErrorCalled);

            Assert.Less(ActualDuration, 150);
        }

        [Test]
        public void CancelledExecutionStopsEarly()
        {
            var worker1 = new BackgroundWorker();
            worker1.DoWork += (sender, e) => Run(EndlessLoop(), 1000);
            worker1.RunWorkerAsync();

            Thread.Sleep(100);
            _tokenSource.Cancel();
            Thread.Sleep(100);

            Assert.False(worker1.IsBusy);
            Assert.False(_onSuccessCalled);
            Assert.False(_onTimeoutCalled);
            Assert.False(_onErrorCalled);
        }

        [Test]
        public void ExecutionTimeout()
        {
            Run(Ok("X", 1000), 100);

            Assert.False(_onSuccessCalled);
            Assert.True(_onTimeoutCalled);
            Assert.False(_onErrorCalled);

            Assert.Less(ActualDuration, 150);
        }

        [Test]
        public void FailingExectution()
        {
            var e = new Exception();
            Run(Fail(e, 100), 1000);

            Assert.False(_onSuccessCalled);
            Assert.False(_onTimeoutCalled);
            Assert.True(_onErrorCalled);

            Assert.AreSame(e, _actualException);
            Assert.Less(ActualDuration, 150);
        }

        [Test]
        public void FailingExectutionStopsEarly()
        {
            var e = new Exception("X");
            Run(Fail(e, 100), 1000);

            Assert.False(_onSuccessCalled);
            Assert.False(_onTimeoutCalled);
            Assert.True(_onErrorCalled);

            Assert.AreSame(e, _actualException);
            Assert.Less(ActualDuration, 150);
        }

        private void Run(Func<string> task, int timeOutInMS)
        {
            _stopWatch.Start();
            TimeLimitRunner.Run(task, timeOutInMS, _tokenSource.Token, OnSuccess, OnTimeout, OnError);
        }

        private void OnSuccess(string res)
        {
            _onSuccessCalled = true;
            _stopWatch.Stop();
            _actualResult = res;
        }

        private void OnTimeout()
        {
            _onTimeoutCalled = true;
            _stopWatch.Stop();
        }

        private void OnError(Exception exception)
        {
            _onErrorCalled = true;
            _stopWatch.Stop();
            _actualException = exception;
        }

        private static Func<string> Ok(string res, int durationInMS)
        {
            return () =>
            {
                Thread.Sleep(durationInMS);
                return res;
            };
        }

        private static Func<string> Fail(Exception exception, int durationBeforeFailInMS)
        {
            return () =>
            {
                Thread.Sleep(durationBeforeFailInMS);
                throw exception;
            };
        }

        private static Func<string> EndlessLoop()
        {
            return () => { while (true) {} };
        }
    }
}