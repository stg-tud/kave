using System.ComponentModel.Composition;
using EnvDTE;
using KAVE.EventGenerator_VisualStudio10.Model;

namespace KAVE.EventGenerator_VisualStudio10.Generators
{
    [Export(typeof(VisualStudioEventGenerator))]
    internal class OutputWindowEventGenerator : VisualStudioEventGenerator
    {
        private OutputWindowEvents _outputWindowEvents;

        protected override void Initialize()
        {
            _outputWindowEvents = DTEEvents.OutputWindowEvents;
            _outputWindowEvents.PaneAdded += OutputWindowEvents_PaneAdded;
            _outputWindowEvents.PaneUpdated += _outputWindowEvents_PaneUpdated;
            _outputWindowEvents.PaneClearing += _outputWindowEvents_PaneClearing;
        }

        void OutputWindowEvents_PaneAdded(OutputWindowPane pPane)
        {
            Fire(OutputWindowEvent.OutputWindowAction.AddPane, pPane);
        }

        void _outputWindowEvents_PaneUpdated(OutputWindowPane pPane)
        {
            Fire(OutputWindowEvent.OutputWindowAction.UpdatePane, pPane);
        }

        void _outputWindowEvents_PaneClearing(OutputWindowPane pPane)
        {
            Fire(OutputWindowEvent.OutputWindowAction.ClearPane, pPane);
        }

        private void Fire(OutputWindowEvent.OutputWindowAction action, OutputWindowPane pPane)
        {
            var outputWindowEvent = Create<OutputWindowEvent>();
            outputWindowEvent.Action = action;
            outputWindowEvent.PaneName = pPane.Name;
            Fire(outputWindowEvent);
        }
    }
}