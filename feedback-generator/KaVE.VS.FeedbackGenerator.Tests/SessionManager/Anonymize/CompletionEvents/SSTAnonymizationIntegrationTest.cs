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
using KaVE.Commons.Model.SSTs.Expressions;
using KaVE.Commons.Model.SSTs.Impl;
using KaVE.Commons.Model.SSTs.Impl.Blocks;
using KaVE.Commons.Model.SSTs.Impl.Declarations;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Assignable;
using KaVE.Commons.Model.SSTs.Impl.Expressions.LoopHeader;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Simple;
using KaVE.Commons.Model.SSTs.Impl.References;
using KaVE.Commons.Model.SSTs.Impl.Statements;
using KaVE.VS.FeedbackGenerator.SessionManager.Anonymize.CompletionEvents;
using NUnit.Framework;

namespace KaVE.VS.FeedbackGenerator.Tests.SessionManager.Anonymize.CompletionEvents
{
    internal class SSTAnonymizationIntegrationTest
    {
        private SSTAnonymization _sut;

        [SetUp]
        public void Setup()
        {
            var refAnon = new SSTReferenceAnonymization();
            var exprAnon = new SSTExpressionAnonymization(refAnon);
            var stmtAnon = new SSTStatementAnonymization(exprAnon, refAnon);
            _sut = new SSTAnonymization(stmtAnon);
        }

        [Test]
        public void Tets()
        {
            var orig = CreateSST();
            var anon = _sut.Anonymize(orig);
            // the test is obviously meaningless, until something very serious is broken
            // the main goal here is to ensure that Equals/HashCode do not crash
            Assert.AreNotEqual(orig, anon);
            Assert.AreNotEqual(orig.GetHashCode(), anon.GetHashCode());
        }

        private static SST CreateSST()
        {
            return new SST
            {
                EnclosingType = Names.Type("T,P"),
                Delegates =
                {
                    new DelegateDeclaration()
                },
                Events =
                {
                    new EventDeclaration()
                },
                Fields =
                {
                    new FieldDeclaration()
                },
                Methods =
                {
                    CreateMethod()
                },
                Properties =
                {
                    new PropertyDeclaration()
                }
            };
        }

        private static IMethodDeclaration CreateMethod()
        {
            var md = new MethodDeclaration
            {
                Body =
                {
                    // blocks
                    new DoLoop(),
                    new ForEachLoop(),
                    new ForLoop(),
                    new IfElseBlock(),
                    new LockBlock(),
                    new SwitchBlock
                    {
                        Sections =
                        {
                            new CaseBlock()
                        }
                    },
                    new TryBlock
                    {
                        CatchBlocks =
                        {
                            new CatchBlock()
                        }
                    },
                    new UncheckedBlock(),
                    new UnsafeBlock(),
                    new UsingBlock(),
                    new WhileLoop(),

                    // statements
                    new Assignment(),
                    new BreakStatement(),
                    new ContinueStatement(),
                    new EventSubscriptionStatement(),
                    new ExpressionStatement(),
                    new GotoStatement(),
                    new LabelledStatement(),
                    new ReturnStatement(),
                    new ThrowStatement(),
                    new UnknownStatement(),
                    new VariableDeclaration(),

                    // expressions -assignable
                    Expr(new BinaryExpression()),
                    Expr(new CastExpression()),
                    Expr(new CompletionExpression()),
                    Expr(new ComposedExpression()),
                    Expr(new IfElseExpression()),
                    Expr(new IndexAccessExpression()),
                    Expr(new InvocationExpression()),
                    Expr(new LambdaExpression()),
                    Expr(new TypeCheckExpression()),
                    Expr(new UnaryExpression()),
                    // expressions -loop header
                    Expr(new LoopHeaderBlockExpression()),
                    // expressions -loop header
                    Expr(new ConstantValueExpression()),
                    Expr(new NullExpression()),
                    Expr(new ReferenceExpression()),
                    Expr(new UnknownExpression()),

                    // references
                    Ref(new EventReference()),
                    Ref(new FieldReference()),
                    Ref(new IndexAccessReference()),
                    Ref(new MethodReference()),
                    Ref(new PropertyReference()),
                    Ref(new UnknownReference()),
                    Ref(new VariableReference())
                }
            };

            return md;
        }

        private static IStatement Expr(IExpression expr)
        {
            var assignExpr = expr as IAssignableExpression;
            if (assignExpr != null)
            {
                return new ExpressionStatement
                {
                    Expression = assignExpr
                };
            }
            var lhExpr = expr as ILoopHeaderExpression;
            if (lhExpr != null)
            {
                return new WhileLoop {Condition = lhExpr};
            }
            Assert.Fail("unhandled case");
            return null;
        }

        private static IStatement Ref(IReference reference)
        {
            return Expr(
                new ReferenceExpression
                {
                    Reference = reference
                });
        }
    }
}