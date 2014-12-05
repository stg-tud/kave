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

using JetBrains.ProjectModel;
using JetBrains.ReSharper.Intentions.Extensibility;
using JetBrains.TextControl;
using KaVE.VsFeedbackGenerator.MessageBus;
using KaVE.VsFeedbackGenerator.Utils;

namespace KaVE.VsFeedbackGenerator.Generators.ReSharper
{
    internal class EventGeneratingBulbActionProxy :
        CommandEventGeneratorBase<EventGeneratingBulbActionProxy.BulbActionContext>, IBulbAction
    {
        internal class BulbActionContext
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
            Execute(new BulbActionContext {Solution = solution, TextControl = textControl});
        }

        public string Text
        {
            get { return _target.Text; }
        }

        protected override string GetCommandId()
        {
            return _target.GetType().FullName;
        }

        protected override void InvokeOriginalCommand(BulbActionContext context)
        {
            _target.Execute(context.Solution, context.TextControl);
        }
    }
}