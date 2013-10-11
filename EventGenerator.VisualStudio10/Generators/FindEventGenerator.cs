using System.ComponentModel.Composition;
using EnvDTE;
using KAVE.EventGenerator_VisualStudio10.Model;

namespace KAVE.EventGenerator_VisualStudio10.Generators
{
    [Export(typeof(VisualStudioEventGenerator))]
    internal class FindEventGenerator : VisualStudioEventGenerator
    {
        private FindEvents _findEvents;

        public FindEventGenerator(DTE dte) : base(dte) {}

        public override void Initialize()
        {
            _findEvents = DTEEvents.FindEvents;

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