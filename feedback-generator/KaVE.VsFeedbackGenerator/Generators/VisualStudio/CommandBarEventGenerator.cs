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

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using EnvDTE;
using JetBrains.Application;
using JetBrains.Application.Components;
using JetBrains.Util;
using KaVE.JetBrains.Annotations;
using KaVE.Model.Events;
using KaVE.Utils.Assertion;
using KaVE.VsFeedbackGenerator.MessageBus;
using KaVE.VsFeedbackGenerator.Utils;
using KaVE.VsFeedbackGenerator.VsIntegration;
using Microsoft.VisualStudio.CommandBars;
using Window = EnvDTE.Window;

namespace KaVE.VsFeedbackGenerator.Generators.VisualStudio
{
    [ShellComponent(ProgramConfigurations.VS_ADDIN)]
    public class CommandBarEventGenerator : EventGeneratorBase
    {
        private IEnumerable<CommandBar> _commandBars;
        private IEnumerable<CommandBarControl> _commandBarsControls;
        private readonly ISet<Window> _registeredWindows = new HashSet<Window>();

        [UsedImplicitly]
        private WindowEvents _windowEvents;

        public CommandBarEventGenerator([NotNull] IIDESession session,
            [NotNull] IMessageBus messageBus,
            [NotNull] IDateUtils dateUtils) : base(session, messageBus, dateUtils)
        {
            AttachToCommandBars((CommandBars) DTE.CommandBars);
            _windowEvents = DTE.Events.WindowEvents;
            _windowEvents.WindowActivated += OnWindowActivated;
        }

        private void OnWindowActivated(Window gotFocus, Window lostfocus)
        {
            if (!_registeredWindows.Contains(gotFocus))
            {
                _registeredWindows.Add(gotFocus);
                var windowType = gotFocus.GetType();
                var propertyInfo = windowType.GetProperty("CommandBars", BindingFlags.Public | BindingFlags.Instance);
                if (propertyInfo != null)
                {
                    AttachToCommandBars((CommandBars) propertyInfo.GetValue(gotFocus, new object[0]));
                }
            }
        }

        private void AttachToCommandBars(CommandBars commandBars)
        {
            _commandBars = commandBars.Cast<CommandBar>().ToList();
            _commandBarsControls = _commandBars.GetLeafControls().ToList();
            foreach (var control in _commandBarsControls)
            {
                var button = control as CommandBarButton;
                if (button != null)
                {
                    button.Click += _commandBarEvents_Button_Click;
                }

                var box = control as CommandBarComboBox;
                if (box != null)
                {
                    box.Change += _commandBarEvents_Dropdown_Change;
                }

                Asserts.Not(button == null && box == null, "unknown type of control: {0}", control.GetType());
            }
        }

        private void _commandBarEvents_Dropdown_Change(CommandBarComboBox comboBox)
        {
            FireCommandBarEvent(comboBox);
        }

        private void _commandBarEvents_Button_Click(CommandBarButton button, ref bool cancelDefault)
        {
            FireCommandBarEvent(button);
        }

        private void FireCommandBarEvent(CommandBarControl command)
        {
            // TODO @Sven: Capture the parents's name, as "Bug" is not helpfull (or unique) whereas "Create Working Item" > "Bug" probably is
            // We can walk up the control's and menu bar's parents until an instance of Sytem._ComObject is reached
            var commandEvent = Create<CommandEvent>();
            commandEvent.TriggeredBy = IDEEvent.Trigger.Click;
            commandEvent.CommandId = command.GetFullQualifiedId();
            Fire(commandEvent);
        }
    }

    internal static class CommandBarHelper
    {
        internal static IEnumerable<CommandBarControl> GetLeafControls(this IEnumerable<CommandBar> commandBars)
        {
            return commandBars.SelectMany(bar => GetLeafControls(bar.Controls));
        }

        private static IEnumerable<CommandBarControl> GetLeafControls(CommandBarControls controls)
        {
            ISet<CommandBarControl> leafs = new HashSet<CommandBarControl>();
            foreach (CommandBarControl commandBarControl in controls)
            {
                var popup = commandBarControl as CommandBarPopup;
                if (popup != null)
                {
                    leafs.AddRange(GetLeafControls(popup.Controls));
                }
                else
                {
                    // TODO @Sven: Find parent of "Bug" button in popup and see why its siblings are not captured!!!
                    leafs.Add(commandBarControl);
                }
            }
            return leafs;
        }

        internal static string GetFullQualifiedId(this CommandBarControl control)
        {
            /*
             * We can compute the whole tree path, by stepping up the parents (ControlBars and Popups).
             * Sometimes there are both a Bar and a Popup with the same name, these should be mergen then:
             * 
             * + MenuBar (Bar)
             *   + Project (Popup)
             *     + Project (Bar)
             *       + ControlX
             *       + ControlY
             *       + ...
             * 
             * Maybe it's easier to build the names when we walk down the tree. We should avoid to create
             * throusands of unecessary strings, though.
             */
            return control.Caption.Replace("&", "");
        }
    }
}