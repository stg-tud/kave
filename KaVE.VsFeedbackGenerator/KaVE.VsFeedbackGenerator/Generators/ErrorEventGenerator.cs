using System;
using System.Linq;
using KaVE.Model.Events;
using KaVE.VsFeedbackGenerator.MessageBus;
using KaVE.VsFeedbackGenerator.VsIntegration;

namespace KaVE.VsFeedbackGenerator.Generators
{
    internal class ErrorEventGenerator : AbstractEventGenerator
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
            e.Content = content;

            if (exception != null)
            {
                var lines = exception.ToString().Split(new[] {"\r\n"}, StringSplitOptions.None);
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