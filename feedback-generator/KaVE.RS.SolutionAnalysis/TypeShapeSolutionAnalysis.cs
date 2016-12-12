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
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.Model2.Assemblies.Interfaces;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Caches;
using JetBrains.ReSharper.Psi.Modules;
using KaVE.Commons.Model.TypeShapes;
using KaVE.Commons.Utils.Exceptions;
using KaVE.JetBrains.Annotations;
using KaVE.RS.Commons.Analysis;

namespace KaVE.RS.SolutionAnalysis
{
    public class TypeShapeSolutionAnalysis : BaseSolutionAnalysis
    {
        private readonly Action<ITypeShape> _cbTypeShape;

        public TypeShapeSolutionAnalysis(ISolution solution,
            ILogger logger,
            Action<ITypeShape> cbTypeShape) : base(solution, logger)
        {
            _cbTypeShape = cbTypeShape;
        }

        protected override void AnalyzePrimaryPsiModule(IPsiModule primaryPsiModule)
        {
            var psiServices = primaryPsiModule.GetPsiServices();
            var symbolScope = psiServices.Symbols.GetSymbolScope(primaryPsiModule, true, true);

            var globalNamespace = symbolScope.GlobalNamespace;
            foreach (var te in FindTypeElements(globalNamespace, symbolScope))
            {
                var clrTypeName = te.GetClrName();
                Console.WriteLine(clrTypeName);

                if (!te.CanBeVisibleToSolution())
                {
                    Console.WriteLine("--> skip (invisible)\n");
                    continue;
                }

                if (!IsDefinedInDependency(te))
                {
                    Console.WriteLine("--> skip (no dependency)\n");
                    continue;
                }

                var ts = AnalyzeTypeElement(te);
                Console.WriteLine("--> ok\n", ts);
                _cbTypeShape(ts);
            }
        }

        private static bool IsDefinedInDependency(ITypeElement te)
        {
            var containingModule = te.Module.ContainingProjectModule;
            var asm = containingModule as IAssembly;
            return asm != null;
        }

        private static IEnumerable<ITypeElement> FindTypeElements(INamespace ns,
            ISymbolScope symbolScope)
        {
            foreach (var te in ns.GetNestedTypeElements(symbolScope))
            {
                yield return te;

                foreach (var nte in te.NestedTypes)
                {
                    yield return nte;
                }
            }

            foreach (var nsNested in ns.GetNestedNamespaces(symbolScope))
            {
                foreach (var te in FindTypeElements(nsNested, symbolScope))
                {
                    yield return te;
                }
            }
        }

        [NotNull]
        private static TypeShape AnalyzeTypeElement(ITypeElement typeElement)
        {
            // TODO @seb: TypeShapeAnalysis für RootTypes implementieren
            if (IsRootType(typeElement))
            {
                return new TypeShape();
            }
            var typeShapeAnalysis = new TypeShapeAnalysis();
            var typeShape = typeShapeAnalysis.Analyze(typeElement);
            return typeShape;
        }

        private static bool IsRootType(ITypeElement type)
        {
            var fn = type.GetClrName().FullName;
            return "System.Object".Equals(fn) || "System.ValueType".Equals(fn) || "System.Enum".Equals(fn);
        }
    }
}