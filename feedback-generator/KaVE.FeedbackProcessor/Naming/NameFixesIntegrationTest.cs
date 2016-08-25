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
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using KaVE.Commons.Model.Events.CompletionEvents;
using KaVE.Commons.Model.Naming;
using KaVE.Commons.Model.Naming.Impl.v0;
using KaVE.Commons.Model.SSTs.Declarations;
using KaVE.Commons.Model.SSTs.Expressions.Assignable;
using KaVE.Commons.Model.SSTs.Impl.Visitor;
using KaVE.Commons.Model.SSTs.Statements;
using KaVE.Commons.Utils.IO.Archives;

namespace KaVE.FeedbackProcessor.Naming
{
    // this app serves as a debugger to find valid ids that are broken by the fixes.
    // It searches for cases for which "id != fix(id)", which should not be the case for any context
    public class NameFixesIntegrationTest
    {
        private readonly int _numWorkers;
        private readonly string _dirContexts;

        private ConcurrentBag<string> _zips;

        public NameFixesIntegrationTest(int numWorkers, string dirContexts)
        {
            _numWorkers = numWorkers;
            _dirContexts = dirContexts;
        }

        public void TryToNameFixSomeNamesFromContexts()
        {
            var zips = FindZips();
            _zips = new ConcurrentBag<string>(zips);


            var tasks = new Task[_numWorkers];
            for (var i = 0; i < tasks.Length; i ++)
            {
                var taskId = i;
                tasks[i] = Task.Factory.StartNew(() => Run(taskId));
            }
            Task.WaitAll(tasks);
        }

        private IEnumerable<string> FindZips()
        {
            return Directory.EnumerateFiles(_dirContexts, "*.zip", SearchOption.AllDirectories);
        }

        private void Run(int taskId)
        {
            string zip;
            while (_zips.TryTake(out zip))
            {
                Console.WriteLine();
                Console.WriteLine(@"({0}) Next Zip", taskId);
                using (var ra = new ReadingArchive(zip))
                {
                    while (ra.HasNext())
                    {
                        var context = ra.GetNext<Context>();
                        Console.Write('.');
                        context.SST.Accept(new NameFixTester(), -1);
                    }
                }
            }
        }

        private static void Test(IName name, string prefix)
        {
            var id = name.Identifier;

            var fixed1 = id.FixIdentifiers();
            if (!id.Equals(fixed1))
            {
                Console.WriteLine(@"change ({0}, without prefix):", name.GetType().Name);
                Console.WriteLine(@"    before: " + id);
                Console.WriteLine(@"    after:  " + fixed1);
            }

            var fixed2 = id.FixIdentifiers(prefix);
            if (!id.Equals(fixed2))
            {
                Console.WriteLine(@"change ({0}, with prefix {1}):", name.GetType().Name, prefix);
                Console.WriteLine(@"    before: " + id);
                Console.WriteLine(@"    after:  " + fixed2);
            }
        }

        private class NameFixTester : AbstractNodeVisitor<int>
        {
            public override void Visit(IDelegateDeclaration stmt, int context)
            {
                Test(stmt.Name, "0F");
                Test(stmt.Name.ReturnType, "0T");
                Test(stmt.Name.DelegateType, "0T");
                if (stmt.Name.IsNestedType)
                {
                    Test(stmt.Name.DeclaringType, "0T");
                }
                foreach (var param in stmt.Name.Parameters)
                {
                    Test(param.ValueType, "0T");
                }
            }

            public override void Visit(IEventDeclaration stmt, int context)
            {
                Test(stmt.Name, "0F");
                Test(stmt.Name.ValueType, "0T");
                Test(stmt.Name.DeclaringType, "0T");
            }

            public override void Visit(IFieldDeclaration stmt, int context)
            {
                Test(stmt.Name, "0F");
                Test(stmt.Name.ValueType, "0T");
                Test(stmt.Name.DeclaringType, "0T");
            }

            public override void Visit(IPropertyDeclaration stmt, int context)
            {
                Test(stmt.Name, "0F");
                Test(stmt.Name.ValueType, "0T");
                Test(stmt.Name.DeclaringType, "0T");
            }

            public override void Visit(IMethodDeclaration stmt, int context)
            {
                Test(stmt.Name, "0F");
                Test(stmt.Name.ValueType, "0T");
                Test(stmt.Name.DeclaringType, "0T");
                foreach (var param in stmt.Name.Parameters)
                {
                    Test(param.ValueType, "0T");
                }
                base.Visit(stmt, context);
            }

            public override void Visit(IVariableDeclaration stmt, int context)
            {
                Test(stmt.Type, "0T");
            }

            public override void Visit(IInvocationExpression expr, int context)
            {
                Test(expr.MethodName, "0M");
            }

            public override void Visit(ILambdaExpression expr, int context)
            {
                Test(expr.Name, "0L");
            }
        }
    }
}