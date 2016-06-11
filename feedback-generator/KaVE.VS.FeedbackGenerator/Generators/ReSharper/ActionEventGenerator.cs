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
using JetBrains.Application.ActivityTrackingNew;
using JetBrains.Threading;
using KaVE.Commons.Model.Events;
using KaVE.Commons.Utils;
using KaVE.JetBrains.Annotations;
using KaVE.VS.FeedbackGenerator.MessageBus;

namespace KaVE.VS.FeedbackGenerator.Generators.ReSharper
{
    [ShellComponent]
    internal class ActionEventGenerator : EventGeneratorBase, IActivityTracking
    {
        private readonly IThreading _threading;

        public ActionEventGenerator([NotNull] IRSEnv env,
            [NotNull] IMessageBus messageBus,
            [NotNull] IDateUtils dateUtils,
            [NotNull] IThreading threading)
            : base(env, messageBus, dateUtils)
        {
            _threading = threading;
        }

        public void TrackAction(string actionId)
        {
            FireActionEvent(actionId);
        }

        public void TrackActivity(string activityGroup, string activityId, int count = 1)
        {
            FireActionEvent(string.Format("{0}:{1}:{2}", activityGroup, count, activityId));
        }

        protected void FireActionEvent(string actionId)
        {
            _threading.ExecuteOrQueue(
                "ActionEventGenerator.FireActionEvent",
                () =>
                {
                    var actionEvent = Create<CommandEvent>();
                    actionEvent.CommandId = actionId;
                    Fire(actionEvent);
                });
        }
    }
}