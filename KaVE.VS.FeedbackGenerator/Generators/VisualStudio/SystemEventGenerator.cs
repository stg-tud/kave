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

using JetBrains.Application;
using JetBrains.Threading;
using KaVE.Commons.Model.Events;
using KaVE.Commons.Utils;
using KaVE.JetBrains.Annotations;
using KaVE.VS.FeedbackGenerator.MessageBus;
using Microsoft.Win32;

namespace KaVE.VS.FeedbackGenerator.Generators.VisualStudio
{
    [ShellComponent]
    internal class SystemEventGenerator : EventGeneratorBase
    {
        public SystemEventGenerator([NotNull] IRSEnv env,
            [NotNull] IMessageBus messageBus,
            [NotNull] IDateUtils dateUtils,
            [NotNull] IThreading threading)
            : base(env, messageBus, dateUtils, threading)
        {
            SystemEvents.PowerModeChanged += SystemEvents_PowerModeChanged;
            SystemEvents.SessionSwitch += SystemEvents_SessionSwitch;
        }

        ~SystemEventGenerator()
        {
            SystemEvents.PowerModeChanged -= SystemEvents_PowerModeChanged;
            SystemEvents.SessionSwitch -= SystemEvents_SessionSwitch;
        }

        private void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e)
        {
            switch (e.Reason)
            {
                case SessionSwitchReason.SessionLock:
                    FireSystemEvent(SystemEventType.Lock);
                    break;
                case SessionSwitchReason.SessionUnlock:
                    FireSystemEvent(SystemEventType.Unlock);
                    break;
                case SessionSwitchReason.RemoteConnect:
                    FireSystemEvent(SystemEventType.RemoteConnect);
                    break;
                case SessionSwitchReason.RemoteDisconnect:
                    FireSystemEvent(SystemEventType.RemoteDisconnect);
                    break;
            }
        }

        private void SystemEvents_PowerModeChanged(object sender, PowerModeChangedEventArgs e)
        {
            switch (e.Mode)
            {
                case PowerModes.Suspend:
                    FireSystemEvent(SystemEventType.Suspend);
                    break;
                case PowerModes.Resume:
                    FireSystemEvent(SystemEventType.Resume);
                    break;
            }
        }

        private void FireSystemEvent(SystemEventType type)
        {
            var se = Create<SystemEvent>();
            se.Type = type;
            FireNow(se);
        }
    }
}