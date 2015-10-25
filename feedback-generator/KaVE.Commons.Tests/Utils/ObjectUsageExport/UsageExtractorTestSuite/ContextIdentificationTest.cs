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
using KaVE.Commons.Model.TypeShapes;
using NUnit.Framework;
using Fix = KaVE.Commons.Tests.Utils.ObjectUsageExport.UsageExtractorTestSuite.ObjectUsageExporterTestFixture;

namespace KaVE.Commons.Tests.Utils.ObjectUsageExport.UsageExtractorTestSuite
{
    /// <summary>
    ///     tests the correct identification of contexts for the queries
    /// </summary>
    internal class ContextIdentificationTest : BaseObjectUsageExporterTest
    {
        [Test]
        public void ClassContext_Element()
        {
            AddInvocationOnThisToDefaultContext();

            var actual = AssertSingleQuery().classCtx;
            var expected = DefaultClassContext.ToCoReName();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ClassContext_Super()
        {
            AddInvocationOnThisToDefaultContext();

            Context.TypeShape.TypeHierarchy = new TypeHierarchy
            {
                Element = DefaultClassContext,
                Extends = new TypeHierarchy
                {
                    Element = Type("TSuper")
                }
            };

            var actual = AssertSingleQuery().classCtx;
            var expected = Type("TSuper").ToCoReName();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void MethodContext_ElementDeclaration()
        {
            AddInvocationOnThisToDefaultContext();

            ResetMethodHierarchies(
                new MethodHierarchy
                {
                    Element = DefaultMethodContext
                });

            var actual = AssertSingleQuery().methodCtx;
            var expected = DefaultMethodContext.ToCoReName();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void MethodContext_SuperDeclaration()
        {
            AddInvocationOnThisToDefaultContext();

            ResetMethodHierarchies(
                new MethodHierarchy
                {
                    Element = DefaultMethodContext,
                    Super = SomeMethodOnType("TSuper")
                });

            var actual = AssertSingleQuery().methodCtx;
            var expected = SomeMethodOnType("TSuper").ToCoReName();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void MethodContext_FirstDeclaration()
        {
            AddInvocationOnThisToDefaultContext();

            ResetMethodHierarchies(
                new MethodHierarchy
                {
                    Element = DefaultMethodContext,
                    First = SomeMethodOnType("TFirst"),
                    Super = SomeMethodOnType("TSuper")
                });

            var actual = AssertSingleQuery().methodCtx;
            var expected = SomeMethodOnType("TFirst").ToCoReName();
            Assert.AreEqual(expected, actual);
        }

        private void AddInvocationOnThisToDefaultContext()
        {
            SetupDefaultEnclosingMethod(
                InvokeStmt("this", SomeMethodOnType(DefaultClassContext.Name)));
        }
    }
}