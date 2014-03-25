using System;
using System.Threading;
using KaVE.VsFeedbackGenerator.Utils;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.SessionManager
{
    [TestFixture]
    internal class CallbackManagerTest
    {
        private const int AcceptedCallbackImprecisionInMilliseconds = 50;
        private readonly AutoResetEvent _waitLock = new AutoResetEvent(false);
        private CallbackManager _uut;

        [SetUp]
        public void SetUp()
        {
            _waitLock.Reset();
            _uut = new CallbackManager();
        }

        private void TestCallback()
        {
            _waitLock.Set();
        }

        [TestCase(1), TestCase(1000)]
        public void ShouldInvokeCallbackAfterDelay(int delay)
        {
            _uut.RegisterCallback(TestCallback, delay);
            AssertCallbackInvocationInTime(delay);
        }

        [Test]
        public void ShouldInvokeCallbackAfterDatetimeNowImmediately()
        {
            _uut.RegisterCallback(TestCallback, DateTime.Now);
            AssertCallbackInvocationInTime(0);
        }

        [TestCase(100), TestCase(1000)]
        public void ShouldInvokeCallbackAtDatetime(int delay)
        {
            _uut.RegisterCallback(TestCallback, DateTime.Now.AddMilliseconds(delay));
            AssertCallbackInvocationInTime(delay);
        }

        [Test]
        public void ShouldInvokeFinishActionAfterCallback()
        {
            const int delay = 100;
            _uut.RegisterCallback(TestCallback, DateTime.Now.AddMilliseconds(delay), TestCallback);
            AssertCallbackInvocationInTime(delay);
            _waitLock.Reset();
            AssertCallbackInvocationInTime(0);
        }

        [Test]
        public void ShouldNotInvokeMultipleTimes()
        {
            _uut.RegisterCallback(TestCallback, 1);
            AssertCallbackInvocationInTime(1);
            var isCallbackInvokedAgain = _waitLock.WaitOne(100);
            Assert.IsFalse(isCallbackInvokedAgain);
        }

        private void AssertCallbackInvocationInTime(int timeoutInMilliseconds)
        {
            var startTime = DateTime.Now;
            var isCallbackInvoked =
                _waitLock.WaitOne(timeoutInMilliseconds + 2*AcceptedCallbackImprecisionInMilliseconds);
            var duration = DateTime.Now - startTime;
            var differenceFromTimeout = Math.Abs(duration.TotalMilliseconds - timeoutInMilliseconds);
            Assert.IsTrue(isCallbackInvoked);
            Assert.IsTrue(differenceFromTimeout < AcceptedCallbackImprecisionInMilliseconds);
        }
    }
}