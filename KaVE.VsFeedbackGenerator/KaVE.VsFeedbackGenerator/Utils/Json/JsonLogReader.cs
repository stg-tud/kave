using System;
using System.Collections.Generic;
using System.IO;
using KaVE.Utils.Assertion;
using Newtonsoft.Json;

namespace KaVE.VsFeedbackGenerator.Utils.Json
{
    /// <summary>
    /// A reader for streams written by <see cref="JsonLogWriter"/>.
    /// </summary>
    public class JsonLogReader : IDisposable
    {
        private readonly StreamReader _logStreamReader;

        /// <param name="logStream">The reader takes ownership of the stream, i.e., the stream is closed when the reader is disposed.</param>
        public JsonLogReader(Stream logStream)
        {
            Asserts.NotNull(logStream, "log stream");
            Asserts.That(logStream.CanRead, "log stream not readable");
            _logStreamReader = new StreamReader(logStream, JsonLogSerialization.Encoding);
        }

        public TMessage Read<TMessage>()
        {
            var json = _logStreamReader.ReadLine();
            return json == null
                ? default(TMessage)
                : JsonConvert.DeserializeObject<TMessage>(json, JsonLogSerialization.Settings);
        }

        /// <summary>
        /// Returns an Enumarable that lazily reads all log entries from the underlying stream.
        /// </summary>
        public IEnumerable<TEvent> GetEnumeration<TEvent>() where TEvent : class
        {
            TEvent evt;
            while ((evt = Read<TEvent>()) != null)
            {
                yield return evt;
            }
        }

        public void Dispose()
        {
            _logStreamReader.Close();
        }
    }
}