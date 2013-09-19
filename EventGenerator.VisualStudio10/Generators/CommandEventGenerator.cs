using System.ComponentModel.Composition;
using EnvDTE;
using EventGenerator.Commons;
using KAVE.EventGenerator_VisualStudio10.Model;

namespace KAVE.EventGenerator_VisualStudio10.Generators
{
    [Export(typeof (VisualStudioEventGenerator))]
    internal class CommandEventGenerator : VisualStudioEventGenerator
    {
        private CommandEvents _commandEvents;

        protected override void Initialize()
        {
            _commandEvents = DTEEvents.CommandEvents;
            _commandEvents.BeforeExecute += _commandEvents_BeforeExecute;
        }

        void _commandEvents_BeforeExecute(string guid, int id, object customIn, object customOut, ref bool cancelDefault)
        {
            var commandEvent = Create<CommandEvent>();
            var command = DTE.Commands.Item(guid, id);
            commandEvent.Command = VsComponentNameFactory.GetNameOf(command);
            Fire(commandEvent);
        }
    }
}