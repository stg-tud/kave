using System;
using System.Runtime.InteropServices;

namespace KAVE.KAVE_MessageBus.MessageBus
{
    [Guid("04A499BA-CE09-48AF-96D5-F32DEAF0754C")]
    // ReSharper disable once InconsistentNaming
    public interface SMessageBus
    {
        void Publish<TMessage>(TMessage evt) where TMessage : class;

        void Subscribe<TMessage>(Action<TMessage> action, Func<TMessage, bool> filter = null) where TMessage : class;
    }
}