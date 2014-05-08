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
using KaVE.Model.Events;
using KaVE.VsFeedbackGenerator.Generators;
using Moq;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.Generators
{
    [TestFixture]
    internal class LogEventGeneratorTest : EventGeneratorTestBase
    {
        private LogEventGenerator _sut;

        [SetUp]
        public void SetUp()
        {
            _sut = new LogEventGenerator(new TestIDESession(), TestMessageBus);
        }

        [Test]
        public void ExceptionIsTransformedAndPublished()
        {
            var e = CreateException("A", "B");
            _sut.Error(e, "some custom payload");

            var actual = WaitForNewEvent<ErrorEvent>();
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
            _sut.Error(e, "t2");

            var actual = WaitForNewEvent<ErrorEvent>();
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
            _sut.Error(e, "t3");

            var actual = WaitForNewEvent<ErrorEvent>();
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
            _sut.Error("error");

            var actual = WaitForNewEvent<ErrorEvent>();
            var expected = new ErrorEvent
            {
                Content = "error"
            };

            AssertSimilarity(expected, actual);
        }

        [Test]
        public void LogErrorWithoutContent()
        {
            _sut.Error(CreateException("G"));

            var actual = WaitForNewEvent<ErrorEvent>();
            var expected = new ErrorEvent
            {
                StackTrace = new[] { "G" },
            };

            AssertSimilarity(expected, actual);
        }

        [Test]
        public void ContentDoesNotContainNewLines()
        {
            _sut.Error("A\r\nB");

            var actual = WaitForNewEvent<ErrorEvent>();
            var expected = new ErrorEvent
            {
                Content = "A<br />B"
            };

            AssertSimilarity(expected, actual);
        }

        [Test]
        public void ContentDoesNotContainUnixLikeNewLines()
        {
            _sut.Error("A\nB");

            var actual = WaitForNewEvent<ErrorEvent>();
            var expected = new ErrorEvent
            {
                Content = "A<br />B"
            };

            AssertSimilarity(expected, actual);
        }

        [Test]
        public void LogInfo()
        {
            _sut.Info("test");

            var actual = WaitForNewEvent<InfoEvent>();
            var expected = new InfoEvent
            {
                Info = "test"
            };

            AssertSimilarity(expected, actual);
        }

        [Test]
        public void LogInfoDoesNotContainNewLines()
        {
            _sut.Info("A\r\nB");

            var actual = WaitForNewEvent<InfoEvent>();
            var expected = new InfoEvent
            {
                Info = "A<br />B"
            };

            AssertSimilarity(expected, actual);
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