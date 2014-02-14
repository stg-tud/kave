using KaVE.JetBrains.Annotations;

namespace KaVE.VsFeedbackGenerator.Utils
{
    public interface ILogWriter<in TMessage> {
        void Write([NotNull] TMessage message);
    }
}