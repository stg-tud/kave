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
using KaVE.Commons.Model.Naming;
using KaVE.RS.Commons.Utils.Naming;

namespace KaVE.RS.Commons.Utils.LookupItems
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
            //return new Proposal {Name = Name.Get("deactivated")};
            var name = lookupItem == null ? Names.UnknownGeneral : lookupItem.GetName();
            return new Proposal {Name = name};
        }

        [NotNull]
        public static IEnumerable<Proposal> ToProposals([CanBeNull] this LookupItem<MethodsInfo> lookupItem)
        {
            var result = new List<Proposal>();

            if (lookupItem != null)
            {
                lookupItem.Info.Candidates.ToList()
                          .ForEach(
                              candidate => result.Add(new Proposal {Name = candidate.GetName()}));
            }

            return result;
        }

        private static IName GetName([NotNull] this ILookupItem lookupItem)
        {
            // TODO create debug cases to decide for a final version of this utility class
            var displayName = lookupItem.DisplayName;
            //
            var n1 = TryGetNameFromLookupItem<BasicInfo>(lookupItem);
            var n2 = TryGetNameFromLookupItem<ConstructorInfo>(lookupItem);
            var n3 = TryGetNameFromLookupItem<DeclaredElementInfo>(lookupItem);
            var n4 = TryGetNameFromLookupItem<DelegateInfo>(lookupItem);
            var n5 = TryGetNameFromLookupItem<MethodsInfo>(lookupItem);
            var n6 = TryGetNameFromLookupItem<TypeInfo>(lookupItem);
            var n7 = TryGetNameFromLookupItem<TextualInfo>(lookupItem);
            //
            var n8 = TryGetNameFromLookupItem<ShortArrayConstructorInfo>(lookupItem);

            var pbnItem = TryGetNameFromPBNProposal(lookupItem);
            var csDeclItem = TryGetNameFromLookupItem<CSharpDeclaredElementInfo>(lookupItem);
            var declElemItem = TryGetNameFromLookupItem<DeclaredElementInfo>(lookupItem);
            var methodItem = TryGetNameFromLookupItem<MethodsInfo>(lookupItem);
            var typeItem = TryGetNameFromLookupItem<TypeElementInfo>(lookupItem);
            var initItem = TryGetNameFromConstructorInfoLookupItem(lookupItem);

            // TODO inline variables again, after debugging is over
            return pbnItem ??
                   csDeclItem ??
                   declElemItem ??
                   methodItem ??
                   typeItem ??
                   initItem ??
                   GetNameFromLookupItemDisplayName(lookupItem);
        }

        private static IName TryGetNameFromPBNProposal(ILookupItem lookupItem)
        {
            var pbnItem = lookupItem as PBNProposalWrappedLookupItem;
            return pbnItem != null
                ? Names.General(pbnItem.ToString())
                : null;
        }

        private static IName TryGetNameFromLookupItem<T>(ILookupItem lookupItem) where T : class, ILookupItemInfo
        {
            var li = lookupItem as LookupItem<T>;
            if (li == null || li.Info == null)
            {
                return null;
            }
            var de = li.GetAllDeclaredElements().FirstOrDefault();
            return de == null ? null : de.GetName();
        }

        private static IName TryGetNameFromConstructorInfoLookupItem(ILookupItem lookupItem)
        {
            var li = lookupItem as LookupItem<ConstructorInfo>;
            if (li == null || li.Info == null)
            {
                return null;
            }
            var des = li.GetAllDeclaredElements();
            var de = des.FirstOrDefault();
            if (de == null)
            {
                return null;
            }

            // strangely, the IDE points to the class instead of the constructor. As we don't get
            // the method, we introduce an artificial constructor name to at least capture the
            // information that a constructor was selected
            var typeName = de.GetName();
            return Names.Method(string.Format("[p:void] [{0}]..ctor()", typeName));
        }

        private static IName GetNameFromLookupItemDisplayName(ILookupItem item)
        {
            return Names.General(string.Format("{0}:{1}", GetPossiblyGenericTypeName(item), item.DisplayName.Text));
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