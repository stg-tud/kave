using EnvDTE;
using JetBrains.Application;
using JetBrains.Application.Components;
using KaVE.Model.Events.VisualStudio;
using KaVE.VsFeedbackGenerator.MessageBus;
using KaVE.VsFeedbackGenerator.Utils;

namespace KaVE.VsFeedbackGenerator.Generators.VisualStudio
{
    [ShellComponent(ProgramConfigurations.VS_ADDIN)]
    internal class WindowEventGenerator : AbstractEventGenerator
    {
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly WindowEvents _windowEvents;

        public WindowEventGenerator(DTE dte, IMessageBus messageBus) : base(dte, messageBus)
        {
            _windowEvents = DTE.Events.WindowEvents;
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