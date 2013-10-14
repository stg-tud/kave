using System.IO;
using CodeCompletion.Utils.Assertion;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace KAVE.KAVE_MessageBus.Json
{
    internal class JsonLogWriter
    {
        private readonly StreamWriter _logStreamWriter;

        public JsonLogWriter(Stream logStream)
        {
            Asserts.NotNull(logStream, "log stream");
            Asserts.That(logStream.CanWrite, "log stream not writable");
            _logStreamWriter = new StreamWriter(logStream, JsonLogSerialization.Encoding);
        }

        public void Write<TMessage>([NotNull] TMessage message)
        {
            var jsonWriter = new JsonTextWriter(_logStreamWriter);
            var serializer = JsonSerializer.Create(JsonLogSerialization.Settings);
            serializer.Serialize(jsonWriter, message);
            jsonWriter.Flush();
            _logStreamWriter.WriteLine();
            _logStreamWriter.Flush();
        }
    }
}