using System.IO;
using CodeCompletion.Model.CompletionEvent;

namespace CompletionEventSerializer
{
    public interface IEventSerializer
    {
        void Serialize(CompletionEvent completionEvent, Stream target);
        CompletionEvent Deserialize(Stream source);
    }
}
