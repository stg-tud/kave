using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TinyMessenger.Tests.TestData;
using System.Threading;

namespace TinyMessenger.Tests
{

    [TestClass]
    public class TinyMessengerTests
    {
        [TestMethod]
        public void TinyMessenger_Ctor_DoesNotThrow()
        {
            UtilityMethods.GetMessenger();
        }

        [TestMethod]
        public void Subscribe_ValidDeliverAction_DoesNotThrow()
        {
            var messenger = UtilityMethods.GetMessenger();

            messenger.Subscribe<TestMessage>(UtilityMethods.FakeDeliveryAction);
        }

        [TestMethod]
        public void SubScribe_ValidDeliveryAction_ReturnsRegistrationObject()
        {
            var messenger = UtilityMethods.GetMessenger();

            var output = messenger.Subscribe<TestMessage>(UtilityMethods.FakeDeliveryAction);

            Assert.IsInstanceOfType(output, typeof(TinyMessageSubscriptionToken));
        }

        [TestMethod]
        public void Subscribe_ValidDeliverActionWIthStrongReferences_DoesNotThrow()
        {
            var messenger = UtilityMethods.GetMessenger();

            messenger.Subscribe<TestMessage>(UtilityMethods.FakeDeliveryAction, true);
        }

        [TestMethod]
        public void Subscribe_ValidDeliveryActionAndFilter_DoesNotThrow()
        {
            var messenger = UtilityMethods.GetMessenger();

            messenger.Subscribe<TestMessage>(UtilityMethods.FakeDeliveryAction, UtilityMethods.FakeMessageFilter);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Subscribe_NullDeliveryAction_Throws()
        {
            var messenger = UtilityMethods.GetMessenger();

            messenger.Subscribe<TestMessage>(null, UtilityMethods.FakeMessageFilter);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Subscribe_NullFilter_Throws()
        {
            var messenger = UtilityMethods.GetMessenger();

            messenger.Subscribe<TestMessage>(UtilityMethods.FakeDeliveryAction, null, new TestProxy());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Subscribe_NullProxy_Throws()
        {
            var messenger = UtilityMethods.GetMessenger();

            messenger.Subscribe<TestMessage>(UtilityMethods.FakeDeliveryAction, UtilityMethods.FakeMessageFilter, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Unsubscribe_NullSubscriptionObject_Throws()
        {
            var messenger = UtilityMethods.GetMessenger();

            messenger.Unsubscribe<TestMessage>(null);
        }

        [TestMethod]
        public void Unsubscribe_PreviousSubscription_DoesNotThrow()
        {
            var messenger = UtilityMethods.GetMessenger();
            var subscription = messenger.Subscribe<TestMessage>(UtilityMethods.FakeDeliveryAction, UtilityMethods.FakeMessageFilter);

            messenger.Unsubscribe<TestMessage>(subscription);
        }

        [TestMethod]
        public void Subscribe_PreviousSubscription_ReturnsDifferentSubscriptionObject()
        {
            var messenger = UtilityMethods.GetMessenger();
            var sub1 = messenger.Subscribe<TestMessage>(UtilityMethods.FakeDeliveryAction, UtilityMethods.FakeMessageFilter);
            var sub2 = messenger.Subscribe<TestMessage>(UtilityMethods.FakeDeliveryAction, UtilityMethods.FakeMessageFilter);

            Assert.IsFalse(ReferenceEquals(sub1, sub2));
        }

        [TestMethod]
        public void Subscribe_CustomProxyNoFilter_DoesNotThrow()
        {
            var messenger = UtilityMethods.GetMessenger();
            var proxy = new TestProxy();

            messenger.Subscribe<TestMessage>(UtilityMethods.FakeDeliveryAction, proxy);
        }

        [TestMethod]
        public void Subscribe_CustomProxyWithFilter_DoesNotThrow()
        {
            var messenger = UtilityMethods.GetMessenger();
            var proxy = new TestProxy();

            messenger.Subscribe<TestMessage>(UtilityMethods.FakeDeliveryAction, UtilityMethods.FakeMessageFilter, proxy);
        }

        [TestMethod]
        public void Subscribe_CustomProxyNoFilterStrongReference_DoesNotThrow()
        {
            var messenger = UtilityMethods.GetMessenger();
            var proxy = new TestProxy();

            messenger.Subscribe<TestMessage>(UtilityMethods.FakeDeliveryAction, true, proxy);
        }

        [TestMethod]
        public void Subscribe_CustomProxyFilterStrongReference_DoesNotThrow()
        {
            var messenger = UtilityMethods.GetMessenger();
            var proxy = new TestProxy();

            messenger.Subscribe<TestMessage>(UtilityMethods.FakeDeliveryAction, UtilityMethods.FakeMessageFilter, true, proxy);
        }

        [TestMethod]
        public void Publish_CustomProxyNoFilter_UsesCorrectProxy()
        {
            var messenger = UtilityMethods.GetMessenger();
            var proxy = new TestProxy();
            messenger.Subscribe<TestMessage>(UtilityMethods.FakeDeliveryAction, proxy);
            var message = new TestMessage(this);

            messenger.Publish(message);

            Assert.AreSame(message, proxy.Message);
        }

        [TestMethod]
        public void Publish_CustomProxyWithFilter_UsesCorrectProxy()
        {
            var messenger = UtilityMethods.GetMessenger();
            var proxy = new TestProxy();
            messenger.Subscribe<TestMessage>(UtilityMethods.FakeDeliveryAction, UtilityMethods.FakeMessageFilter, proxy);
            var message = new TestMessage(this);

            messenger.Publish(message);

            Assert.AreSame(message, proxy.Message);
        }

        [TestMethod]
        public void Publish_CustomProxyNoFilterStrongReference_UsesCorrectProxy()
        {
            var messenger = UtilityMethods.GetMessenger();
            var proxy = new TestProxy();
            messenger.Subscribe<TestMessage>(UtilityMethods.FakeDeliveryAction, true, proxy);
            var message = new TestMessage(this);

            messenger.Publish(message);

            Assert.AreSame(message, proxy.Message);
        }

        [TestMethod]
        public void Publish_CustomProxyFilterStrongReference_UsesCorrectProxy()
        {
            var messenger = UtilityMethods.GetMessenger();
            var proxy = new TestProxy();
            messenger.Subscribe<TestMessage>(UtilityMethods.FakeDeliveryAction, UtilityMethods.FakeMessageFilter, true, proxy);
            var message = new TestMessage(this);

            messenger.Publish(message);

            Assert.AreSame(message, proxy.Message);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Publish_NullMessage_Throws()
        {
            var messenger = UtilityMethods.GetMessenger();

            messenger.Publish<TestMessage>(null);
        }

        [TestMethod]
        public void Publish_NoSubscribers_DoesNotThrow()
        {
            var messenger = UtilityMethods.GetMessenger();

            messenger.Publish(new TestMessage(this));
        }

        [TestMethod]
        public void Publish_Subscriber_DoesNotThrow()
        {
            var messenger = UtilityMethods.GetMessenger();
            messenger.Subscribe<TestMessage>(UtilityMethods.FakeDeliveryAction, UtilityMethods.FakeMessageFilter);

            messenger.Publish(new TestMessage(this));
        }

        [TestMethod]
        public void Publish_SubscribedMessageNoFilter_GetsMessage()
        {
            var messenger = UtilityMethods.GetMessenger();
            bool received = false;
            messenger.Subscribe<TestMessage>(m => { received = true; });

            messenger.Publish(new TestMessage(this));

            Assert.IsTrue(received);
        }

        [TestMethod]
        public void Publish_SubscribedThenUnsubscribedMessageNoFilter_DoesNotGetMessage()
        {
            var messenger = UtilityMethods.GetMessenger();
            bool received = false;
            var token = messenger.Subscribe<TestMessage>(m => { received = true; });
            messenger.Unsubscribe<TestMessage>(token);

            messenger.Publish(new TestMessage(this));

            Assert.IsFalse(received);
        }

        [TestMethod]
        public void Publish_SubscribedMessageButFiltered_DoesNotGetMessage()
        {
            var messenger = UtilityMethods.GetMessenger();
            bool received = false;
            messenger.Subscribe<TestMessage>(m => { received = true; }, m => false);

            messenger.Publish(new TestMessage(this));

            Assert.IsFalse(received);
        }

        [TestMethod]
        public void Publish_SubscribedMessageNoFilter_GetsActualMessage()
        {
            var messenger = UtilityMethods.GetMessenger();
            ITinyMessage receivedMessage = null;
            var payload = new TestMessage(this);
            messenger.Subscribe<TestMessage>(m => { receivedMessage = m; });

            messenger.Publish(payload);

            Assert.AreSame(payload, receivedMessage);
        }

        [TestMethod]
        public void GenericTinyMessage_String_SubscribeDoesNotThrow()
        {
            var messenger = UtilityMethods.GetMessenger();
            messenger.Subscribe<GenericTinyMessage<string>>(m => { });
        }

        [TestMethod]
        public void GenericTinyMessage_String_PubishDoesNotThrow()
        {
            var messenger = UtilityMethods.GetMessenger();
            messenger.Publish(new GenericTinyMessage<string>(this, "Testing"));
        }

        [TestMethod]
        public void GenericTinyMessage_String_PubishAndSubscribeDeliversContent()
        {
            var messenger = UtilityMethods.GetMessenger();
            var output = string.Empty;
            messenger.Subscribe<GenericTinyMessage<string>>(m => { output = m.Content; });
            messenger.Publish(new GenericTinyMessage<string>(this, "Testing"));

            Assert.AreEqual("Testing", output);
        }

        [TestMethod]
        public void Publish_SubscriptionThrowingException_DoesNotThrow()
        {
            var messenger = UtilityMethods.GetMessenger();
            messenger.Subscribe<GenericTinyMessage<string>>((m) => { throw new NotImplementedException(); });

            messenger.Publish(new GenericTinyMessage<string>(this, "Testing"));
        }

        [TestMethod]
        public void PublishAsync_NoCallback_DoesNotThrow()
        {
            var messenger = UtilityMethods.GetMessenger();

            messenger.PublishAsync(new TestMessage(this));
        }

        [TestMethod]
        public void PublishAsync_NoCallback_PublishesMessage()
        {
            var messenger = UtilityMethods.GetMessenger();
            bool received = false;
            messenger.Subscribe<TestMessage>(m => { received = true; });

            messenger.PublishAsync(new TestMessage(this));

            // Horrible wait loop!
            int waitCount = 0;
            while (!received && waitCount < 100)
            {
                Thread.Sleep(10);
                waitCount++;
            }
            Assert.IsTrue(received);
        }

        [TestMethod]
        public void PublishAsync_Callback_DoesNotThrow()
        {
            var messenger = UtilityMethods.GetMessenger();
#pragma warning disable 219
            messenger.PublishAsync(new TestMessage(this), r => { });
#pragma warning restore 219
        }

        [TestMethod]
        public void PublishAsync_Callback_PublishesMessage()
        {
            var messenger = UtilityMethods.GetMessenger();
            bool received = false;
            messenger.Subscribe<TestMessage>(m => { received = true; });

#pragma warning disable 219
            messenger.PublishAsync(new TestMessage(this), r => { });
#pragma warning restore 219

            // Horrible wait loop!
            int waitCount = 0;
            while (!received && waitCount < 100)
            {
                Thread.Sleep(10);
                waitCount++;
            }
            Assert.IsTrue(received);
        }

        [TestMethod]
        public void PublishAsync_Callback_CallsCallback()
        {
            var messenger = UtilityMethods.GetMessenger();
            bool received = false;
            bool callbackReceived = false;
            messenger.Subscribe<TestMessage>(m => { received = true; });

            messenger.PublishAsync(new TestMessage(this), r => { callbackReceived = true; });

            // Horrible wait loop!
            int waitCount = 0;
            while (!callbackReceived && waitCount < 100)
            {
                Thread.Sleep(10);
                waitCount++;
            }
            Assert.IsTrue(received);
        }

        [TestMethod]
        public void CancellableGenericTinyMessage_Publish_DoesNotThrow()
        {
            var messenger = UtilityMethods.GetMessenger();
#pragma warning disable 219
            messenger.Publish(new CancellableGenericTinyMessage<string>(this, "Testing", () => { }));
#pragma warning restore 219
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CancellableGenericTinyMessage_PublishWithNullAction_Throws()
        {
            var messenger = UtilityMethods.GetMessenger();
            messenger.Publish(new CancellableGenericTinyMessage<string>(this, "Testing", null));
        }

        [TestMethod]
        public void CancellableGenericTinyMessage_SubscriberCancels_CancelActioned()
        {
            var messenger = UtilityMethods.GetMessenger();
            bool cancelled = false;
            messenger.Subscribe<CancellableGenericTinyMessage<string>>(m => m.Cancel());

            messenger.Publish(new CancellableGenericTinyMessage<string>(this, "Testing", () => { cancelled = true; }));

            Assert.IsTrue(cancelled);
        }

        [TestMethod]
        public void CancellableGenericTinyMessage_SeveralSubscribersOneCancels_CancelActioned()
        {
            var messenger = UtilityMethods.GetMessenger();
            bool cancelled = false;
#pragma warning disable 219
            messenger.Subscribe<CancellableGenericTinyMessage<string>>(m => { });
            messenger.Subscribe<CancellableGenericTinyMessage<string>>(m => m.Cancel());
            messenger.Subscribe<CancellableGenericTinyMessage<string>>(m => { });
#pragma warning restore 219
            messenger.Publish(new CancellableGenericTinyMessage<string>(this, "Testing", () => { cancelled = true; }));

            Assert.IsTrue(cancelled);
        }
    }
}