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

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Timers;
using JetBrains.Application;
using JetBrains.Application.Components;
using KaVE.Commons.Model.Events.VisualStudio;
using KaVE.Commons.Utils;
using KaVE.JetBrains.Annotations;
using KaVE.VS.FeedbackGenerator.MessageBus;
using KaVE.VS.FeedbackGenerator.Utils.Names;

namespace KaVE.VS.FeedbackGenerator.Generators.VisualStudio
{
    [ShellComponent(ProgramConfigurations.VS_ADDIN)]
    internal class VsFocusEventGenerator : EventGeneratorBase
    {
        [UsedImplicitly]
        private Timer _timer;

        private bool _wasActive;

        public VsFocusEventGenerator(IRSEnv env, IMessageBus messageBus, IDateUtils dateUtils)
            : base(env, messageBus, dateUtils)
        {
            _timer = new Timer();
            _timer.Elapsed += OnTimerElapsed;
            _timer.Interval = 1000;
            _timer.Enabled = true;
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            var isActive = IsCurrentApplicationActive();
            if (isActive != _wasActive)
            {
                var windowEvent = Create<WindowEvent>();
                windowEvent.Action = isActive ? WindowEvent.WindowAction.Activate : WindowEvent.WindowAction.Deactivate;
                windowEvent.Window = DTE.MainWindow.GetName();
                FireNow(windowEvent);
            }
            _wasActive = isActive;
        }

        public static bool IsCurrentApplicationActive()
        {
            var activeWindow = GetForegroundWindow();

            if (IsNoWindow(activeWindow))
            {
                return false;
            }

            var ownProcessId = Process.GetCurrentProcess().Id;
            var activeWindowProcessId = GetWindowProcessId(activeWindow);
            return activeWindowProcessId == ownProcessId;
        }

        private static bool IsNoWindow(IntPtr activeWindowHandle)
        {
            return activeWindowHandle == IntPtr.Zero;
        }

        private static int GetWindowProcessId(IntPtr activeWindow)
        {
            int activeProcId;
            GetWindowThreadProcessId(activeWindow, out activeProcId);
            return activeProcId;
        }


        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowThreadProcessId(IntPtr handle, out int processId);
    }
}