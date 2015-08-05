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

using JetBrains.ActionManagement;
using JetBrains.Application.DataContext;
using JetBrains.UI.ActionsRevised;
using KaVE.Commons.Model.Events;
using KaVE.Commons.Utils;
using KaVE.VS.FeedbackGenerator.MessageBus;

namespace KaVE.VS.FeedbackGenerator.Generators.ReSharper
{
    /// <summary>
    ///     Fires an <see cref="CommandEvent" /> on execution of a ReSharper action. Passes handling of the action on the
    ///     the default handler.
    /// </summary>
    public class EventGeneratingActionHandler : CommandEventGeneratorBase<DelegateExecute>, IExecutableAction,
        ICheckableAction, IPresentableAction
    {
        private readonly string _actionId;

        public EventGeneratingActionHandler(string actionId, IRSEnv env, IMessageBus messageBus, IDateUtils dateUtils)
            : base(env, messageBus, dateUtils)
        {
            _actionId = actionId;
        }

        public bool Update(IDataContext context, ActionPresentation presentation, DelegateUpdate nextUpdate)
        {
            // TODO: It seems like nextUpdate disables some ReSharper Actions that can still be used (e.g. CodeCleanup), disabling our Events for them.
            nextUpdate.Invoke();
            return true;
        }

        public bool Update(IDataContext context, CheckedActionPresentation presentation)
        {
            return true;
        }

        public bool Update(IDataContext context, ActionPresentation actionPresentation)
        {
            return true;
        }

        public void Execute(IDataContext context, DelegateExecute nextExecute)
        {
            FireActionEvent();
            nextExecute.Invoke();
        }

        public void Execute(IDataContext context)
        {
            FireActionEvent();
        }

        protected override string GetCommandId()
        {
            return _actionId;
        }
    }
}