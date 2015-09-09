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

using System.Text;
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
    internal class CompletionPrefixVisitorTest
    {
        [Test]
        public void ShouldGetThePrefix()
        {
            const string testPrefix = "SomeObj";

            var context = new StringBuilder();

            var sst = new SST();
            var methodDeclarationContainingCompletionExpression = new MethodDeclaration
            {
                Body =
                    Lists.NewList<IStatement>(
                        new ExpressionStatement
                        {
                            Expression = new CompletionExpression {Token = testPrefix}
                        })
            };
            sst.Methods.Add(methodDeclarationContainingCompletionExpression);

            sst.Accept(new CompletionPrefixVisitor(), context);

            Assert.AreEqual(testPrefix, context.ToString());
        }

        [Test]
        public void ShouldUseEmptyStringAsDefault()
        {
            var context = new StringBuilder();

            var sst = new SST();
            sst.Accept(new CompletionPrefixVisitor(), context);

            Assert.AreEqual(string.Empty, context.ToString());
        }
    }
}