using System.ComponentModel.Composition;
using System.IO;
using CodeCompletion.Model.CompletionEvent;
using Newtonsoft.Json;

namespace CompletionEventSerializer
{
    [Export, ExportMetadata("usage-mode", "debug")]
    public class EventJsonSerializer : IEventSerializer
    {
        public void AppendTo(Stream targetStream, CompletionEvent completionEvent)
        {
            completionEvent.ToJson().AppendTo(targetStream);
        }

        public CompletionEvent ReadFrom(Stream source)
        {
            var streamReader = new StreamReader(source);
            var json = streamReader.ReadToEnd();
            return json.To<CompletionEvent>();
        }
    }

    internal static class JsonExtension
    {
        public static string ToJson<TMessage>(this TMessage message)
        {
            return JsonConvert.SerializeObject(message, Formatting.Indented);
        }

        public static TMessage To<TMessage>(this string json)
        {
            return JsonConvert.DeserializeObject<TMessage>(json);
        }

        public static void AppendTo(this string json, Stream target)
        {
            var streamWriter = new StreamWriter(target);
            streamWriter.Write(json);
            streamWriter.Flush();
        }
    }
}
