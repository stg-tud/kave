using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using CodeCompletion.Model;
using CodeCompletion.Utils.Assertion;
using CodeCompletion.Utils.IO;
using EnvDTE;
using EventGenerator.Commons;
using KAVE.EventGenerator_VisualStudio10.Model;
using KAVE.KAVE_MessageBus.MessageBus;
using Microsoft.VisualStudio.CommandBars;

namespace KAVE.EventGenerator_VisualStudio10.Generators
{
    [Export(typeof(VisualStudioEventGenerator))]
    internal class CommandEventGenerator : VisualStudioEventGenerator
    {
        private CommandEvents _commandEvents;
        private IEnumerable<CommandBar> _commandBars;
        private IEnumerable<CommandBarControl> _commandBarsControls;

        private CommandEvent _preceedingCommandBarEvent;
        private Dictionary<string, CommandEvent> _eventQueue;

        public CommandEventGenerator(DTE dte, SMessageBus messageBus) : base(dte, messageBus) {}

        private CommandBars CommandBars
        {
            get { return (CommandBars)DTE.CommandBars; }
        }

        public override void Initialize()
        {
            _eventQueue = new Dictionary<string, CommandEvent>();
            InitCommandBarObservation();
            InitCommandObservation();
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
            SetCommandBarCommandEvent(comboBox);
        }

        private void _commandBarEvents_Button_Click(CommandBarButton button, ref bool cancelDefault)
        {
            SetCommandBarCommandEvent(button);
        }

        private void SetCommandBarCommandEvent(CommandBarControl control)
        {
            _preceedingCommandBarEvent = Create<CommandEvent>();
            _preceedingCommandBarEvent.Source = VsComponentNameFactory.GetNameOf(control);
            _preceedingCommandBarEvent.TriggeredBy = IDEEvent.Trigger.Click;
        }

        void _commandEvents_BeforeExecute(string guid, int id, object customIn, object customOut, ref bool cancelDefault)
        {
            var command = GetCommand(guid, id);
            var commandEvent = _preceedingCommandBarEvent ?? Create<CommandEvent>();

            if (_preceedingCommandBarEvent == null && command.HasPressedKeyBinding())
            {
                commandEvent.TriggeredBy = IDEEvent.Trigger.Shortcut;
            }

            commandEvent.Command = VsComponentNameFactory.GetNameOf(command);
            EnqueueEvent(commandEvent);

            _preceedingCommandBarEvent = null;
        }

        private Command GetCommand(string guid, int id)
        {
            try
            {
                return DTE.Commands.Item(guid, id);
            }
            catch (ArgumentException)
            {
                return new UnknownCommand { DTE = DTE, Guid = guid, ID = id };
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
        }

        private CommandEvent TakeFromQueue(string commandKey)
        {
            CommandEvent evt;
            _eventQueue.TryGetValue(commandKey, out evt);
            _eventQueue.Remove(commandKey);
            return evt;
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
            var bindings = ((object[])command.Bindings).Cast<string>();
            return KeyUtils.ParseBindings(bindings).Any(b => b.IsPressed());
        }
    }

    /// <summary>
    /// Internal dummy used to represent commands that are not known to the DTE.
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