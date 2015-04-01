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
 * 
 * Contributors:
 *    - Sven Amann
 */

using System;
using System.Collections.Generic;
using System.IO;
using KaVE.Commons.Utils.Assertion;
using KaVE.Commons.Utils.Exceptions;
using KaVE.VsFeedbackGenerator.Utils.Logging;

namespace KaVE.VsFeedbackGenerator.Utils.Json
{
    /// <summary>
    ///     A reader for streams written by <see cref="JsonLogWriter{TMessage}" />.
    /// </summary>
    public class JsonLogReader<TMessage> : ILogReader<TMessage> where TMessage : class
    {
        private readonly StreamReader _logStreamReader;

        /// <param name="logStream">
        ///     The reader takes ownership of the stream, i.e., the stream is closed when the reader is
        ///     disposed.
        /// </param>
        public JsonLogReader(Stream logStream)
        {
            Asserts.NotNull(logStream, "log stream");
            Asserts.That(logStream.CanRead, "log stream not readable");
            _logStreamReader = new StreamReader(logStream, JsonSerialization.Encoding);
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
                return json.ParseJsonTo<TMessage>();
            }
                // TODO think about more concrete exception types
            catch (Exception jre)
            {
                Registry.GetComponent<ILogger>().Error(jre, json);
                // supressing broken lines
                return ReadNext();
            }
        }

        /// <summary>
        ///     Returns an Enumarable that lazily reads all log entries from the underlying stream.
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