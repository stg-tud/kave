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
using KaVE.VS.FeedbackGenerator.Utils.Logging;
using NUnit.Framework;

namespace KaVE.VS.FeedbackGenerator.Tests.Utils.Logging
{
    [TestFixture]
    internal abstract class LogManagerContractTest
    {
        protected static readonly DateTime Today = DateTime.Today;
        protected static readonly DateTime Yesterday = DateTime.Today.AddDays(-1);
        protected static readonly DateTime TwoDaysAgo = DateTime.Today.AddDays(-2);
        protected static readonly DateTime SomeDay = new DateTime(2015, 05, 23);

        protected ILogManager Uut { get; private set; }

        [SetUp]
        public void SetUp()
        {
            Uut = CreateLogManager();
        }

        protected abstract ILogManager CreateLogManager();

        [Test]
        public void ReturnsNoLogs()
        {
            GivenNoLogsExist();

            var actualLogs = Uut.Logs;

            Assert.IsEmpty(actualLogs);
        }

        [Test]
        public void ReturnsExistingLog()
        {
            GivenLogsExist(Today);

            var actualLogs = Uut.Logs;

            Assert.AreEqual(1, actualLogs.Count());
        }

        [Test]
        public void ReturnsExistingLogs()
        {
            GivenLogsExist(Today, Yesterday, TwoDaysAgo);

            var actualLogs = Uut.Logs;

            Assert.AreEqual(3, actualLogs.Count());
        }

        [Test]
        public void ReturnsLogForDate()
        {
            GivenLogsExist(SomeDay);

            var actualLog = Uut.Logs.First();

            Assert.AreEqual(SomeDay, actualLog.Date);
        }

        [Test]
        public void ReturnsSameLogInstances()
        {
            GivenLogsExist(SomeDay);

            var log1 = Uut.Logs.First();
            var log2 = Uut.Logs.First();

            Assert.AreSame(log1, log2);
        }

        [Test]
        public void DetectsNewLog()
        {
            Assert.IsEmpty(Uut.Logs);

            GivenLogsExist(SomeDay);

            Assert.IsNotEmpty(Uut.Logs);
        }

        [Test]
        public void DropsDeletedLog()
        {
            GivenLogsExist(SomeDay);

            Assert.IsNotEmpty(Uut.Logs);

            GivenNoLogsExist();

            Assert.IsEmpty(Uut.Logs);
        }

        [Test]
        public void ReturnsCurrentLog()
        {
            var todaysLog = Uut.CurrentLog;

            Assert.AreEqual(Today, todaysLog.Date);
        }

        [Test]
        public void RaisesLogCreatedIfCurrentLogDidNotExist()
        {
            ILog newLog = null;
            Uut.LogCreated += log => newLog = log;

            var currentLog = Uut.CurrentLog;

            Assert.AreSame(currentLog, newLog);
        }

        [Test]
        public void DoesNotRaiseLogCreatedIfCurrentLogExists()
        {
            GivenLogsExist(Today);
            Uut.LogCreated += log => Assert.Fail("created existing log: '{0}'", log);

            // ReSharper disable once UnusedVariable
            var currentLog = Uut.CurrentLog;
        }


        [Test(Description = "Manager needs to be thread safe.")]
        public void CreatesNewLogWhileLogsAreIterated()
        {
            GivenLogsExist(SomeDay, SomeDay.AddDays(1));

            // ReSharper disable UnusedVariable
            foreach (var log in Uut.Logs)
            {
                var currentLog = Uut.CurrentLog;
            }
            // ReSharper restore UnusedVariable
        }

        [Test]
        public void DeletesAllLogs()
        {
            GivenLogsExist(Today, Yesterday, TwoDaysAgo, SomeDay);

            Uut.DeleteAllLogs();

            Assert.IsEmpty(Uut.Logs);
        }

        [Test]
        public void DeletesOldLogs()
        {
            GivenLogsExist(Yesterday);

            Uut.DeleteLogsOlderThan(Today);

            Assert.IsEmpty(Uut.Logs);
        }

        [Test]
        public void KeepsNewLogs()
        {
            GivenLogsExist(Today, Yesterday);
            var expectedLogs = Uut.Logs.ToList();

            Uut.DeleteLogsOlderThan(Yesterday);

            Assert.AreEqual(expectedLogs, Uut.Logs);
        }

        // TODO add test for partial deletion of log on same day

        // TODO add tests for determining logs size

        protected void GivenNoLogsExist()
        {
            GivenLogsExist(/* none */);
        }

        protected abstract void GivenLogsExist(params DateTime[] logDates);
    }
}