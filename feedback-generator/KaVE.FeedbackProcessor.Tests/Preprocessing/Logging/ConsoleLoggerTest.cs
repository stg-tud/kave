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
using KaVE.Commons.Utils;
using KaVE.FeedbackProcessor.Preprocessing.Logging;
using Moq;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.Preprocessing.Logging
{
    internal class ConsoleLoggerTest
    {
        private ConsoleLogger _sut;

        private IDateUtils _dateUtils;
        private DateTime _now;

        [SetUp]
        public void SetUp()
        {
            _now = DateTime.MinValue;
            _dateUtils = Mock.Of<IDateUtils>();
            Mock.Get(_dateUtils).Setup(u => u.Now).Returns(() => _now);

            _sut = new ConsoleLogger(_dateUtils);
        }

        // it is hard to write a real test here...  so we just provide an integration test that is to be manually checked
        [Test]
        public void IntegrationTest()
        {
            _sut.Log("a");
            _sut.Append("b");
            _sut.Log("c");
            _now = _now + TimeSpan.FromSeconds(1);
            _sut.Log();
            _sut.Log("d");
        }

        [Test]
        public void CanLogInvalidReplacementStrings()
        {
            _sut.Log("{x}");
        }

        [Test]
        public void CanAppendInvalidReplacementStrings()
        {
            _sut.Append("{x}");
        }

        [Test]
        public void AppendAlsoBreaksFirstLine()
        {
            _sut.Append("a");
            _sut.Log("b");
        }
    }
}