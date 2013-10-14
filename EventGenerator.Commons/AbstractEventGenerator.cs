using System;
using System.Diagnostics;
using System.Windows.Input;
using CodeCompletion.Model;
using CodeCompletion.Utils;
using CodeCompletion.Utils.Serialization;
using EnvDTE;
using JetBrains.Annotations;
using KAVE.KAVE_MessageBus;
using KAVE.KAVE_MessageBus.MessageBus;

namespace EventGenerator.Commons
{
    public abstract class AbstractEventGenerator
    {
        protected AbstractEventGenerator(DTE dte)
        {
            DTE = dte;
        }

        [Inject]
        public SMessageBus MessageChannel
        {
            get;
            set; }

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
                catch (Exception)
                {
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
                catch (Exception)
                {
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
            // TODO find reason for System.NullReferenceException "somewhere" in Commons
            MessageChannel.Publish(ideEvent);
            WriteToDebugConsole(ideEvent);
        }

        [Conditional("DEBUG")]
        private static void WriteToDebugConsole<TEvent>(TEvent ideEvent) where TEvent : IDEEvent
        {
            Debug.WriteLine(JsonExtension.ToString(ideEvent));
        }
    }
}
