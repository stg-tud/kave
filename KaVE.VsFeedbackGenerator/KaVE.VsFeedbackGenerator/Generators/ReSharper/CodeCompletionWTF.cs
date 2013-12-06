using System.Collections.Generic;
using JetBrains.ReSharper.Feature.Services.CSharp.CodeCompletion.Infrastructure;
using JetBrains.ReSharper.Feature.Services.Lookup;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using KaVE.VsFeedbackGenerator.Utils;

namespace KaVE.VsFeedbackGenerator.Generators.ReSharper
{
    [Language(typeof (CSharpLanguage))]
    internal class CodeCompletionWTF : CSharpItemsProviderBase<CSharpCodeCompletionContext>, ILookupItemsPreference
    {
        private int filtered = 0;

        public override bool IsDynamic
        {
            get { return true; }
        }

        protected override bool IsAvailable(CSharpCodeCompletionContext context)
        {
            return true;
        }

        protected override bool AddLookupItems(CSharpCodeCompletionContext context, GroupedItemsCollector collector)
        {
            collector.AddFilter(this);
            return false;
        }

        public IEnumerable<ILookupItem> FilterItems(ICollection<ILookupItem> items)
        {
            var proposalCollection = items.ToProposalCollection();
            filtered += 1;
            return items;
        }

        public int Order
        {
            get { return int.MaxValue; }
        }
    }
}