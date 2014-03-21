using System;
using JetBrains.Application;
using JetBrains.ReSharper.Feature.Services.CSharp.CodeCompletion.Infrastructure;
using JetBrains.ReSharper.Feature.Services.Lookup;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using KaVE.Model.Events.CompletionEvent;
using KaVE.VsFeedbackGenerator.Analysis;

namespace KaVE.VsFeedbackGenerator.RS8Tests.Analysis
{
    [ShellComponent, Language(typeof (CSharpLanguage))]
    public class TestAnalysisTrigger : CSharpItemsProviderBase<CSharpCodeCompletionContext>
    {
        public Context LastContext { get; private set; }
        public Exception LastException { get; private set; }

        public bool HasFailed
        {
            get { return LastException != null; }
        }

        protected override bool AddLookupItems(CSharpCodeCompletionContext context, GroupedItemsCollector collector)
        {
            try
            {
                LastException = null;
                LastContext = ContextAnalysis.Analyze(context);
            }
            catch (Exception e)
            {
                LastException = e;
                LastContext = null;
            }
            return false;
        }
    }
}