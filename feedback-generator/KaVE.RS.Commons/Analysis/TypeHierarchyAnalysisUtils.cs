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

using System.Linq;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Util;
using KaVE.Commons.Model.Naming;
using KaVE.Commons.Model.Naming.CodeElements;
using KaVE.Commons.Model.TypeShapes;
using KaVE.RS.Commons.Utils.Naming;

namespace KaVE.RS.Commons.Analysis
{
    internal static class TypeHierarchyAnalysisUtils
    {
        public static TH CollectDeclarationInfo<T, TH>([NotNull] this IOverridableMember member, [NotNull] T name) 
            where T:class, IMemberName
            where TH:IHierarchy<T>, new()
        {
            var decl = new TH {Element = name};

            OverridableMemberInstance superMember = null;

            foreach (var super in member.GetImmediateSuperMembers())
            {
                superMember = super;

                if (super.DeclaringType.IsInterfaceType() ||
                    super.GetRootSuperMembers(false).Any(m => m.DeclaringType.IsInterfaceType()))
                {
                    break;
                }
            }

            if (superMember != null)
            {
                decl.Super = superMember.GetName<T>();

                var firstMember = superMember.GetRootSuperMembers(false).FirstOrDefault() ?? superMember;

                decl.First = firstMember.GetName<T>();
            }

            if (decl.First != null && decl.Super != null && decl.First.Equals(decl.Super))
            {
                if (decl.Super.DeclaringType.IsInterfaceType)
                {
                    decl.Super = null;
                }
                else
                {
                    decl.First = null;
                }
            }

            return decl;
        }
    }
}