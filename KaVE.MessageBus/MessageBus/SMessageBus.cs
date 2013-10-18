using System;
using System.Runtime.InteropServices;

namespace KAVE.KAVE_MessageBus.MessageBus
{
    [Guid(GuidList.GuidKaveMessageBusService)]
    // ReSharper disable once InconsistentNaming
    public interface SMessageBus
    {
        void Publish<TMessage>(TMessage evt) where TMessage : class;

        void Subscribe<TMessage>(Action<TMessage> action, Func<TMessage, bool> filter = null) where TMessage : class;
    }
}