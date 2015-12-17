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
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Timers;
using JetBrains.Application;
using KaVE.Commons.Model.Events.VisualStudio;
using KaVE.Commons.Utils;
using KaVE.JetBrains.Annotations;
using KaVE.VS.FeedbackGenerator.MessageBus;

namespace KaVE.VS.FeedbackGenerator.Generators.VisualStudio
{
    // TODO RS9: disabled for now
    //[ShellComponent(ProgramConfigurations.VS_ADDIN)]
    internal class VsFocusEventGenerator : EventGeneratorBase
    {
        private readonly IFocusHelper _focusHelper;

        [UsedImplicitly]
        private Timer _timer;

        private bool _wasActive;

        public VsFocusEventGenerator(IRSEnv env, IMessageBus messageBus, IDateUtils dateUtils, IFocusHelper focusHelper)
            : base(env, messageBus, dateUtils)
        {
            _focusHelper = focusHelper;
            _timer = new Timer();
            _timer.Elapsed += OnTimerElapsed;
            _timer.Interval = 1000;
            _timer.Enabled = true;
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            _timer.Enabled = false;
            var isActive = _focusHelper.IsCurrentApplicationActive();
            if (isActive != _wasActive)
            {
                var windowEvent = Create<WindowEvent>();
                windowEvent.Action = isActive ? WindowEvent.WindowAction.Activate : WindowEvent.WindowAction.Deactivate;
                // TODO unecessary?: windowEvent.Window = DTE.MainWindow.GetName();
                FireNow(windowEvent);
            }
            _wasActive = isActive;
            _timer.Enabled = true;
        }
    }

    public interface IFocusHelper
    {
        bool IsCurrentApplicationActive();
    }

    [ShellComponent]
    internal class FocusHelper : IFocusHelper
    {
        public bool IsCurrentApplicationActive()
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