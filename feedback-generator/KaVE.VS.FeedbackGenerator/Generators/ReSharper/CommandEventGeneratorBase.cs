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
using KaVE.Commons.Model.Events;
using KaVE.Commons.Utils;
using KaVE.Commons.Utils.Exceptions;
using KaVE.RS.Commons.Utils;
using KaVE.VS.FeedbackGenerator.MessageBus;

namespace KaVE.VS.FeedbackGenerator.Generators.ReSharper
{
    public abstract class CommandEventGeneratorBase : EventGeneratorBase
    {
        protected CommandEventGeneratorBase(IRSEnv env, IMessageBus messageBus, IDateUtils dateUtils)
            : base(env, messageBus, dateUtils) {}

        protected void FireActionEvent(string actionId)
        {
            var actionEvent = CreateActionEvent(actionId);
            FireActionEventNow(actionEvent);
        }

        private CommandEvent CreateActionEvent(string actionId)
        {
            var actionEvent = Create<CommandEvent>();
            actionEvent.CommandId = actionId;
            return actionEvent;
        }

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