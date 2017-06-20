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
using KaVE.Commons.Utils;
using KaVE.Commons.Utils.Assertion;
using KaVE.FeedbackProcessor.Preprocessing.Logging;
using Moq;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.Preprocessing.Logging
{
    internal class AppendingFileLoggerTest
    {
        #region setup and helpers

        private DateTime _now;
        private IDateUtils _dateUtils;

        private string _dirTmp;
        private AppendingFileLogger _sut;

        [SetUp]
        public void SetUp()
        {
            CultureUtils.SetDefaultCultureForThisThread();

            _now = DateTime.MinValue;
            _dateUtils = Mock.Of<IDateUtils>();
            Mock.Get(_dateUtils).Setup(u => u.Now).Returns(() => _now);

            _dirTmp = CreateTempDir();
            _sut = Create(Log("a.log"));
        }

        private static string CreateTempDir()
        {
            var path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(path);
            return path;
        }

        private AppendingFileLogger Create(string logFile)
        {
            return new AppendingFileLogger(logFile, _dateUtils);
        }

        private string Log(string fileName)
        {
            return Path.Combine(_dirTmp, fileName);
        }

        [TearDown]
        public void Teardown()
        {
            _sut.Dispose();
            if (Directory.Exists(_dirTmp))
            {
                Directory.Delete(_dirTmp, true);
            }
        }

        private void AssertLog(string expected)
        {
            _sut.Dispose();
            var logFile = Log("a.log");
            Assert.IsTrue(File.Exists(logFile));
            var actual = File.ReadAllText(logFile);
            Assert.AreEqual(expected, actual);
        }

        #endregion

        [Test, ExpectedException(typeof(AssertException))]
        public void UtilsCannotBeNull()
        {
            _sut = new AppendingFileLogger(Log("a.log"), null);
        }

        [Test, ExpectedException(typeof(AssertException))]
        public void FileCannotBeNull()
        {
            _sut = Create(null);
        }

        [Test, ExpectedException(typeof(AssertException))]
        public void FileCannotBeEmpty()
        {
            _sut = Create("");
        }

        [Test, ExpectedException(typeof(AssertException))]
        public void MustHaveParent()
        {
            _sut = Create("a.log");
        }

        [Test, ExpectedException(typeof(AssertException))]
        public void ParentMustExist()
        {
            _sut = Create(Log(Path.Combine("x", "a.log")));
        }

        [Test, Ignore("changed when switching to FileStream based solution")]
        public void FileIsNotCreatedWithoutLog()
        {
            Assert.IsFalse(File.Exists(Log("a.log")));
        }

        [Test]
        public void FileIsCreatedWhenLogging()
        {
            _sut.Log("...");
            Assert.IsTrue(File.Exists(Log("a.log")));
        }

        [Test]
        public void LogIsCorrect()
        {
            _sut.Log("...");
            AssertLog("1/1/0001 12:00:00 AM ...");
        }

        [Test]
        public void ExistingFileIsAppended()
        {
            _sut.Log("a");
            _sut.Dispose();

            _sut = Create(Log("a.log"));
            _sut.Log("b");

            AssertLog(
                "1/1/0001 12:00:00 AM a\n" +
                "1/1/0001 12:00:00 AM b");
        }

        [Test]
        public void AllMethodsWorkAsExpected()
        {
            _sut.Log("a");
            _sut.Append("b");
            _sut.Log("c");
            _sut.Log();
            _now += TimeSpan.FromSeconds(1);
            _sut.Log("d");

            AssertLog(
                "1/1/0001 12:00:00 AM ab\n" +
                "1/1/0001 12:00:00 AM c\n" +
                "1/1/0001 12:00:00 AM \n" +
                "1/1/0001 12:00:01 AM d");
        }

        [Test]
        public void AppendAlsoBreaksFirstLine()
        {
            _sut.Append("a");
            _sut.Log("b");

            AssertLog(
                "a\n" +
                "1/1/0001 12:00:00 AM b");
        }

        [Test]
        public void CanLogInvalidReplacementStrings()
        {
            _sut.Log("{x}");

            AssertLog(
                "1/1/0001 12:00:00 AM {x}");
        }

        [Test]
        public void CanAppendInvalidReplacementStrings()
        {
            _sut.Append("{x}");

            AssertLog(
                "{x}");
        }
    }
}