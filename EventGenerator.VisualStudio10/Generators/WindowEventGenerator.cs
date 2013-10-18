using System.ComponentModel.Composition;
using EnvDTE;
using EventGenerator.Commons;
using KAVE.KAVE_MessageBus.MessageBus;
using KaVE.Model.Events.VisualStudio;

namespace KAVE.EventGenerator_VisualStudio10.Generators
{
    [Export(typeof(VisualStudioEventGenerator))]
    internal class WindowEventGenerator : VisualStudioEventGenerator
    {
        private WindowEvents _windowEvents;

        public WindowEventGenerator(DTE dte, SMessageBus messageBus) : base(dte, messageBus) {}

        public override void Initialize()
        {
            _windowEvents = DTEEvents.WindowEvents;
            _windowEvents.WindowCreated += _windowEvents_WindowCreated;
            _windowEvents.WindowActivated += _windowEvents_WindowActivated;
            _windowEvents.WindowMoved += _windowEvents_WindowMoved;
            _windowEvents.WindowClosing += _windowEvents_WindowClosing;
        }

        void _windowEvents_WindowClosing(Window window)
        {
            Fire(window, WindowEvent.WindowAction.Close);
        }

        void _windowEvents_WindowMoved(Window window, int top, int left, int width, int height)
        {
            Fire(window, WindowEvent.WindowAction.Move);
        }

        void _windowEvents_WindowActivated(Window gotFocus, Window lostFocus)
        {
            Fire(gotFocus, WindowEvent.WindowAction.Activate);
        }

        void _windowEvents_WindowCreated(Window window)
        {
            Fire(window, WindowEvent.WindowAction.Create);
        }

        private void Fire(Window window, WindowEvent.WindowAction action)
        {
            var windowEvent = Create<WindowEvent>();
            windowEvent.Window = VsComponentNameFactory.GetNameOf(window);
            windowEvent.Action = action;
            Fire(windowEvent);
        }
    }
}