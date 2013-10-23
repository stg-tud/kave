using System;
using System.Linq;
using JetBrains.ReSharper.Feature.Services.CSharp.CodeCompletion.Infrastructure;
using JetBrains.ReSharper.Feature.Services.Lookup;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.Impl.reflection2.elements.Context;

namespace KaVE.EventGenerator.ReSharper8.Generators
{
    [Language(typeof(CSharpLanguage))]
    public class CodeCompletionEventGenerator : CSharpItemsProviderBase<CSharpCodeCompletionContext>
    {
        protected override bool IsAvailable(CSharpCodeCompletionContext context)
        {
            var codeCompletionType = context.BasicContext.CodeCompletionType;
            return true;
        }

        protected override bool AddLookupItems(CSharpCodeCompletionContext context, GroupedItemsCollector collector)
        {
            var lookupItems = collector.Items;
            var lookupItem = lookupItems.First();
            GetAssemblyQualifiedNameOfTypeProposal(lookupItem);
            return base.AddLookupItems(context, collector);
        }

        private static string GetAssemblyQualifiedNameOfTypeProposal(ILookupItem lookupItem)
        {
            var declaredElementLookupItem = lookupItem as DeclaredElementLookupItem;
            if (declaredElementLookupItem != null && declaredElementLookupItem.PreferredDeclaredElement != null)
            {
                var enclosingElement = declaredElementLookupItem.PreferredDeclaredElement.Element;
                var classElement = enclosingElement as ITypeElement;
                if (classElement != null)
                {
                    var fullName = classElement.GetClrName().FullName;
                    // this is not working yet..
                    var type = Type.GetType(fullName);
                    return type.AssemblyQualifiedName;
                }
            }
        }
    }
}
