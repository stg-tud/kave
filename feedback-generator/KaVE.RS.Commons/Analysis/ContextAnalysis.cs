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
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Util;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Resources.Shell;
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
using Task = System.Threading.Tasks.Task;

namespace KaVE.RS.Commons.Analysis
{
    public class ContextAnalysis
    {
        public const int DefaultTimeLimitInMs = 1000;

        private static readonly object Lock = new object();
        private static readonly KaVECancellationTokenSource TokenSource = new KaVECancellationTokenSource();

        private static readonly ConcurrentDictionary<int, System.Threading.Tasks.Task<ContextAnalysisResult>>
            CurrentTasks =
                new ConcurrentDictionary<int, System.Threading.Tasks.Task<ContextAnalysisResult>>();

        public static ContextAnalysisResult Analyze(ITreeNode node, ILogger logger)
        {
            lock (Lock)
            {
                var analysisTask = AnalyseAsyncInternal(node, logger, CancellationToken.None);
                analysisTask.Wait();
                return analysisTask.Result;
            }
        }

        public static void AnalyseAsync(ITreeNode node,
            ILogger logger,
            Action<Context> onSuccess,
            Action<Exception> onFailure,
            Action onTimeout,
            int timeLimitInMs = DefaultTimeLimitInMs)
        {
            var analysisTask = AnalyseAsyncWithCache(node, logger);
            var timeLimit = TaskUtils.Delay(timeLimitInMs);

            Task.WaitAny(analysisTask, timeLimit);

            if (analysisTask.IsCompleted)
            {
                onSuccess(analysisTask.Result.Context);
            }
            else if (analysisTask.IsFaulted)
            {
                onFailure(analysisTask.Exception);
                logger.Error(analysisTask.Exception, "analysis error!");
            }
            else if (timeLimit.IsCompleted)
            {
                onTimeout();
                logger.Error("timeout! analysis did not finish within {0}ms", timeLimitInMs);
            }
        }

        private static System.Threading.Tasks.Task<ContextAnalysisResult> AnalyseAsyncInternal(ITreeNode node,
            ILogger logger,
            CancellationToken token)
        {
            return Task.Factory.StartNew(
                () =>
                {
                    var analysis = new ContextAnalysisInternal(logger, token);
                    ContextAnalysisResult result = null;
                    ReadLockCookie.Execute(() => { result = analysis.AnalyzeInternal(node); });
                    return result;
                });
        }

        private static System.Threading.Tasks.Task<ContextAnalysisResult> AnalyseAsyncWithCache(ITreeNode node,
            ILogger logger)
        {
            lock (Lock)
            {
                var hashCode = node.GetHashCode();

                System.Threading.Tasks.Task<ContextAnalysisResult> task;
                if (!CurrentTasks.TryGetValue(hashCode, out task))
                {
                    var token = TokenSource.CancelAndCreate();
                    task = AnalyseAsyncInternal(node, logger, token);
                    CurrentTasks.TryAdd(hashCode, task);

                    task.ContinueWith(RemoveOldContextAnalysisResultAfterTimeout(hashCode, 30000));
                }

                return task;
            }
        }

        private static Action<System.Threading.Tasks.Task<ContextAnalysisResult>>
            RemoveOldContextAnalysisResultAfterTimeout(int hashCode, int timeout)
        {
            return _ =>
            {
                Thread.Sleep(timeout);
                System.Threading.Tasks.Task<ContextAnalysisResult> t;
                CurrentTasks.TryRemove(hashCode, out t);
            };
        }

        public static ITreeNode FindEntryNode(Func<ITreeNode> getTreeNode)
        {
            lock (Lock)
            {
                ITreeNode node = null;
                ReadLockCookie.Execute(
                    () =>
                    {
                        node = getTreeNode();

                        if (node == null)
                        {
                            return;
                        }

                        if (!HasSourroundingMethod(node))
                        {
                            node = FindSourroundingClassDeclaration(node);
                        }
                    });
                return node;
            }
        }

        private static bool HasSourroundingMethod(ITreeNode node)
        {
            var method = node.GetContainingNode<IMethodDeclaration>(true);
            return method != null;
        }

        private static IClassDeclaration FindSourroundingClassDeclaration(ITreeNode psiFile)
        {
            return psiFile.GetContainingNode<IClassDeclaration>(true);
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