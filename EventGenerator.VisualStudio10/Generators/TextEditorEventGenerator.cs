using System;
using System.ComponentModel.Composition;
using System.Timers;
using EnvDTE;
using KAVE.EventGenerator_VisualStudio10.Model;
using KAVE.KAVE_MessageBus.MessageBus;

namespace KAVE.EventGenerator_VisualStudio10.Generators
{
    [Export(typeof (VisualStudioEventGenerator))]
    internal class TextEditorEventGenerator : VisualStudioEventGenerator, IDisposable
    {
        // TODO evaluate good threshold value
        private const int InactivityPeriodToCompleteEditAction = 2000;

        private TextEditorEvents _textEditorEvents;
        private EditEvent _currentEditEvent;

        private readonly Timer _eventSendingTimer = new Timer(InactivityPeriodToCompleteEditAction);
        private readonly object _eventLock = new object();

        public TextEditorEventGenerator(DTE dte, SMessageBus messageBus) : base(dte, messageBus) {}

        public override void Initialize()
        {
            _textEditorEvents = DTEEvents.TextEditorEvents;
            _textEditorEvents.LineChanged += TextEditorEvents_LineChanged;
            _eventSendingTimer.Elapsed += (source, e) => FireCurrentEditEvent();
        }

        void TextEditorEvents_LineChanged(TextPoint startPoint, TextPoint endPoint, int hint)
        {
            _eventSendingTimer.Stop();
            lock (_eventLock)
            {
                _currentEditEvent = _currentEditEvent ?? Create<EditEvent>();
                _currentEditEvent.NumberOfChanges += 1;
                // TODO subtract whitespaces from change size
                _currentEditEvent.SizeOfChanges += endPoint.LineCharOffset - startPoint.LineCharOffset;
            }
            _eventSendingTimer.Start();
        }

        void FireCurrentEditEvent()
        {
            _eventSendingTimer.Stop();
            lock (_eventLock)
            {
                Fire(_currentEditEvent);
                _currentEditEvent = null;
            }
        }

        public void Dispose()
        {
            _eventSendingTimer.Close();
        }
    }
}