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

using KaVE.Commons.Model.Naming;
using KaVE.Commons.Model.SSTs.Impl.Declarations;
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
                InvokeStmt("this", Constructor("N.C, TestProject", Param(Fix.Int, "i")), Const("1")));

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
                InvokeStmt("base", Constructor("N.S, TestProject", Param(Fix.Int, "i")), Const("1")));

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
                InvokeStmt("this", Constructor("N.C, TestProject", Param(Fix.Int, "i")), Const("1")));

            var ctor2 = ConstructorDecl("N.C, TestProject", Param(Fix.Int, "i"));
            ctor2.Body.Add(InvokeStmt("base", Constructor("N.S, TestProject", Param(Fix.Int, "i")), RefExpr("i")));

            AssertAllMethods(ctor1, ctor2);
        }

        [Test]
        public void ComplexParameter()
        {
            CompleteInNamespace(@"
                class C
                {
                    C() : this(new object()) {}
                    C(object o) {}
                    $
                }
            ");

            var ctor1 = ConstructorDecl("N.C, TestProject");
            ctor1.Body.Add(
                VarDecl("$0", Fix.Object));
            ctor1.Body.Add(
                Assign("$0", InvokeCtor(Fix.Object_ctor)));
            ctor1.Body.Add(
                InvokeStmt("this", Constructor("N.C, TestProject", Param(Fix.Object, "o")), RefExpr("$0")));

            var ctor2 = ConstructorDecl("N.C, TestProject", Param(Fix.Object, "o"));

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

        [Test]
        public void ConstructorsOfNestedClasses()
        {
            CompleteInNamespace(@"
                class C
                {
                    public C() {}
                    $

                    class Nested
                    {
                        public Nested() {}
                    }
                }
            ");

            AssertAllMethods(ConstructorDecl("N.C, TestProject"));
        }

        [Test]
        public void StaticConstructorsAreCaptured()
        {
            CompleteInNamespace(@"
                class C
                {
                    static C() {}
                    $
                }
            ");

            AssertAllMethods(
                new MethodDeclaration
                {
                    Name = Names.Method("static [p:void] [N.C, TestProject]..cctor()"),
                    IsEntryPoint = true
                });
        }
    }
}