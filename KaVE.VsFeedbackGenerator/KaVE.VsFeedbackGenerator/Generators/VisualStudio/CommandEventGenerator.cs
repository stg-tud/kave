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
using System.Collections.ObjectModel;
using System.Linq;
using EnvDTE;
using JetBrains.Application;
using JetBrains.Application.Components;
using KaVE.Model.Events;
using KaVE.Model.Events.VisualStudio;
using KaVE.Utils.Assertion;
using KaVE.Utils.IO;
using KaVE.VsFeedbackGenerator.MessageBus;
using KaVE.VsFeedbackGenerator.Utils;
using KaVE.VsFeedbackGenerator.Utils.Names;
using KaVE.VsFeedbackGenerator.VsIntegration;
using Microsoft.VisualStudio.CommandBars;
using CommandEvent = KaVE.Model.Events.VisualStudio.CommandEvent;

namespace KaVE.VsFeedbackGenerator.Generators.VisualStudio
{
    [ShellComponent(ProgramConfigurations.VS_ADDIN)]
    internal class CommandEventGenerator : EventGeneratorBase
    {
        private static readonly ICollection<string> EventsDuplicatedByReSharper = new Collection<string>
        {
            "{1496A755-94DE-11D0-8C3F-00C04FC2AAE2}:2:Edit.DeleteBackwards",
            "{1496A755-94DE-11D0-8C3F-00C04FC2AAE2}:3:Edit.BreakLine",
            "{1496A755-94DE-11D0-8C3F-00C04FC2AAE2}:4:Edit.InsertTab",
            "{1496A755-94DE-11D0-8C3F-00C04FC2AAE2}:7:Edit.CharLeft",
            "{1496A755-94DE-11D0-8C3F-00C04FC2AAE2}:8:Edit.CharLeftExtend",
            "{1496A755-94DE-11D0-8C3F-00C04FC2AAE2}:9:Edit.CharRight",
            "{1496A755-94DE-11D0-8C3F-00C04FC2AAE2}:10:Edit.CharRightExtend",
            "{1496A755-94DE-11D0-8C3F-00C04FC2AAE2}:11:Edit.LineUp",
            "{1496A755-94DE-11D0-8C3F-00C04FC2AAE2}:12:Edit.LineUpExtend",
            "{1496A755-94DE-11D0-8C3F-00C04FC2AAE2}:13:Edit.LineDown",
            "{1496A755-94DE-11D0-8C3F-00C04FC2AAE2}:14:Edit.LineDownExtend",
            "{1496A755-94DE-11D0-8C3F-00C04FC2AAE2}:27:Edit.PageUp",
            "{1496A755-94DE-11D0-8C3F-00C04FC2AAE2}:29:Edit.PageDown",
            "{1496A755-94DE-11D0-8C3F-00C04FC2AAE2}:107:Edit.CompleteWord",
            "{5EFC7975-14BC-11CF-9B2B-00AA00573819}:627:Window.CloseAllDocuments"
        };

        private static readonly ICollection<string> EventsFiredAutomatically = new Collection<string>
        {
            "{5EFC7975-14BC-11CF-9B2B-00AA00573819}:1096:View.ObjectBrowsingScope",
            "{1496A755-94DE-11D0-8C3F-00C04FC2AAE2}:1627:",
            "{5EFC7975-14BC-11CF-9B2B-00AA00573819}:337:Edit.GoToFindCombo",
            "{5EFC7975-14BC-11CF-9B2B-00AA00573819}:684:Build.SolutionConfigurations",
            "{1496A755-94DE-11D0-8C3F-00C04FC2AAE2}:1990:Build.SolutionPlatforms",
            "{5EFC7975-14BC-11CF-9B2B-00AA00573819}:1657:",
            "{5EFC7975-14BC-11CF-9B2B-00AA00573819}:1717:",
            "{CB26E292-901A-419C-B79D-49BD45C43929}:120:",
            "{FFE1131C-8EA1-4D05-9728-34AD4611BDA9}:4820:",
            "{FFE1131C-8EA1-4D05-9728-34AD4611BDA9}:6155:",
            "{FFE1131C-8EA1-4D05-9728-34AD4611BDA9}:4800:"
        };

        private CommandEvents _commandEvents;
        private IEnumerable<CommandBar> _commandBars;
        private IEnumerable<CommandBarControl> _commandBarsControls;

        private CommandEvent _preceedingCommandBarEvent;
        private readonly Dictionary<string, CommandEvent> _eventQueue;

        public CommandEventGenerator(IIDESession session, IMessageBus messageBus, IDateUtils dateUtils)
            : base(session, messageBus, dateUtils)
        {
            _eventQueue = new Dictionary<string, CommandEvent>();
            InitCommandBarObservation();
            InitCommandObservation();
        }

        private CommandBars CommandBars
        {
            get { return (CommandBars) DTE.CommandBars; }
        }

