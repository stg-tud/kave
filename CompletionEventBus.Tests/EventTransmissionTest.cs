using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using NUnit.Framework;

namespace CompletionEventBus.Tests
{
    [TestFixture]
    public class EventTransmissionTest
    {
        private class TestEvent
        {
            public string Id { get; set; }
        }

        private IEventChannel GetEventChannel()
        {
            var catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new AssemblyCatalog(typeof (IEventChannel).Assembly));
            var container = new CompositionContainer(catalog);
            var channel = container.GetExport<IEventChannel>();
            if (channel != null)
            {
                return channel.Value;
            }
            Assert.Fail("Could not instantiate channel");
            return null;
        }

        [Test]
        public void ShouldSucceedToSendEvent()
        {
            var eventChannel = GetEventChannel();
            eventChannel.Publish(new TestEvent {Id = "DEADBEEF"});
        }

        [Test]
        public void ShouldSucceedOnListenerRegistration()
        {
            var eventChannel = GetEventChannel();
            eventChannel.Subscribe<TestEvent>(e => {});
        }

        [Test]
        public void ShouldTransmitMessage()
        {
            var evt = new TestEvent { Id = "jodeldiplom!" };
            var received = false;
            var eventChannel = GetEventChannel();
            eventChannel.Subscribe<TestEvent>(e => { received = true; Assert.AreEqual(e.Id, evt.Id); });
            eventChannel.Publish(evt);
            Assert.IsTrue(received);
        }
    }
}
