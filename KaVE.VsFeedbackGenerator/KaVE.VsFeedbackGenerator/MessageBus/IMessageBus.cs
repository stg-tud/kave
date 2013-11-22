using System;

namespace KaVE.VsFeedbackGenerator.MessageBus
{
    public interface IMessageBus
    {
        void Publish<TMessage>(TMessage evt) where TMessage : class;

        void Subscribe<TMessage>(Action<TMessage> action, Func<TMessage, bool> filter = null) where TMessage : class;
    }
}