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
    internal class IdReaderLoggerTest : LoggerTestBase
    {
        private IdReaderLogger _sut;

        [SetUp]
        public void Setup()
        {
            _sut = new IdReaderLogger(Log);
        }

        [Test]
        public void FirstRead()
        {
            _sut.Processing(@"C:\a\b\c.zip");
            _sut.CacheMiss();
            _sut.FoundIds(new[] {"id1", "id2"});

            AssertLog(
                @"",
                @"############################################################",
                @"# reading ids",
                @"############################################################",
                @"",
                @"#### C:\a\b\c.zip",
                @"reading zip...",
                @"- id1",
                @"- id2");
        }

        [Test]
        public void CachedRead()
        {
            _sut.Processing(@"C:\a\b\c.zip");
            _sut.CacheHit();
            _sut.FoundIds(new[] {"id1", "id2"});

            AssertLog(
                @"",
                @"############################################################",
                @"# reading ids",
                @"############################################################",
                @"",
                @"#### C:\a\b\c.zip",
                @"cache hit, reading cache...",
                @"- id1",
                @"- id2");
        }

        [Test]
        public void Multiple()
        {
            _sut.Processing(@"C:\a\b\c1.zip");
            _sut.CacheMiss();
            _sut.FoundIds(new[] {"id11", "id12"});

            _sut.Processing(@"C:\a\b\c2.zip");
            _sut.CacheHit();
            _sut.FoundIds(new[] {"id21", "id22"});

            AssertLog(
                @"",
                @"############################################################",
                @"# reading ids",
                @"############################################################",
                @"",
                @"#### C:\a\b\c1.zip",
                @"reading zip...",
                @"- id11",
                @"- id12",
                @"",
                @"#### C:\a\b\c2.zip",
                @"cache hit, reading cache...",
                @"- id21",
                @"- id22");
        }
    }
}