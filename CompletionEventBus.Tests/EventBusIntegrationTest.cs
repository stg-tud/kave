
using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Threading;
using NUnit.Framework;

namespace CompletionEventBus.Tests
{
    [TestFixture]
    public class EventBusIntegrationTest
    {
        [Import] private IMessageChannel _messageChannel;

        [Test]
        public void ShouldTransmitMessages()
        {
            var messageChannelAssembly = typeof (IMessageChannel).Assembly;
            var catalog = new AssemblyCatalog(messageChannelAssembly);
            var container = new CompositionContainer(catalog);
            container.SatisfyImportsOnce(this);

            var messageReceivedEvent = new ManualResetEvent(false);
            _messageChannel.Subscribe<object>(e => messageReceivedEvent.Set());
            _messageChannel.Publish(new Object());

            Assert.IsTrue(messageReceivedEvent.WaitOne(50));
        }
    }
}
