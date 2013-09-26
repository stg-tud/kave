using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using CodeCompletion.Model;
using CodeCompletion.Utils.Assertion;
using EnvDTE;
using EventGenerator.Commons;
using KAVE.EventGenerator_VisualStudio10.Model;
using Microsoft.VisualStudio.CommandBars;

namespace KAVE.EventGenerator_VisualStudio10.Generators
{
    [Export(typeof(VisualStudioEventGenerator))]
    internal class CommandEventGenerator : VisualStudioEventGenerator
    {
        private CommandEvents _commandEvents;
        private IEnumerable<CommandBar> _commandBars;
        private IEnumerable<CommandBarControl> _commandBarsControls;

        private CommandEvent _commandBarCommandEvent;
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
            SetCommandBarCommandEvent(comboBox);
        }

        private void _commandBarEvents_Button_Click(CommandBarButton button, ref bool cancelDefault)
        {
            SetCommandBarCommandEvent(button);
        }

        private void SetCommandBarCommandEvent(CommandBarControl control)
        {
            _commandBarCommandEvent = Create<CommandEvent>();
            _commandBarCommandEvent.Source = VsComponentNameFactory.GetNameOf(control);
            _commandBarCommandEvent.TriggeredBy = IDEEvent.Trigger.Click;
        }

        void _commandEvents_BeforeExecute(string guid, int id, object customIn, object customOut, ref bool cancelDefault)
        {
            var command = GetCommand(guid, id);
            var commandEvent = _commandBarCommandEvent ?? Create<CommandEvent>();

            // if event was not triggered by selecting a menu control
            if (_commandBarCommandEvent == null)
            {
                if (IsTriggeredByBinding(command))
                {
                    commandEvent.TriggeredBy = IDEEvent.Trigger.Shortcut;
                }
            }

            commandEvent.Command = VsComponentNameFactory.GetNameOf(command);
            EnqueueEvent(commandEvent);

            _commandBarCommandEvent = null;
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

        private static bool IsTriggeredByBinding(Command command)
        {
            return Binding.CreateFrom(((object[]) command.Bindings).Cast<string>()).Any(b => b.IsPressed());
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

    internal class Binding
    {
        private readonly string _scope;
        private readonly Key[][] _keyCombinations;

        public static Binding CreateFrom(string bindingIdentifier)
        {
            var endOfScope = bindingIdentifier.IndexOf(':');
            var scope = bindingIdentifier.Substring(0, endOfScope);
            var keyConverter = new KeyConverter();
            var keyBinding = bindingIdentifier.Substring(endOfScope + 2);
            var keyBindingParts = keyBinding.Split(',').Select(s => s.Trim());
            var keyCombinations = keyBindingParts.Select(s => s.Split('+').Select(keyConverter.ConvertFromString).Cast<Key>().ToArray()).ToArray();
            Asserts.True(keyCombinations.Any(), "binding contains no key combination: " + bindingIdentifier);
            Asserts.True(keyCombinations.Count() <= 2, "binding has more than two key combinations: " + bindingIdentifier);
            return new Binding(scope, keyCombinations);
        }

        public static IEnumerable<Binding> CreateFrom(IEnumerable<string> bindingsIdentifiers)
        {
            return bindingsIdentifiers.Select(CreateFrom);
        }

        private Binding(string scope, Key[][] keyCombinations)
        {
            _scope = scope;
            _keyCombinations = keyCombinations;
        }

        public bool IsPressed()
        {
            return _keyCombinations.Last().All(Keyboard.IsKeyDown);
        }
    }
}