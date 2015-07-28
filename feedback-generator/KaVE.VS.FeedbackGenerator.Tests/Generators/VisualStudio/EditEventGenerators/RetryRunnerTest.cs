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
using KaVE.VS.FeedbackGenerator.Generators.VisualStudio.EditEventGenerators;
using NUnit.Framework;

namespace KaVE.VS.FeedbackGenerator.Tests.Generators.VisualStudio.EditEventGenerators
{
    internal class RetryRunnerTest
    {
        private DateTime? _lastTime;

        private IRetryRunner _uut;

        [SetUp]
        public void InitializeUut()
        {
            _uut = new RetryRunner();
        }

        [Test, MaxTime(1100)]
        public void ShouldCallOnSuccess()
        {
            var wasSet = false;

            _uut.Try(() => false, TimeSpan.Zero, 1, result => wasSet = true, () => {});

            Assert.That(() => wasSet, Is.True.After(1000, 100));
        }

        [Test, MaxTime(1100)]
        public void ShouldCallOnFailure()
        {
            var wasSet = false;

            _uut.Try<object>(() => { throw new RetryException(); }, TimeSpan.Zero, 1, result => { }, () => wasSet = true);

            Assert.That(() => wasSet, Is.True.After(1000, 100));
        }

        [Test, MaxTime(1100)]
        public void ShouldAtLeastWaitForRetryInterval()
        {
            var retryInterval = TimeSpan.FromMilliseconds(1);
            const int numberOfTries = 42;

            var didNotWaitLongEnough = false;

            _uut.Try(
                () =>
                {
                    if (DidNotWaitForInterval(DateTime.Now, retryInterval))
                    {
                        return true;
                    }
                    throw new RetryException();
                },
                retryInterval,
                numberOfTries,
                result => { didNotWaitLongEnough = result; },
                () => {});

            Assert.That(() => didNotWaitLongEnough, Is.False.After(1000, 100));
        }

        [Test, MaxTime(1100)]
        public void ShouldTryExactlyNumberOfTriesTimes()
        {
            const int numberOfTries = 42;
            var actualNumberOfTries = 0;

            _uut.Try<int>(
                () =>
                {
                    actualNumberOfTries++;
                    throw new RetryException();
                },
                TimeSpan.Zero,
                numberOfTries,
                result => { },
                () => { });

            Assert.That(() => actualNumberOfTries, Is.EqualTo(numberOfTries).After(1000, 100));
        }

        public bool DidNotWaitForInterval(DateTime now, TimeSpan minDifference)
        {
            if (_lastTime != null)
            {
                var difference = now - _lastTime.Value;
                return difference < minDifference;
            }
            _lastTime = now;
            return false;
        }
    }
}