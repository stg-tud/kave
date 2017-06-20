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

using System.Collections.Generic;
using KaVE.FeedbackProcessor.Preprocessing.Logging;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.Preprocessing.Logging
{
    internal class CleanerLoggerTest : LoggerTestBase
    {
        private CleanerLogger _sut;

        [SetUp]
        public void Setup()
        {
            _sut = new CleanerLogger(Log);
        }

        [Test]
        public void IntegrationTest()
        {
            _sut.WorkingIn("<dirIn>", "<dirOut>");
            _sut.RegisteredFilters(new[] {"f1", "f2"});
            _sut.ReadingZip("a");
            _sut.WritingEvents();
            _sut.FinishedWriting(new Dictionary<string, int> {{"f1", 1}, {"f2", 2}});
            _sut.Dispose();

            AssertLog(
                "",
                "############################################################",
                "# started cleaning...",
                "############################################################",
                "",
                "folders:",
                "- in: <dirIn>",
                "- out: <dirOut>",
                "",
                "registered filters:",
                "- f1",
                "- f2",
                "",
                "#### zip: a",
                "reading... done",
                "writing... done",
                "- f1: 1",
                "- f2: 2",
                "",
                "#### cleaning stats over all files ####",
                "- f1: 1",
                "- f2: 2");
        }

        [Test]
        public void IntegrationTest_TwoTimes()
        {
            _sut.WorkingIn("<dirIn>", "<dirOut>");
            _sut.RegisteredFilters(new[] {"f1", "f2"});

            _sut.ReadingZip("a");
            _sut.WritingEvents();
            _sut.FinishedWriting(new Dictionary<string, int> {{"f1", 1}, {"f2", 2}});

            _sut.ReadingZip("b");
            _sut.WritingEvents();
            _sut.FinishedWriting(new Dictionary<string, int> {{"f2", 1}, {"f3", 2}});

            _sut.Dispose();

            AssertLog(
                "",
                "############################################################",
                "# started cleaning...",
                "############################################################",
                "",
                "folders:",
                "- in: <dirIn>",
                "- out: <dirOut>",
                "",
                "registered filters:",
                "- f1",
                "- f2",
                "",
                "#### zip: a",
                "reading... done",
                "writing... done",
                "- f1: 1",
                "- f2: 2",
                "",
                "#### zip: b",
                "reading... done",
                "writing... done",
                "- f2: 1",
                "- f3: 2",
                "",
                "#### cleaning stats over all files ####",
                "- f1: 1",
                "- f2: 3",
                "- f3: 2");
        }
    }
}