        private void InitCommandBarObservation()
        {
            _commandBars = CommandBars.Cast<CommandBar>().ToList();
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

        private void InitCommandObservation()
        {
            _commandEvents = DTE.Events.CommandEvents;
            _commandEvents.BeforeExecute += HandleCommandStarts;
            _commandEvents.AfterExecute += HangleCommandEnded;
        }

        private void _commandBarEvents_Dropdown_Change(CommandBarComboBox comboBox)
        {
            SetCommandBarCommandEvent(comboBox);
        }

        private void _commandBarEvents_Button_Click(CommandBarButton button, ref bool cancelDefault)
        {
            SetCommandBarCommandEvent(button);
        }

        private void SetCommandBarCommandEvent(CommandBarControl control)
        {
            _preceedingCommandBarEvent = Create<CommandEvent>();
            _preceedingCommandBarEvent.Source = control.GetName();
            _preceedingCommandBarEvent.TriggeredBy = IDEEvent.Trigger.Click;
        }

        private void HandleCommandStarts(string guid,
            int id,
            object customIn,
            object customOut,
            ref bool cancelDefault)
        {
            var commandEvent = CreateCommandEvent(guid, id);
            EnqueueEvent(commandEvent);

            _preceedingCommandBarEvent = null;
        }

        private CommandEvent CreateCommandEvent(string guid, int id)
        {
            var command = GetCommand(guid, id);
            var commandEvent = _preceedingCommandBarEvent ?? Create<CommandEvent>();

            if (_preceedingCommandBarEvent == null && command.HasPressedKeyBinding())
            {
                commandEvent.TriggeredBy = IDEEvent.Trigger.Shortcut;
            }

            commandEvent.Command = command.GetName();
            return commandEvent;
        }

        private Command GetCommand(string guid, int id)
        {
            try
            {
                return DTE.Commands.Item(guid, id);
            }
            catch (ArgumentException)
            {
                return new UnknownCommand {DTE = DTE, Guid = guid, ID = id};
            }
        }

        private void EnqueueEvent(CommandEvent evt)
        {
            var commandKey = CommandKey(evt.Command.Guid, evt.Command.Id);
            Asserts.Not(_eventQueue.ContainsKey(commandKey), "executing same event twice at a time: {0}", evt);
            _eventQueue.Add(commandKey, evt);
        }

        private static string CommandKey(string guid, int id)
        {
            return guid + ":" + id;
        }

        private void HangleCommandEnded(string guid, int id, object customIn, object customOut)
        {
            var commandEvent = TakeFromQueue(CommandKey(guid, id));
            if (commandEvent == null && id == 107 && guid.Equals("{1496A755-94DE-11D0-8C3F-00C04FC2AAE2}"))
            {
                // for some reason code-completion command is not started...
                commandEvent = CreateCommandEvent(guid, id);
            }
            Asserts.NotNull(commandEvent, "command finished that didn't start: {0}", GetCommand(guid, id).GetName());
            if (IsSuperfluousCommand(commandEvent))
            {
                return;
            }
            FireNow(commandEvent);
        }

        private CommandEvent TakeFromQueue(string commandKey)
        {
            CommandEvent evt;
            _eventQueue.TryGetValue(commandKey, out evt);
            _eventQueue.Remove(commandKey);
            return evt;
        }

        private static bool IsSuperfluousCommand(CommandEvent @event)
        {
            return IsDuplicatedByReSharper(@event) || IsAutomaticEvent(@event);
        }

        /// <summary>
        ///     There exist ReSharper Actions for these commands, we don't need to track them twice.
        /// </summary>
        private static bool IsDuplicatedByReSharper(CommandEvent @event)
        {
            return EventsDuplicatedByReSharper.Contains(@event.Command.Identifier) ||
                   IsReSharperActionEquivalent(@event);
        }

        private static bool IsReSharperActionEquivalent(CommandEvent @event)
        {
            return @event.Command.Name.Contains("ReSharper_");
        }

        /// <summary>
        ///     These commands are automatically triggered after every keyboard input. Furthermore, some of them are
        ///     triggered at regular intervals.
        /// </summary>
        private static bool IsAutomaticEvent(CommandEvent @event)
        {
            return EventsFiredAutomatically.Contains(@event.Command.Identifier);
        }
    }

    internal static class CommandHelper
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
                    leafs = new HashSet<CommandBarControl>(leafs.Union(GetLeafControls(popup.Controls)));
                }
                else
                {
                    leafs.Add(commandBarControl);
                }
            }
            return leafs;
        }

        internal static bool HasPressedKeyBinding(this Command command)
        {
            var bindings = ((object[]) command.Bindings).Cast<string>();
            return KeyUtils.ParseBindings(bindings).Any(b => b.IsPressed());
        }
    }

    /// <summary>
    ///     Internal dummy used to represent commands that are not known to the DTE.
    /// </summary>
    internal class UnknownCommand : Command
    {
        public object AddControl(object owner, int position = 1)
        {
            throw new NotImplementedException();
        }

        public void Delete()
        {
            throw new NotImplementedException();
        }

        public string Name
        {
            get { return ""; }
        }

        public Commands Collection
        {
            get { return null; }
        }

        public DTE DTE { get; internal set; }
        public string Guid { get; internal set; }
        public int ID { get; internal set; }

        public bool IsAvailable
        {
            get { return true; }
        }

        public object Bindings
        {
            get { return new string[0]; }
            set { }
        }

        public string LocalizedName
        {
            get { return ""; }
        }
    }
}