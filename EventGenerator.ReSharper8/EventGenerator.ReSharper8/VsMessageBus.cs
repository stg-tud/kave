using System;
using JetBrains.Application;
using JetBrains.Application.Components;
using JetBrains.VsIntegration.Application;
using KaVE.MessageBus.MessageBus;

namespace KaVE.EventGenerator.ReSharper8
{
    [ShellComponent(ProgramConfigurations.VS_ADDIN)]
    public class VsMessageBus : SMessageBus
    {
        private readonly SMessageBus _messageBus;

        public VsMessageBus(RawVsServiceProvider serviceProvider)
        {
            _messageBus = serviceProvider.Value.GetService<SMessageBus, SMessageBus>();
        }

        public void Publish<TMessage>(TMessage evt) where TMessage : class
        {
            _messageBus.Publish(evt);
        }

        public void Subscribe<TMessage>(Action<TMessage> action, Func<TMessage, bool> filter = null) where TMessage : class
        {
            _messageBus.Subscribe(action, filter);
        }
    }
}
