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

using KaVE.Commons.Model.SSTs;
using KaVE.Commons.Model.SSTs.Declarations;
using KaVE.Commons.Model.SSTs.Expressions;
using KaVE.Commons.Model.SSTs.Expressions.Assignable;
using KaVE.Commons.Model.SSTs.Impl;
using KaVE.Commons.Model.SSTs.Impl.Blocks;
using KaVE.Commons.Model.SSTs.Impl.Declarations;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Assignable;
using KaVE.Commons.Model.SSTs.Impl.Expressions.LoopHeader;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Simple;
using KaVE.Commons.Model.SSTs.Impl.References;
using KaVE.Commons.Model.SSTs.Impl.Statements;
using KaVE.Commons.Model.SSTs.Impl.Visitor;
using KaVE.Commons.Model.SSTs.References;
using KaVE.Commons.Model.SSTs.Statements;
using KaVE.Commons.Model.SSTs.Visitor;
using Moq;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Model.SSTs.Impl.Visitor.AbstractNodeVisitorTestSuite
{
    internal class Context
    {
        #region test setup

        private const int DefaultArgument = 42;

        private IVariableReference _vr;
        private ISimpleExpression _e1;
        private ISimpleExpression _e2;
        private ISimpleExpression _e3;
        private IStatement _s1;
        private IStatement _s2;
        private IStatement _s3;

        private TestNodeVisitor _sut;

        [SetUp]
        public void SetUp()
        {
            _vr = Mock.Of<IVariableReference>();
            _e1 = Mock.Of<ISimpleExpression>();
            _e2 = Mock.Of<ISimpleExpression>();
            _e3 = Mock.Of<ISimpleExpression>();
            _s1 = Mock.Of<IStatement>();
            _s2 = Mock.Of<IStatement>();
            _s3 = Mock.Of<IStatement>();
            _sut = new TestNodeVisitor();
        }

        private void Visit(ISSTNode node)
        {
            node.Accept(_sut, DefaultArgument);
        }

        private void AssertAccept(ISSTNode node)
        {
            Mock.Get(node).Verify(n => n.Accept(_sut, DefaultArgument));
        }

        internal class TestNodeVisitor : AbstractNodeVisitor<int> {}

        #endregion

        #region declarations

        [Test]
        public void SST()
        {
            var d = Mock.Of<IDelegateDeclaration>();
            var e = Mock.Of<IEventDeclaration>();
            var f = Mock.Of<IFieldDeclaration>();
            var m = Mock.Of<IMethodDeclaration>();
            var p = Mock.Of<IPropertyDeclaration>();

            Visit(
                new SST
                {
                    Delegates = {d},
                    Events = {e},
                    Fields = {f},
                    Methods = {m},
                    Properties = {p}
                });

            AssertAccept(d);
            AssertAccept(e);
            AssertAccept(f);
            AssertAccept(m);
        }

        [Test]
        public void DelegateDeclaration()
        {
            Visit(new DelegateDeclaration());
        }

        [Test]
        public void EventDeclaration()
        {
            Visit(new EventDeclaration());
        }

        [Test]
        public void FieldDeclaration()
        {
            Visit(new FieldDeclaration());
        }

        [Test]
        public void MethodDeclaration()
        {
            Visit(
                new MethodDeclaration
                {
                    Body = {_s1}
                });

            AssertAccept(_s1);
        }

        [Test]
        public void PropertyDeclaration()
        {
            Visit(
                new PropertyDeclaration
                {
                    Get = {_s1},
                    Set = {_s2}
                });

            AssertAccept(_s1);
            AssertAccept(_s2);
        }

        #endregion

        #region blocks

        [Test]
        public void DoLoop()
        {
            Visit(
                new DoLoop
                {
                    Condition = _e1,
                    Body = {_s1}
                });

            AssertAccept(_e1);
            AssertAccept(_s1);
        }

        [Test]
        public void ForEachLoop()
        {
            var vd = Mock.Of<IVariableDeclaration>();

            Visit(
                new ForEachLoop
                {
                    Declaration = vd,
                    LoopedReference = _vr,
                    Body = {_s1}
                });

            AssertAccept(vd);
            AssertAccept(_vr);
            AssertAccept(_s1);
        }

        [Test]
        public void ForLoop()
        {
            Visit(
                new ForLoop
                {
                    Init = {_s1},
                    Condition = _e1,
                    Step = {_s2},
                    Body = {_s3}
                });

            AssertAccept(_e1);
            AssertAccept(_s1);
            AssertAccept(_s2);
            AssertAccept(_s3);
        }

        [Test]
        public void IfElseBlock()
        {
            Visit(
                new IfElseBlock
                {
                    Condition = _e1,
                    Then = {_s1},
                    Else = {_s2}
                });

            AssertAccept(_e1);
            AssertAccept(_s1);
            AssertAccept(_s2);
        }

        [Test]
        public void LockBlock()
        {
            Visit(
                new LockBlock
                {
                    Reference = _vr,
                    Body = {_s1}
                });

            AssertAccept(_vr);
            AssertAccept(_s1);
        }

        [Test]
        public void SwitchBlock()
        {
            Visit(
                new SwitchBlock
                {
                    Reference = _vr,
                    Sections =
                    {
                        new CaseBlock
                        {
                            Label = _e1,
                            Body = {_s1}
                        }
                    },
                    DefaultSection = {_s2}
                });

            AssertAccept(_vr);
            AssertAccept(_e1);
            AssertAccept(_s1);
            AssertAccept(_s2);
        }

        [Test]
        public void TryBlock()
        {
            Visit(
                new TryBlock
                {
                    Body = {_s1},
                    CatchBlocks =
                    {
                        new CatchBlock
                        {
                            Body = {_s2}
                        }
                    },
                    Finally = {_s3}
                });

            AssertAccept(_s1);
            AssertAccept(_s2);
            AssertAccept(_s3);
        }

        [Test]
        public void UncheckedBlock()
        {
            Visit(
                new UncheckedBlock
                {
                    Body = {_s1}
                });

            AssertAccept(_s1);
        }

        [Test]
        public void UnsafeBlock()
        {
            Visit(new UnsafeBlock());
        }

        [Test]
        public void UsingBlock()
        {
            Visit(
                new UsingBlock
                {
                    Reference = _vr,
                    Body = {_s1}
                });

            AssertAccept(_vr);
            AssertAccept(_s1);
        }

        [Test]
        public void WhileLoop()
        {
            Visit(
                new WhileLoop
                {
                    Condition = _e1,
                    Body = {_s1}
                });

            AssertAccept(_e1);
            AssertAccept(_s1);
        }

        #endregion

        #region statements

        [Test]
        public void Assignment()
        {
            Visit(
                new Assignment
                {
                    Reference = _vr,
                    Expression = _e1
                });

            AssertAccept(_vr);
            AssertAccept(_e1);
        }

        [Test]
        public void BreakStatement()
        {
            Visit(new BreakStatement());
        }

        [Test]
        public void ContinueStatement()
        {
            Visit(new ContinueStatement());
        }

        [Test]
        public void EventSubscriptionStatement()
        {
            Visit(
                new EventSubscriptionStatement
                {
                    Reference = _vr,
                    Expression = _e1
                });

            AssertAccept(_vr);
            AssertAccept(_e1);
        }

        [Test]
        public void ExpressionStatement()
        {
            Visit(
                new ExpressionStatement
                {
                    Expression = _e1
                });

            AssertAccept(_e1);
        }

        [Test]
        public void GotoStatement()
        {
            Visit(new GotoStatement());
        }

        [Test]
        public void LabelledStatement()
        {
            Visit(
                new LabelledStatement
                {
                    Statement = _s1
                });

            AssertAccept(_s1);
        }

        [Test]
        public void ReturnStatement()
        {
            Visit(
                new ReturnStatement
                {
                    Expression = _e1
                });

            AssertAccept(_e1);
        }

        [Test]
        public void ThrowStatement()
        {
            Visit(new ThrowStatement());
        }

        [Test]
        public void VariableDeclaration()
        {
            Visit(
                new VariableDeclaration
                {
                    Reference = _vr
                });

            AssertAccept(_vr);
        }

        #endregion

        #region expressions

        [Test]
        public void CastExpression()
        {
            Visit(new CastExpression());
        }

        [Test]
        public void CompletionExpression()
        {
            Visit(new CompletionExpression());
        }

        [Test]
        public void ComposedExpression()
        {
            Visit(
                new ComposedExpression
                {
                    References = {_vr}
                });

            AssertAccept(_vr);
        }

        [Test]
        public void IfElseExpression()
        {
            Visit(
                new IfElseExpression
                {
                    Condition = _e1,
                    ThenExpression = _e2,
                    ElseExpression = _e3
                });

            AssertAccept(_e1);
            AssertAccept(_e2);
            AssertAccept(_e3);
        }

        [Test]
        public void IndexAccessExpression()
        {
            Visit(new IndexAccessExpression {Reference = _vr});

            AssertAccept(_vr);
        }

        [Test]
        public void InvocationExpression()
        {
            Visit(
                new InvocationExpression
                {
                    Reference = _vr,
                    Parameters = {_e1}
                });

            AssertAccept(_vr);
            AssertAccept(_e1);
        }

        [Test]
        public void LambdaExpression()
        {
            Visit(
                new LambdaExpression
                {
                    Body = {_s1}
                });

            AssertAccept(_s1);
        }

        [Test]
        public void LoopHeaderBlockExpression()
        {
            Visit(
                new LoopHeaderBlockExpression
                {
                    Body = {_s1}
                });

            AssertAccept(_s1);
        }

        [Test]
        public void ConstantValueExpression()
        {
            Visit(new ConstantValueExpression());
        }

        [Test]
        public void NullExpression()
        {
            Visit(new NullExpression());
        }

        [Test]
        public void ReferenceExpression()
        {
            Visit(
                new ReferenceExpression
                {
                    Reference = _vr
                });

            AssertAccept(_vr);
        }

        [Test]
        public void TypeCheckExpression()
        {
            Visit(new TypeCheckExpression {Reference = _vr});
            AssertAccept(_vr);
        }

        [Test]
        public void UnaryExpression()
        {
            Visit(new UnaryExpression {Operand = _e1});
            AssertAccept(_e1);
        }

        [Test]
        public void BinaryExpression()
        {
            Visit(new BinaryExpression {LeftOperand = _e1, RightOperand = _e2});
            AssertAccept(_e1);
            AssertAccept(_e2);
        }

        #endregion

        #region references

        [Test]
        public void EventReference()
        {
            Visit(
                new EventReference
                {
                    Reference = _vr
                });

            AssertAccept(_vr);
        }

        [Test]
        public void FieldReference()
        {
            Visit(
                new FieldReference
                {
                    Reference = _vr
                });

            AssertAccept(_vr);
        }

        [Test]
        public void MethodReference()
        {
            Visit(
                new MethodReference
                {
                    Reference = _vr
                });

            AssertAccept(_vr);
        }

        [Test]
        public void PropertyReference()
        {
            Visit(
                new PropertyReference
                {
                    Reference = _vr
                });

            AssertAccept(_vr);
        }

        [Test]
        public void VariableReference()
        {
            Visit(new VariableReference());
        }

        [Test]
        public void IndexAccessReference()
        {
            var iae = Mock.Of<IIndexAccessExpression>();

            Visit(new IndexAccessReference {Expression = iae});

            AssertAccept(iae);
        }

        #endregion

        #region unknowns

        [Test]
        public void UnknownStatement()
        {
            Visit(new UnknownStatement());
        }

        [Test]
        public void UnknownExpression()
        {
            Visit(new UnknownExpression());
        }

        [Test]
        public void UnknownReference()
        {
            Visit(new UnknownReference());
        }

        #endregion
    }
}