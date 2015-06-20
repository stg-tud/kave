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
 *    - Sebastian Proksch
 */

using System;
using KaVE.Commons.Model.Events;
using KaVE.VS.FeedbackGenerator.Generators;
using Moq;
using NUnit.Framework;

namespace KaVE.VS.FeedbackGenerator.Tests.Generators
{
    [TestFixture]
    internal class LogEventGeneratorTest : EventGeneratorTestBase
    {
        private LogEventGenerator _uut;

        [SetUp]
        protected void SetUp()
        {
            _uut = new LogEventGenerator(TestRSEnv, TestMessageBus, TestDateUtils);
        }

        [Test]
        public void ExceptionIsTransformedAndPublished()
        {
            var e = CreateException("A", "B");
            _uut.Error(e, "some custom payload");

            var actual = GetSinglePublished<ErrorEvent>();
            var expected = new ErrorEvent
            {
                StackTrace = new[] {"A", "B"},
                Content = "some custom payload"
            };

            AssertSimilarity(expected, actual);
        }

        [Test]
        public void EmptyLinesAreRemoved()
        {
            var e = CreateException("C", "", "D");
            _uut.Error(e, "t2");

            var actual = GetSinglePublished<ErrorEvent>();
            var expected = new ErrorEvent
            {
                StackTrace = new[] { "C", "D" },
                Content = "t2"
            };

            AssertSimilarity(expected, actual);
        }

        [Test]
        public void LinesAreTrimmed()
        {
            var e = CreateException(" E", " ", " F");
            _uut.Error(e, "t3");

            var actual = GetSinglePublished<ErrorEvent>();
            var expected = new ErrorEvent
            {
                StackTrace = new[] { "E", "F" },
                Content = "t3"
            };

            AssertSimilarity(expected, actual);
        }

        [Test]
        public void LogErrorWithoutException()
        {
            _uut.Error("error");

            var actual = GetSinglePublished<ErrorEvent>();
            var expected = new ErrorEvent
            {
                Content = "error"
            };

            AssertSimilarity(expected, actual);
        }

        [Test]
        public void LogErrorWithoutContent()
        {
            _uut.Error(CreateException("G"));

            var actual = GetSinglePublished<ErrorEvent>();
            var expected = new ErrorEvent
            {
                StackTrace = new[] { "G" },
            };

            AssertSimilarity(expected, actual);
        }

        [Test]
        public void ContentDoesNotContainNewLines()
        {
            _uut.Error("A\r\nB");

            var actual = GetSinglePublished<ErrorEvent>();
            var expected = new ErrorEvent
            {
                Content = "A<br />B"
            };

            AssertSimilarity(expected, actual);
        }

        [Test]
        public void ContentDoesNotContainUnixLikeNewLines()
        {
            _uut.Error("A\nB");

            var actual = GetSinglePublished<ErrorEvent>();
            var expected = new ErrorEvent
            {
                Content = "A<br />B"
            };

            AssertSimilarity(expected, actual);
        }

        [Test]
        public void LogInfo()
        {
            _uut.Info("test");

            var actual = GetSinglePublished<InfoEvent>();
            var expected = new InfoEvent
            {
                Info = "test"
            };

            AssertSimilarity(expected, actual);
        }

        [Test]
        public void LogInfoDoesNotContainNewLines()
        {
            _uut.Info("A\r\nB");

            var actual = GetSinglePublished<InfoEvent>();
            var expected = new InfoEvent
            {
                Info = "A<br />B"
            };

            AssertSimilarity(expected, actual);
        }

        [Test]
        public void ShouldNotFailWhenMessageBusFails()
        {
            MockTestMessageBus.Setup(mb => mb.Publish(It.IsAny<IDEEvent>())).Throws(new Exception("TestException"));

            _uut.Error(new Exception("LoggedException"), "comment");
        }

        private static Exception CreateException(params string[] stackTraceParts)
        {
            var toStringOutput = string.Join("\r\n", stackTraceParts);

            var mock = new Mock<Exception>();
            mock.Setup(e => e.ToString()).Returns(toStringOutput);

            return mock.Object;
        }

        private static void AssertSimilarity(ErrorEvent expected, ErrorEvent actual)
        {
            Assert.AreEqual(expected.TriggeredBy, actual.TriggeredBy);
            Assert.AreEqual(expected.Content, actual.Content);
            Assert.AreEqual(expected.StackTrace, actual.StackTrace);
        }

        private static void AssertSimilarity(InfoEvent expected, InfoEvent actual)
        {
            Assert.AreEqual(expected.TriggeredBy, actual.TriggeredBy);
            Assert.AreEqual(expected.Info, actual.Info);
        }
    }
}