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
 *    - Sven Amann
 */

using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using KaVE.Model.Events.CompletionEvent;
using KaVE.Model.Names;
using KaVE.VsFeedbackGenerator.Utils.Names;
using NuGet;

namespace KaVE.VsFeedbackGenerator.Analysis
{
    internal class CalledMethodsForEntryPointsAnalysis
    {
        private class EntryPoint
        {
            public IMethodName Name;
            public IMethodDeclaration Declaration;
        }

        private readonly CalledMethodsAnalysis _calledMethodsAnalysis = new CalledMethodsAnalysis();
        private TypeShape _typeShape;

        public IDictionary<IMethodName, ISet<IMethodName>> Analyze(ITypeDeclaration typeDeclaration, TypeShape typeShape)
        {
            _typeShape = typeShape;
            var result = new Dictionary<IMethodName, ISet<IMethodName>>();
            var entryPointCandidates = GetEntryPointCandidates(typeDeclaration);
            var definiteEntryPoints = GetDefiniteEntryPoints(entryPointCandidates);

            var transitivelyAnalyzedMethods = new HashSet<IMethodName>();
            result.AddRange(Analyze(definiteEntryPoints, transitivelyAnalyzedMethods));

            entryPointCandidates.RemoveAll(definiteEntryPoints.Contains);
            entryPointCandidates.RemoveAll(epc => transitivelyAnalyzedMethods.Contains(epc.Name));
            transitivelyAnalyzedMethods.Clear();

            result.AddRange(Analyze(entryPointCandidates, transitivelyAnalyzedMethods));
            result.RemoveAll(kvp => transitivelyAnalyzedMethods.Contains(kvp.Key));

            return result;
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
            return _typeShape.MethodHierarchies.Any(mh => mh.Element == methodName && mh.IsOverrideOrImplementation);
        }

        private IEnumerable<KeyValuePair<IMethodName, ISet<IMethodName>>> Analyze(IEnumerable<EntryPoint> entryPoints,
            ISet<IMethodName> allAnalyzedMethods)
        {
            var result = new Dictionary<IMethodName, ISet<IMethodName>>();
            foreach (var entryPoint in entryPoints)
            {
                var partialResult = Analyze(entryPoint, allAnalyzedMethods);
                try
                {
                    result.Add(entryPoint.Name, partialResult);
                }
                catch (ArgumentException)
                {
                    // this happens in case of duplicated methods in a class,
                    // the only thing we can do here is to skip the second
                    // method, because only one entry per key is possible.
                }
            }
            return result;
        }

        private ISet<IMethodName> Analyze(EntryPoint entryPoint, ISet<IMethodName> allAnalyzedMethods)
        {
            var collectionContext = _calledMethodsAnalysis.Analyze(entryPoint.Declaration, _typeShape);
            allAnalyzedMethods.AddRange(collectionContext.AnalyzedMethods);
            return collectionContext.CalledMethods;
        }
    }
}