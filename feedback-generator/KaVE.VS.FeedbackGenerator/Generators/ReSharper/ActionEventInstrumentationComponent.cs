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

using System.Collections.Generic;
using System.Linq;
using JetBrains.ActionManagement;
using JetBrains.Application;
using JetBrains.Application.Components;
using JetBrains.UI.ActionsRevised.Loader;
using KaVE.Commons.Utils;
using KaVE.VS.FeedbackGenerator.Export;
using KaVE.VS.FeedbackGenerator.MessageBus;
using KaVE.VS.FeedbackGenerator.Utils;

namespace KaVE.VS.FeedbackGenerator.Generators.ReSharper
{
    [ShellComponent(ProgramConfigurations.VS_ADDIN)]
    internal class ActionEventInstrumentationComponent
    {
        public ActionEventInstrumentationComponent(IActionManager actionManager,
            IRSEnv env,
            IMessageBus messageBus,
            IDateUtils dateUtils)
        {
            foreach (var action in GetInstrumentableActions(actionManager))
            {
                var handler = new EventGeneratingActionHandler(action.ActionId, env, messageBus, dateUtils);
                actionManager.Handlers.AddHandler(action, handler);
            }
        }

        private static IEnumerable<IActionDefWithId> GetInstrumentableActions(IActionManager actionManager)
        {
            // TODO @seb/@sven: there are two implementations of IActionManager, one offers access to VS actions... might be interesting to look at
            return actionManager.Defs.GetAllActionDefs().Where(IsNoPrivateAction);
        }

        private static bool IsNoPrivateAction(IActionDefWithId updatableAction)
        {
            var id = updatableAction.ActionId;
            return id != SettingsCleaner.ActionId && id != UploadWizardAction.Id;
        }
    }
}