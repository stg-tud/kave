/*
 * Copyright 2014 Technische Universität Darmstadt
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *    http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.CodeCompletion.Infrastructure.AspectLookupItems.BaseInfrastructure;
using JetBrains.ReSharper.Feature.Services.CodeCompletion.Infrastructure.AspectLookupItems.Info;
using JetBrains.ReSharper.Feature.Services.CodeCompletion.Infrastructure.LookupItems;
using JetBrains.ReSharper.Features.Intellisense.CodeCompletion.CSharp.AspectLookupItems;
using KaVE.Commons.Model.Events.CompletionEvents;
using KaVE.Commons.Model.Names;
using KaVE.Commons.Model.Names.CSharp;
using KaVE.RS.Commons.Utils.Names;

namespace KaVE.RS.Commons.Utils
{
    public static class LookupItemUtils
    {
        [NotNull]
        public static ProposalCollection ToProposalCollection([NotNull] this IEnumerable<ILookupItem> items)
        {
            return new ProposalCollection(items.Select(ToProposal).ToList());
        }

        [NotNull]
        public static Proposal ToProposal([CanBeNull] this ILookupItem lookupItem)
        {
            var name = lookupItem == null ? Name.UnknownName : lookupItem.GetName();
            return new Proposal {Name = name};
        }

        private static IName GetName([NotNull] this ILookupItem lookupItem)
        {
            return TryGetNameFromDeclaredElementLookupItem(lookupItem) ??
                   //TryGetNameFromWrappedLookupItem(lookupItem) ??
                   TryGetNameFromKeywordOrTextualLookupItem(lookupItem) ??
                   //TryGetNameFromTemplateLookupItem(lookupItem) ??
                   GetNameFromLookupItemIdentity(lookupItem);
        }

        private static IName TryGetNameFromDeclaredElementLookupItem(ILookupItem lookupItem)
        {
            var deli = lookupItem as LookupItem<DeclaredElementInfo>;
            if (deli == null || deli.Info == null)
            {
                return null;
            }
            var de = deli.GetAllDeclaredElements().FirstOrDefault();
            return de == null ? null : de.GetName();
        }

        private static IName TryGetNameFromKeywordOrTextualLookupItem(ILookupItem lookupItem)
        {
            var tli = lookupItem as LookupItem<TextualInfo>;
            if (tli == null || tli.Info == null)
            {
                return null;
            }

            return Name.Get(GetPossiblyGenericTypeName(tli) + ":" + tli.DisplayName);
        }

        // TODO RS9: I doubt that all of them are obsolete... review that later
        /*
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
         * 

        private static IName TryGetNameFromWrappedLookupItem(ILookupItem lookupItem)
        {
            var wrappedLookupItem = lookupItem as IWrappedLookupItem;
            if (wrappedLookupItem != null)
            {
                return Name.Get(wrappedLookupItem.GetType().Name + ":" + wrappedLookupItem.Item.GetName());
            }
            return null;
        }

       

        private static IName TryGetNameFromTemplateLookupItem(ILookupItem lookupItem)
        {
            var templateLookupItem = lookupItem as TemplateLookupItem;
            return templateLookupItem == null ? null : templateLookupItem.Template.GetName();
        }

        private static LiveTemplateName GetName(this Template template)
        {
            return LiveTemplateName.Get(template.Shortcut + ":" + template.Description);
        }
        */

        private static IName GetNameFromLookupItemIdentity(ILookupItem item)
        {
            return Name.Get(GetPossiblyGenericTypeName(item) + ":" + item.Identity);
        }

        private static string GetPossiblyGenericTypeName(ILookupItem item)
        {
            var type = item.GetType();

            return CreateName(type);
        }

        private static string CreateName(Type type)
        {
            var sb = new StringBuilder();
            sb.Append(type.Name);
            if (type.IsGenericType)
            {
                sb.Append('[');
                var argTypes = type.GetGenericArguments().Select(t => "[" + CreateName(t) + "]");
                sb.Append(string.Join(", ", argTypes));
                sb.Append(']');
            }

            return sb.ToString();
        }
    }
}