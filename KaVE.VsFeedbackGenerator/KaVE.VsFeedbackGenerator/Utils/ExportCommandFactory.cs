using System;
using System.Collections.Generic;
using JetBrains.UI.Extensions.Commands;
using KaVE.Model.Events;
using KaVE.Utils.Assertion;
using KaVE.VsFeedbackGenerator.SessionManager;

namespace KaVE.VsFeedbackGenerator.Utils
{
    public class ExportCommandFactory
    {
        private readonly ILogFileManager<IDEEvent> _logFileManager;
        private readonly IFeedbackViewModelDialog _feedbackViewModelDialog;
        private readonly IIoUtils _ioUtils = Registry.GetComponent<IIoUtils>();

        public ExportCommandFactory(ILogFileManager<IDEEvent> logFileManager, IFeedbackViewModelDialog feedbackViewModelDialog)
        {
            _logFileManager = logFileManager;
            _feedbackViewModelDialog = feedbackViewModelDialog;
        }

        public DelegateCommand Create(IPublisher publisher)
        {
            Action<object> action = o =>
            {
                try
                {
                    var events = _feedbackViewModelDialog.ExtractEventsForExport();
                    Export(events, publisher);
                    _feedbackViewModelDialog.ShowExportSucceededMessage(events.Count);
                }
                catch (AssertException e)
                {
                    _feedbackViewModelDialog.ShowExportFailedMessage(e.Message);
                }
            };
            return new DelegateCommand(action, o => _feedbackViewModelDialog.AreAnyEventsPresent);
        }


        private void Export(IEnumerable<IDEEvent> events, IPublisher publisher)
        {
            var tempFileName = _ioUtils.GetTempFileName();
            using (var logWriter = _logFileManager.NewLogWriter(tempFileName))
            {
                logWriter.WriteAll(events);
            }
            publisher.Publish(tempFileName);
        }
    }
}