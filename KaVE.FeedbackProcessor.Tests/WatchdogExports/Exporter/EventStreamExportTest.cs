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
using System.IO;
using KaVE.Commons.Model.Events;
using KaVE.Commons.TestUtils;
using KaVE.Commons.Utils.Assertion;
using KaVE.FeedbackProcessor.WatchdogExports;
using KaVE.FeedbackProcessor.WatchdogExports.Exporter;
using Moq;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.WatchdogExports.Exporter
{
    [Ignore]
    internal class EventStreamExportTest : FileBasedTestBase
    {
        #region setup & helpers

        private EventStreamExport _sut;
        private string _someRelFileName;
        private string _someFileName;
        private IList<IDEEvent> _events;

        [SetUp]
        public void SetUp()
        {
            _events = new List<IDEEvent>();
            _someRelFileName = "logFile.log";
            _someFileName = Path.Combine(DirTestRoot, _someRelFileName);

            var fixer = Mock.Of<IEventFixer>();
            Mock.Get(fixer)
                .Setup(f => f.FixAndFilter(It.IsAny<IEnumerable<IDEEvent>>()))
                .Returns<IEnumerable<IDEEvent>>(es => es);

            _sut = new EventStreamExport(DirTestRoot, fixer);
        }

        private void Given(IDEEvent e)
        {
            _events.Add(e);
        }

        private void AssertExport(params string[] expecteds)
        {
            _sut.Write(_events, _someRelFileName);
            Assert.IsTrue(File.Exists(_someFileName));
            var actuals = File.ReadAllLines(_someFileName);
            CollectionAssert.AreEqual(expecteds, actuals);
        }

        #endregion

        [Test, ExpectedException(typeof(AssertException))]
        public void RootMustExist()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new EventStreamExport(@"C:\Does\Not\Exist\", null);
        }

        [Test]
        public void HappyPath()
        {
            Given(new ActivityEvent());
            AssertExport("...");
        }
    }
}