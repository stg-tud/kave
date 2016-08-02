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
using System.Linq;
using System.Threading;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using KaVE.Commons.Model.Events.CompletionEvents;
using KaVE.Commons.Model.Naming;
using KaVE.Commons.Model.Naming.CodeElements;
using KaVE.Commons.Model.Naming.Types;
using KaVE.Commons.Model.SSTs.Impl;
using KaVE.Commons.Utils.Collections;
using KaVE.Commons.Utils.Exceptions;
using KaVE.RS.Commons.Analysis.CompletionTarget;
using KaVE.RS.Commons.Analysis.Transformer;
using KaVE.RS.Commons.Utils.Naming;

namespace KaVE.RS.Commons.Analysis
{
    public class ContextAnalysis
    {
        public const int DefaultTimeLimitInMs = 1000;
        public const int CacheTimeout = 10000;

        private static readonly object Lock = new object();
        private static readonly KaVECancellationTokenSource TokenSource = new KaVECancellationTokenSource();

        private static readonly FifoCache<int, Context> ResultCache = new FifoCache<int, Context>(4);

        public static ContextAnalysisResult Analyze(ITreeNode node, IPsiSourceFile srcFile, ILogger logger)
        {
            lock (Lock)
            {
                var res = AnalyseAsyncInternal(node, srcFile, logger, CancellationToken.None);
                return res;
            }
        }

        public static void Analyse(ITreeNode node,
            IPsiSourceFile srcFile,
            ILogger logger,
            Action<Context> onSuccess,
            Action<Exception> onFailure,
            Action onTimeout,
            int timeLimitInMs = DefaultTimeLimitInMs)
        {
            var context = AnalyseWithCache(node, srcFile, logger);
            onSuccess(context);
        }

        private static ContextAnalysisResult AnalyseAsyncInternal(ITreeNode node,
            IPsiSourceFile srcFile,
            ILogger logger,
            CancellationToken token)
        {
            var analysis = new ContextAnalysisInternal(srcFile, logger, token);
            var result = analysis.AnalyzeInternal(node);
            return result;
        }

        private static Context AnalyseWithCache(ITreeNode node,
            IPsiSourceFile srcFile,
            ILogger logger)
        {
            lock (Lock)
            {
                var token = TokenSource.CancelAndCreate();
                var hashCode = node.GetHashCode();

                if (ResultCache.ContainsKey(hashCode))
                {
                    return ResultCache.GetValue(hashCode);
                }

                var res = AnalyseAsyncInternal(node, srcFile, logger, token);
                ResultCache.SetValue(hashCode, res.Context);

                return res.Context;
            }
        }

        public class ContextAnalysisResult
        {
            [NotNull]
            public Context Context { get; set; }

            public CompletionTargetMarker CompletionMarker { get; set; }
            public IKaVESet<IMethodName> EntryPoints { get; set; }

            public ContextAnalysisResult()
            {
                Context = new Context();
            }
        }

        private class ContextAnalysisInternal
        {
            private readonly ILogger _logger;
            private readonly TypeShapeAnalysis _typeShapeAnalysis = new TypeShapeAnalysis();
            private readonly CompletionTargetAnalysis _completionTargetAnalysis = new CompletionTargetAnalysis();
            private readonly IPsiSourceFile _srcFile;
            private readonly CancellationToken _token;

            public ContextAnalysisInternal(IPsiSourceFile srcFile, ILogger logger, CancellationToken token)
            {
                _srcFile = srcFile;
                _token = token;
                _logger = logger;
            }

            public ContextAnalysisResult AnalyzeInternal(ITreeNode type)
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

                var typeDecl = FindEnclosing<ICSharpTypeDeclaration>(nodeInFile);
                if (typeDecl != null && typeDecl.DeclaredElement != null)
                {
                    context.TypeShape = _typeShapeAnalysis.Analyze(typeDecl);

                    var entryPointRefs = new EntryPointSelector(typeDecl, context.TypeShape).GetEntryPoints();
                    res.EntryPoints = Sets.NewHashSetFrom(entryPointRefs.Select(epr => epr.Name));

                    sst.EnclosingType = typeDecl.DeclaredElement.GetName<ITypeName>();

                    if (typeDecl.IsPartial)
                    {
                        var file = nodeInFile.GetSourceFile();
                        sst.PartialClassIdentifier = file != null
                            ? file.DisplayName
                            : _srcFile != null ? _srcFile.DisplayName : "partial";
                    }

                    res.CompletionMarker = _completionTargetAnalysis.Analyze(nodeInFile);
                    typeDecl.Accept(
                        new DeclarationVisitor(_logger, res.EntryPoints, res.CompletionMarker, _token),
                        sst);
                }
                else
                {
                    sst.EnclosingType = Names.UnknownType;
                }
            }

            [CanBeNull]
            private static TIDeclaration FindEnclosing<TIDeclaration>(ITreeNode node)
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
}