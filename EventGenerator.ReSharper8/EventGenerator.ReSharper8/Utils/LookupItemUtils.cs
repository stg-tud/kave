using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.ReSharper.Feature.Services.Lookup;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.DeclaredElements;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using KaVE.JetBrains.Annotations;
using KaVE.Model.Events.CompletionEvent;
using KaVE.Model.Names;
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
                var enclosingElement = declaredElementLookupItem.PreferredDeclaredElement.Element;

                var namespaceProposal = enclosingElement as INamespace;
                if (namespaceProposal != null) // namespace proposal
                {
                    return null;//CSharpNameFactory.GetName(namespaceProposal);
                }
                var typeElement = enclosingElement as ITypeElement;
                if (typeElement != null) // type proposal
                {
                    return null;//CSharpNameFactory.GetName(typeElement);
                }
                var constructor = enclosingElement as IConstructor;
                if (constructor != null) // instantiation proposal
                {
                    return null;//CSharpNameFactory.GetName(constructor);
                }
                var method = enclosingElement as IMethod;
                if (method != null)
                {
                    return null;// CSharpNameFactory.GetName(method);
                }
                var parameter = enclosingElement as IParameter;
                if (parameter != null)
                {
                    return null;// CSharpNameFactory.GetName(parameter);
                }
                var field = enclosingElement as IField;
                if (field != null)
                {
                    return null;//CSharpNameFactory.GetName(field);
                }
                var variableDeclaration = enclosingElement as IVariableDeclaration;
                if (variableDeclaration != null)
                {
                    return null;
                }
                var alias = enclosingElement as IAlias;
                if (alias != null)
                {
                    // things such as global::System...
                    // custom defined by "using alias = <import>;"
                    return null;
                }

                // TODO identify other cases and add them here

                Asserts.Fail("unknown kind of declared element: {0}", enclosingElement.GetType());
            }

            var keywordLookupItem = lookupItem as IKeywordLookupItem;
            if (keywordLookupItem != null)
            {
                // TODO decide what to do with keywords
                return null;
            }

            Asserts.Fail("unknown kind of lookup item: {0}", lookupItem.GetType());
            return null;
        }
    }
}
