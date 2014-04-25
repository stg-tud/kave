using System;
using System.Collections.Generic;
using KaVE.JetBrains.Annotations;

namespace KaVE.VsFeedbackGenerator.Utils.Logging
{
    public interface ILogReader<out TMessage> : IDisposable
    {
        TMessage ReadNext();

        [NotNull]
        IEnumerable<TMessage> ReadAll();
    }
}