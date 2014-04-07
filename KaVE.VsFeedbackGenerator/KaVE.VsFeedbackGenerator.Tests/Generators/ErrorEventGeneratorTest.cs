using System;
using KaVE.Model.Events;
using KaVE.VsFeedbackGenerator.Generators;
using Moq;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.Generators
{
    [TestFixture]
    internal class ErrorEventGeneratorTest : EventGeneratorTestBase
    {
        private ErrorEventGenerator _sut;

        [SetUp]
        public void SetUp()
        {
            _sut = new ErrorEventGenerator(new TestIDESession(), TestMessageBus);
        }

        [Test]
        public void ExceptionIsTransformedAndPublished()
        {
            var e = CreateException("A", "B");
            _sut.Log(e, "some custom payload");

            var actual = WaitForNewEvent<ErrorEvent>();
            var expected = new ErrorEvent
            {
                TriggeredBy = IDEEvent.Trigger.Automatic,
                StackTrace = new[] {"A", "B"},
                Content = "some custom payload"
            };

            AssertSimilarity(expected, actual);
        }

        [Test]
        public void EmptyLinesAreRemoved()
        {
            var e = CreateException("C", "", "D");
            _sut.Log(e, "t2");

            var actual = WaitForNewEvent<ErrorEvent>();
            var expected = new ErrorEvent
            {
                TriggeredBy = IDEEvent.Trigger.Automatic,
                StackTrace = new[] { "C", "D" },
                Content = "t2"
            };

            AssertSimilarity(expected, actual);
        }

        [Test]
        public void LinesAreTrimmed()
        {
            var e = CreateException(" E", " ", " F");
            _sut.Log(e, "t3");

            var actual = WaitForNewEvent<ErrorEvent>();
            var expected = new ErrorEvent
            {
                TriggeredBy = IDEEvent.Trigger.Automatic,
                StackTrace = new[] { "E", "F" },
                Content = "t3"
            };

            AssertSimilarity(expected, actual);
        }

        [Test]
        public void LogErrorWithoutException()
        {
            _sut.Log("error");

            var actual = WaitForNewEvent<ErrorEvent>();
            var expected = new ErrorEvent
            {
                TriggeredBy = IDEEvent.Trigger.Automatic,
                Content = "error"
            };

            AssertSimilarity(expected, actual);
        }

        [Test]
        public void LogErrorWithoutContent()
        {
            _sut.Log(CreateException("G"));

            var actual = WaitForNewEvent<ErrorEvent>();
            var expected = new ErrorEvent
            {
                TriggeredBy = IDEEvent.Trigger.Automatic,
                StackTrace = new[] { "G" },
            };

            AssertSimilarity(expected, actual);
        }

        [Test]
        public void ContentDoesNotContainNewLines()
        {
            _sut.Log("A\r\nB");

            var actual = WaitForNewEvent<ErrorEvent>();
            var expected = new ErrorEvent
            {
                TriggeredBy = IDEEvent.Trigger.Automatic,
                Content = "A<br />B"
            };

            AssertSimilarity(expected, actual);
        }

        [Test]
        public void ContentDoesNotContainUnixLikeNewLines()
        {
            _sut.Log("A\nB");

            var actual = WaitForNewEvent<ErrorEvent>();
            var expected = new ErrorEvent
            {
                TriggeredBy = IDEEvent.Trigger.Automatic,
                Content = "A<br />B"
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
    }
}