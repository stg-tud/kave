using System.Collections.Generic;
using System.IO;
using KaVE.Utils.Assertion;
using KaVE.VsFeedbackGenerator.Generators;
using Newtonsoft.Json;

namespace KaVE.VsFeedbackGenerator.Utils.Json
{
    /// <summary>
    /// A reader for streams written by <see cref="JsonLogWriter{TMessage}"/>.
    /// </summary>
    public class JsonLogReader<TMessage> : ILogReader<TMessage> where TMessage : class
    {
        private readonly StreamReader _logStreamReader;

        /// <param name="logStream">The reader takes ownership of the stream, i.e., the stream is closed when the reader is disposed.</param>
        public JsonLogReader(Stream logStream)
        {
            Asserts.NotNull(logStream, "log stream");
            Asserts.That(logStream.CanRead, "log stream not readable");
            _logStreamReader = new StreamReader(logStream, JsonLogSerialization.Encoding);
        }

        public TMessage ReadNext()
        {
            var json = _logStreamReader.ReadLine();
            return json == null
                ? default(TMessage)
                : Deserialize(json);
        }

        private TMessage Deserialize(string json)
        {
            try
            {
                return JsonConvert.DeserializeObject<TMessage>(json, JsonLogSerialization.Settings);
            }
            catch (JsonReaderException jre)
            {
                Registry.GetComponent<ILogger>().Error(jre, json);
                // supressing broken lines
                return ReadNext();
            }
        }

        /// <summary>
        /// Returns an Enumarable that lazily reads all log entries from the underlying stream.
        /// </summary>
        public IEnumerable<TMessage> ReadAll()
        {
            TMessage mess;
            while ((mess = ReadNext()) != null)
            {
                yield return mess;
            }
        }

        public void Dispose()
        {
            _logStreamReader.Close();
        }
    }
}