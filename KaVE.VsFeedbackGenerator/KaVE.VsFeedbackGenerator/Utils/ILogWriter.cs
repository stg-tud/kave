using System;
using KaVE.JetBrains.Annotations;

namespace KaVE.VsFeedbackGenerator.Utils
{
    public interface ILogWriter<in TMessage> : IDisposable
    {
        void Write([NotNull] TMessage message);
    }
}