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
 *    - Uli Fahrer
 */

using JetBrains.ActionManagement;
using JetBrains.Application.DataContext;
using JetBrains.UI.ActionsRevised;
using JetBrains.UI.Options;
using KaVE.ReSharper.Commons.Utils;
using KaVE.VsFeedbackGenerator.SessionManager.Presentation;

namespace KaVE.VsFeedbackGenerator.Menu
{
    [Action(Id, "Options...", Id = 123451)]
    public class OptionPageAction : IExecutableAction
    {
        internal const string Id = "KaVE.Options";
        private readonly IActionManager _actionManager;

        public OptionPageAction()
        {
            _actionManager = Registry.GetComponent<IActionManager>();
        }

        public bool Update(IDataContext context, ActionPresentation presentation, DelegateUpdate nextUpdate)
        {
            return true;
        }

        public void Execute(IDataContext context, DelegateExecute nextExecute)
        {
            _actionManager.ExecuteActionGuarded<ShowOptionsAction>(SessionManagerWindowRegistrar._lifetime);
        }
    }
}