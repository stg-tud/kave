using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using JetBrains.ReSharper.Feature.Services.Lookup;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.Util;
using KaVE.Model.Events;
using KaVE.Model.Events.CompletionEvent;
using KaVE.Utils.Assertion;
using KaVE.Utils.IO;
using KaVE.VsFeedbackGenerator.MessageBus;
using KaVE.VsFeedbackGenerator.Utils;
using KaVE.VsFeedbackGenerator.VsIntegration;
using Key = System.Windows.Input.Key;

namespace KaVE.VsFeedbackGenerator.Generators.ReSharper
{
    [Language(typeof(CSharpLanguage))]
    public class CodeCompletionEventGenerator
    {
        private readonly ILookupWindowManager _lookupWindowManager;
        private readonly IVsDTE _dte;
        private readonly IMessageBus _messageBus;

        public CodeCompletionEventGenerator(ILookupWindowManager lookupWindowManager, IVsDTE dte, IMessageBus messageBus)
        {
            _lookupWindowManager = lookupWindowManager;
            _dte = dte;
            _messageBus = messageBus;
            _lookupWindowManager.BeforeLookupWindowShown += OnBeforeLookupShown;
            // Notes:
            // - AfterLookupWindowShown is fired immediately after the window pops up
            // - LookupWindowClosed and CuurentLookup.Closed are fired before CurrentLookup.ItemCompleted
        }

        private void OnBeforeLookupShown(Object sender, EventArgs e)
        {
            var handler = new CodeCompletionEventHandler(_lookupWindowManager.CurrentLookup, _dte, _messageBus);
        }
    }
}
