using System.Collections.Generic;
using System.Linq;
using JetBrains.ReSharper.Feature.Services.Lookup;
using KaVE.JetBrains.Annotations;
using KaVE.Model.Events.CompletionEvent;
using KaVE.Model.Names;
using KaVE.Model.Names.CSharp;
using KaVE.Utils.Assertion;

namespace KaVE.EventGenerator.ReSharper8.Utils
{
    public static class LookupItemUtils
    {
        public static ProposalCollection ToProposalCollection([NotNull] this IEnumerable<ILookupItem> items)
        {
            return new ProposalCollection(items.Select(ToProposal).ToList());
        }

        public static Proposal ToProposal([CanBeNull] this ILookupItem lookupItem)
        {
            return lookupItem == null ? null : new Proposal { Name = lookupItem.GetName() };
        }

        private static IName GetName([NotNull] this ILookupItem lookupItem)
        {
            var declaredElementLookupItem = lookupItem as IDeclaredElementLookupItem;
            if (declaredElementLookupItem != null && declaredElementLookupItem.PreferredDeclaredElement != null)
            {
                return declaredElementLookupItem.PreferredDeclaredElement.Element.GetName();
            }

            var keywordLookupItem = lookupItem as IKeywordLookupItem;
            if (keywordLookupItem != null)
            {
                return Name.Get(lookupItem.DisplayName);
            }

            Asserts.Fail("unknown kind of lookup item: {0}", lookupItem.GetType());
            return null;
        }
    }
}
