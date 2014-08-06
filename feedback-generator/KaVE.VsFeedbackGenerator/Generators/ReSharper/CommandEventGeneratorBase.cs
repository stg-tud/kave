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
using KaVE.Model.Events;
using KaVE.VsFeedbackGenerator.MessageBus;
using KaVE.VsFeedbackGenerator.Utils;
using KaVE.VsFeedbackGenerator.VsIntegration;

namespace KaVE.VsFeedbackGenerator.Generators.ReSharper
{
    internal abstract class CommandEventGeneratorBase : CommandEventGeneratorBase<object>
    {
        protected CommandEventGeneratorBase(IIDESession session, IMessageBus messageBus, IDateUtils dateUtils) : base(session, messageBus, dateUtils) {}

        public void Execute()
        {
            Execute(null);
        }

        protected override void InvokeOriginalCommand(object context)
        {
            InvokeOriginalCommand();
        }

        protected abstract void InvokeOriginalCommand();
    }

    internal abstract class CommandEventGeneratorBase<TC> : EventGeneratorBase where TC : class
    {
        protected CommandEventGeneratorBase(IIDESession session, IMessageBus messageBus, IDateUtils dateUtils)
            : base(session, messageBus, dateUtils) {}

        protected void Execute(TC context)
        {
            var actionEvent = CreateActionEvent();
            InvokeOriginalCommand(context);
            FireActionEventNow(actionEvent);
        }

        private CommandEvent CreateActionEvent()
        {
            var actionEvent = Create<CommandEvent>();
            actionEvent.CommandId = GetCommandId();
            return actionEvent;
        }

        protected abstract string GetCommandId();

        protected abstract void InvokeOriginalCommand(TC context);

        private void FireActionEventNow(CommandEvent actionEvent)
        {
            try
            {
                FireNow(actionEvent);
            }
            catch (Exception e)
            {
                e = new Exception("generating command event failed", e);
                Registry.GetComponent<ILogger>().Error(e);
            }
        }
    }
}