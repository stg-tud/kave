using System;

namespace CompletionEventBus
{
    public interface IEventChannel
    {
        void Publish<TEvent>(TEvent evt) where TEvent : class;

        void Subscribe<TEvent>(Action<TEvent> action, Func<TEvent, bool> filter = null) where TEvent : class;
    }
}
