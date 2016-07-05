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
using KaVE.Commons.Model.Naming.CodeElements;
using KaVE.Commons.Model.TypeShapes;
using KaVE.RS.Commons.Utils.Naming;

namespace KaVE.RS.Commons.Analysis
{
    internal static class TypeHierarchyAnalysisUtils
    {
        public static MethodHierarchy CollectDeclarationInfo([NotNull] this IMethod method, [NotNull] IMethodName name)
        {
            var decl = new MethodHierarchy(name);

            OverridableMemberInstance superMethod = null;

            foreach (var super in method.GetImmediateSuperMembers())
            {
                superMethod = super;

                if (super.DeclaringType.IsInterfaceType() ||
                    super.GetRootSuperMembers(false).Any(m => m.DeclaringType.IsInterfaceType()))
                {
                    break;
                }
            }

            if (superMethod != null)
            {
                decl.Super = superMethod.GetName<IMethodName>();

                var firstMethod = superMethod.GetRootSuperMembers(false).FirstOrDefault() ?? superMethod;

                decl.First = firstMethod.GetName<IMethodName>();
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