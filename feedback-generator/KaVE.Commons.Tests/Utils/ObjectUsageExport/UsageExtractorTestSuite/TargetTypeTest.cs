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
using KaVE.Commons.Model.ObjectUsage;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Assignable;
using KaVE.Commons.Model.SSTs.Impl.References;
using KaVE.Commons.Utils.ObjectUsageExport;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Utils.ObjectUsageExport.UsageExtractorTestSuite
{
    internal class TargetTypeTest
    {
        private UsageExtractionVisitor _sut;

        [SetUp]
        public void SetUp()
        {
            _sut = new UsageExtractionVisitor();
        }

        [Test]
        public void ReferenceIsNull()
        {
            var expr = new CompletionExpression
            {
                VariableReference = null
            };
            var context = new UsageContext();

            _sut.Visit(expr, context);

            Assert.Null(context.TargetType);
        }

        [Test]
        public void ReferenceIsMissing()
        {
            var expr = new CompletionExpression
            {
                VariableReference = new VariableReference()
            };
            var context = new UsageContext();

            _sut.Visit(expr, context);

            Assert.Null(context.TargetType);
        }

        [Test]
        public void ReferenceExistsButIsUndefinedInScope()
        {
            var expr = new CompletionExpression
            {
                VariableReference = new VariableReference
                {
                    Identifier = "a"
                }
            };
            var context = new UsageContext();

            _sut.Visit(expr, context);

            Assert.Null(context.TargetType);
        }

        [Test]
        public void HappyPath()
        {
            var expr = new CompletionExpression
            {
                VariableReference = new VariableReference
                {
                    Identifier = "a"
                }
            };
            var context = new UsageContext();
            context.DefineVariable("a", Names.Type("T,P"), DefinitionSites.CreateUnknownDefinitionSite());
            _sut.Visit(expr, context);

            Assert.AreEqual(new CoReTypeName("LT"), context.TargetType);
        }
    }
}