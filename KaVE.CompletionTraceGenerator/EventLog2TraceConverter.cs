using System.Collections.Generic;
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

        private readonly JsonLogFileManager _logFileManager;
        private readonly CompletionEventToTraceConverter _toTraceConverter;

        public static void Main()
        {
            new EventLog2TraceConverter().Run();
        }

        private EventLog2TraceConverter()
        {
            _logFileManager = new JsonLogFileManager();
            _toTraceConverter = new CompletionEventToTraceConverter(_logFileManager.NewLogWriter(OutputFileName));
        }

        private void Run()
        {
            foreach (var completionEvent in InputCompletionEvents)
            {
                _toTraceConverter.Process(completionEvent);
            }
        }

        private IEnumerable<CompletionEvent> InputCompletionEvents
        {
            get { return InputFileNames.SelectMany(ReadIDEEvents).OfType<CompletionEvent>(); }
        }

        private IEnumerable<IDEEvent> ReadIDEEvents(string logFileName)
        {
            return _logFileManager.NewLogReader(logFileName).ReadAll<IDEEvent>();
        }
    }
}