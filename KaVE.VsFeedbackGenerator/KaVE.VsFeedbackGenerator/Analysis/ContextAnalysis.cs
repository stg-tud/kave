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

using System;
using JetBrains.ReSharper.Feature.Services.CSharp.CodeCompletion.Infrastructure;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using KaVE.JetBrains.Annotations;
using KaVE.Model.Events.CompletionEvent;
using KaVE.VsFeedbackGenerator.Generators;
using KaVE.VsFeedbackGenerator.Utils.Names;

namespace KaVE.VsFeedbackGenerator.Analysis
{
    internal class ContextAnalysis
    {
        private readonly ILogger _logger;
        private readonly TypeShapeAnalysis _typeShapeAnalysis = new TypeShapeAnalysis();
        private readonly CompletionTargetAnalysis _completionTargetAnalysis = new CompletionTargetAnalysis();

        private readonly CalledMethodsForEntryPointsAnalysis _calledMethodsForEntryPointsAnalysis =
            new CalledMethodsForEntryPointsAnalysis();

        private ContextAnalysis(ILogger logger)
        {
            _logger = logger;
        }

        public static Context Analyze(CSharpCodeCompletionContext rsContext, ILogger logger)
        {
            return new ContextAnalysis(logger).AnalyzeInternal(rsContext);
        }

        private Context AnalyzeInternal(CSharpCodeCompletionContext rsContext)
        {
            var context = Context.Empty;

            try
            {
                AnalyzeInternal(rsContext.NodeInFile, context);
            }
            catch (Exception e)
            {
                _logger.Error(new Exception("analysis failed", e));
            }

            return context;
        }

        private void AnalyzeInternal(ITreeNode nodeInFile, Context context)
        {
            var typeDeclaration = FindEnclosing<ITypeDeclaration>(nodeInFile);
            if (typeDeclaration != null)
            {
                context.TypeShape = _typeShapeAnalysis.Analyze(typeDeclaration);
                context.TriggerTarget = _completionTargetAnalysis.Analyze(nodeInFile);

                var entryPoints = new EntryPointSelector(typeDeclaration, context.TypeShape).GetEntryPoints();
                context.EntryPointToCalledMethods = _calledMethodsForEntryPointsAnalysis.Analyze(entryPoints);

                var methodDeclaration = FindEnclosing<IMethodDeclaration>(nodeInFile);
                if (methodDeclaration != null)
                {
                    context.EnclosingMethod = methodDeclaration.GetName();
                }
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
    }
}