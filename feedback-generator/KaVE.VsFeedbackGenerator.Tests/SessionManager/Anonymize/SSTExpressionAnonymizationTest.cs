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

using KaVE.Model.SSTs;
using KaVE.Model.SSTs.Impl.Expressions.Assignable;
using KaVE.Model.SSTs.Impl.Expressions.LoopHeader;
using KaVE.Model.SSTs.Impl.Expressions.Simple;
using KaVE.Utils.Assertion;
using KaVE.VsFeedbackGenerator.SessionManager.Anonymize;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.SessionManager.Anonymize
{
    public class SSTExpressionAnonymizationTest : SSTAnonymizationBaseTest
    {
        private SSTExpressionAnonymization _sut;

        [SetUp]
        public void Setup()
        {
            _sut = new SSTExpressionAnonymization(ReferenceAnonymizationMock);
        }

        private void AssertAnonymization(IExpression expr, IExpression expected)
        {
            var actual = expr.Accept(_sut, 0);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void CompletionExpression()
        {
            AssertAnonymization(
                new CompletionExpression
                {
                    ObjectReference = AnyVarReference,
                    TypeReference = Type("a"),
                    Token = "t"
                },
                new CompletionExpression
                {
                    ObjectReference = AnyVarReferenceAnonymized,
                    TypeReference = TypeAnonymized("a"),
                    Token = "t" // not anonymized
                });
        }

        [Test]
        public void CompletionExpression_NullSafe()
        {
            _sut.Visit(new CompletionExpression(), 0);
        }

        [Test]
        public void ComposedExpression()
        {
            AssertAnonymization(
                new ComposedExpression
                {
                    References = {AnyVarReference}
                },
                new ComposedExpression
                {
                    References = {AnyVarReferenceAnonymized}
                });
        }

        [Test]
        public void ComposedExpression_NullSafe()
        {
            _sut.Visit(new ComposedExpression(), 0);
        }

        [Test]
        public void IfElseExpression()
        {
            AssertAnonymization(
                new IfElseExpression
                {
                    Condition = AnyExpression,
                    ThenExpression = AnyExpression,
                    ElseExpression = AnyExpression
                },
                new IfElseExpression
                {
                    Condition = AnyExpressionAnonymized,
                    ThenExpression = AnyExpressionAnonymized,
                    ElseExpression = AnyExpressionAnonymized
                });
        }

        [Test]
        public void IfElseExpression_NullSafe()
        {
            _sut.Visit(new IfElseExpression(), 0);
        }

        [Test]
        public void InvocationExpression()
        {
            AssertAnonymization(
                new InvocationExpression
                {
                    Reference = AnyVarReference,
                    MethodName = Method("a"),
                    Parameters = {AnyExpression}
                },
                new InvocationExpression
                {
                    Reference = AnyVarReferenceAnonymized,
                    MethodName = MethodAnonymized("a"),
                    Parameters = {AnyExpressionAnonymized}
                });
        }

        [Test]
        public void InvocationExpression_NullSafe()
        {
            _sut.Visit(new InvocationExpression(), 0);
        }

        [Test, ExpectedException(typeof (AssertException))]
        public void LambdaExpression()
        {
            _sut.Visit(new LambdaExpression(), 0);
        }

        [Test, ExpectedException(typeof (AssertException))]
        public void LoopHeaderBlockExpression()
        {
            _sut.Visit(new LoopHeaderBlockExpression(), 0);
        }

        [Test]
        public void ConstantValueExpression()
        {
            AssertAnonymization(
                new ConstantValueExpression
                {
                    Value = "a"
                },
                new ConstantValueExpression
                {
                    Value = "a".ToHash()
                });
        }

        [Test]
        public void ConstantValueExpression_NullSafe()
        {
            _sut.Visit(new ConstantValueExpression(), 0);
        }

        [Test]
        public void NullExpression()
        {
            AssertAnonymization(
                new NullExpression(),
                new NullExpression());
        }

        [Test]
        public void NullExpression_NullSafe()
        {
            _sut.Visit(new NullExpression(), 0);
        }

        [Test]
        public void ReferenceExpression()
        {
            AssertAnonymization(
                new ReferenceExpression
                {
                    Reference = AnyVarReference
                },
                new ReferenceExpression
                {
                    Reference = AnyVarReferenceAnonymized
                });
        }

        [Test]
        public void ReferenceExpression_NullSafe()
        {
            _sut.Visit(new ReferenceExpression(), 0);
        }
    }
}