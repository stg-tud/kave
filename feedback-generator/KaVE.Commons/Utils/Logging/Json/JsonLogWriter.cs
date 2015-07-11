/*
 * Copyright 2014 Technische Universität Darmstadt
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *    http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System.Collections.Generic;
using System.IO;
using KaVE.Commons.Utils.Assertion;
using KaVE.Commons.Utils.Json;

namespace KaVE.Commons.Utils.Logging.Json
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