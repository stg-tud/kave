using System;
using System.Timers;
using EnvDTE;
using JetBrains.Application;
using JetBrains.Application.Components;
using KaVE.Model.Events.VisualStudio;
using KaVE.VsFeedbackGenerator.MessageBus;
using KaVE.VsFeedbackGenerator.VsIntegration;

namespace KaVE.VsFeedbackGenerator.Generators.VisualStudio
{
    [ShellComponent(ProgramConfigurations.VS_ADDIN)]
    internal class TextEditorEventGenerator : AbstractEventGenerator, IDisposable
    {
        // TODO evaluate good threshold value
        private const int InactivityPeriodToCompleteEditAction = 2000;

        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly TextEditorEvents _textEditorEvents;
        private EditEvent _currentEditEvent;

        private readonly Timer _eventSendingTimer = new Timer(InactivityPeriodToCompleteEditAction);
        private readonly object _eventLock = new object();

        public TextEditorEventGenerator(IIDESession session, IMessageBus messageBus)
            : base(session, messageBus)
        {
            _textEditorEvents = DTE.Events.TextEditorEvents;
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
                FireNow(_currentEditEvent);
                _currentEditEvent = null;
            }
        }

        public void Dispose()
        {
            _eventSendingTimer.Close();
        }
    }
}