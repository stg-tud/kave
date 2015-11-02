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
using KaVE.Commons.Model.SSTs.Impl.Expressions.Assignable;
using KaVE.Commons.Model.SSTs.Impl.Expressions.LoopHeader;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Simple;
using KaVE.Commons.Utils.Assertion;
using KaVE.VS.FeedbackGenerator.SessionManager.Anonymize;
using KaVE.VS.FeedbackGenerator.SessionManager.Anonymize.CompletionEvents;
using NUnit.Framework;

namespace KaVE.VS.FeedbackGenerator.Tests.SessionManager.Anonymize.CompletionEvents
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
        public void CastExpression()
        {
            AssertAnonymization(
                new CastExpression
                {
                    TargetType = Type("a"),
                    Reference = AnyVarReference
                },
                new CastExpression
                {
                    TargetType = Type("a").ToAnonymousName(),
                    Reference = AnyVarReferenceAnonymized
                });
        }

        [Test]
        public void CastExpression_DefaultSafe()
        {
            AssertAnonymization(new CastExpression(), new CastExpression());
        }

        [Test]
        public void CompletionExpression()
        {
            AssertAnonymization(
                new CompletionExpression
                {
                    VariableReference = AnyVarReference,
                    TypeReference = Type("a"),
                    Token = "t"
                },
                new CompletionExpression
                {
                    VariableReference = AnyVarReferenceAnonymized,
                    TypeReference = TypeAnonymized("a"),
                    Token = "t" // not anonymized
                });
        }

        [Test]
        public void CompletionExpression_DefaultSafe()
        {
            AssertAnonymization(new CompletionExpression(), new CompletionExpression());
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
        public void ComposedExpression_DefaultSafe()
        {
            AssertAnonymization(new ComposedExpression(), new ComposedExpression());
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
        public void IfElseExpression_DefaultSafe()
        {
            AssertAnonymization(new IfElseExpression(), new IfElseExpression());
        }

        [Test]
        public void IndexAccessExpression()
        {
            AssertAnonymization(
                new IndexAccessExpression
                {
                    Reference = AnyVarReference,
                    Indices =
                    {
                        AnyExpression
                    }
                },
                new IndexAccessExpression
                {
                    Reference = AnyVarReferenceAnonymized,
                    Indices =
                    {
                        AnyExpressionAnonymized
                    }
                });
        }

        [Test]
        public void IndexAccessExpression_DefaultSafe()
        {
            AssertAnonymization(new IndexAccessExpression(), new IndexAccessExpression());
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
        public void InvocationExpression_DefaultSafe()
        {
            AssertAnonymization(new InvocationExpression(), new InvocationExpression());
        }

        [Test, ExpectedException(typeof (AssertException))]
        public void LambdaExpression()
        {
            _sut.Visit(new LambdaExpression(), 0);
        }

        [Test]
        public void TypeCheckExpression()
        {
            AssertAnonymization(
                new TypeCheckExpression
                {
                    Reference = AnyVarReference,
                    Type = Type("a")
                },
                new TypeCheckExpression
                {
                    Reference = AnyVarReferenceAnonymized,
                    Type = Type("a").ToAnonymousName()
                });
        }

        [Test]
        public void TypeCheckExpression_DefaultSafe()
        {
            AssertAnonymization(new TypeCheckExpression(), new TypeCheckExpression());
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
        public void ConstantValueExpression_DefaultSafe()
        {
            AssertAnonymization(new ConstantValueExpression(), new ConstantValueExpression());
        }

        [Test]
        public void NullExpression()
        {
            AssertAnonymization(
                new NullExpression(),
                new NullExpression());
        }

        [Test]
        public void NullExpression_DefaultSafe()
        {
            AssertAnonymization(new NullExpression(), new NullExpression());
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
        public void ReferenceExpression_DefaultSafe()
        {
            AssertAnonymization(new ReferenceExpression(), new ReferenceExpression());
        }

        [Test]
        public void UnknownExpression()
        {
            AssertAnonymization(new UnknownExpression(), new UnknownExpression());
        }
    }
}