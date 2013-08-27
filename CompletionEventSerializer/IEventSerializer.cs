using System.IO;
using CodeCompletion.Model.CompletionEvent;

namespace CompletionEventSerializer
{
    public interface IEventSerializer
    {
        void AppendTo(Stream targetStream, CompletionEvent completionEvent);
        CompletionEvent ReadFrom(Stream source);
    }
}
