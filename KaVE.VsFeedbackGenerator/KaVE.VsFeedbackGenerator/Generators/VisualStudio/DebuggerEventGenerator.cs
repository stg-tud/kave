using EnvDTE;
using JetBrains.Application;
using JetBrains.Application.Components;
using KaVE.Model.Events.VisualStudio;
using KaVE.VsFeedbackGenerator.MessageBus;

namespace KaVE.VsFeedbackGenerator.Generators.VisualStudio
{
    [ShellComponent(ProgramConfigurations.VS_ADDIN)]
    internal class DebuggerEventGenerator : AbstractEventGenerator
    {
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly DebuggerEvents _debuggerEvents;

        public DebuggerEventGenerator(DTE dte, IMessageBus messageBus) : base(dte, messageBus)
        {
            _debuggerEvents = DTE.Events.DebuggerEvents;
            _debuggerEvents.OnEnterBreakMode += _debuggerEvents_OnEnterBreakMode;
            _debuggerEvents.OnEnterDesignMode += _debuggerEvents_OnEnterDesignMode;
            _debuggerEvents.OnEnterRunMode += _debuggerEvents_OnEnterRunMode;
            _debuggerEvents.OnExceptionNotHandled += _debuggerEvents_OnExceptionNotHandled;
            _debuggerEvents.OnExceptionThrown += _debuggerEvents_OnExceptionThrown;
        }

        private DebuggerEvent _lastEvent;

        void _debuggerEvents_OnExceptionThrown(string exceptionType, string name, int code, string description, ref dbgExceptionAction exceptionAction)
        {
            FireLastEvent();
            StartEvent(DebuggerEvent.DebuggerMode.ExceptionThrown, name, exceptionAction.ToString());
        }

        void _debuggerEvents_OnExceptionNotHandled(string exceptionType, string name, int code, string description, ref dbgExceptionAction exceptionAction)
        {
            FireLastEvent();
            StartEvent(DebuggerEvent.DebuggerMode.ExceptionNotHandled, name, exceptionAction.ToString());
        }

        void _debuggerEvents_OnEnterRunMode(dbgEventReason reason)
        {
            FireLastEvent();
            StartEvent(DebuggerEvent.DebuggerMode.Run, reason.ToString());
            CheckIfDebuggingStopped(reason);
        }

        void _debuggerEvents_OnEnterDesignMode(dbgEventReason reason)
        {
            FireLastEvent();
            StartEvent(DebuggerEvent.DebuggerMode.Design, reason.ToString());
            CheckIfDebuggingStopped(reason);
        }

        void _debuggerEvents_OnEnterBreakMode(dbgEventReason reason, ref dbgExecutionAction executionAction)
        {
            FireLastEvent();
            StartEvent(DebuggerEvent.DebuggerMode.Break, reason.ToString(), executionAction.ToString());
            CheckIfDebuggingStopped(reason);
        }

        private void FireLastEvent()
        {
            if (_lastEvent != null)
            {
                Fire(_lastEvent);
                _lastEvent = null;
            }
        }

        private void StartEvent(DebuggerEvent.DebuggerMode mode, string reason, string action = null)
        {
            _lastEvent = Create<DebuggerEvent>();
            _lastEvent.Mode = mode;
            _lastEvent.Reason = reason;
            _lastEvent.Action = action;
        }

        private void CheckIfDebuggingStopped(dbgEventReason reason)
        {
            switch (reason)
            {
                case dbgEventReason.dbgEventReasonStopDebugging:
                case dbgEventReason.dbgEventReasonDetachProgram:
                case dbgEventReason.dbgEventReasonEndProgram:
                    FireLastEvent();
                    break;
            }
        }
    }
}