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

using EnvDTE;
using JetBrains.Application;
using JetBrains.Application.Components;
using KaVE.Commons.Model.Events.VisualStudio;
using KaVE.Commons.Utils;
using KaVE.VS.FeedbackGenerator.MessageBus;

namespace KaVE.VS.FeedbackGenerator.Generators.VisualStudio
{
    [ShellComponent(ProgramConfigurations.VS_ADDIN)]
    internal class FindEventGenerator : EventGeneratorBase
    {
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly FindEvents _findEvents;

        public FindEventGenerator(IRSEnv env, IMessageBus messageBus, IDateUtils dateUtils)
            : base(env, messageBus, dateUtils)
        {
            _findEvents = DTE.Events.FindEvents;

            _findEvents.FindDone += _findEvents_FindDone;
        }

        private FindEvent _lastEvent;

        private void _findEvents_FindDone(vsFindResult result, bool cancelled)
        {
            if (_lastEvent == null)
            {
                _lastEvent = Create<FindEvent>();
                _lastEvent.Cancelled = cancelled;
            }
            FireNow(_lastEvent);
            _lastEvent = null;
        }
    }
}