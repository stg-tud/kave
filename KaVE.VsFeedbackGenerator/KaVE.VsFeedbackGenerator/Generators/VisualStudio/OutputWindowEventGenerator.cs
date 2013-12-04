using EnvDTE;
using JetBrains.Application;
using JetBrains.Application.Components;
using KaVE.Model.Events.VisualStudio;
using KaVE.VsFeedbackGenerator.MessageBus;
using KaVE.VsFeedbackGenerator.VsIntegration;

namespace KaVE.VsFeedbackGenerator.Generators.VisualStudio
{
    [ShellComponent(ProgramConfigurations.VS_ADDIN)]
    internal class OutputWindowEventGenerator : AbstractEventGenerator
    {
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly OutputWindowEvents _outputWindowEvents;

        public OutputWindowEventGenerator(IIDESession session, IMessageBus messageBus)
            : base(session, messageBus)
        {
            _outputWindowEvents = DTE.Events.OutputWindowEvents;
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
            FireNow(outputWindowEvent);
        }
    }
}