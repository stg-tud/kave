using System;
using CodeCompletion.Model;
using EnvDTE;
using JetBrains.Annotations;

namespace EventGenerator.Commons
{
    public abstract class AbstractEventGenerator
    {
        //[Import] private IMessageChannel _messageChannel;

        [NotNull]
        protected abstract DTE DTE { get; }

        /// <summary>
        /// Sets <see cref="IDEEvent.FinishedAt"/> to the current time and publishes the event.
        /// </summary>
        protected void Fire([NotNull] IDEEvent ideEvent)
        {
            ideEvent.FinishedAt = DateTime.Now;
            // TODO actually send messages
            // _messageChannel.Publish(ideEvent);
        }

        protected TIDEEvent Create<TIDEEvent>() where TIDEEvent : IDEEvent, new()
        {
            return new TIDEEvent
            {
                ActiveWindow = VsComponentNameFactory.GetNameOf(DTE.ActiveWindow),
                ActiveDocument = VsComponentNameFactory.GetNameOf(DTE.ActiveDocument),
                OpenWindows = VsComponentNameFactory.GetNamesOf(DTE.Windows),
                OpenDocuments = VsComponentNameFactory.GetNamesOf(DTE.Documents),
                OpenSolution = VsComponentNameFactory.GetNameOf(DTE.Solution)
            };
        }


    }
}
