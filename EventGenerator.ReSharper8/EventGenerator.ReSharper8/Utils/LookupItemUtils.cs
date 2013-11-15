using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.ReSharper.Feature.Services.CodeCompletion;
using JetBrains.ReSharper.Feature.Services.CSharp.CodeCompletion;
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
            return lookupItem == null ? null : new Proposal {Name = lookupItem.GetName()};
        }

        private static IName GetName([NotNull] this ILookupItem lookupItem)
        {
            var declaredElementLookupItem = lookupItem as IDeclaredElementLookupItem;
            if (declaredElementLookupItem != null && declaredElementLookupItem.PreferredDeclaredElement != null)
            {
                var constructorLookupItem = declaredElementLookupItem as ConstructorLookupItem;
                return constructorLookupItem != null
                    ? constructorLookupItem.GetName()
                    : declaredElementLookupItem.PreferredDeclaredElement.Element.GetName();
            }

            var wrappedLookupItem = lookupItem as IWrappedLookupItem;
            if (wrappedLookupItem != null)
            {
                // TODO find example of this case and decide whether or not to include wrapping information
                // there are no implementations of this interface found by hierarchy inspection...
                return wrappedLookupItem.Item.GetName();
            }

            if (lookupItem is IKeywordLookupItem || lookupItem is ITextualLookupItem)
            {
                // TODO distinguish lookup-item types here?
                return Name.Get(lookupItem.DisplayName);
            }

            return Asserts.Fail<IName>("unknown kind of lookup item: {0}", lookupItem.GetType());
        }

        private static IMethodName GetName(this ConstructorLookupItem constructor)
        {
            var typeName = constructor.PreferredDeclaredElement.Element.GetName();
            var identifier = new StringBuilder();
            identifier.Append('[')
                .Append(typeName.Identifier)
                .Append("] [")
                .Append(typeName.Identifier)
                .Append("]..ctor()");
            return MethodName.Get(identifier.ToString());
        }
    }
}