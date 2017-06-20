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
using KaVE.Commons.Model.SSTs;
using KaVE.Commons.Model.SSTs.Declarations;
using KaVE.Commons.Model.SSTs.Impl.Declarations;
using KaVE.Commons.Model.SSTs.Impl.References;
using KaVE.Commons.Utils.Collections;
using NUnit.Framework;
using Fix = KaVE.RS.Commons.Tests_Integration.Analysis.SSTAnalysisTestSuite.SSTAnalysisFixture;

namespace KaVE.RS.Commons.Tests_Integration.Analysis.SSTAnalysisTestSuite.Declarations
{
    internal class MemberInitializationAnalysisTest : BaseSSTAnalysisTest
    {
        [Test]
        public void FieldInit()
        {
            CompleteInClass(@"
                public int _f = 1;
                $
            ");

            AssertAllMethods(
                Init(
                    Assign(
                        new FieldReference
                        {
                            Reference = VarRef("this"),
                            FieldName = Names.Field("[p:int] [N.C, TestProject]._f")
                        },
                        Const("1"))));
        }

        [Test]
        public void FieldInit_TwoFields()
        {
            CompleteInClass(@"
                public int _f1 = 1;
                public int _f2 = 2;
                $
            ");

            AssertAllMethods(
                Init(
                    Assign(
                        new FieldReference
                        {
                            Reference = VarRef("this"),
                            FieldName = Names.Field("[p:int] [N.C, TestProject]._f1")
                        },
                        Const("1")),
                    Assign(
                        new FieldReference
                        {
                            Reference = VarRef("this"),
                            FieldName = Names.Field("[p:int] [N.C, TestProject]._f2")
                        },
                        Const("2"))));
        }

        [Test]
        public void FieldInit_NamesDoNotClash()
        {
            CompleteInClass(@"
                public int _f1 = 1.GetHashCode();
                public int _f2 = 2.GetHashCode();
                $
            ");

            AssertAllMethods(
                Init(
                    VarDecl("$0", Fix.Int),
                    Assign("$0", Const("1")),
                    Assign(
                        new FieldReference
                        {
                            Reference = VarRef("this"),
                            FieldName = Names.Field("[p:int] [N.C, TestProject]._f1")
                        },
                        Invoke("$0", Fix.Int_GetHashCode)),
                    VarDecl("$1", Fix.Int),
                    Assign("$1", Const("2")),
                    Assign(
                        new FieldReference
                        {
                            Reference = VarRef("this"),
                            FieldName = Names.Field("[p:int] [N.C, TestProject]._f2")
                        },
                        Invoke("$1", Fix.Int_GetHashCode)))
                );
        }

        [Test]
        public void StaticFieldInit()
        {
            CompleteInClass(@"
                public static int _f = 1;
                $
            ");

            AssertAllMethods(
                ClassInit(
                    Assign(
                        new FieldReference
                        {
                            FieldName = Names.Field("static [p:int] [N.C, TestProject]._f")
                        },
                        Const("1"))));
        }

        [Test]
        public void CollectionInit()
        {
            // the different initialization ways should already be covered in other tests,
            // but just to make sure
            CompleteInClass(@"
                List<int> _f = new List<int> {0};
                $
            ");

            AssertAllMethods(
                Init(
                    VarDecl("$0", Fix.ListOfInt),
                    Assign("$0", InvokeCtor(Fix.ListOfInt_Init)),
                    InvokeStmt("$0", Fix.ListOfInt_Add, Const("0")),
                    Assign(
                        new FieldReference
                        {
                            Reference = VarRef("this"),
                            FieldName = Names.Field("[{0}] [N.C, TestProject]._f", Fix.ListOfInt)
                        },
                        RefExpr("$0"))));
        }

        [Test]
        public void EventInit()
        {
            CompleteInClass(@"
                public event Action E = null;
                $
            ");

            AssertAllMethods(
                Init(
                    Assign(
                        new EventReference
                        {
                            Reference = VarRef("this"),
                            EventName = Names.Event("[{0}] [N.C, TestProject].E", Fix.Action)
                        },
                        Const("null"))));
        }

        [Test]
        public void StaticEventInit()
        {
            CompleteInClass(@"
                public static event Action E = null;
                $
            ");

            AssertAllMethods(
                ClassInit(
                    Assign(
                        new EventReference
                        {
                            EventName = Names.Event("static [{0}] [N.C, TestProject].E", Fix.Action)
                        },
                        Const("null"))
                    ));
        }

        public static IMethodDeclaration Init(params IStatement[] stmts)
        {
            return new MethodDeclaration
            {
                Name = Names.Method("[p:void] [N.C, TestProject]..init()"),
                Body = Lists.NewList(stmts)
            };
        }

        public static IMethodDeclaration ClassInit(params IStatement[] stmts)
        {
            return new MethodDeclaration
            {
                Name = Names.Method("static [p:void] [N.C, TestProject]..cinit()"),
                Body = Lists.NewList(stmts)
            };
        }
    }
}