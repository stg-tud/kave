using System;
using System.Diagnostics;
using System.Windows.Input;
using CodeCompletion.Model.Events;
using CodeCompletion.Utils.Serialization;
using EnvDTE;
using JetBrains.Annotations;
using KAVE.KAVE_MessageBus.MessageBus;

namespace EventGenerator.Commons
{
    public abstract class AbstractEventGenerator
    {
        private readonly SMessageBus _messageBus;

        protected AbstractEventGenerator(DTE dte, SMessageBus messageBus)
        {
            _messageBus = messageBus;
            DTE = dte;
        }

        [NotNull]
        protected DTE DTE { get; private set; }

        protected TIDEEvent Create<TIDEEvent>() where TIDEEvent : IDEEvent, new()
        {
            return new TIDEEvent
            {
                ActiveWindow = VsComponentNameFactory.GetNameOf(DTEActiveWindow),
                ActiveDocument = VsComponentNameFactory.GetNameOf(DTEActiveDocument),
                TriggeredBy = CurrentTrigger,
            };
        }

        private static IDEEvent.Trigger CurrentTrigger
        {
            get
            {
                var leftMouseButtonPressed = Mouse.LeftButton == MouseButtonState.Pressed;
                return leftMouseButtonPressed ? IDEEvent.Trigger.Click : IDEEvent.Trigger.Unknown;
            }
        }

        private Window DTEActiveWindow
        {
            get
            {
                try
                {
                    return DTE.ActiveWindow;
                }
                catch (NullReferenceException)
                {
                    // accessing the active window throws an NullReferenceException
                    // if no windows have been opened yet
                    return null;
                }
            }
        }

        private Document DTEActiveDocument
        {
            get { return DTE.ActiveDocument; }
        }

        /// <summary>
        /// Sets <see cref="IDEEvent.FinishedAt"/> to the current time and publishes the event.
        /// </summary>
        protected void Fire<TEvent>([NotNull] TEvent ideEvent) where TEvent : IDEEvent
        {
            ideEvent.IDESessionUUID = IDESession.GetUUID(DTE);
            ideEvent.FinishedAt = DateTime.Now;
            _messageBus.Publish<IDEEvent>(ideEvent);
            WriteToDebugConsole(ideEvent);
        }

        [Conditional("DEBUG")]
        private static void WriteToDebugConsole<TEvent>(TEvent ideEvent) where TEvent : IDEEvent
        {
            Debug.WriteLine(ideEvent.ToJson<IDEEvent>());
        }
    }
}