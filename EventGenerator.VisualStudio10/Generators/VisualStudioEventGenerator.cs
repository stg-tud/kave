using System;
using System.ComponentModel.Composition;
using CodeCompletion.Model;
using EnvDTE;
using EventGenerator.Commons;
using JetBrains.Annotations;
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

        [NotNull]
        protected Events DTEEvents
        {
            get { return DTE.Events; }
        }

        public void OnImportsSatisfied()
        {
            Initialize();
        }

        protected abstract void Initialize();
    }
}