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
using JetBrains.ActionManagement;
using JetBrains.Application.DataContext;
using KaVE.Model.Events.ReSharper;
using KaVE.VsFeedbackGenerator.MessageBus;
using KaVE.VsFeedbackGenerator.Utils;
using KaVE.VsFeedbackGenerator.VsIntegration;

namespace KaVE.VsFeedbackGenerator.Generators.ReSharper
{
    /// <summary>
    ///     Fires an <see cref="ActionEvent" /> on execution of a ReSharper action. Passes handling of the action on the
    ///     the default handler.
    /// </summary>
    internal class EventGeneratingActionHandler : EventGeneratorBase, IActionHandler
    {
        private readonly IUpdatableAction _updatableAction;

        public EventGeneratingActionHandler(IUpdatableAction updatableAction,
            IIDESession session,
            IMessageBus messageBus)
            : base(session, messageBus)
        {
            _updatableAction = updatableAction;
        }

        public bool Update(IDataContext context, ActionPresentation presentation, DelegateUpdate nextUpdate)
        {
            return nextUpdate.Invoke();
        }

        public void Execute(IDataContext context, DelegateExecute nextExecute)
        {
            FireActionEvent();
            nextExecute.Invoke();
        }

        private void FireActionEvent()
        {
            try
            {
                var actionEvent = Create<ActionEvent>();
                actionEvent.ActionId = _updatableAction.Id;
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