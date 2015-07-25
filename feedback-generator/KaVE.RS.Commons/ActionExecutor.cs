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
using JetBrains.Application;
using JetBrains.Application.DataContext;
using JetBrains.DataFlow;
using JetBrains.UI.ActionsRevised;

namespace KaVE.RS.Commons
{
    // TODO RS9: TESTEN!!!
    [ShellComponent]
    public class ActionExecutor : IActionExecutor
    {
        private readonly IActionManager _manager;
        private readonly Lifetime _lifetime;

        public ActionExecutor(IActionManager manager, Lifetime lifetime)
        {
            _manager = manager;
            _lifetime = lifetime;
        }

        public void ExecuteActionGuarded<T>() where T : IExecutableAction
        {
            _manager.ExecuteActionGuarded<T>(_lifetime);
        }

        public void ExecuteActionGuarded<T>(IDataContext dataContext) where T : IExecutableAction
        {
            _manager.ExecuteActionGuarded<T>(_lifetime, dataContext: dataContext);
        }
    }

    public interface IActionExecutor
    {
        void ExecuteActionGuarded<T>() where T : IExecutableAction;
        void ExecuteActionGuarded<T>(IDataContext dataContext) where T : IExecutableAction;
    }
}