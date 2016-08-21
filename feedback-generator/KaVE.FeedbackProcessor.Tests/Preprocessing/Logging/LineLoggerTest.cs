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

using KaVE.FeedbackProcessor.Preprocessing.Logging;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.Preprocessing.Logging
{
    internal class LineLoggerTest
    {
        private LineLogger _sut;

        [SetUp]
        public void Setup()
        {
            _sut = new LineLogger();
        }

        private void AssertLines(params string[] expecteds)
        {
            CollectionAssert.AreEqual(expecteds, _sut.LoggedLines);
        }

        [Test]
        public void ShouldCaptureLines()
        {
            _sut.Log("a");
            _sut.Append("b");
            _sut.Append("c");
            _sut.Log();
            _sut.Log("d");

            AssertLines("abc", "", "d");
        }

        [Test]
        public void ShouldCaptureLines2()
        {
            _sut.Append("a");
            _sut.Log();
            _sut.Log("b");

            AssertLines("a", "", "b");
        }

        [Test]
        public void CanLogInvalidReplacement()
        {
            _sut.Log("{x}");
            AssertLines("{x}");
        }

        [Test]
        public void CanAppendInvalidReplacement()
        {
            _sut.Append("{x}");
            AssertLines("{x}");
        }
    }
}