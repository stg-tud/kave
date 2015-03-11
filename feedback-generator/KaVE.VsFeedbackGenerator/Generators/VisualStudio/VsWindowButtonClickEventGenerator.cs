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
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using JetBrains.Util;
using KaVE.JetBrains.Annotations;
using KaVE.Model.Events;
using KaVE.Utils.Assertion;
using KaVE.VsFeedbackGenerator.MessageBus;
using KaVE.VsFeedbackGenerator.Utils;
using ILogger = KaVE.Utils.Exceptions.ILogger;
using Window = EnvDTE.Window;

namespace KaVE.VsFeedbackGenerator.Generators.VisualStudio
{
    internal class VsWindowButtonClickEventGenerator : EventGeneratorBase
    {
        private readonly object _frame;
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

            RegisterToFrameUpdates();
        }

        private static object GetFrame(Window window)
        {
            var windowImpl = GetWindowImpl(window);
            return GetFrameFromWindowImpl(windowImpl);
        }

        private static object GetWindowImpl(Window target)
        {
            var type = target.GetType();
            Asserts.That(type.Name.Equals("WindowBase"), "Only work for WindowBase");
            var windowField = type.GetField("_impl", BindingFlags.NonPublic | BindingFlags.Instance);
            Asserts.NotNull(windowField, "WindowBase has this field");
            return windowField.GetValue(target);
        }

        private static object GetFrameFromWindowImpl(object windowImpl)
        {
            var frameProperty = windowImpl.GetType()
                                          .GetProperty("Frame", BindingFlags.Public | BindingFlags.Instance);
            return frameProperty.GetValue(windowImpl, new object[0]);
        }

        private FrameworkElement FrameContent
        {
            get
            {
                var contentProperty = _frame.GetType()
                                            .GetProperty("Content", BindingFlags.NonPublic | BindingFlags.Instance);
                return (FrameworkElement) contentProperty.GetValue(_frame, new object[0]);
            }
        }

        public void WindowChanged()
        {
            FrameContent.LayoutUpdated += delegate { RegisterToAllNewButtons(); };
        }

        private void RegisterToAllNewButtons()
        {
            FrameContent.FindChildren<Button>().Where(WasNotSeenBefore).ForEach(
                button =>
                {
                    button.Click += delegate
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
                            _logger.Error(e);
                        }
                    };
                });
        }

        private static readonly ISet<WeakReference<Button>> ButtonRegistry = new HashSet<WeakReference<Button>>();

        private static bool WasNotSeenBefore(Button view)
        {
            return ButtonRegistry.Add(new WeakReference<Button>(view));
        }

        private void RegisterToFrameUpdates()
        {
            ((INotifyPropertyChanged) _frame).PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName.Equals("FrameView"))
                {
                    WindowChanged();
                }
            };
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