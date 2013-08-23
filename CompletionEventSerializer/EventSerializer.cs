using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeCompletion.Model.CompletionEvent;

namespace CompletionEventSerializer
{
    public interface IEventSerializer
    {
        void Serialize(CompletionEvent completionEvent);
        CompletionEvent Deserialize();
    }
}
