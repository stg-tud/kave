/*
 * Copyright 2014 Technische Universität Darmstadt
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *    http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 * 
 * Contributors:
 *    - Sven Amann
 */

using EnvDTE;
using JetBrains.Application;
using JetBrains.Application.Components;
using KaVE.Model.Events.VisualStudio;
using KaVE.VsFeedbackGenerator.MessageBus;
using KaVE.VsFeedbackGenerator.Utils;

namespace KaVE.VsFeedbackGenerator.Generators.VisualStudio
{
    [ShellComponent(ProgramConfigurations.VS_ADDIN)]
    internal class DebuggerEventGenerator : EventGeneratorBase
    {
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly DebuggerEvents _debuggerEvents;

        public DebuggerEventGenerator(IRSEnv env, IMessageBus messageBus, IDateUtils dateUtils)
            : base(env, messageBus, dateUtils)
        {
            _debuggerEvents = DTE.Events.DebuggerEvents;
            _debuggerEvents.OnEnterBreakMode += _debuggerEvents_OnEnterBreakMode;
            _debuggerEvents.OnEnterDesignMode += _debuggerEvents_OnEnterDesignMode;
            _debuggerEvents.OnEnterRunMode += _debuggerEvents_OnEnterRunMode;
            _debuggerEvents.OnExceptionNotHandled += _debuggerEvents_OnExceptionNotHandled;
            _debuggerEvents.OnExceptionThrown += _debuggerEvents_OnExceptionThrown;
        }

        private DebuggerEvent _lastEvent;

        private void _debuggerEvents_OnExceptionThrown(string exceptionType,
            string name,
            int code,
            string description,
            ref dbgExceptionAction exceptionAction)
        {
            FireLastEvent();
            StartEvent(DebuggerEvent.DebuggerMode.ExceptionThrown, name, exceptionAction.ToString());
        }

        private void _debuggerEvents_OnExceptionNotHandled(string exceptionType,
            string name,
            int code,
            string description,
            ref dbgExceptionAction exceptionAction)
        {
            FireLastEvent();
            StartEvent(DebuggerEvent.DebuggerMode.ExceptionNotHandled, name, exceptionAction.ToString());
        }

        private void _debuggerEvents_OnEnterRunMode(dbgEventReason reason)
        {
            FireLastEvent();
            StartEvent(DebuggerEvent.DebuggerMode.Run, reason.ToString());
            CheckIfDebuggingStopped(reason);
        }

        private void _debuggerEvents_OnEnterDesignMode(dbgEventReason reason)
        {
            FireLastEvent();
            StartEvent(DebuggerEvent.DebuggerMode.Design, reason.ToString());
            CheckIfDebuggingStopped(reason);
        }

        private void _debuggerEvents_OnEnterBreakMode(dbgEventReason reason, ref dbgExecutionAction executionAction)
        {
            FireLastEvent();
            StartEvent(DebuggerEvent.DebuggerMode.Break, reason.ToString(), executionAction.ToString());
            CheckIfDebuggingStopped(reason);
        }

        private void FireLastEvent()
        {
            if (_lastEvent != null)
            {
                FireNow(_lastEvent);
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