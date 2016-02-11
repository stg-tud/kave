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
using KaVE.Commons.Model.Events.VisualStudio;
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
            [NotNull] IDateUtils dateUtils) : base(env, messageBus, dateUtils)
        {
            SystemEvents.PowerModeChanged += SystemEvents_PowerModeChanged;
            SystemEvents.SessionSwitch += SystemEvents_SessionSwitch;
        }

        ~SystemEventGenerator()
        {
            SystemEvents.PowerModeChanged -= SystemEvents_PowerModeChanged;
            SystemEvents.SessionSwitch -= SystemEvents_SessionSwitch;
        }

        void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e)
        {
            if (e.Reason == SessionSwitchReason.SessionLock)
            {
                FireSystemEvent(SystemEvent.SystemEventType.Lock);
            }
            else if (e.Reason == SessionSwitchReason.SessionUnlock)
            {
                FireSystemEvent(SystemEvent.SystemEventType.Unlock);
            }
            else if (e.Reason == SessionSwitchReason.RemoteConnect)
            {
                FireSystemEvent(SystemEvent.SystemEventType.RemoteConnect);
            }
            else if (e.Reason == SessionSwitchReason.RemoteDisconnect)
            {
                FireSystemEvent(SystemEvent.SystemEventType.RemoteDisconnect);
            }
        }

        void SystemEvents_PowerModeChanged(object sender, PowerModeChangedEventArgs e)
        {
            if (e.Mode == PowerModes.Suspend)
            {
                FireSystemEvent(SystemEvent.SystemEventType.Suspend);
            }
            else if (e.Mode == PowerModes.Resume)
            {
                FireSystemEvent(SystemEvent.SystemEventType.Resume);
            }
        }

        private void FireSystemEvent(SystemEvent.SystemEventType type)
        {
            var se = Create<SystemEvent>();
            se.Type = type;
            FireNow(se);
        }
    }
}