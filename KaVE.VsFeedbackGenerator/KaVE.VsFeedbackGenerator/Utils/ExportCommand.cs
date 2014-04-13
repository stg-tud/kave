using System;
using JetBrains.Annotations;
using JetBrains.UI.Extensions.Commands;
using KaVE.Model.Events;
using KaVE.Utils.Assertion;
using KaVE.VsFeedbackGenerator.SessionManager;

namespace KaVE.VsFeedbackGenerator.Utils
{
    public static class ExportCommand
    {
        public static DelegateCommand Create([NotNull] ISessionExport export,
            [NotNull] Func<string, ILogWriter<IDEEvent>> writer,
            [NotNull] IFeedbackViewModelDialog model)
        {
            Action<object> execute = o =>
            {
                try
                {
                    var eventsToExport = model.ExtractEventsForExport();
                    export.Export(eventsToExport, writer);
                    model.ShowExportSucceededMessage(eventsToExport.Count);
                }
                catch (AssertException e)
                {
                    model.ShowExportFailedMessage(e.Message);
                }
            };
            return new DelegateCommand(execute, o => model.AreAnyEventsPresent);
        }
    }
}