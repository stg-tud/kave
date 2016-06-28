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
using JetBrains.Threading;
using KaVE.Commons.Model.Events.VisualStudio;
using KaVE.Commons.Model.Naming;
using KaVE.Commons.Utils;
using KaVE.JetBrains.Annotations;
using KaVE.VS.FeedbackGenerator.MessageBus;

namespace KaVE.VS.FeedbackGenerator.Generators.VisualStudio
{
    [ShellComponent]
    internal class VsFocusEventGenerator : EventGeneratorBase
    {
        public const int TimerIntervalSize = 1000;
        public const string MainWindowName = "main Microsoft Visual Studio";

        private readonly IFocusHelper _focusHelper;

        [UsedImplicitly]
        private Timer _timer;

        private bool _wasActive;

        public VsFocusEventGenerator(IRSEnv env,
            IMessageBus messageBus,
            IDateUtils dateUtils,
            IFocusHelper focusHelper,
            IThreading threading)
            : base(env, messageBus, dateUtils, threading)
        {
            _focusHelper = focusHelper;

            _timer = new Timer();
            _timer.Elapsed += OnTimerElapsed;
            _timer.Interval = TimerIntervalSize;
            _timer.Enabled = true;
        }

        public void OnTimerElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            _timer.Enabled = false;
            var isActive = _focusHelper.IsCurrentApplicationActive();
            if (isActive != _wasActive)
            {
                var windowAction = isActive ? WindowEvent.WindowAction.Activate : WindowEvent.WindowAction.Deactivate;

                var windowEvent = Create<WindowEvent>();
                windowEvent.Action = windowAction;
                        windowEvent.Window = Names.Window(MainWindowName);
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