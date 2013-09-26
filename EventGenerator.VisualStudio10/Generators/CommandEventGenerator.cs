using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using CodeCompletion.Model;
using CodeCompletion.Model.Names.VisualStudio;
using CodeCompletion.Utils.Assertion;
using EnvDTE;
using EventGenerator.Commons;
using KAVE.EventGenerator_VisualStudio10.Model;
using Microsoft.VisualStudio.CommandBars;

namespace KAVE.EventGenerator_VisualStudio10.Generators
{
    [Export(typeof (VisualStudioEventGenerator))]
    internal class CommandEventGenerator : VisualStudioEventGenerator
    {
        private CommandEvents _commandEvents;
        private IEnumerable<CommandBar> _commandBars;
        private IEnumerable<CommandBarControl> _commandBarsControls;

        private CommandEvent _currentEvent;
        private Dictionary<string, CommandEvent> _eventQueue;

        private CommandBars CommandBars
        {
            get { return (CommandBars)DTE.CommandBars; }
        }

        private IEnumerable<CommandBarControl> CommandBarsLeafControls()
        {
                return _commandBars.SelectMany(bar => CommandBarsLeafControls(bar.Controls));
        }

        private IEnumerable<CommandBarControl> CommandBarsLeafControls(CommandBarControls controls)
        {
            ISet<CommandBarControl> leafs = new HashSet<CommandBarControl>();
            foreach (CommandBarControl commandBarControl in controls)
            {
                var popup = commandBarControl as CommandBarPopup;
                if (popup != null)
                {
                    leafs = new HashSet<CommandBarControl>(leafs.Union(CommandBarsLeafControls(popup.Controls)));
                }
                else
                {
                    leafs.Add(commandBarControl);
                }
            }
            return leafs;
        }

        protected override void Initialize()
        {
            _eventQueue = new Dictionary<string, CommandEvent>();
            InitCommandBarObservation();
            InitCommandObservation();
        }

        private void InitCommandBarObservation()
        {
            _commandBars = CommandBars.Cast<CommandBar>().ToList();
            _commandBarsControls = CommandBarsLeafControls().ToList();
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

                Asserts.Not(button == null && box == null, "unknown type of control: " + control.GetType());
            }
        }

        private void InitCommandObservation()
        {
            _commandEvents = DTEEvents.CommandEvents;
            _commandEvents.BeforeExecute += _commandEvents_BeforeExecute;
            _commandEvents.AfterExecute += _commandEvents_AfterExecute;
        }

        private void _commandBarEvents_Dropdown_Change(CommandBarComboBox comboBox)
        {
            SetCurrentEventToCommandBarMouseInteractionEvent(comboBox);
        }

        private void _commandBarEvents_Button_Click(CommandBarButton button, ref bool cancelDefault)
        {
            SetCurrentEventToCommandBarMouseInteractionEvent(button);
        }

        private void SetCurrentEventToCommandBarMouseInteractionEvent(CommandBarControl control)
        {
            _currentEvent = Create<CommandEvent>();
            _currentEvent.Source = VsComponentNameFactory.GetNameOf(control);
            _currentEvent.TriggeredBy = IDEEvent.Trigger.Click;
        }

        void _commandEvents_BeforeExecute(string guid, int id, object customIn, object customOut, ref bool cancelDefault)
        {
            if (_currentEvent == null)
            {
                _currentEvent = Create<CommandEvent>();
                // TODO this is not finegrained enough, I think, as it will map shortcuts to automatic as well
                // maybe we can capture keyboard state to see wheather a modifier key is pressed?
                _currentEvent.TriggeredBy = IDEEvent.Trigger.Automatic;
            }
            
            _currentEvent.Command = GetCommandName(guid, id);
            EnqueueEvent(_currentEvent);
            _currentEvent = null;
        }

        private CommandName GetCommandName(string guid, int id)
        {
            try
            {
                var command = DTE.Commands.Item(guid, id);
                return VsComponentNameFactory.GetNameOf(command);
            }
            catch (ArgumentException)
            {
                return VsComponentNameFactory.GetNameOfCommand(guid, id);
            }
        }

        private void EnqueueEvent(CommandEvent evt)
        {
            var commandKey = CommandKey(evt.Command.Guid, evt.Command.Id);
            Asserts.Not(_eventQueue.ContainsKey(commandKey), "executing same event twice at a time: " + evt);
            _eventQueue.Add(commandKey, evt);
        }

        private static string CommandKey(string guid, int id)
        {
            return guid + ":" + id;
        }

        void _commandEvents_AfterExecute(string guid, int id, object customIn, object customOut)
        {
            var commandEvent = TakeFromQueue(CommandKey(guid, id));
            Asserts.NotNull(commandEvent, "command finished that didn't start: " + guid + ":" + id);
            Fire(commandEvent);
            Debug.WriteLine("FIRED " + commandEvent.Command + " TRIGGERED BY " + commandEvent.TriggeredBy + " (" + commandEvent.Source + ") #####################################");
        }

        private CommandEvent TakeFromQueue(string commandKey)
        {
            CommandEvent evt;
            _eventQueue.TryGetValue(commandKey, out evt);
            _eventQueue.Remove(commandKey);
            return evt;
        }
    }
}