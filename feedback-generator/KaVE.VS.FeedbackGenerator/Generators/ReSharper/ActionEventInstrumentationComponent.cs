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
using System.Collections.Generic;
using System.Linq;
using JetBrains.ActionManagement;
using JetBrains.Application;
using JetBrains.Application.Components;
using JetBrains.UI.ActionsRevised.Loader;
using JetBrains.Util;
using KaVE.Commons.Utils;
using KaVE.VS.FeedbackGenerator.MessageBus;
using KaVE.VS.FeedbackGenerator.Settings;
using KaVE.VS.FeedbackGenerator.UserControls.Export;
using ILogger = KaVE.Commons.Utils.Exceptions.ILogger;

namespace KaVE.VS.FeedbackGenerator.Generators.ReSharper
{
    [ShellComponent(ProgramConfigurations.VS_ADDIN)]
    internal class ActionEventInstrumentationComponent
    {
        public ActionEventInstrumentationComponent(IActionManager actionManager,
            IRSEnv env,
            IMessageBus messageBus,
            IDateUtils dateUtils,
            ILogger logger)
        {
            var instrumentableActions = GetInstrumentableActions(actionManager);
            foreach (var action in instrumentableActions)
            {
                var handler = new EventGeneratingActionHandler(action.ActionId, env, messageBus, dateUtils);
                try
                {
                    actionManager.Handlers.AddHandler(action, handler);
                }
                catch (InvalidOperationException e)
                {
                    if (e.Message.Equals("Cannot add handlers to a StaticBound action."))
                    {
                        // There's no way for us the check whether the action is static bound, beforehand.
                        continue;
                    }
                    logger.Error(e, "exception while adding handler for actionId '{0}'", action.ActionId);
                }
                catch (Assertion.AssertionException e)
                {
                    logger.Error(e, "exception while adding handler for actionId '{0}'", action.ActionId);
                }
            }
        }

        private static IEnumerable<IActionDefWithId> GetInstrumentableActions(IActionManager actionManager)
        {
            return actionManager.Defs.GetAllActionDefs().Where(IsNoPrivateAction);
        }

        private static bool IsNoPrivateAction(IActionDefWithId actionDef)
        {
            var id = actionDef.ActionId;
            return id != SettingsCleaner.ActionId && id != UploadWizardAction.Id;
        }
    }
}