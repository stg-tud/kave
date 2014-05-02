using System.Collections.Generic;
using System.IO;
using System.Linq;
using KaVE.Model.Events;
using KaVE.Model.Events.CompletionEvent;
using KaVE.VsFeedbackGenerator.Utils.Json;

namespace KaVE.CompletionTraceGenerator
{
    internal class EventLog2TraceConverter
    {
        private static readonly string[] InputFileNames = {@"C:\Users\Sven\test.log"};
        private const string OutputFileName = @"C:\Users\Sven\test.trace";

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
                    var converter = new CompletionEventToTraceConverter(writer);
                    foreach (var completionEvent in InputCompletionEvents)
                    {
                        converter.Process(completionEvent);
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
                using (var logReader = new JsonLogReader<IDEEvent>(stream))
                {
                    return logReader.ReadAll().ToList();
                }
            }
        }
    }
}