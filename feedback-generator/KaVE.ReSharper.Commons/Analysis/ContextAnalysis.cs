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
using JetBrains.ReSharper.Feature.Services.CSharp.CodeCompletion.Infrastructure;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using KaVE.ReSharper.Commons.Analysis.CompletionTarget;
using KaVE.ReSharper.Commons.Analysis.Transformer;

namespace KaVE.ReSharper.Commons.Analysis
{
    public class ContextAnalysis
    {
        private readonly ILogger _logger;
        private readonly TypeShapeAnalysis _typeShapeAnalysis = new TypeShapeAnalysis();
        private readonly CompletionTargetAnalysis _completionTargetAnalysis = new CompletionTargetAnalysis();

        private ContextAnalysis(ILogger logger)
        {
            _logger = logger;
        }

        public static ContextAnalysisResult Analyze(CSharpCodeCompletionContext rsContext, ILogger logger)
        {
            return Analyze(rsContext.NodeInFile, logger);
        }

        public static ContextAnalysisResult Analyze(ITreeNode node, ILogger logger)
        {
            return new ContextAnalysis(logger).AnalyzeInternal(node);
        }

        private ContextAnalysisResult AnalyzeInternal(ITreeNode type)
        {
            var res = new ContextAnalysisResult
            {
                Context = new Context()
            };

            Execute.WithExceptionLogging(_logger, () => AnalyzeInternal(type, res));

            return res;
        }

        private void AnalyzeInternal(ITreeNode nodeInFile, ContextAnalysisResult res)
        {
            var context = res.Context;
            var sst = new SST();
            context.SST = sst;

            var classDeclaration = FindEnclosing<IClassDeclaration>(nodeInFile);
            if (classDeclaration != null && classDeclaration.DeclaredElement != null)
            {
                context.TypeShape = _typeShapeAnalysis.Analyze(classDeclaration);

                var entryPointRefs = new EntryPointSelector(classDeclaration, context.TypeShape).GetEntryPoints();
                res.EntryPoints = Sets.NewHashSetFrom(entryPointRefs.Select(epr => epr.Name));

                sst.EnclosingType = classDeclaration.DeclaredElement.GetName<ITypeName>();
                res.CompletionMarker = _completionTargetAnalysis.Analyze(nodeInFile);
                classDeclaration.Accept(new DeclarationVisitor(res.EntryPoints, res.CompletionMarker), sst);
            }
            else
            {
                sst.EnclosingType = TypeName.UnknownName;
            }
        }

        [CanBeNull]
        public static TIDeclaration FindEnclosing<TIDeclaration>(ITreeNode node)
            where TIDeclaration : class, IDeclaration
        {
            while (node != null)
            {
                var declaration = node as TIDeclaration;
                if (declaration != null)
                {
                    return declaration;
                }
                node = node.Parent;
            }
            return null;
        }

        // TODO discuss change in return values
        public class ContextAnalysisResult
        {
            [NotNull]
            public Context Context { get; set; }

            public CompletionTargetMarker CompletionMarker { get; set; }
            public IKaVESet<IMethodName> EntryPoints { get; set; }
        }
    }
}