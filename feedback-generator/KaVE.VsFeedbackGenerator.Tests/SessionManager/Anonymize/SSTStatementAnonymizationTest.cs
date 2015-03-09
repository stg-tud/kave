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
using KaVE.Model.SSTs.Impl.Statements;
using KaVE.VsFeedbackGenerator.SessionManager.Anonymize;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.SessionManager.Anonymize
{
    public class SSTStatementAnonymizationTest : SSTAnonymizationBaseTest
    {
        private SSTStatementAnonymization _sut;

        [SetUp]
        public void Setup()
        {
            _sut = new SSTStatementAnonymization(ExpressionAnonymizationMock, ReferenceAnonymizationMock);
        }

        private void AssertAnonymization(IStatement statement, IStatement expected)
        {
            var actual = statement.Accept(_sut, 0);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Assignment()
        {
            AssertAnonymization(
                new Assignment
                {
                    Reference = AnyVarReference,
                    Expression = AnyExpression
                },
                new Assignment
                {
                    Reference = AnyVarReferenceAnonymized,
                    Expression = AnyExpressionAnonymized
                });
        }

        [Test]
        public void Assignment_Lambda()
        {
            AssertAnonymization(
                new Assignment
                {
                    Reference = AnyVarReference,
                    Expression = AnyLambdaExpr
                },
                new Assignment
                {
                    Reference = AnyVarReferenceAnonymized,
                    Expression = AnyLambdaExprAnonymized
                });
        }

        [Test]
        public void Assignment_NullSafe()
        {
            _sut.Visit(new Assignment(), 0);
        }

        [Test]
        public void BreakStatement()
        {
            AssertAnonymization(new BreakStatement(), new BreakStatement());
        }

        [Test]
        public void BreakStatement_NullSafe()
        {
            _sut.Visit(new BreakStatement(), 0);
        }

        [Test]
        public void ContinueStatement()
        {
            AssertAnonymization(new ContinueStatement(), new ContinueStatement());
        }

        [Test]
        public void ContinueStatement_NullSafe()
        {
            _sut.Visit(new ContinueStatement(), 0);
        }

        [Test]
        public void ExpressionStatement()
        {
            AssertAnonymization(
                new ExpressionStatement
                {
                    Expression = AnyExpression
                },
                new ExpressionStatement
                {
                    Expression = AnyExpressionAnonymized
                });
        }

        [Test]
        public void ExpressionStatement_Block()
        {
            AssertAnonymization(
                new ExpressionStatement
                {
                    Expression = AnyBlockExpr
                },
                new ExpressionStatement
                {
                    Expression = AnyBlockExprAnonymized
                });
        }

        [Test]
        public void ExpressionStatement_Lambda()
        {
            AssertAnonymization(
                new ExpressionStatement
                {
                    Expression = AnyLambdaExpr
                },
                new ExpressionStatement
                {
                    Expression = AnyLambdaExprAnonymized
                });
        }

        [Test]
        public void ExpressionStatement_NullSafe()
        {
            _sut.Visit(new ExpressionStatement(), 0);
        }

        [Test]
        public void GotoStatement()
        {
            AssertAnonymization(
                new GotoStatement
                {
                    Label = "g"
                },
                new GotoStatement
                {
                    Label = "g" // not anonymized
                });
        }

        [Test]
        public void GotoStatement_NullSafe()
        {
            _sut.Visit(new GotoStatement(), 0);
        }

        [Test]
        public void LabelledStatement()
        {
            AssertAnonymization(
                new LabelledStatement
                {
                    Label = "g",
                    Statement = AnyStatement
                },
                new LabelledStatement
                {
                    Label = "g", // not anonymized
                    Statement = AnyStatementAnonymized
                });
        }

        [Test]
        public void LabelledStatement_NullSafe()
        {
            _sut.Visit(new LabelledStatement(), 0);
        }

        [Test]
        public void ReturnStatement()
        {
            AssertAnonymization(
                new ReturnStatement
                {
                    Expression = AnyExpression
                },
                new ReturnStatement
                {
                    Expression = AnyExpressionAnonymized
                });
        }

        [Test]
        public void ReturnStatement_NullSafe()
        {
            _sut.Visit(new ReturnStatement(), 0);
        }

        [Test]
        public void ThrowStatement()
        {
            AssertAnonymization(
                new ThrowStatement
                {
                    Exception = Type("a")
                },
                new ThrowStatement
                {
                    Exception = TypeAnonymized("a")
                });
        }

        [Test]
        public void ThrowStatement_NullSafe()
        {
            _sut.Visit(new ThrowStatement(), 0);
        }
    }
}