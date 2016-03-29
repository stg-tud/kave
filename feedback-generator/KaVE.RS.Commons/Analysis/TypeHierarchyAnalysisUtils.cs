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
using KaVE.Commons.Model.Names;
using KaVE.Commons.Model.TypeShapes;
using KaVE.RS.Commons.Utils.Names;

namespace KaVE.RS.Commons.Analysis
{
    internal static class TypeHierarchyAnalysisUtils
    {
        public static MethodHierarchy CollectDeclarationInfo([NotNull] this IMethod method, [NotNull] IMethodName name)
        {
            var decl = new MethodHierarchy(name);

            var immediate = method.GetImmediateSuperMembers().FirstOrDefault().Cast<IMethod>();
            if (immediate != null)
            {
                decl.Super = immediate.GetName<IMethodName>();
            }

            var root = method.GetRootSuperMembers().FirstOrDefault().Cast<IMethod>();
            if (root != null)
            {
                decl.First = root.GetName<IMethodName>();
            }

            if (decl.First != null && decl.First.Equals(decl.Super))
            {
                decl.First = null;
            }
            return decl;
        }
    }
}