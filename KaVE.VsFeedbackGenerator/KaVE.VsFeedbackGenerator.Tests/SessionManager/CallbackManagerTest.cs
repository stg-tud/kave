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
        private readonly AutoResetEvent _testCallbackLock = new AutoResetEvent(false);
        private readonly AutoResetEvent _finishActionLock = new AutoResetEvent(false);
        private CallbackManager _uut;

        [SetUp]
        public void SetUp()
        {
            _testCallbackLock.Reset();
            _finishActionLock.Reset();
            _uut = new CallbackManager();
        }

        private void TestCallback()
        {
            _testCallbackLock.Set();
        }

        private void FinishAction()
        {
            _finishActionLock.Set();
        }

        [TestCase(1), TestCase(1000)]
        public void ShouldInvokeCallbackAfterDelay(int delay)
        {
            _uut.RegisterCallback(TestCallback, delay);
            AssertCallbackInvocationInTime(delay, _testCallbackLock);
        }

        [Test]
        public void ShouldInvokeCallbackAfterDatetimeNowImmediately()
        {
            _uut.RegisterCallback(TestCallback, DateTime.Now);
            AssertCallbackInvocationInTime(0, _testCallbackLock);
        }

        [TestCase(100), TestCase(1000)]
        public void ShouldInvokeCallbackAtDatetime(int delay)
        {
            _uut.RegisterCallback(TestCallback, DateTime.Now.AddMilliseconds(delay));
            AssertCallbackInvocationInTime(delay, _testCallbackLock);
        }


        [Test]
        public void ShouldInvokeFinishActionAfterCallback()
        {
            const int delay = 100;
            _uut.RegisterCallback(TestCallback, DateTime.Now.AddMilliseconds(delay), FinishAction);
            AssertCallbackInvocationInTime(delay, _testCallbackLock);
            AssertCallbackInvocationInTime(0, _finishActionLock);
        }

        [Test]
        public void ShouldNotInvokeMultipleTimes()
        {
            _uut.RegisterCallback(TestCallback, 1);
            AssertCallbackInvocationInTime(1, _testCallbackLock);
            var isCallbackInvokedAgain = _testCallbackLock.WaitOne(100);
            Assert.IsFalse(isCallbackInvokedAgain);
        }

        private void AssertCallbackInvocationInTime(int timeoutInMilliseconds, WaitHandle functionLock)
        {
            var startTime = DateTime.Now;
            var isCallbackInvoked =
                functionLock.WaitOne(timeoutInMilliseconds + 2*AcceptedCallbackImprecisionInMilliseconds);
            var duration = DateTime.Now - startTime;
            var differenceFromTimeout = Math.Abs(duration.TotalMilliseconds - timeoutInMilliseconds);
            Assert.IsTrue(isCallbackInvoked);
            Assert.IsTrue(differenceFromTimeout < AcceptedCallbackImprecisionInMilliseconds);
        }
    }
}