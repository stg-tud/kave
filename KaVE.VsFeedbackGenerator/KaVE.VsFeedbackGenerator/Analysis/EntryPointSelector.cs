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
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using KaVE.Model.Names;
using KaVE.VsFeedbackGenerator.Utils.Names;

namespace KaVE.VsFeedbackGenerator.Analysis
{
    internal class EntryPoint
    {
        public IMethodName Name;
        public IMethodDeclaration Declaration;
    }

    internal class EntryPointSelector
    {
        // not yet ready to use
        public IEnumerable<EntryPoint> GetEntrypoints(ITypeDeclaration typeDeclaration)
        {
            var eps = new HashSet<EntryPoint>();
            var entryPointCandidates = GetEntryPointCandidates(typeDeclaration);
            var definiteEntryPoints = GetDefiniteEntryPoints(entryPointCandidates);

            var transitivelyAnalyzedMethods = new HashSet<IMethodName>();

            entryPointCandidates.RemoveAll(definiteEntryPoints.Contains);
            entryPointCandidates.RemoveAll(epc => transitivelyAnalyzedMethods.Contains(epc.Name));
            transitivelyAnalyzedMethods.Clear();

            return eps;
        }

        private static List<EntryPoint> GetEntryPointCandidates(ITypeDeclaration typeDeclaration)
        {
            return
                typeDeclaration.MemberDeclarations.OfType<IMethodDeclaration>()
                               .Where(md => !md.IsAbstract)
                               .Select(md => new EntryPoint {Declaration = md, Name = md.GetName()})
                               .Where(nmd => nmd.Name != null)
                               .ToList();
        }


        private List<EntryPoint> GetDefiniteEntryPoints(IEnumerable<EntryPoint> candidates)
        {
            return candidates.Where(nmd => IsOverrideOrImplementation(nmd.Name)).ToList();
        }

        private bool IsOverrideOrImplementation(IMethodName methodName)
        {
            return false;
                //_typeShape.MethodHierarchies.Any(mh => mh.Element == methodName && mh.IsOverrideOrImplementation);
        }
    }
}