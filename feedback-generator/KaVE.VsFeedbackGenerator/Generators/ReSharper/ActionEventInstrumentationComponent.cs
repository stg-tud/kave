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
using JetBrains.DataFlow;
using KaVE.Commons.Utils;
using KaVE.VS.FeedbackGenerator.Export;
using KaVE.VS.FeedbackGenerator.MessageBus;
using KaVE.VS.FeedbackGenerator.Utils;

namespace KaVE.VS.FeedbackGenerator.Generators.ReSharper
{
    [ShellComponent(ProgramConfigurations.VS_ADDIN)]
    internal class ActionEventInstrumentationComponent
    {
        public ActionEventInstrumentationComponent(Lifetime lifetime,
            IActionManager actionManager,
            IRSEnv env,
            IMessageBus messageBus,
            IDateUtils dateUtils)
        {
            foreach (var actionId in GetInstrumentableActionIds(actionManager))
            {
                var handler = new EventGeneratingActionHandler(actionId, env, messageBus, dateUtils);
                // TODO RS9
                //action.AddHandler(lifetime, handler);
            }
        }

        private IEnumerable<string> GetInstrumentableActionIds(IActionManager actionManager)
        {
            var allIds = actionManager.Defs.GetAllActionDefs().Select(d => d.ActionId);
            return allIds.Where(id => id != SettingsCleaner.ActionId && id != UploadWizardAction.Id);
        }

        //TODO RS9
        /*
        private static IEnumerable<IUpdatableAction> GetInstrumentableActions(IActionManager actionManager)
        {
            return actionManager.GetAllActions().OfType<IUpdatableAction>().Where(IsNoPrivateAction);
        }

        private static bool IsNoPrivateAction(IUpdatableAction updatableAction)
        {
            var id = updatableAction.Id;
            return id != SettingsCleaner.ActionId && id != UploadWizardActionHandler.ActionId;
        }*/
    }
}