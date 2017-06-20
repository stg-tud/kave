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
using System.Collections.Generic;
using System.Linq;
using KaVE.Commons.Model.Events;
using KaVE.VS.FeedbackGenerator.Utils.Logging;
using Moq;

namespace KaVE.VS.FeedbackGenerator.Tests.SessionManager.FeedbackViewModelTestSuite
{
    static class LogTestHelper
    {
        public static Mock<ILog> MockLog()
        {
            var mockLog = new Mock<ILog>();
            mockLog.Setup(log => log.Delete()).Callback(() => mockLog.Raise(log => log.Deleted += null, mockLog.Object));
            mockLog.Setup(log => log.RemoveRange(It.IsAny<IEnumerable<IDEEvent>>()))
                   .Callback<IEnumerable<IDEEvent>>(
                       entries => mockLog.Raise(log => log.EntriesRemoved += null, entries));
            return mockLog;
        }

        public static IEnumerable<Mock<ILog>> MockLogs(int numberOfLogs)
        {
            return Enumerable.Range(0, numberOfLogs).Select(i => MockLog());
        }

        public static ILog SomeLog()
        {
            return MockLog().Object;
        }

        public static ILog SomeLog(DateTime startTime, IEnumerable<IDEEvent> events)
        {
            var log = MockLog();
            log.Setup(l => l.ReadAll()).Returns(events);
            log.Setup(l => l.Date).Returns(startTime);
            return log.Object;
        }

        public static ILog SomeLog(IEnumerable<IDEEvent> events)
        {
            var log = MockLog();
            log.Setup(l => l.ReadAll()).Returns(events);
            return log.Object;
        }

        public static IList<ILog> SomeLogs(int numberOfLogs)
        {
            return MockLogs(numberOfLogs).Select(mock => mock.Object).ToList();
        }
    }
}
