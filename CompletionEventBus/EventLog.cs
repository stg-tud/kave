using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using CodeCompletion.Utils.Assertion;
using CompletionEventSerializer;

namespace CompletionEventBus
{
    public class EventLogWriter
    {
        private ISerializer _eventSerializer;

        public EventLogWriter(Stream logStream)
        {
            Asserts.That(logStream.CanWrite, "log-stream not writable");
            LogStream = logStream;
        }

        protected Stream LogStream
        {
            get; private set;
        }

        public void Append<TEvent>(TEvent evt) where TEvent : class
        {
            _eventSerializer.AppendTo(LogStream, evt);
        }
    }

    public class EventLogReader
    {
        private ISerializer _eventSerializer;

        public EventLogReader(Stream logStream)
        {
            Asserts.That(logStream.CanRead, "log-stream not readable");
            LogStream = logStream;
        }

        protected Stream LogStream
        {
            get; private set;
        }

        public TEvent Read<TEvent>() where TEvent : class
        {
            return _eventSerializer.Read<TEvent>(LogStream);
        }

        /// <summary>
        /// Returns an Enumarable that lazily reads all events from the underlying stream.
        /// </summary>
        public IEnumerable<TEvent> GetEnumeration<TEvent>() where TEvent : class
        {
            TEvent evt;
            while ((evt = Read<TEvent>()) != null)
            {
                yield return evt;
            }
        }
    }

    public class CompressedEventLogWriter : EventLogWriter
    {
        public CompressedEventLogWriter(Stream logStream) : base(new GZipStream(logStream, CompressionMode.Compress)) {}
    }

    public class CompressedEventLogReader : EventLogReader
    {
        public CompressedEventLogReader(Stream logStream) : base(new GZipStream(logStream, CompressionMode.Decompress)) {}
    }
}
