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

using System.Collections.Specialized;
using System.Linq;
using JetBrains.DataFlow;
using JetBrains.Interop.WinApi;
using JetBrains.ProjectModel;
using JetBrains.TextControl;
using KaVE.Commons.Model.Events;
using KaVE.Commons.Model.Names;
using KaVE.Commons.Utils;
using KaVE.JetBrains.Annotations;
using KaVE.VS.FeedbackGenerator.MessageBus;

namespace KaVE.VS.FeedbackGenerator.Generators.Navigation
{
    [SolutionComponent]
    public class ClickNavigationEventGenerator : EventGeneratorBase
    {
        private readonly Lifetime _myLifetime;
        private readonly INavigationUtils _navigationUtils;

        [CanBeNull]
        private IName _oldLocation;

        public ClickNavigationEventGenerator([NotNull] IRSEnv env,
            [NotNull] IMessageBus messageBus,
            [NotNull] IDateUtils dateUtils,
            [NotNull] ITextControlManager textControlManager,
            [NotNull] INavigationUtils navigationUtils,
            [NotNull] Lifetime lifetime) : base(env, messageBus, dateUtils)
        {
            _navigationUtils = navigationUtils;
            _myLifetime = lifetime;

            foreach (var textControl in textControlManager.TextControls)
            {
                AdviceOnClick(textControl);
            }

            textControlManager.TextControls.CollectionChanged += AdviceOnNewControls;
        }

        public void OnClick(TextControlMouseEventArgs args)
        {
            var newLocation = _navigationUtils.GetLocation(args.TextControl);

            if (_oldLocation != null && args.KeysAndButtons == KeyStateMasks.MK_LBUTTON)
            {
                if (!Equals(newLocation, _oldLocation))
                {
                    var clickEvent = Create<NavigationEvent>();
                    clickEvent.Target = newLocation;
                    clickEvent.Location = _oldLocation;
                    clickEvent.TypeOfNavigation = NavigationEvent.NavigationType.Click;
                    clickEvent.TriggeredBy = IDEEvent.Trigger.Click;
                    Fire(clickEvent);
                }
            }

            _oldLocation = newLocation;
        }

        private void AdviceOnNewControls(object sender, NotifyCollectionChangedEventArgs args)
        {
            if (args.NewItems != null)
            {
                foreach (var newTextControl in args.NewItems.Cast<ITextControl>())
                {
                    AdviceOnClick(newTextControl);
                }
            }
        }

        private void AdviceOnClick(ITextControl textControl)
        {
            textControl.Window.MouseDown.Advise(_myLifetime, OnClick);
        }
    }
}