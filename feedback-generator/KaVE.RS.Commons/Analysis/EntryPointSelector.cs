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

using System.Collections.Generic;
using System.Linq;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Parsing;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using KaVE.Commons.Model.Names;
using KaVE.Commons.Model.TypeShapes;
using KaVE.RS.Commons.Utils.Names;

namespace KaVE.RS.Commons.Analysis
{
    public class EntryPointSelector
    {
        private readonly ITypeDeclaration _typeDeclaration;
        private readonly ITypeShape _typeShape;
        private readonly ITypeElement _typeElem;

        private IList<MethodRef> _entryPoints;
        private IList<MethodRef> _analyzed;


        public EntryPointSelector(ITypeDeclaration typeDeclaration, ITypeShape typeShape)
        {
            _typeDeclaration = typeDeclaration;
            _typeShape = typeShape;
            _typeElem = typeDeclaration.DeclaredElement;
        }

        public ISet<MethodRef> GetEntryPoints()
        {
            if (_typeElem == null)
            {
                return new HashSet<MethodRef>();
            }

            var entryPointCandidates = GetEntryPointCandidates();
            _entryPoints = GetDefiniteEntryPoints(entryPointCandidates);
            _analyzed = new List<MethodRef>();

            foreach (var ep in _entryPoints)
            {
                AnalyzeTransitiveCallsIn(ep);
            }

            foreach (var ep in entryPointCandidates.Where(IsNotAnalyzed))
            {
                _entryPoints.Add(ep);
                AnalyzeTransitiveCallsIn(ep);
            }

            var eps = new HashSet<MethodRef>(_entryPoints);
            return eps;
        }

        private IList<MethodRef> GetEntryPointCandidates()
        {
            var unfiltered = _typeDeclaration.MemberDeclarations.OfType<IMethodDeclaration>();
            var nonAbstract = unfiltered.Where(md => !md.IsAbstract);
            // sometimes, e.g. in incomplete code snippets, the body of a method is not yet set
            var onlyValid = nonAbstract.Where(md => md.Body != null);
            var filtered = onlyValid.Where(IsNotPrivateOrInternal);
            var mapped = filtered.Select(CreateRef);
            return mapped.ToList();
        }

        private static MethodRef CreateRef(IMethodDeclaration md)
        {
            return MethodRef.CreateLocalReference(md.GetName(), md.DeclaredElement, md);
        }

        private static bool IsNotPrivateOrInternal(IMethodDeclaration md)
        {
            if (md.ModifiersList == null)
            {
                return false;
            }
            var isPrivate = md.ModifiersList.HasModifier(CSharpTokenType.PRIVATE_KEYWORD);
            var isInternal = md.ModifiersList.HasModifier(CSharpTokenType.INTERNAL_KEYWORD);
            return !(isPrivate || isInternal);
        }

        private IList<MethodRef> GetDefiniteEntryPoints(IEnumerable<MethodRef> candidates)
        {
            return candidates.Where(nmd => IsDefiniteEntryPoint(nmd.Name)).ToList();
        }

        private bool IsDefiniteEntryPoint(IMethodName methodName)
        {
            return _typeShape.MethodHierarchies.Any(mh => mh.Element == methodName && mh.IsDeclaredInParentHierarchy);
        }

        private void AnalyzeTransitiveCallsIn(MethodRef ep)
        {
            if (IsDeclaredBy(ep.Method, _typeElem))
            {
                ReallyAnalyzeTransitiveCallsIn(ep);
            }
        }

        private void ReallyAnalyzeTransitiveCallsIn(MethodRef ep)
        {
            _analyzed.Add(ep);

            foreach (var method in ReSharperUtils.FindMethodsInvokedIn(ep))
            {
                if (IsNotAnalyzed(method) && !method.IsAssemblyReference)
                {
                    AnalyzeTransitiveCallsIn(method);
                }
                else
                {
                    var isSimpleRecursion = method.Equals(ep);
                    if (IsEntryPoint(method) && !IsDefiniteEntryPoint(method.Name) && !isSimpleRecursion)
                    {
                        _entryPoints.Remove(method);
                    }
                }
            }
        }

        private bool IsEntryPoint(MethodRef ep)
        {
            return _entryPoints.Contains(ep);
        }

        private bool IsNotAnalyzed(MethodRef ep2)
        {
            return !_analyzed.Contains(ep2);
        }

        private static bool IsDeclaredBy(IMethod declaredElement, ITypeElement typeElem)
        {
            return typeElem.Equals(declaredElement.GetContainingType());
        }
    }
}