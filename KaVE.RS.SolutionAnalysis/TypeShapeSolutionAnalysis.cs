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
using KaVE.Commons.Utils.Collections;
using KaVE.Commons.Utils.Exceptions;
using KaVE.JetBrains.Annotations;
using KaVE.RS.Commons.Analysis;

namespace KaVE.RS.SolutionAnalysis
{
    public class TypeShapeSolutionAnalysis : BaseSolutionAnalysis
    {
        private readonly Action<ITypeShape> _cbTypeShape;
        private readonly IKaVESet<string> _seenClrNames;

        public TypeShapeSolutionAnalysis(ISolution solution,
            ILogger logger,
            Action<ITypeShape> cbTypeShape) : base(solution, logger)
        {
            _cbTypeShape = cbTypeShape;
            _seenClrNames = Sets.NewHashSet<string>();
        }

        protected override void AnalyzePrimaryPsiModule(IPsiModule primaryPsiModule)
        {
            var psiServices = primaryPsiModule.GetPsiServices();
            var symbolScope = psiServices.Symbols.GetSymbolScope(primaryPsiModule, true, true);
            var globalNamespace = symbolScope.GlobalNamespace;

            foreach (var te in FindTypeElements(globalNamespace, symbolScope))
            {
                // ignore private and internal types
                if (!te.CanBeVisibleToSolution())
                {
                    continue;
                }

                // ignore types defined in solution
                if (!IsDefinedInDependency(te))
                {
                    continue;
                }

                // ignore types that are already processed
                var clrName = te.GetClrName().FullName;
                if (!_seenClrNames.Add(clrName))
                {
                    continue;
                }

                // see http://stackoverflow.com/questions/4603139/a-c-sharp-class-with-a-null-namespace
                var isMetaDataClass = "FXAssembly".Equals(clrName) || "ThisAssembly".Equals(clrName) ||
                                      "AssemblyRef".Equals(clrName);
                if (isMetaDataClass)
                {
                    continue;
                }

                // ignore private
                if (clrName.StartsWith("<PrivateImplementationDetails>"))
                {
                    continue;
                }

                // ignore c++ impl details
                if (clrName.StartsWith("<CppImplementationDetails>"))
                {
                    continue;
                }

                // ignore crt impl details
                if (clrName.StartsWith("<CrtImplementationDetails>"))
                {
                    continue;
                }

                // ignore anonymous
                if (clrName.StartsWith("<>"))
                {
                    continue;
                }

                // ignore gcroots
                if (clrName.StartsWith("gcroot<"))
                {
                    continue;
                }

                // ignore global module
                if (clrName.Equals("<Module>"))
                {
                    continue;
                }

                // ignore unnnamed type values
                if (clrName.Contains("<unnamed-type-value>"))
                {
                    continue;
                }

                // ignore anonymous
                if (clrName.StartsWith("<"))
                {
                    Console.WriteLine("Inspect: " + clrName);
                }

                Execute.WithExceptionCallback(
                    () =>
                    {
                        var ts = AnalyzeTypeElement(te);
                        _cbTypeShape(ts);
                    },
                    e =>
                    {
                        Console.WriteLine("error: " + e.Message);
                        Console.WriteLine(te);
                    });
            }
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

        private static bool IsDefinedInDependency(IClrDeclaredElement te)
        {
            var containingModule = te.Module.ContainingProjectModule;
            var asm = containingModule as IAssembly;
            return asm != null;
        }

        [NotNull]
        private static TypeShape AnalyzeTypeElement(ITypeElement typeElement)
        {
            var typeShape = new TypeShapeAnalysis().Analyze(typeElement);
            return typeShape;
        }
    }
}