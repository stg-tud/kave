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
using JetBrains.TextControl;
using KaVE.Commons.Model.Events;
using KaVE.Commons.Model.Names;
using KaVE.Commons.Utils;
using KaVE.JetBrains.Annotations;
using KaVE.VS.FeedbackGenerator.MessageBus;

namespace KaVE.VS.FeedbackGenerator.Generators.Navigation
{
    public abstract class NavigationEventGeneratorBase : EventGeneratorBase
    {
        private readonly ITextControlManager _textControlManager;
        private readonly Lifetime _myLifetime;

        protected NavigationEventGeneratorBase([NotNull] IRSEnv env,
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

        protected void FireNavigationEvent(
            [NotNull] IName target,
            [NotNull] IName location,
            NavigationType typeOfNavigation,
            IDEEvent.Trigger triggeredBy)
        {
            var navigationEvent = Create<NavigationEvent>();
            navigationEvent.Target = target;
            navigationEvent.Location = location;
            navigationEvent.TypeOfNavigation = typeOfNavigation;
            navigationEvent.TriggeredBy = triggeredBy;
            Fire(navigationEvent);
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