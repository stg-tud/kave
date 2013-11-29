using System;
using System.Diagnostics;
using System.Windows.Input;
using EnvDTE;
using KaVE.JetBrains.Annotations;
using KaVE.Model.Events;
using KaVE.Utils.Serialization;
using KaVE.VsFeedbackGenerator.MessageBus;
using KaVE.VsFeedbackGenerator.Utils;

namespace KaVE.VsFeedbackGenerator.Generators
{
    public abstract class AbstractEventGenerator
    {
        private readonly IMessageBus _messageBus;

        protected AbstractEventGenerator(DTE dte, IMessageBus messageBus)
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
            get
            {
                try
                {
                    return DTE.ActiveDocument;
                }
                catch (ArgumentException)
                {
                    // accessing active document throws an ArgumentException, for
                    // example, when the ActiveWindow is the properties page of a project
                    return null;
                }
            }
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
        private static void WriteToDebugConsole(IDEEvent ideEvent)
        {
            //Debug.WriteLine(ideEvent.ToJson());
        }
    }
}