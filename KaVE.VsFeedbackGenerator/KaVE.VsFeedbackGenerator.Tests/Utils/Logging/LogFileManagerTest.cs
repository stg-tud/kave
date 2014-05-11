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
using System.IO;
using System.Linq;
using KaVE.Utils.IO;
using KaVE.VsFeedbackGenerator.Utils;
using KaVE.VsFeedbackGenerator.Utils.Logging;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.Utils.Logging
{
    [TestFixture]
    internal class LogFileManagerTest
    {
        private LogFileManager<string> _uut;

        [SetUp]
        public void SetUp()
        {
            //TODO: from Uli: Dead code? IoUtil is never used 
            Registry.RegisterComponent<IIoUtils>(new IoUtils());
            _uut = new LogFileManager<string>(IoTestHelper.GetTempDirectoryName());
        }

        [TearDown]
        public void CleanUp()
        {
            Registry.Clear();
        }

        [Test]
        public void ShouldFindNoLogFilesInEmptyDirectory()
        {
            Assert.AreEqual(0, _uut.GetLogs().Count());
        }

        [Test]
        public void ShouldFindSingleLog()
        {
            GivenLogExists(DateTime.Today);

            Assert.AreEqual(1, _uut.GetLogs().Count());
        }

        [Test]
        public void ShouldFindMultipleLogs()
        {
            GivenLogExists(DateTime.Today);
            GivenLogExists(DateTime.Today.AddDays(-1));
            GivenLogExists(DateTime.Today.AddDays(-2));

            Assert.AreEqual(3, _uut.GetLogs().Count());
        }

        [Test]
        public void ShouldIgnoreNonLogDirectory()
        {
            //TODO from Uli: Use IoUtil methods for Directory create?
            Directory.CreateDirectory(Path.Combine(_uut.BaseLocation, "NonLog"));

            Assert.AreEqual(0, _uut.GetLogs().Count());
        }

        [Test]
        public void ShouldIgnoreNonLogFile()
        {
            File.Create(Path.Combine(_uut.BaseLocation, "NonLog"));

            Assert.AreEqual(0, _uut.GetLogs().Count());
        }

        [Test]
        public void ShouldReturnTodaysLog()
        {
            var todaysLog = (LogFile<string>) _uut.CurrentLog;

            Assert.AreEqual(DateTime.Today, todaysLog.Date);
            Assert.AreEqual(_uut.BaseLocation, Path.GetDirectoryName(todaysLog.Path));
        }

        [Test]
        public void ShouldDeleteOldLogs()
        {
            GivenLogExists(DateTime.Today);
            GivenLogExists(DateTime.Today.AddDays(-1));
            GivenLogExists(DateTime.Today.AddDays(-2));

            _uut.DeleteLogsOlderThan(DateTime.Now.AddDays(-1));

            var expected = new[] {_uut.CurrentLog};
            var actual = _uut.GetLogs();
            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldDeleteLogFileDirectory()
        {
            GivenLogExists(DateTime.Today);

            _uut.DeleteLogFileDirectory();

            var actual = !Directory.EnumerateFiles(_uut.BaseLocation).Any();
            Assert.IsTrue(actual);
        }

        private void GivenLogExists(DateTime logDate)
        {
            var filename = "Log_" + logDate.ToString("yyyy-MM-dd");
            var logPath = Path.Combine(_uut.BaseLocation, filename);
            File.Create(logPath).Close();
        }
    }
}