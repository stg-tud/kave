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
using KaVE.Commons.Model.SSTs.Impl;
using KaVE.Commons.Model.SSTs.Impl.Declarations;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Assignable;
using KaVE.Commons.Model.SSTs.Impl.Statements;
using KaVE.Commons.Model.SSTs.Impl.Visitor;
using KaVE.Commons.Utils.Collections;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Model.SSTs.Impl.Visitor
{
    internal class HasCompletionExpressionVisitorTest
    {
        [Test]
        public void HasNoCompletion()
        {
            var sst = CreateSST(new ContinueStatement());
            var actual = HasCompletionExpressionVisitor.On(sst);
            Assert.False(actual.HasCompletionExpression);
            Assert.AreEqual(0, actual.Count);
        }

        [Test]
        public void HasCompletion()
        {
            var sst = CreateSST(new ContinueStatement(), EmptyCompletion());
            var actual = HasCompletionExpressionVisitor.On(sst);
            Assert.True(actual.HasCompletionExpression);
            Assert.AreEqual(1, actual.Count);
        }

        [Test]
        public void HasTwoCompletion()
        {
            var sst = CreateSST(new ContinueStatement(), EmptyCompletion(), EmptyCompletion());
            var actual = HasCompletionExpressionVisitor.On(sst);
            Assert.True(actual.HasCompletionExpression);
            Assert.AreEqual(2, actual.Count);
        }

        private static ISST CreateSST(params IStatement[] stmts)
        {
            return new SST
            {
                Methods =
                {
                    new MethodDeclaration
                    {
                        Body = Lists.NewListFrom(stmts)
                    }
                }
            };
        }

        private static IStatement EmptyCompletion()
        {
            return new ExpressionStatement
            {
                Expression = new CompletionExpression()
            };
        }
    }
}