using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.ReSharper.Feature.Services.CSharp.CodeCompletion;
using JetBrains.ReSharper.Feature.Services.Lookup;
using KaVE.JetBrains.Annotations;
using KaVE.Model.Events.CompletionEvent;
using KaVE.Model.Names;
using KaVE.Model.Names.CSharp;
using KaVE.Utils.Assertion;
using KaVE.VsFeedbackGenerator.Utils.Names;

namespace KaVE.VsFeedbackGenerator.Utils
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
            return TryGetNameFromDeclaredElementLookupItem(lookupItem) ??
                   TryGetNameFromWrappedLookupItem(lookupItem) ??
                   TryGetNameFromKeywordOrTextualLookupItem(lookupItem) ??
                   Asserts.Fail<IName>("unknown kind of lookup item: {0}", lookupItem.GetType());
        }

        private static IName TryGetNameFromDeclaredElementLookupItem(ILookupItem lookupItem)
        {
            var declaredElementLookupItem = lookupItem as IDeclaredElementLookupItem;
            if (declaredElementLookupItem == null || declaredElementLookupItem.PreferredDeclaredElement == null)
            {
                return null;
            }
            // Only the lookup-item type tells whether this is a proposal for a constructor call or not.
            // In fact, ConstructorLookupItem is derived from TypeLookupItem and the additional internface
            // IConstructorLookupItem doesnot provide anything new. Hence, the special treatment.
            var constructorLookupItem = declaredElementLookupItem as ConstructorLookupItem;
            return constructorLookupItem != null
                ? constructorLookupItem.GetName()
                : declaredElementLookupItem.PreferredDeclaredElement.GetName();
        }

        private static IName TryGetNameFromWrappedLookupItem(ILookupItem lookupItem)
        {
            var wrappedLookupItem = lookupItem as IWrappedLookupItem;
            // TODO find example of wrapped case and decide whether or not to include wrapping information
            // there are no implementations of this interface found by hierarchy inspection...
            return wrappedLookupItem != null ? wrappedLookupItem.Item.GetName() : null;
        }

        private static IName TryGetNameFromKeywordOrTextualLookupItem(ILookupItem lookupItem)
        {
            // TODO implement specitic name subclasses?
            return (lookupItem is IKeywordLookupItem || lookupItem is ITextualLookupItem)
                ? Name.Get(lookupItem.GetType().FullName + " " + lookupItem.DisplayName)
                : null;
        }

        private static IMethodName GetName(this ConstructorLookupItem constructor)
        {
            var typeName = constructor.PreferredDeclaredElement.GetName();
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