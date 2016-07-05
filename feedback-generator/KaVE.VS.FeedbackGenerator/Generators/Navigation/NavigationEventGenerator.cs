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
using KaVE.Commons.Model.Naming;
using KaVE.Commons.Model.Naming.Impl.v0;
using KaVE.Commons.Utils;
using KaVE.JetBrains.Annotations;
using KaVE.VS.FeedbackGenerator.MessageBus;

namespace KaVE.VS.FeedbackGenerator.Generators.Navigation
{
    [SolutionComponent]
    public class NavigationEventGenerator : NavigationEventGeneratorEventSubscriber
    {
        [NotNull]
        private readonly INavigationUtils _navigationUtils;

        [NotNull]
        private IName _currentLocation = Name.UnknownName;

        public NavigationEventGenerator([NotNull] IRSEnv env,
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
            var oldLocation = _currentLocation;
            var newLocation = _currentLocation = _navigationUtils.GetLocation(args.TextControl);

            var ctrlIsPressed = args.KeysAndButtons == KeyStateMasks.MK_CONTROL;
            if (ctrlIsPressed)
            {
                var ctrlClickEvent = Create<NavigationEvent>();
                ctrlClickEvent.Target = _navigationUtils.GetTarget(args.TextControl);
                ctrlClickEvent.Location = newLocation;
                ctrlClickEvent.TypeOfNavigation = NavigationType.CtrlClick;
                ctrlClickEvent.TriggeredBy = IDEEvent.Trigger.Click;

                _currentLocation = ctrlClickEvent.Target;

                Fire(ctrlClickEvent);
            }
            else if (IsNewLocation(oldLocation, newLocation))
            {
                var clickNavigationEvent = Create<NavigationEvent>();
                clickNavigationEvent.Location = newLocation;
                clickNavigationEvent.TypeOfNavigation = NavigationType.Click;
                clickNavigationEvent.TriggeredBy = IDEEvent.Trigger.Click;
                Fire(clickNavigationEvent);
            }
        }

        public void OnKeyPress(EventArgs<ITextControl> args)
        {
            var oldLocation = _currentLocation;
            var newLocation = _currentLocation = _navigationUtils.GetLocation(args.Value);

            if (!IsNewLocation(oldLocation, newLocation))
            {
                return;
            }

            var keyboardNavigationEvent = Create<NavigationEvent>();
            keyboardNavigationEvent.Location = newLocation;
            keyboardNavigationEvent.TypeOfNavigation = NavigationType.Keyboard;
            keyboardNavigationEvent.TriggeredBy = IDEEvent.Trigger.Typing;
            Fire(keyboardNavigationEvent);
        }

        private static bool IsNewLocation(IName location, IName newLocation)
        {
            return !newLocation.Equals(location);
        }

        protected override void Advice(ITextControlWindow textControlWindow, Lifetime lifetime)
        {
            textControlWindow.MouseUp.Advise(lifetime, OnClick);
            textControlWindow.Keyboard.Advise(lifetime, OnKeyPress);
        }
    }

    public abstract class NavigationEventGeneratorEventSubscriber : EventGeneratorBase
    {
        private readonly ITextControlManager _textControlManager;
        private readonly Lifetime _myLifetime;

        protected NavigationEventGeneratorEventSubscriber([NotNull] IRSEnv env,
            [NotNull] IMessageBus messageBus,
            [NotNull] IDateUtils dateUtils,
            [NotNull] ITextControlManager textControlManager,
            [NotNull] Lifetime lifetime) : base(env, messageBus, dateUtils)
        {
            _myLifetime = lifetime;
            _textControlManager = textControlManager;

            AdviceOnAllTextControls();
            UnsubscribeOnTerminate();
        }

        protected abstract void Advice([NotNull] ITextControlWindow textControlWindow, [NotNull] Lifetime lifetime);

        private void AdviceOnAllTextControls()
        {
            foreach (var textControl in _textControlManager.TextControls)
            {
                Advice(textControl.Window, _myLifetime);
            }

            _textControlManager.TextControls.CollectionChanged += AdviceOnNewControls;
        }

        private void AdviceOnNewControls(object sender, NotifyCollectionChangedEventArgs args)
        {
            if (args.NewItems != null)
            {
                foreach (var newTextControl in args.NewItems.Cast<ITextControl>())
                {
                    Advice(newTextControl.Window, _myLifetime);
                }
            }
        }

        private void UnsubscribeOnTerminate()
        {
            _myLifetime.AddAction(() => _textControlManager.TextControls.CollectionChanged -= AdviceOnNewControls);
        }
    }
}