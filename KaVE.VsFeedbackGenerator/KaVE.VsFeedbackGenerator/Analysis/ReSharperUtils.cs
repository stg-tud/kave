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
 * 
 * Contributors:
 *    - Sebastian Proksch
 *    - Sven Amann
 */

using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Resolve;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.Util;
using KaVE.Model.Names;
using KaVE.Utils.Assertion;

namespace KaVE.VsFeedbackGenerator.Analysis
{
    public static class ReSharperUtils
    {
        public static DeclaredElementInstance<IMethod> ResolveMethod(
            [NotNull] this ICSharpInvocationReference invocationRef)
        {
            var resolvedRef = invocationRef.Resolve().Result;
            IMethod declaration = null;
            ISubstitution substitution = null;
            if (resolvedRef.DeclaredElement != null)
            {
                declaration = (IMethod) resolvedRef.DeclaredElement;
                substitution = resolvedRef.Substitution;
            }
            else if (!resolvedRef.Candidates.IsEmpty())
            {
                // TODO reconsider this, maybe switch to "invocations" as analysis result, where an invocation can have zero, one, or more methods as its target
                declaration = (IMethod) resolvedRef.Candidates.First();
                substitution = resolvedRef.CandidateSubstitutions.First();
            }

            if (declaration != null)
            {
                return new DeclaredElementInstance<IMethod>(declaration, substitution);
            }
            return null;
        }

        public static IMethodDeclaration GetDeclaration([NotNull] this IMethod method)
        {
            var declarations = method.GetDeclarations();
            Asserts.That(declarations.Count <= 1, "more than one declaration for invoked method");
            if (declarations.Count == 0)
            {
                return null;
            }
            return (IMethodDeclaration) declarations.First();
        }

        public static IEnumerable<MethodRef> GetMethodRefsIn(MethodRef ep)
        {
            var eps = new HashSet<MethodRef>();
            var declarationHasBody = !ep.Declaration.IsAbstract;
            var isInvalid = !ep.Declaration.IsAbstract && ep.Declaration.Body == null;
            if (declarationHasBody && !isInvalid)
            {
                ep.Declaration.Body.Accept(new InvocationCollector(), eps);
            }
            return eps;
        }

        // TODO Quality: Move hierarchy juggling to ReSharperUtils
        public static IMethodName GetFirstDeclaration(this MethodRef self)
        {
            if (self.Method != null)
            {
                var hierarchy = self.Method.CollectDeclarationInfo(self.Name);

                if (hierarchy.First == null && hierarchy.Super != null)
                {
                    return hierarchy.Super;
                }
                if (hierarchy.First != null)
                {
                    return hierarchy.First;
                }
            }
            return self.Name;
        }
    }
}