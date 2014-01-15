using System;
using NUnit.Framework;

namespace KaVE.Utils.Tests.Events
{
    [TestFixture]
    internal class EventHandlingTest
    {
        private event EventHandler MyEvent = delegate { };

        private event EventHandler MyDelegatingEvent = delegate { };

        [Test]
        public void ShouldInvokeHandler()
        {
            var invoked = false;

            MyEvent += (sender, args) => invoked = true;
            MyEvent(this, EventArgs.Empty);

            Assert.IsTrue(invoked);
        }

        [Test]
        public void ShouldDelegateEvent()
        {
            var invoked = false;

            MyDelegatingEvent += (sender, args) => MyEvent(sender, args);
            MyEvent += (sender, args) => invoked = true;
            MyDelegatingEvent(this, EventArgs.Empty);

            Assert.IsTrue(invoked);
        }
    }
}