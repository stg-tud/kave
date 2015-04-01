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

using JetBrains.ActionManagement;
using JetBrains.Application.DataContext;
using KaVE.Commons.Model.Events;
using KaVE.VsFeedbackGenerator.MessageBus;
using KaVE.VsFeedbackGenerator.Utils;

namespace KaVE.VsFeedbackGenerator.Generators.ReSharper
{
    /// <summary>
    ///     Fires an <see cref="CommandEvent" /> on execution of a ReSharper action. Passes handling of the action on the
    ///     the default handler.
    /// </summary>
    internal class EventGeneratingActionHandler : CommandEventGeneratorBase<DelegateExecute>, IActionHandler
    {
        private readonly IUpdatableAction _updatableAction;

        public EventGeneratingActionHandler(IUpdatableAction updatableAction,
            IRSEnv env,
            IMessageBus messageBus,
            IDateUtils dateUtils)
            : base(env, messageBus, dateUtils)
        {
            _updatableAction = updatableAction;
        }

        public bool Update(IDataContext context, ActionPresentation presentation, DelegateUpdate nextUpdate)
        {
            return nextUpdate.Invoke();
        }

        public void Execute(IDataContext context, DelegateExecute nextExecute)
        {
            Execute(nextExecute);
        }

        protected override string GetCommandId()
        {
            return _updatableAction.Id;
        }

        protected override void InvokeOriginalCommand(DelegateExecute nextExecute)
        {
            nextExecute.Invoke();
        }
    }
}