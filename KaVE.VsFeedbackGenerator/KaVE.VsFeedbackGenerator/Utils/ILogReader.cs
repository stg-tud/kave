using System;
using System.Collections.Generic;
using KaVE.JetBrains.Annotations;

namespace KaVE.VsFeedbackGenerator.Utils
{
    public interface ILogReader<out TMessage> : IDisposable
    {
        TMessage ReadNext();

        [NotNull]
        IEnumerable<TMessage> ReadAll();
    }
}