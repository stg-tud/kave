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
using KaVE.Commons.Model.SSTs.Impl.Blocks;
using KaVE.Commons.Model.SSTs.Impl.Declarations;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Simple;
using KaVE.Commons.Model.SSTs.Impl.References;
using NUnit.Framework;
using Fix = KaVE.Commons.Tests.Utils.ObjectUsageExport.UsageExtractorTestSuite.ObjectUsageExporterTestFixture;

namespace KaVE.Commons.Tests.Utils.ObjectUsageExport.UsageExtractorTestSuite
{
    /// <summary>
    ///     tests if all definition sites are identified correctly
    /// </summary>
    internal class DefinitionSiteDetectionTest : BaseObjectUsageExporterTest
    {
        [Test]
        public void VarDefinitionByConstant()
        {
            SetupDefaultEnclosingMethod(
                VarDecl("a", Type("A")),
                Assign("a", new ConstantValueExpression()),
                InvokeStmt("a", Method(Fix.Void, Type("A"), "M")));

            AssertSingleQueryWithType(Type("A"));
            AssertSingleQueryWithDefinition(DefinitionSites.CreateDefinitionByConstant());
        }

        [Test]
        public void VarDefinitionByConstructor_SameType()
        {
            var ctor = Constructor(Type("A"));

            SetupDefaultEnclosingMethod(
                VarDecl("a", Type("A")),
                Assign("a", ctor),
                InvokeStmt("a", SomeMethodOnType("A")));

            AssertSingleQueryWithType(Type("A"));
            AssertSingleQueryWithDefinition(DefinitionSites.CreateDefinitionByConstructor(ctor.MethodName));
        }

        [Test]
        public void VarDefinitionByConstructor_Subtype()
        {
            var ctor = Constructor(Type("B"));

            SetupDefaultEnclosingMethod(
                VarDecl("a", Type("A")),
                Assign("a", ctor),
                InvokeStmt("a", SomeMethodOnType("A")));

            AssertSingleQueryWithType(Type("A"));
            AssertSingleQueryWithDefinition(DefinitionSites.CreateDefinitionByConstructor(ctor.MethodName));
        }

        [Test]
        public void DefinitionAsParam_Method()
        {
            var enclosingMethod = Method(Fix.Void, DefaultClassContext, "M", Parameter(Type("P"), "p"));

            SetupEnclosingMethod(
                enclosingMethod,
                InvokeStmt("p", SomeMethodOnType("Q")));

            AssertSingleQueryWithType(Type("P"));
            AssertSingleQueryWithDefinition(DefinitionSites.CreateDefinitionByParam(enclosingMethod, 0));
        }

        [Test]
        public void DefinitionAsParam_CatchBlock()
        {
            SetupDefaultEnclosingMethod(
                new TryBlock
                {
                    CatchBlocks =
                    {
                        new CatchBlock
                        {
                            Parameter = Parameter(Type("P"), "p"),
                            Body =
                            {
                                InvokeStmt("p", SomeMethodOnType("Q"))
                            }
                        }
                    }
                });

            AssertSingleQueryWithType(Type("P"));
            AssertSingleQueryWithDefinition(DefinitionSites.CreateUnknownDefinitionSite());
        }

        [Test]
        public void VarDefinitionByReturn_ReturnsSameType()
        {
            var callee = Method(Type("A"), DefaultClassContext, "M");

            SetupDefaultEnclosingMethod(
                VarDecl("a", Type("A")),
                Assign("a", Invoke("this", callee)),
                InvokeStmt("a", SomeMethodOnType("A")));

            AssertQueriesExistFor(Type("A"), DefaultClassContext);

            var actual = FindQueryWith(Type("A")).definition;
            var expected = DefinitionSites.CreateDefinitionByReturn(callee);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void VarDefinitionByReturn_ReturnsSubtype()
        {
            var callee = Method(Type("B"), DefaultClassContext, "M");

            SetupDefaultEnclosingMethod(
                VarDecl("a", Type("A")),
                Assign("a", Invoke("this", callee)),
                InvokeStmt("a", SomeMethodOnType("A")));

            AssertQueriesExistFor(Type("A"), DefaultClassContext);

            var actual = FindQueryWith(Type("A")).definition;
            var expected = DefinitionSites.CreateDefinitionByReturn(callee);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void DefinitionByThis()
        {
            SetupDefaultEnclosingMethod(
                InvokeStmt("this", Method(Fix.Void, DefaultClassContext, "M")));

            AssertSingleQueryWithDefinition(DefinitionSites.CreateDefinitionByThis());
        }

        [Test]
        public void DefinitionByThis_Base()
        {
            SetupDefaultEnclosingMethod(
                InvokeStmt("base", Method(Fix.Void, DefaultClassContext, "M")));

            AssertSingleQueryWithDefinition(DefinitionSites.CreateDefinitionByThis());
        }

        [Test]
        public void DefinitionAsProperty()
        {
            SetupDefaultEnclosingMethod(
                InvokeStmt("p", SomeMethodOnType("P")));

            Context.SST.Properties.Add(
                new PropertyDeclaration
                {
                    Name = Property(Type("P"), DefaultClassContext, "p")
                });

            // TODO @seb: add support for fields
            AssertSingleQueryWithDefinition(DefinitionSites.CreateDefinitionByField(Names.UnknownField));
        }

        [Test]
        public void DefinitionAsField()
        {
            SetupDefaultEnclosingMethod(
                InvokeStmt("f", SomeMethodOnType("F")));

            var fieldName = Field(Type("F"), DefaultClassContext, "f");
            Context.SST.Fields.Add(
                new FieldDeclaration
                {
                    Name = fieldName
                });

            AssertSingleQueryWithDefinition(DefinitionSites.CreateDefinitionByField(fieldName));
        }

        [Test]
        public void VarDefinitionByAssignment_Variable()
        {
            SetupDefaultEnclosingMethod(
                VarDecl("a", Type("A")),
                VarDecl("b", Type("B")),
                Assign(
                    "a",
                    new ReferenceExpression
                    {
                        Reference = VarRef("b")
                    }),
                InvokeStmt("a", SomeMethodOnType("A")));

            AssertSingleQueryWithDefinition(DefinitionSites.CreateUnknownDefinitionSite());
        }

        [Test]
        public void VarDefinitionByAssignment_Field()
        {
            SetupDefaultEnclosingMethod(
                VarDecl("other", Type("O")),
                VarDecl("a", Type("A")),
                Assign(
                    "a",
                    new ReferenceExpression
                    {
                        Reference = new FieldReference
                        {
                            Reference = VarRef("Other"),
                            FieldName = Field(Type("A"), Type("O"), "f")
                        }
                    }),
                InvokeStmt("a", SomeMethodOnType("A")));

            AssertSingleQueryWithDefinition(DefinitionSites.CreateUnknownDefinitionSite());
        }

        [Test]
        public void VarDefinitionByAssignment_Property()
        {
            SetupDefaultEnclosingMethod(
                VarDecl("other", Type("O")),
                VarDecl("a", Type("A")),
                Assign(
                    "a",
                    new ReferenceExpression
                    {
                        Reference = new PropertyReference
                        {
                            Reference = VarRef("Other"),
                            PropertyName = Property(Type("A"), Type("O"), "p")
                        }
                    }),
                InvokeStmt("a", SomeMethodOnType("A")));

            AssertSingleQueryWithDefinition(DefinitionSites.CreateUnknownDefinitionSite());
        }
    }
}