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

using KaVE.Commons.Model.ObjectUsage;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Simple;
using NUnit.Framework;
using Fix = KaVE.Commons.Tests.Utils.ObjectUsageExport.UsageExtractorTestSuite.ObjectUsageExporterTestFixture;

namespace KaVE.Commons.Tests.Utils.ObjectUsageExport.UsageExtractorTestSuite
{
    /// <summary>
    ///     tests how cases are handled in which the SST contains incomplete information
    /// </summary>
    internal class MissingInformationTest : BaseObjectUsageExporterTest
    {
        [Test]
        public void VarDefinitionByUnknown_NotInitialized()
        {
            SetupDefaultEnclosingMethod(
                VarDecl("a", Type("A")),
                InvokeStmt("a", SomeMethodOnType("B")));

            AssertQueriesInDefault(
                new Query
                {
                    type = Type("A").ToCoReName(),
                    definition = DefinitionSites.CreateUnknownDefinitionSite(),
                    sites =
                    {
                        SomeCallSiteOnType("B")
                    }
                });
        }

        [Test]
        public void VarDefinitionByUnknown_InvocationOnNotDeclared()
        {
            SetupDefaultEnclosingMethod(
                InvokeStmt("a", SomeMethodOnType("B")));

            AssertQueriesInDefault(
                new Query
                {
                    type = Type("B").ToCoReName(),
                    definition = DefinitionSites.CreateUnknownDefinitionSite(),
                    sites =
                    {
                        SomeCallSiteOnType("B")
                    }
                });
        }

        [Test]
        public void VarDefinitionByUnknown_InvocationOnNotDeclaredButExistingType()
        {
            SetupDefaultEnclosingMethod(
                VarDecl("a", Type("A")),
                InvokeStmt("a", SomeMethodOnType("B")),
                InvokeStmt("b", SomeMethodOnType("A")));

            AssertQueriesInDefault(
                new Query
                {
                    type = Type("A").ToCoReName(),
                    sites =
                    {
                        SomeCallSiteOnType("B"),
                        SomeCallSiteOnType("A")
                    }
                });
        }

        [Test]
        public void VarDefinitionByUnknown_AssignmentToNotDeclared()
        {
            SetupDefaultEnclosingMethod(
                Assign("a", new ConstantValueExpression()));

            AssertQueriesInDefault();
        }
    }
}