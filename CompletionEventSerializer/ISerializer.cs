using System.IO;

namespace CompletionEventSerializer
{
    public interface ISerializer
    {
        void AppendTo<TMessage>(Stream targetStream, TMessage instance);
        TMessage Read<TMessage>(Stream source);
    }
}