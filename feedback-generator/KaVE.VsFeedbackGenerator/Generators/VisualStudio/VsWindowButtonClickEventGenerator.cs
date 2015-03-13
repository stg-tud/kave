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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using JetBrains.Util;
using KaVE.JetBrains.Annotations;
using KaVE.Model.Events;
using KaVE.Utils.Reflection;
using KaVE.VsFeedbackGenerator.MessageBus;
using KaVE.VsFeedbackGenerator.Utils;
using ILogger = KaVE.Utils.Exceptions.ILogger;
using Window = EnvDTE.Window;

namespace KaVE.VsFeedbackGenerator.Generators.VisualStudio
{
    internal class VsWindowButtonClickEventGenerator : EventGeneratorBase
    {
        private static readonly ISet<WeakReference<Button>> ButtonRegistry = new HashSet<WeakReference<Button>>();

        private readonly object _frame;
        private FrameworkElement _frameContent;
        private readonly ILogger _logger;

        public VsWindowButtonClickEventGenerator(Window window,
            [NotNull] IRSEnv env,
            [NotNull] IMessageBus messageBus,
            [NotNull] IDateUtils dateUtils,
            [NotNull] ILogger logger)
            : base(env, messageBus, dateUtils)
        {
            _frame = GetFrame(window);
            _logger = logger;

            RegisterToFrameEvents();
        }

        private static object GetFrame(Window window)
        {
            var windowImpl = window.GetPrivateFieldValue<object>("_impl");
            return windowImpl.GetPublicPropertyValue<object>("Frame");
        }

        private void RegisterToFrameEvents()
        {
            // if view changes, content also changes
            ((INotifyPropertyChanged) _frame).PropertyChanged += (sender, args) =>
            {
                if (Equals(args.PropertyName, "FrameView"))
                {
                    WindowChanged();
                }
            };
            // dispose of frame invalidates content (and window)
            _frame.RegisterToEvent(
                "Disposing",
                (EventHandler) ((content, args) => UnregisterFromContentUpdates()));
        }

        public void WindowChanged()
        {
            UnregisterFromContentUpdates();
            UpdateFrameContent();
            RegisterToContentUpdates();
        }

        private void UnregisterFromContentUpdates()
        {
            if (_frameContent != null)
            {
                _frameContent.LayoutUpdated -= RegisterToAllNewButtons;
            }
        }

        private void UpdateFrameContent()
        {
            try
            {
                _frameContent = _frame.GetPrivatePropertyValue<FrameworkElement>("Content");
            }
            catch (Exception e)
            {
                _logger.Error(e, "couldn't get frame content");
                _frameContent = null;
            }
        }

        private void RegisterToContentUpdates()
        {
            if (_frameContent != null)
            {
                _frameContent.LayoutUpdated += RegisterToAllNewButtons;
            }
        }

        private void RegisterToAllNewButtons(object sender, EventArgs eventArgs)
        {
            _frameContent.FindChildren<Button>().Where(WasNotSeenBefore).ForEach(RegisterToButton);
        }

        private static bool WasNotSeenBefore(Button view)
        {
            return ButtonRegistry.Add(new WeakReference<Button>(view));
        }

        private void RegisterToButton(Button button)
        {
            button.Click += delegate { FireCommandEvent(button); };
        }

        private void FireCommandEvent(Button button)
        {
            try
            {
                var commandEvent = Create<CommandEvent>();
                commandEvent.TriggeredBy = IDEEvent.Trigger.Click;
                commandEvent.CommandId = button.GetId();
                Fire(commandEvent);
            }
            catch (Exception e)
            {
                _logger.Error(e, "failed to fire command event");
            }
        }
    }

    internal static class WpfHelper
    {
        internal static string GetId(this Button b)
        {
            return ToString(b.ToolTip) ?? ToString(b.Content) ?? ToString(b.Name) ?? "unknown button";
        }

        private static string ToString(object wpfContent)
        {
            var stringContent = wpfContent as string;
            if (!stringContent.IsNullOrEmpty())
            {
                return stringContent;
            }
            var xmlContent = wpfContent as System.Xml.XmlElement;
            if (xmlContent != null)
            {
                return xmlContent.InnerText;
            }
            // Don't fall back on ToString(), because it sometimes throws!
            return null;
        }

        internal static IEnumerable<T> FindChildren<T>(this DependencyObject parent)
            where T : DependencyObject
        {
            if (parent == null)
            {
                return null;
            }

            IList<T> foundChildren = new List<T>();

            var childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (var i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                var childType = child as T;
                if (childType == null)
                {
                    foundChildren.AddRange(FindChildren<T>(child));
                }
                else
                {
                    foundChildren.Add((T) child);
                }
            }

            return foundChildren;
        }
    }
}