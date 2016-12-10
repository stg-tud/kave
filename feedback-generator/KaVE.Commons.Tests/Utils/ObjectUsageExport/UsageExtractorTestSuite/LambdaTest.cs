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
using KaVE.Commons.Model.Naming.CodeElements;
using KaVE.Commons.Model.Naming.Types;
using KaVE.Commons.Model.ObjectUsage;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Assignable;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Simple;
using KaVE.Commons.Model.TypeShapes;
using KaVE.Commons.Utils.Collections;
using NUnit.Framework;
using Fix = KaVE.Commons.Tests.Utils.ObjectUsageExport.UsageExtractorTestSuite.ObjectUsageExporterTestFixture;

namespace KaVE.Commons.Tests.Utils.ObjectUsageExport.UsageExtractorTestSuite
{
    internal class LambdaTest : BaseObjectUsageExporterTest
    {
        private void SetupLambdaExample()
        {
            SetupDefaultEnclosingMethod(
                VarDecl("i", Fix.Int),
                Assign("i", new ConstantValueExpression()),
                InvokeStmt("i", Fix.Int_GetHashCode),
                VarDecl("a", Fix.Action),
                Assign(
                    "a",
                    new LambdaExpression
                    {
                        Name = Names.Lambda("[{0}] ()", Fix.Void),
                        Body =
                        {
                            InvokeStmt("i", Fix.Int_GetHashCode)
                        }
                    }));
        }

        private static Query CreateQuery(ITypeName classCtx, IMethodName methodCtx)
        {
            return new Query
            {
                type = Fix.Int.ToCoReName(),
                classCtx = classCtx.ToCoReName(),
                methodCtx = methodCtx.ToCoReName(),
                definition = DefinitionSites.CreateDefinitionByConstant(),
                sites =
                {
                    CallSites.CreateReceiverCallSite(Fix.Int_GetHashCode)
                }
            };
        }

        [Test]
        public void CallsInLambdasAreRegisteredInCorrectContext()
        {
            SetupLambdaExample();

            AssertQueriesWithoutSettingContexts(
                CreateQuery(DefaultClassContext, DefaultMethodContext),
                CreateQuery(Type("TDecl$Lambda"), Method(Type("A"), DefaultClassContext, "M$Lambda")));
        }

        [Test]
        public void SuperTypePreservesLambdaMarker()
        {
            SetupLambdaExample();

            Context.TypeShape.TypeHierarchy = new TypeHierarchy
            {
                Element = DefaultClassContext,
                Extends = new TypeHierarchy
                {
                    Element = Names.Type("Super, P")
                }
            };

            AssertQueriesWithoutSettingContexts(
                CreateQuery(Type("Super"), DefaultMethodContext),
                CreateQuery(Type("Super$Lambda"), Method(Type("A"), DefaultClassContext, "M$Lambda")));
        }

        [Test, Ignore]
        public void SuperTypePreservesDoubleLambdaMarker()
        {
            // e.g. lambda in lambda
            Assert.Fail("not implemented yet");
        }

        [Test]
        public void OverriddenMethodPreservesLambdaMarker()
        {
            SetupLambdaExample();

            Context.TypeShape.MethodHierarchies = Sets.NewHashSet<IMemberHierarchy<IMethodName>>(
                new MethodHierarchy
                {
                    Element = DefaultMethodContext,
                    First = Method(Type("A"), Type("Super"), "M")
                });

            AssertQueriesWithoutSettingContexts(
                CreateQuery(DefaultClassContext, Method(Type("A"), Type("Super"), "M")),
                CreateQuery(Type("TDecl$Lambda"), Method(Type("A"), Type("Super"), "M$Lambda")));
        }

        [Test, Ignore]
        public void OverriddenMethodPreservesDoubleLambdaMarker()
        {
            // e.g. lambda in lambda
            Assert.Fail("not implemented yet");
        }
    }
}