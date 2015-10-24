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
using KaVE.Commons.Model.Names.CSharp;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Assignable;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Simple;
using KaVE.Commons.Model.SSTs.Impl.Statements;
using KaVE.Commons.Model.SSTs.Statements;
using NUnit.Framework;
using Fix = KaVE.RS.Commons.Tests_Integration.Analysis.SSTAnalysisTestSuite.SSTAnalysisFixture;

namespace KaVE.RS.Commons.Tests_Integration.Analysis.SSTAnalysisTestSuite.Statements
{
    internal class EventSubscriptionAnalysisTest : BaseSSTAnalysisTest
    {
        [Test]
        public void Operation_Adding()
        {
            CompleteInClass(@"
                private event Action<int> E;
                public void M()
                {
                    E += Listener;
                    $
                }
                private void Listener(int i) {}
            ");

            var listenerName =
                MethodName.Get(string.Format("[{0}] [{1}].Listener([{2}] i)", Fix.Void, Fix.TestClass, Fix.Int));

            AssertBody(
                "M",
                new EventSubscriptionStatement
                {
                    Reference = EventRef("E", Fix.ActionOfInt),
                    Operation = EventSubscriptionOperation.Add,
                    Expression = RefExpr(MethodRef(listenerName, VarRef("this")))
                },
                ExprStmt(new CompletionExpression()));
        }

        [Test]
        public void Operation_Removing()
        {
            CompleteInClass(@"
                private event Action<int> E;
                public void M()
                {
                    E -= Listener;
                    $
                }
                private void Listener(int i) {}
            ");

            var listenerName =
                MethodName.Get(string.Format("[{0}] [{1}].Listener([{2}] i)", Fix.Void, Fix.TestClass, Fix.Int));

            AssertBody(
                "M",
                new EventSubscriptionStatement
                {
                    Reference = EventRef("E", Fix.ActionOfInt),
                    Operation = EventSubscriptionOperation.Remove,
                    Expression = RefExpr(MethodRef(listenerName, VarRef("this")))
                },
                ExprStmt(new CompletionExpression()));
        }

        [Test]
        public void Right_Lambda()
        {
            CompleteInClass(@"
                private event Action<int> E;
                public void M()
                {
                    E += i => { };
                    $
                }
            ");

            AssertBody(
                new EventSubscriptionStatement
                {
                    Reference = EventRef("E", Fix.ActionOfInt),
                    Expression = new LambdaExpression
                    {
                        Name = LambdaName.Get(string.Format("[{0}] ([{1}] i)", Fix.Void, Fix.Int))
                    }
                },
                ExprStmt(new CompletionExpression()));
        }

        [Test]
        public void Right_DefaultDelegate()
        {
            CompleteInClass(@"
                private event Action<int> E;
                public void M()
                {
                    E += delegate { };
                    $
                }
            ");

            AssertBody(
                new EventSubscriptionStatement
                {
                    Reference = EventRef("E", Fix.ActionOfInt),
                    Expression = new UnknownExpression() // TODO: LambdaExpression
                },
                ExprStmt(new CompletionExpression()));
        }

        [Test]
        public void Right_Method()
        {
            CompleteInClass(@"
                private event Action<int> E;
                public void M()
                {
                    E += Listener;
                    $
                }
                private void Listener(int i) {}
            ");

            var listenerName =
                MethodName.Get(string.Format("[{0}] [{1}].Listener([{2}] i)", Fix.Void, Fix.TestClass, Fix.Int));

            AssertBody(
                "M",
                new EventSubscriptionStatement
                {
                    Reference = EventRef("E", Fix.ActionOfInt),
                    Operation = EventSubscriptionOperation.Add,
                    Expression = RefExpr(MethodRef(listenerName, VarRef("this")))
                },
                ExprStmt(new CompletionExpression()));
        }

        [Test]
        public void Right_NewHandler()
        {
            CompleteInClass(@"
                private event Handler E;
                private delegate void Handler(int i);
                private void Listener(int i) { }

                public void M()
                {
                    E += new Handler(Listener);
                    $
                }
            ");

            var delegateType =
                TypeName.Get(string.Format("d:[{0}] [N.C+Handler, TestProject].([{1}] i)", Fix.Void, Fix.Int));
            var parameter = ParameterName.Get(string.Format("[{0}] target", delegateType));
            var ctor = Fix.Ctor(delegateType, parameter);

            var listenerName =
                MethodName.Get(string.Format("[{0}] [{1}].Listener([{2}] i)", Fix.Void, Fix.TestClass, Fix.Int));

            AssertBody(
                "M",
                new EventSubscriptionStatement
                {
                    Reference = EventRef("E", delegateType),
                    Operation = EventSubscriptionOperation.Add,
                    Expression = InvokeCtor(ctor, RefExpr(MethodRef(listenerName, VarRef("this"))))
                },
                ExprStmt(new CompletionExpression()));
        }

        [Test]
        public void Left_ImpliciteThis()
        {
            CompleteInClass(@"
                private event Action<int> E;
                public void M()
                {
                    E += delegate{};
                    $
                }
            ");

            AssertBody(
                new EventSubscriptionStatement
                {
                    Reference = EventRef("E", Fix.ActionOfInt),
                    Operation = EventSubscriptionOperation.Add,
                    Expression = new UnknownExpression() // TODO
                },
                ExprStmt(new CompletionExpression()));
        }

        [Test]
        public void Left_ExpliciteThis()
        {
            CompleteInClass(@"
                private event Action<int> E;
                public void M()
                {
                    this.E += delegate{};
                    $
                }
            ");

            AssertBody(
                new EventSubscriptionStatement
                {
                    Reference = EventRef("E", Fix.ActionOfInt),
                    Operation = EventSubscriptionOperation.Add,
                    Expression = new UnknownExpression() // TODO
                },
                ExprStmt(new CompletionExpression()));
        }

        [Test]
        public void Left_Field()
        {
            CompleteInClass(@"
                private Action<int> _f;
                public void M(C c)
                {
                    c._f += delegate{};
                    $
                }
            ");

            AssertBody(
                new EventSubscriptionStatement
                {
                    Reference = FieldRef("_f", Fix.ActionOfInt, "c"),
                    Operation = EventSubscriptionOperation.Add,
                    Expression = new UnknownExpression() // TODO
                },
                ExprStmt(new CompletionExpression()));
        }

        [Test]
        public void Left_Property()
        {
            CompleteInClass(@"
                private Action<int> P {get;set;}
                public void M(C c)
                {
                    c.P += delegate{};
                    $
                }
            ");

            AssertBody(
                new EventSubscriptionStatement
                {
                    Reference = PropRef("P", Fix.ActionOfInt, "c"),
                    Operation = EventSubscriptionOperation.Add,
                    Expression = new UnknownExpression() // TODO
                },
                ExprStmt(new CompletionExpression()));
        }

        [Test]
        public void Left_Variable()
        {
            CompleteInMethod(@"
                Action<int> e = null;
                e += delegate{};
                $
            ");
            Action<int> e = null;
            e += delegate { };
            AssertBody(
                VarDecl("e", Fix.ActionOfInt),
                Assign("e", new NullExpression()),
                new EventSubscriptionStatement
                {
                    Reference = VarRef("e"),
                    Operation = EventSubscriptionOperation.Add,
                    Expression = new UnknownExpression() // TODO
                },
                ExprStmt(new CompletionExpression()));
        }
    }
}