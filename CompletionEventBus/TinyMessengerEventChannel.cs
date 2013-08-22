using System;
using System.ComponentModel.Composition;
using TinyMessenger;

namespace CompletionEventBus
{
    [Export(typeof(IEventChannel))]
    class TinyMessengerEventChannel : IEventChannel
    {
        [Import] private TinyMessengerHub _hub;

        public void Publish<TEvent>(TEvent evt) where TEvent : class
        {
            _hub.Publish(new GenericTinyMessage<TEvent>(this, evt));
        }

        public void Subscribe<TEvent>(Action<TEvent> action, Func<TEvent, bool> filter)
            where TEvent : class
        {
            Action<GenericTinyMessage<TEvent>> delegateAction = m => action.Invoke(m.Content);

            if (filter == null)
            {
                Subscribe(delegateAction);
            }
            else
            {
                SubscribeWithFilter(delegateAction, m => filter.Invoke(m.Content));
            }
        }

        private void SubscribeWithFilter<TEvent>(Action<TEvent> action, Func<TEvent, bool> filter)
            where TEvent : class, ITinyMessage
        {
            _hub.Subscribe(action, filter, false);
        }

        private void Subscribe<TEvent>(Action<TEvent> action) where TEvent : class, ITinyMessage
        {
            _hub.Subscribe(action, false);
        }
    }
}
