using System;

namespace KAVE.KAVE_MessageBus.MessageBus
{
    // ReSharper disable once InconsistentNaming
    public interface SMessageBus
    {
        void Publish<TMessage>(TMessage evt) where TMessage : class;

        void Subscribe<TMessage>(Action<TMessage> action, Func<TMessage, bool> filter = null) where TMessage : class;
    }
}
