﻿/*
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

using KaVE.Commons.Model.SSTs.Impl.Expressions.Simple;
using KaVE.Commons.Model.SSTs.Impl.Statements;
using NUnit.Framework;
using Fix = KaVE.RS.Commons.Tests_Integration.Analysis.SSTAnalysisTestSuite.SSTAnalysisFixture;

namespace KaVE.RS.Commons.Tests_Integration.Analysis.SSTAnalysisTestSuite.Declarations
{
    internal class ConstructorDeclarationAnalysisTest : BaseSSTAnalysisTest
    {
        [Test]
        public void DefaultConstructorIsNotCaptured()
        {
            CompleteInNamespace(@"
                class C {
                    $
                }
            ");

            AssertAllMethods();
        }

        [Test]
        public void ExplicitDefinition()
        {
            CompleteInNamespace(@"
                class C {
                    public C() {}
                    $
                }
            ");

            AssertAllMethods(ConstructorDecl("N.C, TestProject"));
        }

        [Test]
        public void BodyIsAnalyzed()
        {
            CompleteInNamespace(@"
                class C {
                    public C() {
                        return;
                    }
                    $
                }
            ");

            var ctor = ConstructorDecl("N.C, TestProject");
            ctor.Body.Add(new ReturnStatement {IsVoid = true});
            AssertAllMethods(ctor);
        }

        [Test]
        public void ThisCall()
        {
            CompleteInNamespace(@"
                class C {
                    public C() : this(1) {}
                    public C(int i) {}
                    $
                }
            ");

            var ctor1 = ConstructorDecl("N.C, TestProject");
            ctor1.Body.Add(
                InvokeStmt("this", Constructor("N.C, TestProject", Param(Fix.Int, "i")), new ConstantValueExpression()));

            var ctor2 = ConstructorDecl("N.C, TestProject", Param(Fix.Int, "i"));

            AssertAllMethods(ctor1, ctor2);
        }

        [Test]
        public void BaseCall()
        {
            CompleteInNamespace(@"
                class S {
                    public S(int i) {}
                }
                class C : S {
                    public C() : base(1) {}
                    $
                }
            ");

            var ctor1 = ConstructorDecl("N.C, TestProject");
            ctor1.Body.Add(
                InvokeStmt("base", Constructor("N.S, TestProject", Param(Fix.Int, "i")), new ConstantValueExpression()));

            AssertAllMethods(ctor1);
        }

        [Test]
        public void RedefinedWithParameter()
        {
            CompleteInNamespace(@"
                class S {
                    public S(int i) {}
                }
                class C : S {
                    public C() : this(1) {}
                    public C(int i) : base(i) {}
                    $
                }
            ");

            var ctor1 = ConstructorDecl("N.C, TestProject");
            ctor1.Body.Add(
                InvokeStmt("this", Constructor("N.C, TestProject", Param(Fix.Int, "i")), new ConstantValueExpression()));

            var ctor2 = ConstructorDecl("N.C, TestProject", Param(Fix.Int, "i"));
            ctor2.Body.Add(InvokeStmt("base", Constructor("N.S, TestProject", Param(Fix.Int, "i")), RefExpr("i")));

            AssertAllMethods(ctor1, ctor2);
        }

        [Test]
        public void Invalid()
        {
            CompleteInNamespace(@"
                class S {
                    public S(int i) {}
                }
                class C : S {
                    public C() {}
                    $
                }
            ");

            AssertAllMethods(ConstructorDecl("N.C, TestProject"));
        }
    }
}