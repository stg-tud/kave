using System;
using System.Collections.Generic;
using JetBrains.Application;
using JetBrains.DataStructures;
using KaVE.MessageBus.MessageBus;

namespace KaVE.EventGenerator.ReSharper8.IntegrationTests.VsIntegrationMocks
{
    [ShellComponent]
    class TestMessageBus : SMessageBus
    {
        private readonly IList<object> _messages = new List<object>();
        private readonly IDictionary<Type, object> _subscribers = new Dictionary<Type, object>();

        public ImmutableArray<object> PublishedMessages
        {
            get
            {
                return _messages.ToImmutableArray();
            }
        }

        public void Publish<TMessage>(TMessage evt) where TMessage : class
        {
            _messages.Add(evt);

            if (_subscribers.ContainsKey(typeof (TMessage)))
            {
                var subscriber = (Action<TMessage>) _subscribers[typeof(TMessage)];
                subscriber(evt);
            }
        }

        public void Subscribe<TMessage>(Action<TMessage> action, Func<TMessage, bool> filter = null)
            where TMessage : class
        {
            _subscribers.Add(
                typeof (TMessage),
                (Action<TMessage>) (m => {
                         if (filter == null || filter(m)) action(m);
                })

    );
        }
    }
}
