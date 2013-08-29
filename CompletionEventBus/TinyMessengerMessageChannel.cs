using System;
using System.ComponentModel.Composition;
using TinyMessenger;

namespace CompletionEventBus
{
    [Export(typeof(IMessageChannel))]
    public class TinyMessengerMessageChannel : IMessageChannel
    {
        private readonly ITinyMessengerHub _hub = new TinyMessengerHub();

        public void Publish<TMessage>(TMessage evt) where TMessage : class
        {
            _hub.PublishAsync(new GenericTinyMessage<TMessage>(this, evt));
        }

        public void Subscribe<TMessage>(Action<TMessage> action, Func<TMessage, bool> filter)
            where TMessage : class
        {
            Action<GenericTinyMessage<TMessage>> delegateAction = m => action.Invoke(m.Content);

            if (filter == null)
            {
                Subscribe(delegateAction);
            }
            else
            {
                SubscribeWithFilter(delegateAction, m => filter.Invoke(m.Content));
            }
        }

        private void Subscribe<TMessage>(Action<TMessage> action) where TMessage : class, ITinyMessage
        {
            _hub.Subscribe(action, false);
        }

        private void SubscribeWithFilter<TMessage>(Action<TMessage> action, Func<TMessage, bool> filter)
            where TMessage : class, ITinyMessage
        {
            _hub.Subscribe(action, filter, false);
        }
    }
}
