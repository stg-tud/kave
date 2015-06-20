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

using System.Linq;
using JetBrains.Application.Components;
using JetBrains.DataFlow;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Intentions;
using JetBrains.ReSharper.Intentions.Bulbs;
using JetBrains.ReSharper.Psi;
using JetBrains.TextControl;
using JetBrains.UI.BulbMenu;
using KaVE.Commons.Utils;
using KaVE.Commons.Utils.Assertion;
using KaVE.VS.FeedbackGenerator.MessageBus;

namespace KaVE.VS.FeedbackGenerator.Generators.ReSharper
{
    [SolutionComponent(ProgramConfigurations.VS_ADDIN)]
    internal class BulbItemInstrumentationComponent : IBulbItemsProvider
    {
        private readonly IRSEnv _env;
        private readonly IMessageBus _messageBus;
        private readonly IDateUtils _dateUtils;

        public BulbItemInstrumentationComponent(IRSEnv env, IMessageBus messageBus, IDateUtils dateUtils)
        {
            _env = env;
            _messageBus = messageBus;
            _dateUtils = dateUtils;
        }

        public void CollectSyncResults(object data, IntentionsBulbItems intentionsBulbItems, ITextControl textControl)
        {
            // TODO RS9
        }

        public int Priority
        {
            get
            {
                // to make sure we catch all bulb actions, we place this provider at the last possible
                // possition in the queue
                return int.MaxValue;
            }
        }

        // TODO RS9
        public object PreExecute(ITextControl textControl)
        {
            return null;
        }

        public void WaitRoslynTasks(object data)
        {
            // TODO RS9
        }

        public void CollectActions(IntentionsBulbItems intentionsBulbItems,
            BulbItems.BulbCache cacheData,
            ITextControl textControl,
            Lifetime caretPositionLifetime,
            IPsiSourceFile psiSourceFile,
            object precalculatedData)
        {
            var allBulbMenuItems = intentionsBulbItems.AllBulbMenuItems;
            foreach (var executableItem in allBulbMenuItems.Select(item => item.ExecutableItem))
            {
                var proxy = executableItem as IntentionAction.MyExecutableProxi;
                if (proxy != null)
                {
                    proxy.WrapBulbAction(_env, _messageBus, _dateUtils);
                    continue;
                }

                var executable = executableItem as ExecutableItem;
                if (executable != null)
                {
                    executable.WrapBulbAction(_env, _messageBus, _dateUtils);
                    continue;
                }

                Asserts.Fail("unexpected item type: {0}", executableItem.GetType().FullName);
            }
        }
    }
}