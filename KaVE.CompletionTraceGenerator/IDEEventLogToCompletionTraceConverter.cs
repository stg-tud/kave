using System;
using System.Collections.Generic;
using System.Linq;
using KaVE.Model.Events;
using KaVE.Model.Events.CompletionEvent;
using KaVE.VsFeedbackGenerator.Utils.Json;

namespace KaVE.CompletionTraceGenerator
{
    internal class IDEEventLogToCompletionTraceConverter
    {
        private static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Pass at least one IDEEvent-log-filename as argument");
            }

            var logFileManager = new JsonLogFileManager();
            var toTraceConverter = new CompletionEventToTraceConverter(logFileManager.NewLogWriter("???"));
            foreach (
                var @event in
                    args.SelectMany(logFileName => logFileManager.NewLogReader(logFileName).GetEnumeration<IDEEvent>()))
            {
                var completionEvent = @event as CompletionEvent;
                if (completionEvent != null)
                {
                    toTraceConverter.Process(completionEvent);
                }
            }
        }

        private static IEnumerable<IDEEvent> GetEvents(string logFileName)
        {
            return null;
        } 
    }
}