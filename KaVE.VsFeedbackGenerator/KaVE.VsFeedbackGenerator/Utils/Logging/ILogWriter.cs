using System;
using System.Collections.Generic;
using KaVE.JetBrains.Annotations;

namespace KaVE.VsFeedbackGenerator.Utils.Logging
{
    public interface ILogWriter<in TMessage> : IDisposable
    {
        void Write([NotNull] TMessage message);

        void WriteAll([NotNull] IEnumerable<TMessage> messages);
    }
}