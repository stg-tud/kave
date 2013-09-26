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
                OpenWindows = VsComponentNameFactory.GetNamesOf(DTE.Windows),
                OpenDocuments = VsComponentNameFactory.GetNamesOf(DTE.Documents),
                OpenSolution = VsComponentNameFactory.GetNameOf(DTE.Solution)
            };
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
