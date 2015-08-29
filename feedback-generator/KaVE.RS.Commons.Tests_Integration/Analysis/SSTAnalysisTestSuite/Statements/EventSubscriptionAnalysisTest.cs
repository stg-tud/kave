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

using KaVE.Commons.Model.SSTs.Impl.Expressions.Assignable;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Simple;
using KaVE.Commons.Model.SSTs.Impl.Statements;
using NUnit.Framework;
using Fix = KaVE.RS.Commons.Tests_Integration.Analysis.SSTAnalysisTestSuite.SSTAnalysisFixture;

namespace KaVE.RS.Commons.Tests_Integration.Analysis.SSTAnalysisTestSuite.Statements
{
    internal class EventSubscriptionAnalysisTest : BaseSSTAnalysisTest
    {
        [Test]
        public void Event_AddingListener_Lambda()
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
                    Expression = new UnknownExpression() // TODO: LambdaExpression
                },
                ExprStmt(new CompletionExpression()));
        }

        [Test]
        public void Event_AddingListener_DefaultDelegate()
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
        public void Event_AddingListener_Method()
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

            AssertBody(
                "M",
                new EventSubscriptionStatement
                {
                    Reference = EventRef("E", Fix.ActionOfInt),
                    Expression = new UnknownExpression() // TODO: RefExpr(MethodReference)
                },
                ExprStmt(new CompletionExpression()));
        }

        [Test]
        public void Event_AddingListener_NewHandler()
        {
            CompleteInClass(@"
                private event Action<int> E;
                public void M()
                {
                    E += new object(); // TODO adapt
                    $
                }
            ");

            Assert.Fail();
        }

        [Test]
        public void Event_RemovingListener_Lambda()
        {
            CompleteInClass(@"
                private event Action<int> E;
                public void M()
                {
                    E -= i => { };
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
        public void Event_RemovingListener_DefaultDelegate()
        {
            CompleteInClass(@"
                private event Action<int> E;
                public void M()
                {
                    E -= delegate { };
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
        public void Event_RemovingListener_Method()
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

            AssertBody(
                "M",
                new EventSubscriptionStatement
                {
                    Reference = EventRef("E", Fix.ActionOfInt),
                    Expression = new UnknownExpression() // TODO: RefExpr(MethodReference)
                },
                ExprStmt(new CompletionExpression()));
        }

        [Test]
        public void Event_RemovingListener_NewHandler()
        {
            CompleteInClass(@"
                private event Action<int> E;
                public void M()
                {
                    E -= new object(); // TODO adapt
                    $
                }
            ");

            Assert.Fail();
        }

        [Test]
        public void LeftSide_ImpliciteThis()
        {
            CompleteInClass(@"
                private event Action<int> E;
                public void M()
                {
                    E -= new object(); // TODO adapt
                    $
                }
            ");

            Assert.Fail();
        }

        [Test]
        public void LeftSide_ExpliciteThis()
        {
            CompleteInClass(@"
                private event Action<int> E;
                public void M()
                {
                    E -= new object(); // TODO adapt
                    $
                }
            ");

            Assert.Fail();
        }

        [Test]
        public void LeftSide_Member()
        {
            // TODO separate cases for field, props, etc.?
            CompleteInClass(@"
                private event Action<int> E;
                public void M()
                {
                    E -= new object(); // TODO adapt
                    $
                }
            ");

            Assert.Fail();
        }

        [Test]
        public void LeftSide_Variable()
        {
            CompleteInClass(@"
                private event Action<int> E;
                public void M()
                {
                    var e = E;
                    e += delegate{};
                    $
                }
            ");

            Assert.Fail();
        }
    }
}