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
 */

using JetBrains.ReSharper.Psi.CSharp.Tree;
using KaVE.Commons.Model.SSTs.Impl.Blocks;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Simple;
using KaVE.VsFeedbackGenerator.Analysis.CompletionTarget;
using NUnit.Framework;
using Fix = KaVE.VsFeedbackGenerator.RS8Tests.Analysis.SSTAnalysisTestSuite.SSTAnalysisFixture;

namespace KaVE.VsFeedbackGenerator.RS8Tests.Analysis.SSTAnalysisTestSuite.Blocks
{
    internal class ForEachLoopAnalysisTest : BaseSSTAnalysisTest
    {
        [Test]
        public void Completion_Before()
        {
            CompleteInMethod(@"
                $
                foreach (var n in null) { } // invalid code
            ");

            AssertCompletionMarker<IForeachStatement>(CompletionCase.EmptyCompletionBefore);

            AssertBody(
                Fix.EmptyCompletion,
                // TODO @seb: think about the "Object" types for erroneous code... better use "Unknown"?
                VarDecl("$0", Fix.Object),
                Assign("$0", new NullExpression()),
                new ForEachLoop
                {
                    Declaration = VarDecl("n", Fix.Object),
                    LoopedReference = VarRef("$0")
                });
        }

        [Test]
        public void Completion_After()
        {
            CompleteInMethod(@"
                foreach (var n in null) { } // invalid code
                $
            ");

            AssertCompletionMarker<IForeachStatement>(CompletionCase.EmptyCompletionAfter);

            AssertBody(
                VarDecl("$0", Fix.Object),
                Assign("$0", new NullExpression()),
                new ForEachLoop
                {
                    Declaration = VarDecl("n", Fix.Object),
                    LoopedReference = VarRef("$0")
                },
                Fix.EmptyCompletion);
        }

        [Test]
        public void Completion_InEmptyBody()
        {
            CompleteInMethod(@"
                foreach (var n in null) { // invalid code
                    $
                }
            ");

            AssertCompletionMarker<IForeachStatement>(CompletionCase.InBody);

            AssertBody(
                VarDecl("$0", Fix.Object),
                Assign("$0", new NullExpression()),
                new ForEachLoop
                {
                    Declaration = VarDecl("n", Fix.Object),
                    LoopedReference = VarRef("$0"),
                    Body =
                    {
                        Fix.EmptyCompletion
                    }
                });
        }

        [Test]
        public void HappyPath()
        {
            CompleteInMethod(@"
                var ns = new List<int>();
                foreach (var n in ns) {
                    n.GetHashCode();
                    $
                }
            ");

            AssertBody(
                VarDecl("ns", Fix.ListOfInt),
                Assign("ns", InvokeCtor(Fix.ListOfInt_Init)),
                new ForEachLoop
                {
                    Declaration = VarDecl("n", Fix.Int),
                    LoopedReference = VarRef("ns"),
                    Body =
                    {
                        InvokeStmt("n", Fix.Int_GetHashCode),
                        Fix.EmptyCompletion,
                    }
                });
        }

        [Test]
        public void InlineDefinitionOfEnumerable()
        {
            CompleteInMethod(@"
                foreach (var n in new List<int>()) {
                    n.GetHashCode();
                    $
                }
            ");

            AssertBody(
                VarDecl("$0", Fix.ListOfInt),
                Assign("$0", InvokeCtor(Fix.ListOfInt_Init)),
                new ForEachLoop
                {
                    Declaration = VarDecl("n", Fix.Int),
                    LoopedReference = VarRef("$0"),
                    Body =
                    {
                        InvokeStmt("n", Fix.Int_GetHashCode),
                        Fix.EmptyCompletion,
                    }
                });
        }
    }
}