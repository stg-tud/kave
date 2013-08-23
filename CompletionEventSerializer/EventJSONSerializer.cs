using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeCompletion.Model.CompletionEvent;
using Newtonsoft.Json;

namespace CompletionEventSerializer
{
    public class EventJsonSerializer : IEventSerializer
    {
        public void Serialize(CompletionEvent completionEvent, Stream target)
        {
            var serialization = JsonConvert.SerializeObject(completionEvent, Formatting.Indented);
            var streamWriter = new StreamWriter(target);
            streamWriter.Write(serialization);
            streamWriter.Flush();
        }

        public CompletionEvent Deserialize(Stream source)
        {
            var streamReader = new StreamReader(source);
            var serialization = streamReader.ReadToEnd();
            return JsonConvert.DeserializeObject<CompletionEvent>(serialization);
        }
    }
}
