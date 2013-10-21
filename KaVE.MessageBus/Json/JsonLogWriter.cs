using System;
using System.IO;
using KaVE.JetBrains.Annotations;
using KaVE.Utils.Assertion;
using Newtonsoft.Json;

namespace KaVE.MessageBus.Json
{
    /// <summary>
    /// Writes objects as Json to a stream. Every object is serialized and written as a single line, delimited by '\r\n'.
    /// </summary>
    internal class JsonLogWriter : IDisposable
    {
        private readonly StreamWriter _logStreamWriter;
        private readonly JsonSerializer _serializer;

        /// <param name="logStream">The writer takes ownership of the stream, i.e., the stream is closed when the reader is disposed.</param>
        public JsonLogWriter(Stream logStream)
        {
            Asserts.NotNull(logStream, "log stream");
            Asserts.That(logStream.CanWrite, "log stream not writable");
            _logStreamWriter = new StreamWriter(logStream, JsonLogSerialization.Encoding);
            _serializer = JsonSerializer.Create(JsonLogSerialization.Settings);
        }

        public void Write<TMessage>([NotNull] TMessage message)
        {
            var jsonWriter = new JsonTextWriter(_logStreamWriter);
            _serializer.Serialize(jsonWriter, message);
            jsonWriter.Flush();
            _logStreamWriter.WriteLine();
            _logStreamWriter.Flush();
        }

        public void Dispose()
        {
            _logStreamWriter.Flush();
            _logStreamWriter.Close();
        }
    }
}