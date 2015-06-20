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
using JetBrains.Annotations;
using JetBrains.Application.DataContext;
using JetBrains.DataFlow;
using JetBrains.Threading;

namespace KaVE.ReSharper.Commons.Utils
{
    public static class ActionManagerExtensions
    {
        public static bool ExecuteActionGuarded(this IActionManager actionManager,
            string actionId,
            string executeName,
            [NotNull] IDataContext dataContext)
        {
            var threading = Registry.GetComponent<IThreading>();
            var action = actionManager.Defs.TryGetActionDefById(actionId);
            if (action != null)
            {
                return threading.ReentrancyGuard.ExecuteOrQueue(
                    EternalLifetime.Instance,
                    executeName,
                    // TODO RS9
                    () => actionManager.Handlers.Evaluate(action, dataContext.Prolongate(Lifetimes.Create())));
            }
            return false;
        }
    }
}