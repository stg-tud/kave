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

using System.Linq;
using KaVE.Commons.Model.Events.CompletionEvents;
using KaVE.Commons.Model.Naming;
using KaVE.Commons.Model.Naming.CodeElements;
using KaVE.Commons.Model.Naming.Types;
using KaVE.Commons.Model.ObjectUsage;
using KaVE.Commons.Model.SSTs;
using KaVE.Commons.Model.SSTs.Expressions;
using KaVE.Commons.Model.SSTs.Expressions.Assignable;
using KaVE.Commons.Model.SSTs.Impl;
using KaVE.Commons.Model.SSTs.Impl.Declarations;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Assignable;
using KaVE.Commons.Model.SSTs.Impl.References;
using KaVE.Commons.Model.SSTs.Impl.Statements;
using KaVE.Commons.Model.SSTs.References;
using KaVE.Commons.Model.SSTs.Statements;
using KaVE.Commons.Model.TypeShapes;
using KaVE.Commons.Utils.Collections;
using KaVE.Commons.Utils.ObjectUsageExport;
using NUnit.Framework;
using Fix = KaVE.Commons.Tests.Utils.ObjectUsageExport.UsageExtractorTestSuite.ObjectUsageExporterTestFixture;

namespace KaVE.Commons.Tests.Utils.ObjectUsageExport.UsageExtractorTestSuite
{
    public class BaseObjectUsageExporterTest
    {
        protected Context Context;
        protected UsageExtractor Sut;

        protected static ITypeName DefaultClassContext
        {
            get { return Type("TDecl"); }
        }

        protected static IMethodName DefaultMethodContext
        {
            get { return Method(Type("A"), DefaultClassContext, "M"); }
        }

        [SetUp]
        public void SetUp()
        {
            Context = new Context();
            Sut = new UsageExtractor();
        }

        protected void SetupDefaultEnclosingMethod(params IStatement[] statements)
        {
            SetupEnclosingMethod(
                DefaultMethodContext,
                statements);
        }

        protected void SetupEnclosingMethod(IMethodName enclosingMethod, params IStatement[] statements)
        {
            Context.TypeShape.TypeHierarchy = new TypeHierarchy
            {
                Element = DefaultClassContext
            };

            Context.TypeShape.MethodHierarchies.Add(
                new MethodHierarchy
                {
                    Element = enclosingMethod
                });

            Context.SST = new SST
            {
                EnclosingType = enclosingMethod.DeclaringType,
                Methods =
                {
                    new MethodDeclaration
                    {
                        Name = enclosingMethod,
                        Body = Lists.NewListFrom(statements)
                    }
                }
            };
        }

        protected void ResetMethodHierarchies(params MethodHierarchy[] methodHierarchies)
        {
            Context.TypeShape.MethodHierarchies.Clear();
            foreach (var methodHierarchy in methodHierarchies)
            {
                Context.TypeShape.MethodHierarchies.Add(methodHierarchy);
            }
        }

        protected void AssertQueriesInDefault(params Query[] expecteds)
        {
            AssertQueries(DefaultMethodContext, expecteds);
        }

        protected void AssertQueries(IMethodName enclosingMethod, params Query[] expecteds)
        {
            AssertQueries(enclosingMethod.DeclaringType, enclosingMethod, expecteds);
        }

        protected void AssertQueries(ITypeName enclosingClass, IMethodName enclosingMethod, params Query[] expecteds)
        {
            foreach (var expected in expecteds)
            {
                expected.classCtx = enclosingClass.ToCoReName();
                expected.methodCtx = enclosingMethod.ToCoReName();
            }
            AssertQueriesWithoutSettingContexts(expecteds);
        }

        protected void AssertQueriesWithoutSettingContexts(params Query[] expectedsArr)
        {
            var actuals = Sut.Export(Context);
            var expecteds = Lists.NewList(expectedsArr);
            CollectionAssert.AreEqual(expecteds, actuals);
        }

        protected Query FindQueryWith(ITypeName type)
        {
            var actuals = Sut.Export(Context);
            foreach (Query actual in actuals)
            {
                if (Equals(actual.type, type.ToCoReName()))
                {
                    return actual;
                }
            }
            Assert.Fail("no query found for type {0}", type);
            return null;
        }

        protected void AssertQueriesExistFor(params ITypeName[] expecteds)
        {
            var expectedsCore = expecteds.Select(t => t.ToCoReName());
            var actuals = Sut.Export(Context).Select(q => q.type);
            CollectionAssert.AreEquivalent(expectedsCore, actuals);
        }

        protected Query AssertSingleQuery()
        {
            var actuals = Sut.Export(Context);
            Assert.AreEqual(1, actuals.Count);
            return actuals[0];
        }

        protected void AssertSingleQueryWithType(ITypeName expected)
        {
            var actual = AssertSingleQuery().type;
            Assert.AreEqual(expected.ToCoReName(), actual);
        }

        protected void AssertSingleQueryWithDefinition(DefinitionSite expected)
        {
            var actual = AssertSingleQuery().definition;
            Assert.AreEqual(expected, actual);
        }

        #region instantiation helpers

        protected static IMethodName Method(ITypeName retType,
            ITypeName declType,
            string simpleName,
            params IParameterName[] parameters)
        {
            var parameterStr = string.Join(", ", parameters.Select(p => p.Identifier));
            return Names.Method("[{0}] [{1}].{2}({3})", retType, declType, simpleName, parameterStr);
        }

        protected static IFieldName Field(ITypeName valType,
            ITypeName declType,
            string fieldName)
        {
            return Names.Field("[{0}] [{1}].{2}", valType, declType, fieldName);
        }

        protected static IParameterName Parameter(ITypeName valType, string paramName)
        {
            return Names.Parameter("[{0}] {1}", valType, paramName);
        }

        protected static ITypeName Type(string simpleName)
        {
            return Names.Type(simpleName + ",P1");
        }

        protected static IPropertyName Property(ITypeName valType,
            ITypeName declType,
            string propertyName)
        {
            return Names.Property("get set [{0}] [{1}].{2}()", valType, declType, propertyName);
        }

        protected static IVariableReference VarRef(string id)
        {
            return new VariableReference
            {
                Identifier = id
            };
        }

        protected static IInvocationExpression Constructor(ITypeName type)
        {
            return InvokeStatic(Method(Fix.Void, type, ".ctor"));
        }

        protected static IAssignment Assign(string varName, IAssignableExpression expr)
        {
            return new Assignment
            {
                Reference = VarRef(varName),
                Expression = expr
            };
        }

        protected static IInvocationExpression Invoke(string varName, IMethodName method)
        {
            return new InvocationExpression
            {
                Reference = VarRef(varName),
                MethodName = method
            };
        }

        private static IInvocationExpression InvokeStatic(IMethodName method)
        {
            return new InvocationExpression
            {
                MethodName = method
            };
        }

        protected static IExpressionStatement InvokeStmt(string varName, IMethodName method)
        {
            return new ExpressionStatement
            {
                Expression = Invoke(varName, method)
            };
        }

        protected static IVariableDeclaration VarDecl(string name, ITypeName type)
        {
            return new VariableDeclaration
            {
                Reference = VarRef(name),
                Type = type
            };
        }

        protected static CallSite SomeCallSiteOnType(string typeName)
        {
            return CallSites.CreateReceiverCallSite(Method(Fix.Void, Type(typeName), "M"));
        }

        protected static IMethodName SomeMethodOnType(string typeName)
        {
            return Method(Fix.Void, Type(typeName), "M");
        }

        #endregion
    }
}