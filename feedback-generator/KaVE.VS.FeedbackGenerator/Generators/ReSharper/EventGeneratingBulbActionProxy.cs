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

using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Bulbs;
using JetBrains.TextControl;
using KaVE.Commons.Utils;
using KaVE.VS.FeedbackGenerator.MessageBus;

namespace KaVE.VS.FeedbackGenerator.Generators.ReSharper
{
    public class EventGeneratingBulbActionProxy : CommandEventGeneratorBase, IBulbAction
    {
        public class BulbActionContext
        {
            public ISolution Solution { get; set; }
            public ITextControl TextControl { get; set; }
        }

        private readonly IBulbAction _target;

        public EventGeneratingBulbActionProxy(IBulbAction target,
            IRSEnv env,
            IMessageBus messageBus,
            IDateUtils dateUtils)
            : base(env, messageBus, dateUtils)
        {
            _target = target;
        }

        public void Execute(ISolution solution, ITextControl textControl)
        {
            var commandId = _target.GetType().FullName;
            FireActionEvent(commandId);
            ExecuteOriginalAction(solution, textControl);
        }

        public string Text
        {
            get { return _target.Text; }
        }

        protected void ExecuteOriginalAction(ISolution solution, ITextControl textControl)
        {
            _target.Execute(solution, textControl);
        }
    }
}