using System;
using JetBrains.Annotations;

namespace KaVE.VsFeedbackGenerator.Generators
{
    public interface ILogger
    {
        void Error(Exception exception, string content);
        void Error([NotNull] Exception exception);
        void Error([NotNull] string content);
        void Info([NotNull] string info);
    }
}