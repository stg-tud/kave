/*
 * Copyright 2017 Sebastian Proksch
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
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using KaVE.Commons.Model.Events.CompletionEvents;
using KaVE.Commons.Model.Naming.CodeElements;
using KaVE.Commons.Model.Naming.Impl.v0;
using KaVE.Commons.Model.Naming.Types;
using KaVE.Commons.Model.SSTs;
using KaVE.Commons.Model.SSTs.Declarations;
using KaVE.Commons.Model.SSTs.Expressions.Assignable;
using KaVE.Commons.Model.SSTs.Impl.Visitor;
using KaVE.Commons.Model.TypeShapes;
using KaVE.Commons.Utils.Assertion;
using KaVE.Commons.Utils.IO.Archives;
using KaVE.FeedbackProcessor.Preprocessing.Model;

namespace KaVE.FeedbackProcessor.StatisticsUltimate
{
    internal class EmDebug : StatisticsRunnerBase
    {
        private readonly IPreprocessingIo _io;
        private readonly IStatisticsLogger _log;


        public EmDebug(IPreprocessingIo io, IStatisticsLogger log, int numProcs)
            : base(io, log, numProcs)
        {
            _io = io;
            _log = log;
        }

        [SuppressMessage("ReSharper", "LocalizableElement")]
        public void Run()
        {
            FindZips();
            InParallel(CreateStatistics);

            Console.WriteLine("\n\nunique types: {0}", typesAll.Keys.Count);
            Console.WriteLine("unique seen methods: {0}", seenMethods.Keys.Count);
            Console.WriteLine("unique first: {0}", firstAll.Keys.Count);
            Console.WriteLine("unique super: {0}", superAll.Keys.Count);
            Console.WriteLine("unique elem: {0}", elemAll.Keys.Count);
            Console.WriteLine("unique inv: {0}", invAll.Keys.Count);

            var esFile = _io.GetFullPath_In("ctxElem.txt");
            Console.WriteLine("\ngenerating {0} ({1})", esFile, DateTime.Now);

            var sb = new StringBuilder();
            sb.AppendLine("Unique Elements:");
            foreach (var m in elemAll.Keys)
            {
                sb.AppendLine(m.ToString());
            }

            File.WriteAllText(esFile, sb.ToString());
        }

        private readonly ConcurrentDictionary<ITypeName, byte> typesAll = new ConcurrentDictionary<ITypeName, byte>();

        private readonly ConcurrentDictionary<IMethodName, byte> seenMethods =
            new ConcurrentDictionary<IMethodName, byte>();

        private readonly ConcurrentDictionary<IMethodName, byte> firstAll =
            new ConcurrentDictionary<IMethodName, byte>();

        private readonly ConcurrentDictionary<IMethodName, byte> superAll =
            new ConcurrentDictionary<IMethodName, byte>();

        private readonly ConcurrentDictionary<IMethodName, byte> elemAll =
            new ConcurrentDictionary<IMethodName, byte>();

        private readonly ConcurrentDictionary<IMethodName, byte> invAll = new ConcurrentDictionary<IMethodName, byte>();


        private void CreateStatistics(int taskId)
        {
            string zip;
            while (GetNextZip(out zip))
            {
                _log.CreatingStats(taskId, zip);
                var file = _io.GetFullPath_In(zip);
                using (var ra = new ReadingArchive(file))
                {
                    var ctxs = ra.GetAllLazy<Context>();
                    foreach (var ctx in ctxs)
                    {
                        ctx.SST.Accept(
                            new EmDebugVisitor(typesAll, seenMethods, firstAll, superAll, elemAll, invAll),
                            ctx.TypeShape);
                    }
                }
            }

            _log.FinishedStatCreation(taskId);
        }
    }

    internal class EmDebugVisitor : AbstractNodeVisitor<ITypeShape>
    {
        private readonly ConcurrentDictionary<IMethodName, byte> _seenElem;

        private readonly ConcurrentDictionary<ITypeName, byte> _types;
        private readonly ConcurrentDictionary<IMethodName, byte> _first;
        private readonly ConcurrentDictionary<IMethodName, byte> _super;
        private readonly ConcurrentDictionary<IMethodName, byte> _elem;
        private readonly ConcurrentDictionary<IMethodName, byte> _inv;

        public EmDebugVisitor(ConcurrentDictionary<ITypeName, byte> types,
            ConcurrentDictionary<IMethodName, byte> seenElem,
            ConcurrentDictionary<IMethodName, byte> first,
            ConcurrentDictionary<IMethodName, byte> super,
            ConcurrentDictionary<IMethodName, byte> elem,
            ConcurrentDictionary<IMethodName, byte> inv)
        {
            _types = types;
            _seenElem = seenElem;
            _first = first;
            _super = super;
            _elem = elem;
            _inv = inv;
        }

        public override void Visit(ISST sst, ITypeShape context)
        {
            if (IsGenerated(sst))
            {
                return;
            }

            _types[sst.EnclosingType] = 1;

            base.Visit(sst, context);
        }

        private static bool IsGenerated(ISST td)
        {
            var partId = td.PartialClassIdentifier;
            return td.IsPartialClass && partId != null
                   && (partId.Contains(".Designer") || partId.Contains(".designer"));
        }

        private IMethodName _ctxElem;
        private IMethodName _ctxFirst;
        private IMethodName _ctxSuper;

        public override void Visit(IMethodDeclaration decl, ITypeShape context)
        {
            var m = decl.Name;
            if (!_seenElem.TryAdd(m.RemoveGenerics(), 1))
            {
                return;
            }

            _ctxElem = decl.Name.RemoveGenerics();
            Asserts.NotNull(_ctxElem);

            foreach (var h in context.MethodHierarchies)
            {
                if (h.Element.Equals(m))
                {
                    _ctxFirst = h.First == null ? null : h.First.RemoveGenerics();
                    _ctxSuper = h.Super == null ? null : h.Super.RemoveGenerics();
                }
            }

            base.Visit(decl, context);
        }

        public override void Visit(IInvocationExpression inv, ITypeShape context)
        {
            var m = inv.MethodName.RemoveGenerics();
            if (ShouldInclude(m))
            {
                _inv.TryAdd(m, 1);
                AddEnclosingMethodIfAvailable();
            }
        }

        private static bool ShouldInclude(IMethodName name)
        {
            if (name.IsUnknown)
            {
                return false;
            }
            return !name.DeclaringType.Assembly.IsLocalProject;
        }

        private void AddEnclosingMethodIfAvailable()
        {
            if (_ctxElem != null)
            {
                _elem.TryAdd(_ctxElem, 1);
                _ctxElem = null;
            }
            if (_ctxFirst != null)
            {
                _first.TryAdd(_ctxFirst, 1);
                _ctxFirst = null;
            }
            if (_ctxSuper != null)
            {
                _super.TryAdd(_ctxSuper, 1);
                _ctxSuper = null;
            }
        }

        public override void Visit(ILambdaExpression inv, ITypeShape context)
        {
            // stop here for now!
        }
    }
}