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
        private CallbackManager _uut;

        [SetUp]
        public void SetUp()
        {
            _testCallbackLock.Reset();
            _uut = new CallbackManager();
        }

        private void TestCallback()
        {
            _testCallbackLock.Set();
        }

        [TestCase(1), TestCase(100)]
        public void ShouldInvokeCallbackAfterDelay(int delay)
        {
            _uut.RegisterCallback(TestCallback, delay);
            AssertCallbackInvocationInTime(delay, _testCallbackLock);
        }

        [Test]
        public void ShouldInvokeCallbackAfterDatetimeNowImmediately()
        {
            _uut.RegisterCallback(TestCallback, DateTime.Now, () => { });
            AssertCallbackInvocationInTime(0, _testCallbackLock);
        }

        [TestCase(1), TestCase(100)]
        public void ShouldInvokeCallbackAtDatetime(int delay)
        {
            _uut.RegisterCallback(TestCallback, DateTime.Now.AddMilliseconds(delay), () => {});
            AssertCallbackInvocationInTime(delay, _testCallbackLock);
        }

        [Test]
        public void ShouldInvokeFinishActionAfterCallback()
        {
            var finishActionLock = new AutoResetEvent(false);
            _uut.RegisterCallback(TestCallback, DateTime.Now, () => finishActionLock.Set());
            Assert.IsTrue(finishActionLock.WaitOne(1000));
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