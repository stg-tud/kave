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
 */
using System;
using System.Collections.Generic;
using EnvDTE;
using JetBrains.Application;
using JetBrains.Application.Components;
using KaVE.Model.Events.VisualStudio;
using KaVE.Model.Names.VisualStudio;
using KaVE.Utils;
using KaVE.Utils.Collections;
using KaVE.VsFeedbackGenerator.MessageBus;
using KaVE.VsFeedbackGenerator.Utils.Names;
using KaVE.VsFeedbackGenerator.VsIntegration;

namespace KaVE.VsFeedbackGenerator.Generators.VisualStudio
{
    [ShellComponent(ProgramConfigurations.VS_ADDIN)]
    internal class WindowEventGenerator : EventGeneratorBase
    {
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly WindowEvents _windowEvents;

        private readonly IDictionary<WindowName, WindowEvent> _delayedMoveEvents;
        private readonly IDictionary<WindowName, ScheduledAction> _delayedMoveEventFireActions;

        public WindowEventGenerator(IIDESession session, IMessageBus messageBus)
            : base(session, messageBus)
        {
            _delayedMoveEvents = new Dictionary<WindowName, WindowEvent>();
            _delayedMoveEventFireActions = new ThreadSafeDictionary<WindowName, ScheduledAction>(name => ScheduledAction.NoOp);

            _windowEvents = DTE.Events.WindowEvents;
            _windowEvents.WindowCreated += _windowEvents_WindowCreated;
            _windowEvents.WindowActivated += _windowEvents_WindowActivated;
            _windowEvents.WindowMoved += _windowEvents_WindowMoved;
            _windowEvents.WindowClosing += _windowEvents_WindowClosing;
        }

        private void _windowEvents_WindowClosing(Window window)
        {
            Fire(window, WindowEvent.WindowAction.Close);
        }

        private void _windowEvents_WindowMoved(Window window, int top, int left, int width, int height)
        {
            var moveEvent = CreateWindowEvent(window, WindowEvent.WindowAction.Move);
            moveEvent.TerminatedAt = DateTime.Now;

            _delayedMoveEventFireActions[moveEvent.Window].Cancel();
            lock (_delayedMoveEvents)
            {
                if (_delayedMoveEvents.ContainsKey(moveEvent.Window))
                {
                    _delayedMoveEvents[moveEvent.Window].TerminatedAt = moveEvent.TerminatedAt;
                }
                else
                {
                    _delayedMoveEvents[moveEvent.Window] = moveEvent;
                }
                _delayedMoveEventFireActions[moveEvent.Window] = Invoke.Later(
                    () =>
                    {
                        _delayedMoveEventFireActions.Remove(moveEvent.Window);
                        Fire(_delayedMoveEvents[moveEvent.Window]);
                        _delayedMoveEvents.Remove(moveEvent.Window);
                    },
                    150);
            }
        }

        private void _windowEvents_WindowActivated(Window gotFocus, Window lostFocus)
        {
            Fire(gotFocus, WindowEvent.WindowAction.Activate);
            // We don't fire lostFocus events, since we track the active window in every event and know that the
            // previously active window looses the focus whenever some other window gains it.
        }

        private void _windowEvents_WindowCreated(Window window)
        {
            Fire(window, WindowEvent.WindowAction.Create);
        }

        private void Fire(Window window, WindowEvent.WindowAction action)
        {
            var windowEvent = CreateWindowEvent(window, action);
            FireNow(windowEvent);
        }

        private WindowEvent CreateWindowEvent(Window window, WindowEvent.WindowAction action)
        {
            var windowEvent = Create<WindowEvent>();
            windowEvent.Window = window.GetName();
            windowEvent.Action = action;
            return windowEvent;
        }
    }
}