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

using KaVE.Commons.Utils;
using KaVE.FeedbackProcessor.Preprocessing.Logging;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.Preprocessing.Logging
{
    internal class GroupMergerLoggerTest : LoggerTestBase
    {
        private GroupMergerLogger _sut;

        [SetUp]
        public void Setup()
        {
            _sut = new GroupMergerLogger(new ConsoleLogger(new DateUtils()));
        }

        [Test]
        public void Integration()
        {
            _sut.WorkingIn("<in>", "<out>");
            _sut.NextGroup(3, "a");
            _sut.Reading("a");
            _sut.Reading("b");
            _sut.Reading("c");
            _sut.Result(123);
        }

        [Test]
        public void Integration_Multiple()
        {
            _sut.WorkingIn("<in>", "<out>");

            _sut.NextGroup(3, "a");
            _sut.Reading("a");
            _sut.Reading("b");
            _sut.Reading("c");
            _sut.Result(123);

            _sut.NextGroup(2, "d");
            _sut.Reading("d");
            _sut.Reading("e");
            _sut.Result(45);
        }
    }
}