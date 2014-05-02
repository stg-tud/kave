using System.Collections.Generic;
using System.IO;
using KaVE.Utils.Assertion;
using KaVE.VsFeedbackGenerator.Utils.Logging;
using Newtonsoft.Json;

namespace KaVE.VsFeedbackGenerator.Utils.Json
{
    /// <summary>
    ///     Writes objects as Json to a stream. Every object is serialized and written as a single line, delimited by '\r\n'.
    /// </summary>
    public class JsonLogWriter<TMessage> : ILogWriter<TMessage>
    {
        private readonly StreamWriter _logStreamWriter;

        /// <param name="logStream">
        ///     The writer takes ownership of the stream, i.e., the stream is closed when the reader is
        ///     disposed.
        /// </param>
        public JsonLogWriter(Stream logStream)
        {
            Asserts.NotNull(logStream, "log stream");
            Asserts.That(logStream.CanWrite, "log stream not writable");
            _logStreamWriter = new StreamWriter(logStream, JsonSerialization.Encoding);
        }

        public void Write(TMessage message)
        {
            SerializeAndWrite(message);
            _logStreamWriter.Flush();
        }

        public void WriteAll(IEnumerable<TMessage> messages)
        {
            foreach (var message in messages)
            {
                SerializeAndWrite(message);
            }
            _logStreamWriter.Flush();
        }

        private void SerializeAndWrite(TMessage message)
        {
            var json = message.ToCompactJson();
            _logStreamWriter.WriteLine(json);
        }

        public void Dispose()
        {
            _logStreamWriter.Flush();
            _logStreamWriter.Close();
        }
    }
}