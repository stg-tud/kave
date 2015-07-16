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

using KaVE.Commons.Model.Names.CSharp;
using KaVE.Commons.Model.SSTs.Blocks;
using KaVE.Commons.Model.SSTs.Impl.Blocks;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Assignable;
using NUnit.Framework;
using Fix = KaVE.RS.Commons.Tests_Integration.Analysis.SSTAnalysisTestSuite.SSTAnalysisFixture;

namespace KaVE.RS.Commons.Tests_Integration.Analysis.SSTAnalysisTestSuite.Blocks
{
    internal class TryBlockAnalysisTest : BaseSSTAnalysisTest
    {
        [Test]
        public void BasicCase_TryCatch()
        {
            CompleteInMethod(@"
                try {}
                catch {}
                $
            ");
            AssertBody(
                new TryBlock
                {
                    CatchBlocks =
                    {
                        new CatchBlock
                        {
                            Kind = CatchBlockKind.General
                        }
                    }
                },
                ExprStmt(new CompletionExpression()));
        }

        [Test]
        public void BasicCase_TryFinally()
        {
            CompleteInMethod(@"
                try {}
                finally {}
                $
            ");
            AssertBody(
                new TryBlock(),
                ExprStmt(new CompletionExpression()));
        }

        [Test]
        public void ExtendedCase()
        {
            CompleteInMethod(@"
                try {
                    GetHashCode();
                }
                catch(Exception e) {
                    Equals(e);
                }
                finally {
                    GetType();
                }
                $
            ");
            AssertBody(
                new TryBlock
                {
                    Body = {InvokeStmt("this", Fix.Object_GetHashCode)},
                    CatchBlocks =
                    {
                        new CatchBlock
                        {
                            Parameter = ParameterName.Get(string.Format("[{0}] e", Fix.Exception)),
                            Body =
                            {
                                InvokeStmt("this", Fix.Object_Equals, RefExpr("e"))
                            }
                        }
                    },
                    Finally =
                    {
                        InvokeStmt("this", Fix.Object_GetType)
                    }
                },
                ExprStmt(new CompletionExpression()));
        }

        [Test]
        public void RegularCatch()
        {
            CompleteInMethod(@"
                try{}
                catch(Exception e) {}
                $
            ");
            AssertBody(
                new TryBlock
                {
                    CatchBlocks =
                    {
                        new CatchBlock
                        {
                            Parameter = ParameterName.Get(string.Format("[{0}] e", Fix.Exception))
                        }
                    }
                },
                ExprStmt(new CompletionExpression()));
        }

        [Test]
        public void UnnamedCatch()
        {
            CompleteInMethod(@"
                try{}
                catch(Exception) {}
                $
            ");
            AssertBody(
                new TryBlock
                {
                    CatchBlocks =
                    {
                        new CatchBlock
                        {
                            Kind = CatchBlockKind.Unnamed,
                            Parameter = ParameterName.Get(string.Format("[{0}] ?", Fix.Exception))
                        }
                    }
                },
                ExprStmt(new CompletionExpression()));
        }

        [Test]
        public void GeneralCatch()
        {
            CompleteInMethod(@"
                try{}
                catch {}
                $
            ");
            AssertBody(
                new TryBlock
                {
                    CatchBlocks =
                    {
                        new CatchBlock
                        {
                            Kind = CatchBlockKind.General,
                            Parameter = ParameterName.UnknownName
                        }
                    }
                },
                ExprStmt(new CompletionExpression()));
        }

        [Test]
        public void TriggerBefore()
        {
            CompleteInMethod(@"
                $
                try {}
                finally {}
            ");
            AssertBody(
                ExprStmt(new CompletionExpression()),
                new TryBlock());
        }

        [Test]
        public void TriggerInBody()
        {
            CompleteInMethod(@"
                try {
                    $
                }
                finally {}
            ");
            AssertBody(
                new TryBlock
                {
                    Body =
                    {
                        ExprStmt(new CompletionExpression())
                    }
                });
        }

        [Test]
        public void TriggerInCatch()
        {
            CompleteInMethod(@"
                try {}
                catch {
                    $
                }
                finally {}
            ");
            AssertBody(
                new TryBlock
                {
                    CatchBlocks =
                    {
                        new CatchBlock
                        {
                            Kind = CatchBlockKind.General,
                            Body =
                            {
                                ExprStmt(new CompletionExpression())
                            }
                        }
                    }
                });
        }

        [Test]
        public void TriggerInFinally()
        {
            CompleteInMethod(@"
                try {}
                finally {
                    $
                }
            ");
            AssertBody(
                new TryBlock
                {
                    Finally =
                    {
                        ExprStmt(new CompletionExpression())
                    }
                });
        }

        [Test]
        public void TriggerAfter()
        {
            CompleteInMethod(@"
                try {}
                finally {}
                $
            ");
            AssertBody(
                new TryBlock(),
                ExprStmt(new CompletionExpression()));
        }
    }
}