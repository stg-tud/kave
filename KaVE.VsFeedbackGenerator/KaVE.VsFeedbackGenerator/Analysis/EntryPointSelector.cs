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
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Parsing;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using KaVE.Model.Events.CompletionEvent;
using KaVE.Model.Names;
using KaVE.VsFeedbackGenerator.Utils.Names;

namespace KaVE.VsFeedbackGenerator.Analysis
{
    public class EntryPointSelector
    {
        private readonly ITypeDeclaration _typeDeclaration;
        private readonly TypeShape _typeShape;
        private readonly ITypeElement _typeElem;

        private IList<MethodRef> _analyzed;
        private IList<MethodRef> _covered;


        public EntryPointSelector(ITypeDeclaration typeDeclaration, TypeShape typeShape)
        {
            _typeDeclaration = typeDeclaration;
            _typeShape = typeShape;
            _typeElem = typeDeclaration.DeclaredElement;
        }

        public ISet<MethodRef> GetEntrypoints()
        {
            if (_typeElem == null)
            {
                return new HashSet<MethodRef>();
            }

            var entryPointCandidates = GetEntryPointCandidates();
            _analyzed = GetDefiniteEntryPoints(entryPointCandidates);
            _covered = new List<MethodRef>();

            foreach (var ep in _analyzed)
            {
                AddTransitiveCalls(ep);
            }

            foreach (var ep in entryPointCandidates)
            {
                if (IsNotCovered(ep))
                {
                    _analyzed.Add(ep);
                    AddTransitiveCalls(ep);
                }
            }

            var eps = new HashSet<MethodRef>(_analyzed);
            return eps;
        }

        private IList<MethodRef> GetEntryPointCandidates()
        {
            var unfiltered = _typeDeclaration.MemberDeclarations.OfType<IMethodDeclaration>();
            var nonAbstract = unfiltered.Where(md => !md.IsAbstract);
            var onlyValid = nonAbstract.Where(md => md.Body != null); // WTF (invalid code?)
            var filtered = onlyValid.Where(IsNotPrivateOrInternal);
            var mapped = filtered.Select(CreateRef)
                                 .Where(md => md.Name != null && md.Declaration != null);
            return mapped.ToList();
        }

        private MethodRef CreateRef(IMethodDeclaration md)
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
            return _typeShape.MethodHierarchies.Any(mh => mh.Element == methodName && mh.IsOverrideOrImplementation);
        }

        private void AddTransitiveCalls(MethodRef ep)
        {
            var methodElem = ep.Declaration.DeclaredElement;
            if (methodElem == null)
            {
                return;
            }

            if (HasSameType(_typeElem, methodElem))
            {
                ReallyAddTransitiveCalls(ep);
            }
        }

        private void ReallyAddTransitiveCalls(MethodRef ep)
        {
            _covered.Add(ep);

            foreach (var ep2 in ReSharperUtils.GetMethodRefsIn(ep))
            {
                if (IsNotCovered(ep2) && !ep2.IsAssemblyReference)
                {
                    AddTransitiveCalls(ep2);
                }
                else
                {
                    var isSimpleRecursion = ep2.Equals(ep);
                    if (WasTreatedAsEntrypoint(ep2) && !IsDefiniteEntryPoint(ep2.Name) && !isSimpleRecursion)
                    {
                        _analyzed.Remove(ep2);
                    }
                }
            }
        }

        private bool WasTreatedAsEntrypoint(MethodRef ep)
        {
            return _analyzed.Contains(ep);
        }

        private bool IsNotCovered(MethodRef ep2)
        {
            return !_covered.Contains(ep2);
        }

        private static bool HasSameType(ITypeElement typeElem, IMethod declaredElement)
        {
            return typeElem.Equals(declaredElement.GetContainingType());
        }
    }
}