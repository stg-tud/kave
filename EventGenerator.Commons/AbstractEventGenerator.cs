using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Input;
using CodeCompletion.Model;
using CompletionEventSerializer;
using EnvDTE;
using JetBrains.Annotations;

namespace EventGenerator.Commons
{
    public abstract class AbstractEventGenerator
    {
        //[Import] private IMessageChannel _messageChannel;

        [NotNull]
        protected abstract DTE DTE { get; }

        protected TIDEEvent Create<TIDEEvent>() where TIDEEvent : IDEEvent, new()
        {
            return new TIDEEvent
            {
                ActiveWindow = VsComponentNameFactory.GetNameOf(DTEActiveWindow),
                ActiveDocument = VsComponentNameFactory.GetNameOf(DTEActiveDocument),
                // TODO Save only on startup, then diff from window events
                OpenWindows = VsComponentNameFactory.GetNamesOf(DTE.Windows),
                // TODO delete open documents property, as it can be recalculated from the history
                OpenDocuments = VsComponentNameFactory.GetNamesOf(DTE.Documents),
                // TODO remove, since we always get Solution.Open
                OpenSolution = VsComponentNameFactory.GetNameOf(DTE.Solution),
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
            ideEvent.FinishedAt = DateTime.Now;
            // TODO actually send messages
            // _messageChannel.Publish(ideEvent);
            string eventSerialization;
            using (var stream = new MemoryStream())
            {
                new JsonSerializer().AppendTo(stream, ideEvent);
                stream.Position = 0;
                eventSerialization = new StreamReader(stream).ReadToEnd();
            }
            Debug.WriteLine(eventSerialization);
        }
    }
}
