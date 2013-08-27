
using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using NUnit.Framework;

namespace CompletionEventBus.Tests
{
    [TestFixture]
    public class EventBusIntegrationTest
    {
        [Import] private IEventChannel _eventChannel;

        [Test]
        public void ShouldTransmitEvents()
        {
            var eventChannelAssembly = typeof (IEventChannel).Assembly;
            var catalog = new AssemblyCatalog(eventChannelAssembly);
            var container = new CompositionContainer(catalog, CompositionOptions.DisableSilentRejection);
            container.SatisfyImportsOnce(this);

            var received = false;
            _eventChannel.Subscribe<object>(e => received = true);
            _eventChannel.Publish(new Object());
            
            Assert.IsTrue(received);
        }
    }
}
