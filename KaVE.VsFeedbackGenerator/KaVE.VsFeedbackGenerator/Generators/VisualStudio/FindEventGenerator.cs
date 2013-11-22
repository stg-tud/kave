using EnvDTE;
using JetBrains.Application;
using JetBrains.Application.Components;
using KaVE.EventGenerator.ReSharper8.MessageBus;
using KaVE.Model.Events.VisualStudio;

namespace KaVE.EventGenerator.ReSharper8.Generators.VisualStudio
{
    [ShellComponent(ProgramConfigurations.VS_ADDIN)]
    internal class FindEventGenerator : AbstractEventGenerator
    {
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly FindEvents _findEvents;

        public FindEventGenerator(DTE dte, IMessageBus messageBus) : base(dte, messageBus)
        {
            _findEvents = DTE.Events.FindEvents;

            _findEvents.FindDone += _findEvents_FindDone;
        }

        private FindEvent _lastEvent;

        void _findEvents_FindDone(vsFindResult result, bool cancelled)
        {
            if (_lastEvent == null)
            {
                _lastEvent = Create<FindEvent>();
                _lastEvent.Cancelled = cancelled;
            }
            Fire(_lastEvent);
            _lastEvent = null;
        }
    }
}