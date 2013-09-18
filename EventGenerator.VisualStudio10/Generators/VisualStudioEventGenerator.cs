using System;
using System.ComponentModel.Composition;
using CodeCompletion.Model;
using EnvDTE;
using EventGenerator.Commons;
using Microsoft.VisualStudio.Shell;

namespace KAVE.EventGenerator_VisualStudio10.Generators
{
    internal abstract class VisualStudioEventGenerator : AbstractEventGenerator, IPartImportsSatisfiedNotification
    {
        [Import] private SVsServiceProvider _serviceProvider;
        //[Import] private IMessageChannel _messageChannel;

        protected override DTE DTE
        {
            get { return (DTE) _serviceProvider.GetService(typeof (DTE)); }
        }

        protected Events DTEEvents
        {
            get { return DTE.Events; }
        }

        /// <summary>
        /// Sets <see cref="IDEEvent.FinishedAt"/> to the current time and publishes the event.
        /// </summary>
        protected void Fire(IDEEvent ideEvent)
        {
            ideEvent.FinishedAt = DateTime.Now;
            // TODO actually send messages
            // _messageChannel.Publish(ideEvent);
        }

        public void OnImportsSatisfied()
        {
            Initialize();
        }

        protected abstract void Initialize();
    }
}