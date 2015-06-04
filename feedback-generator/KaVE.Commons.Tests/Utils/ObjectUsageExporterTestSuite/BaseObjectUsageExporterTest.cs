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
 *    - Roman Fojtik
 */

using System.Collections.Generic;
using System.Linq;
using KaVE.Commons.Model.Events.CompletionEvents;
using KaVE.Commons.Model.Names;
using KaVE.Commons.Model.Names.CSharp;
using KaVE.Commons.Model.ObjectUsage;
using KaVE.Commons.Model.SSTs;
using KaVE.Commons.Model.SSTs.Declarations;
using KaVE.Commons.Model.SSTs.Impl;
using KaVE.Commons.Model.SSTs.Impl.Declarations;
using KaVE.Commons.Model.SSTs.Impl.References;
using KaVE.Commons.Model.SSTs.References;
using KaVE.Commons.Utils.ObjectUsageExport;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Utils.ObjectUsageExporterTestSuite
{
    public class BaseObjectUsageExporterTest
    {
        protected Context Ctx;
        protected ObjectUsageExporter Sut;

        [SetUp]
        public void SetUp()
        {
            Ctx = new Context();
            Sut = new ObjectUsageExporter();
        }

        protected static IMethodName Method(ITypeName retType,
            ITypeName declType,
            string simpleName,
            params IParameterName[] parameters)
        {
            var methodStart = string.Format("[{0}] [{1}].{2}(", retType, declType, simpleName);

            var firstIteration = true;
            foreach (var parameterName in parameters)
            {
                if (firstIteration)
                {
                    firstIteration = false;
                }
                else
                {
                    methodStart += ", ";
                }
                methodStart += parameterName.Identifier;
            }

            return MethodName.Get(methodStart + ")");
        }

        protected static IFieldName Field(ITypeName valType,
            ITypeName declType,
            string fieldName)
        {
            var field = string.Format("[{0}] [{1}].{2}", valType, declType, fieldName);
            return FieldName.Get(field);
        }

        protected static IParameterName Parameter(ITypeName valType, string paramName)
        {
            var param = string.Format("[{0}] {1}", valType, paramName);
            return ParameterName.Get(param);
        }

        protected static ITypeName Type(string simpleName)
        {
            return TypeName.Get(simpleName + ",P1");
        }

        protected static IPropertyName Property(ITypeName valType,
            ITypeName declType,
            string propertyName)
        {
            var property = string.Format("[{0}] [{1}].{2}", valType, declType, propertyName);
            return PropertyName.Get(property);
        }

        protected static IVariableReference VarRef(string id)
        {
            return new VariableReference
            {
                Identifier = id
            };
        }

        public void SetupSST(IMethodName enclosingMethod,
            IFieldDeclaration[] fields,
            IPropertyDeclaration[] properties,
            params IStatement[] statements)
        {
            var methodDeclaration = new MethodDeclaration
            {
                Name = enclosingMethod
            };
            Ctx.SST = new SST
            {
                EnclosingType = enclosingMethod.DeclaringType,
                Methods =
                {
                    methodDeclaration
                }
            };

            foreach (var propertyDeclaration in properties)
            {
                Ctx.SST.Properties.Add(propertyDeclaration);
            }

            foreach (var fieldDeclaration in fields)
            {
                Ctx.SST.Fields.Add(fieldDeclaration);
            }

            foreach (var statement in statements)
            {
                methodDeclaration.Body.Add(statement);
            }
        }

        protected void SetupEnclosingMethod(IMethodName enclosingMethod, params IStatement[] statements)
        {
            SetupSST(enclosingMethod, new IFieldDeclaration[] {}, new IPropertyDeclaration[] {}, statements);
        }

        protected void AssertQueriesWithMethodCtx(IMethodName enclosingMethod,
            IMethodName methodCtx,
            params Query[] expecteds)
        {
            var actuals = ExportAndPreprocess(true, expecteds);

            Assert.AreEqual(expecteds.Length, actuals.Count);

            foreach (var actual in actuals)
            {
                actual.definition = DefinitionSites.CreateUnknownDefinitionSite();
            }

            foreach (var expected in expecteds)
            {
                expected.definition = DefinitionSites.CreateUnknownDefinitionSite();
                expected.classCtx = enclosingMethod.DeclaringType.ToCoReName();
                expected.methodCtx = methodCtx.ToCoReName();
                CollectionAssert.Contains(actuals, expected);
            }
        }

        protected void AssertQueries(IMethodName enclosingMethod,
            bool isIgnoringDefinitionSite,
            params Query[] expecteds)
        {
            var actuals = ExportAndPreprocess(isIgnoringDefinitionSite, expecteds);

            Assert.AreEqual(expecteds.Length, actuals.Count);

            foreach (var expected in expecteds)
            {
                if (isIgnoringDefinitionSite)
                {
                    expected.definition = DefinitionSites.CreateUnknownDefinitionSite();
                }
                expected.classCtx = enclosingMethod.DeclaringType.ToCoReName();
                expected.methodCtx = enclosingMethod.ToCoReName();
                CollectionAssert.Contains(actuals, expected);
            }
        }

        private ICollection<Query> ExportAndPreprocess(bool isIgnoringDefinitionSite, IEnumerable<Query> expecteds)
        {
            var actuals = Sut.Export(Ctx);

            actuals = Preprocess(isIgnoringDefinitionSite, expecteds, actuals);
            return actuals;
        }

        private static ICollection<Query> Preprocess(bool isIgnoringDefinitionSite,
            IEnumerable<Query> expecteds,
            ICollection<Query> actuals)
        {
            var isInterestedInThisQueries = false;
            foreach (var expected in expecteds)
            {
                if (expected.definition.kind == DefinitionSiteKind.THIS)
                {
                    isInterestedInThisQueries = true;
                }
            }
            if (!isInterestedInThisQueries)
            {
                actuals = actuals.Where(query => query.definition.kind != DefinitionSiteKind.THIS).ToList();
            }


            if (isIgnoringDefinitionSite)
            {
                foreach (var actual in actuals)
                {
                    actual.definition = DefinitionSites.CreateUnknownDefinitionSite();
                }
            }
            return actuals;
        }

        protected void AssertQueriesInDefault(params Query[] expecteds)
        {
            AssertQueries(Method(Type("A"), Type("TDecl"), "M"), true, expecteds);
        }

        protected void AssertQueriesCleanInDefault(params Query[] expecteds)
        {
            var enclosingMethod = Method(Type("A"), Type("TDecl"), "M");

            var actuals = Sut.CleanExport(Ctx);

            actuals = Preprocess(true, expecteds, actuals);

            Assert.AreEqual(expecteds.Length, actuals.Count);

            foreach (var expected in expecteds)
            {
                expected.definition = DefinitionSites.CreateUnknownDefinitionSite();
                expected.classCtx = enclosingMethod.DeclaringType.ToCoReName();
                expected.methodCtx = enclosingMethod.ToCoReName();
                CollectionAssert.Contains(actuals, expected);
            }
        }

        protected void AssertQueriesInDefaultWithDefSite(params Query[] expecteds)
        {
            AssertQueries(Method(Type("A"), Type("TDecl"), "M"), false, expecteds);
        }

        protected void SetupDefaultEnclosingMethod(params IStatement[] statements)
        {
            SetupEnclosingMethod(
                Method(Type("A"), Type("TDecl"), "M"),
                statements);
        }

        protected void SetupDefaultEnclosingMethodWithFields(IFieldDeclaration[] fields, params IStatement[] statements)
        {
            SetupSST(
                Method(Type("A"), Type("TDecl"), "M"),
                fields,
                new IPropertyDeclaration[] {},
                statements);
        }

        protected void SetupDefaultEnclosingMethodWithProperties(IPropertyDeclaration[] properties,
            params IStatement[] statements)
        {
            SetupSST(
                Method(Type("A"), Type("TDecl"), "M"),
                new IFieldDeclaration[] {},
                properties,
                statements);
        }
    }
}