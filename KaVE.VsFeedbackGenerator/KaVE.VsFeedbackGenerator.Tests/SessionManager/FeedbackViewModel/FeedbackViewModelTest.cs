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
 *    - Sven Amann
 */

using System.Linq;
using KaVE.Model.Events;
using KaVE.TestUtils;
using KaVE.Utils.Assertion;
using KaVE.VsFeedbackGenerator.Generators;
using KaVE.VsFeedbackGenerator.Tests.Utils;
using KaVE.VsFeedbackGenerator.Utils;
using KaVE.VsFeedbackGenerator.Utils.Logging;
using Moq;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.SessionManager.FeedbackViewModel
{
    [TestFixture]
    internal class FeedbackViewModelTest
    {
        private Mock<ILogManager<IDEEvent>> _mockLogManager;
        private TestDateUtils _dateUtils;
        private Mock<ILogger> _mockLogger;

        private VsFeedbackGenerator.SessionManager.FeedbackViewModel _uut;

        [SetUp]
        public void SetUp()
        {
            _mockLogger = new Mock<ILogger>();
            Registry.RegisterComponent(_mockLogger.Object);

            _mockLogManager = new Mock<ILogManager<IDEEvent>>();
            _dateUtils = new TestDateUtils();

            _uut = new VsFeedbackGenerator.SessionManager.FeedbackViewModel(
                _mockLogManager.Object,
                null,
                null,
                _dateUtils);
        }

        [TearDown]
        public void TearDown()
        {
            Registry.Clear();
        }

        [Test]
        public void ShouldGetAllLogsFromLogManagerOnRefresh()
        {
            _uut.Refresh();
            WaitForRefreshToFinish();

            _mockLogManager.Verify(lm => lm.GetLogs());
        }

        [Test]
        public void ShouldCreateOneSessionSubViewModelPerLogOnRefresh()
        {
            var logs = new[]
            {
                MockLog(),
                MockLog()
            };
            _mockLogManager.Setup(lm => lm.GetLogs()).Returns(logs);

            _uut.Refresh();
            WaitForRefreshToFinish();

            var actuals = _uut.Sessions.ToList();
            Assert.AreEqual(2, actuals.Count);
            Assert.AreEqual(logs[0], actuals[0].Log);
            Assert.AreEqual(logs[1], actuals[1].Log);
        }

        [Test]
        public void ShouldDisplayNothingAndLogErrorWhenRefreshFails()
        {
            var exception = new AssertException("test exception");
            _mockLogManager.Setup(lm => lm.GetLogs()).Throws(exception);

            _uut.Refresh();
            WaitForRefreshToFinish();

            Assert.IsFalse(_uut.Sessions.Any());
            _mockLogger.Verify(l => l.Error(exception));
        }

        private static ILog<IDEEvent> MockLog()
        {
            var log = new Mock<ILog<IDEEvent>>();
            log.Setup(l => l.NewLogReader()).Returns(new Mock<ILogReader<IDEEvent>>().Object);
            return log.Object;
        }

        private void WaitForRefreshToFinish()
        {
            AsyncTestHelper.WaitForCondition(() => !_uut.Refreshing);
        }
    }
}