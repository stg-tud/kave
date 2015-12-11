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

using JetBrains.DataFlow;
using JetBrains.Interop.WinApi;
using JetBrains.ProjectModel;
using JetBrains.TextControl;
using KaVE.Commons.Model.Events;
using KaVE.Commons.Utils;
using KaVE.JetBrains.Annotations;
using KaVE.VS.FeedbackGenerator.MessageBus;

namespace KaVE.VS.FeedbackGenerator.Generators.Navigation
{
    [SolutionComponent]
    internal class CtrlClickEventGenerator : NavigationEventGeneratorBase
    {
        private readonly INavigationUtils _navigationUtils;

        public CtrlClickEventGenerator([NotNull] IRSEnv env,
            [NotNull] IMessageBus messageBus,
            [NotNull] IDateUtils dateUtils,
            [NotNull] ITextControlManager textControlManager,
            [NotNull] INavigationUtils navigationUtils,
            [NotNull] Lifetime lifetime) : base(env, messageBus, dateUtils, textControlManager, lifetime)
        {
            _navigationUtils = navigationUtils;
        }

        public void OnClick(TextControlMouseEventArgs args)
        {
            if (args.KeysAndButtons == KeyStateMasks.MK_CONTROL)
            {
                FireNavigationEvent(
                    _navigationUtils.GetTarget(args.TextControl),
                    _navigationUtils.GetLocation(args.TextControl),
                    NavigationEvent.NavigationType.CtrlClick,
                    IDEEvent.Trigger.Click);
            }
        }

        protected override void Advice(ITextControlWindow textControlWindow, Lifetime lifetime)
        {
            textControlWindow.MouseUp.Advise(lifetime, OnClick);
        }
    }
}