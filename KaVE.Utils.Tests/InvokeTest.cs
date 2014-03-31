using System;
using System.Threading;
using NUnit.Framework;

namespace KaVE.Utils.Tests
{
    [TestFixture]
    internal class InvokeTest
    {
        private const int AcceptedInvocationDelay = 50;

        private AutoResetEvent _scheduledInvocationEvent;
        private AutoResetEvent _finishedInvocationEvent;

        [SetUp]
        public void SetUp()
        {
            _scheduledInvocationEvent = new AutoResetEvent(false);
            _finishedInvocationEvent = new AutoResetEvent(false);
        }

        private void TestAction()
        {
            _scheduledInvocationEvent.Set();
        }

        private void TestFinishAction()
        {
            _finishedInvocationEvent.Set();
        }

        [Test, Timeout(1000)]
        public void ShouldInvokeAction()
        {
            Invoke.Async(TestAction);
            Assert.IsTrue(_scheduledInvocationEvent.WaitOne(1000));
        }

        [Test]
        public void ShouldInvokeActionImmediatelyWhenScheduledWithNoDelay()
        {
            Invoke.Later(TestAction, 0);

            AssertActionIsInvoked(0);
        }

        [Test]
        public void ShouldInvokeImmediatelyWhenScheduledWithNegativeDelay()
        {
            Invoke.Later(TestAction, -5);

            AssertActionIsInvoked(0);
        }

        [TestCase(1), TestCase(10), TestCase(100)]
        public void ShouldInvokeActionAfterExplicitDelay(int delay)
        {
            Invoke.Later(TestAction, delay);

            AssertActionIsInvoked(delay);
        }

        [Test]
        public void ShouldInvokeFinishedActionAfterAction()
        {
            const int delay = 10;

            Invoke.Later(() => { }, delay, TestFinishAction);

            AssertFinishActionIsInvoked(delay);
        }

        [Test]
        public void ShouldInvokeActionImmediatelyWhenScheduledForNow()
        {
            Invoke.Later(TestAction, System.DateTime.Now);

            AssertActionIsInvoked(0);
        }

        [Test]
        public void ShouldInvokeImmediatelyWhenScheduledForPastDate()
        {
            Invoke.Later(TestAction, System.DateTime.Now.AddSeconds(-5));

            AssertActionIsInvoked(0);
        }

        [TestCase(1), TestCase(10), TestCase(100)]
        public void ShouldInvokeActionOnScheduleDate(int delay)
        {
            Invoke.Later(TestAction, System.DateTime.Now.AddMilliseconds(delay));

            AssertActionIsInvoked(delay);
        }

        private void AssertActionIsInvoked(int offsetInMillis)
        {
            AssertIsInvoked(offsetInMillis, _scheduledInvocationEvent);
        }

        private void AssertFinishActionIsInvoked(int offsetInMillis)
        {
            AssertIsInvoked(offsetInMillis, _finishedInvocationEvent);
        }

        private static void AssertIsInvoked(int offsetInMillis, WaitHandle invocationEvent)
        {
            // some time might have passed from scheduling till here
            var earliestInvocationOffset = Math.Max(0, offsetInMillis - 5);
            Assert.IsFalse(invocationEvent.WaitOne(earliestInvocationOffset));
            // invokation might be slightly delayed
            var latestInvocationDelay = offsetInMillis + AcceptedInvocationDelay;
            Assert.IsTrue(invocationEvent.WaitOne(latestInvocationDelay));
        }
    }
}