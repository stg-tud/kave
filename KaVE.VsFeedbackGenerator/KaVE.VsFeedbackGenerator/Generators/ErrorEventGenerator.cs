using System;
using System.Linq;
using JetBrains.Application;
using KaVE.Model.Events;
using KaVE.VsFeedbackGenerator.MessageBus;
using KaVE.VsFeedbackGenerator.VsIntegration;

namespace KaVE.VsFeedbackGenerator.Generators
{
    public interface ILogger
    {
        void Log(Exception exception, string content);
        void Log(Exception exception);
        void Log(string content);
    }

    [ShellComponent]
    public class ErrorEventGenerator : AbstractEventGenerator, ILogger
    {
        private readonly IMessageBus _messageBus;

        public ErrorEventGenerator(IIDESession session, IMessageBus messageBus)
            : base(session, messageBus)
        {
            _messageBus = messageBus;
        }

        public virtual void Log(Exception exception, string content)
        {
            var e = Create<ErrorEvent>();
            e.TriggeredBy = IDEEvent.Trigger.Automatic;
            
            if (content != null)
            {
                // TODO does it make sense to move this to the ErrorEvent class?
                e.Content = content.Replace("\r\n", "<br />").Replace("\n", "<br />");
            }

            if (exception != null)
            {
                var lines = exception.ToString().Split(new[] {"\r\n", "\n"}, StringSplitOptions.None);
                e.StackTrace = lines
                    .Select(line => line.Trim())
                    .Where(line => line.Length > 0)
                    .ToArray();
            }

            _messageBus.Publish(e);
        }

        public void Log(Exception exception)
        {
            Log(exception, null);
        }

        public void Log(string content)
        {
            Log(null, content);
        }
    }
}