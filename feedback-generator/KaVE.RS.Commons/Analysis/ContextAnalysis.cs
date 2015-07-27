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
using System.Linq;
using System.Threading;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using KaVE.Commons.Model.Events.CompletionEvents;
using KaVE.Commons.Model.Names;
using KaVE.Commons.Model.Names.CSharp;
using KaVE.Commons.Model.SSTs.Impl;
using KaVE.Commons.Utils.Collections;
using KaVE.Commons.Utils.Concurrency;
using KaVE.Commons.Utils.Exceptions;
using KaVE.RS.Commons.Analysis.CompletionTarget;
using KaVE.RS.Commons.Analysis.Transformer;
using KaVE.RS.Commons.Utils.Names;

namespace KaVE.RS.Commons.Analysis
{
    public class ContextAnalysis
    {
        public const int DefaultTimeLimitInMs = 1000;

        private static readonly object Lock = new object();
        private static readonly KaVECancellationTokenSource TokenSource = new KaVECancellationTokenSource();

        private static readonly Dictionary<int, Context> ResultCache = new Dictionary<int, Context>();

        public static ContextAnalysisResult Analyze(ITreeNode node, ILogger logger)
        {
            lock (Lock)
            {
                var res = AnalyseAsyncInternal(node, logger, CancellationToken.None);
                return res;
            }
        }

        public static void AnalyseAsync(ITreeNode node,
            ILogger logger,
            Action<Context> onSuccess,
            Action<Exception> onFailure,
            Action onTimeout,
            int timeLimitInMs = DefaultTimeLimitInMs)
        {
            var context = AnalyseAsyncWithCache(node, logger);
            onSuccess(context);
        }

        private static ContextAnalysisResult AnalyseAsyncInternal(ITreeNode node,
            ILogger logger,
            CancellationToken token)
        {
            var analysis = new ContextAnalysisInternal(logger, token);
            var result = analysis.AnalyzeInternal(node);
            return result;
        }

        private static Context AnalyseAsyncWithCache(ITreeNode node,
            ILogger logger)
        {
            lock (Lock)
            {
                var token = TokenSource.CancelAndCreate();
                var hashCode = node.GetHashCode();

                if (ResultCache.ContainsKey(hashCode))
                {
                    logger.Info("cache hit ({0})", hashCode);
                    return ResultCache[hashCode];
                }
                logger.Info("cache miss ({0})", hashCode);

                var res = AnalyseAsyncInternal(node, logger, token);
                ResultCache[hashCode] = res.Context;
                logger.Info("cache set ({0})", hashCode);

                RemoveFromCacheAfterTimeout(hashCode, 5000, logger);
                return res.Context;
            }
        }

        private static void RemoveFromCacheAfterTimeout(int hashCode,
            int timeout,
            ILogger logger)
        {
            Task.Factory.StartNew(
                () =>
                {
                    Thread.Sleep(timeout);
                    lock (Lock)
                    {
                        ResultCache.Remove(hashCode);
                        logger.Info("cache cleanup ({0})", hashCode);
                    }
                });
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
            private readonly CancellationToken _token;

            public ContextAnalysisInternal(ILogger logger, CancellationToken token)
            {
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

                var classDeclaration = FindEnclosing<IClassDeclaration>(nodeInFile);
                if (classDeclaration != null && classDeclaration.DeclaredElement != null)
                {
                    context.TypeShape = _typeShapeAnalysis.Analyze(classDeclaration);

                    var entryPointRefs = new EntryPointSelector(classDeclaration, context.TypeShape).GetEntryPoints();
                    res.EntryPoints = Sets.NewHashSetFrom(entryPointRefs.Select(epr => epr.Name));

                    sst.EnclosingType = classDeclaration.DeclaredElement.GetName<ITypeName>();
                    res.CompletionMarker = _completionTargetAnalysis.Analyze(nodeInFile);
                    classDeclaration.Accept(
                        new DeclarationVisitor(_logger, res.EntryPoints, res.CompletionMarker, _token),
                        sst);
                }
                else
                {
                    sst.EnclosingType = TypeName.UnknownName;
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