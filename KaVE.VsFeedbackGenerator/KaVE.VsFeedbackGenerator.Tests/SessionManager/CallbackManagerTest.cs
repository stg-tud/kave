using System;
using System.Collections;
using System.Threading;
using KaVE.VsFeedbackGenerator.Utils;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.SessionManager
{
    [TestFixture]
    class CallbackManagerTest
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

        [TestCaseSource(typeof(CallbackManagerTest), "DateValuesData")]
        public void ShouldInvokeCallbackAtDatetime(DateTime dateTimeToExecute, int timout)
        {
            _uut.RegisterCallback(TestCallback, dateTimeToExecute);
            AssertCallbackInvocationInTime(timout);
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
            var isCallbackInvoked = _waitLock.WaitOne(timeoutInMilliseconds + 2 * AcceptedCallbackImprecisionInMilliseconds);
            var duration = DateTime.Now - startTime;
            var differenceFromTimeout = Math.Abs(duration.TotalMilliseconds - timeoutInMilliseconds);
            Assert.IsTrue(isCallbackInvoked);
            Assert.IsTrue(differenceFromTimeout < AcceptedCallbackImprecisionInMilliseconds);
        }

        private static IEnumerable DateValuesData()
        {
            //TODO Ask sven
            yield return new TestCaseData(DateTime.Now.AddMilliseconds(1000), 700);
            yield return new TestCaseData(DateTime.Now.AddMilliseconds(100), 50);
        }
    }
}
