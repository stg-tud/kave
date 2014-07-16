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
using KaVE.Model.Events.ReSharper;
using KaVE.VsFeedbackGenerator.MessageBus;
using KaVE.VsFeedbackGenerator.Utils;
using KaVE.VsFeedbackGenerator.VsIntegration;

namespace KaVE.VsFeedbackGenerator.Generators.ReSharper
{
    internal class EventGeneratingActionWrapper : EventGeneratorBase
    {
        private readonly Action _originalAction;

        public EventGeneratingActionWrapper(Action originalAction,
            IIDESession session,
            IMessageBus messageBus,
            IDateUtils dateUtils)
            : base(session, messageBus, dateUtils)
        {
            _originalAction = originalAction;
        }

        public void Execute()
        {
            var actionEvent = CreateActionEvent();
            _originalAction.Invoke();
            FireActionEventNow(actionEvent);
        }

        private BulbActionEvent CreateActionEvent()
        {
            var actionEvent = Create<BulbActionEvent>();
            actionEvent.ActionId = _originalAction.Target.ToString();
            return actionEvent;
        }

        private void FireActionEventNow(BulbActionEvent actionEvent)
        {
            try
            {
                FireNow(actionEvent);
            }
            catch (Exception e)
            {
                e = new Exception("generating action event failed", e);
                Registry.GetComponent<ILogger>().Error(e);
            }
        }
    }
}