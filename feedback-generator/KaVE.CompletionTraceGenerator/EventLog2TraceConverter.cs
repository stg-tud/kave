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
using System.Linq;
using KaVE.Commons.Model.Events;
using KaVE.Commons.Model.Events.CompletionEvents;
using KaVE.Commons.Utils.Exceptions;
using KaVE.Commons.Utils.Logging.Json;

namespace KaVE.CompletionTraceGenerator
{
    internal class EventLog2TraceConverter
    {
        private static readonly string[] InputFileNames = {@"C:\Users\Sven\test.log"};
        private const string OutputFileName = @"C:\Users\Sven\test.trace";

        private readonly ILogger _logger = new ConsoleLogger();

        public static void Main()
        {
            new EventLog2TraceConverter().Run();
        }

        private void Run()
        {
            using (var outputStream = new FileStream(OutputFileName, FileMode.Open, FileAccess.Write))
            {
                using (var writer = new JsonLogWriter<CompletionTrace>(outputStream))
                {
                    using (var converter = new CompletionEventToTraceConverter(writer))
                    {
                        foreach (var completionEvent in InputCompletionEvents)
                        {
                            converter.Process(completionEvent);
                        }
                    }
                }
            }
        }

        private IEnumerable<CompletionEvent> InputCompletionEvents
        {
            get { return InputFileNames.SelectMany(ReadIDEEvents).OfType<CompletionEvent>(); }
        }

        private IEnumerable<IDEEvent> ReadIDEEvents(string logFileName)
        {
            using (var stream = new FileStream(logFileName, FileMode.Open, FileAccess.Read))
            {
                using (var logReader = new JsonLogReader<IDEEvent>(stream, _logger))
                {
                    return logReader.ReadAll().ToList();
                }
            }
        }
    }
}