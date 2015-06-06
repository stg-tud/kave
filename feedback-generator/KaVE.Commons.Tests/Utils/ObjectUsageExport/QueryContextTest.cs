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

using KaVE.Commons.Model.Names;
using KaVE.Commons.Model.Names.CSharp;
using KaVE.Commons.Model.ObjectUsage;
using KaVE.Commons.Utils.Assertion;
using KaVE.Commons.Utils.Collections;
using KaVE.Commons.Utils.ObjectUsageExport;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Utils.ObjectUsageExport
{
    internal class QueryContextTest
    {
        private static readonly ITypeName SomeType = TypeName.Get("T,P");
        private static readonly MethodName SomeMethodName = MethodName.Get("[A,P] [B,P].M()");

        private QueryContext _sut;

        [SetUp]
        public void SetUp()
        {
            _sut = new QueryContext
            {
                EnclosingType = SomeType,
                EnclosingMethod = SomeMethodName
            };
        }

        [Test]
        public void DefaultValues()
        {
            var sut = new QueryContext();
            Assert.AreEqual(TypeName.UnknownName, sut.EnclosingType);
            Assert.AreEqual(MethodName.UnknownName, sut.EnclosingMethod);
            Assert.AreEqual(Lists.NewList<Query>(), sut.AllQueries);
            Assert.AreEqual(new QueryScope(), sut.Scope);
        }

        [Test]
        public void SettingValues()
        {
            var sut = new QueryContext
            {
                EnclosingType = SomeType,
                EnclosingMethod = SomeMethodName
            };
            Assert.AreEqual(SomeType, sut.EnclosingType);
            Assert.AreEqual(SomeMethodName, sut.EnclosingMethod);
        }

        [Test]
        public void CreatingAQuery()
        {
            _sut.DefineVariable("id", Type("T"), DefinitionByThis());

            AssertQueries(
                CreateQueryWithDefaults(
                    Type("T"),
                    DefinitionByThis()));
        }

        [Test]
        public void RegisterInvocation()
        {
            _sut.DefineVariable("t", Type("T"), DefinitionByThis());
            _sut.RegisterCallsite("t", Method(Type("T"), "M"));

            AssertQueries(
                CreateQueryWithDefaults(
                    Type("T"),
                    DefinitionByThis(),
                    Method(Type("T"), "M")));
        }

        [Test]
        public void RegisterDefinition()
        {
            _sut.DefineVariable("t", Type("T"), DefinitionByUnknown());
            _sut.RegisterDefinition("t", DefinitionByThis());

            AssertQueries(
                CreateQueryWithDefaults(
                    Type("T"),
                    DefinitionByThis()));
        }

        [Test]
        public void RegisteringCallsForUndeclaredIdentifierStoresThemUnderValueType()
        {
            _sut.RegisterCallsite("t", Method(Type("T"), "M"));
            _sut.RegisterCallsite("t", Method(Type("U"), "M"));

            AssertQueries(
                CreateQueryWithDefaults(
                    Type("T"),
                    DefinitionByUnknown(),
                    Method(Type("T"), "M")),
                CreateQueryWithDefaults(
                    Type("U"),
                    DefinitionByUnknown(),
                    Method(Type("U"), "M")));
        }

        [Test]
        public void CreatingSameTypeForDifferentIdsGetsMerged()
        {
            _sut.DefineVariable("t1", Type("T"), DefinitionByThis());
            _sut.RegisterCallsite("t1", Method(Type("T"), "M1"));

            _sut.DefineVariable("t2", Type("T"), DefinitionByReturn("X", Type("T")));
            _sut.RegisterCallsite("t2", Method(Type("T"), "M2"));

            AssertQueries(
                CreateQueryWithDefaults(
                    Type("T"),
                    DefinitionByThis(),
                    Method(Type("T"), "M1"),
                    Method(Type("T"), "M2")));
        }

        [Test]
        public void CreatingSameTypeInNewScopeCreatesNewQuery()
        {
            _sut.DefineVariable("t", Type("T"), DefinitionByThis());
            _sut.RegisterCallsite("t", Method(Type("T"), "M1"));
            _sut.EnterNewScope();
            _sut.DefineVariable("t", Type("T"), DefinitionByReturn("X", Type("T")));
            _sut.RegisterCallsite("t", Method(Type("T"), "M2"));

            AssertQueries(
                CreateQueryWithDefaults(
                    Type("T"),
                    DefinitionByThis(),
                    Method(Type("T"), "M1")),
                CreateQueryWithDefaults(
                    Type("T"),
                    DefinitionByReturn("X", Type("T")),
                    Method(Type("T"), "M2")));
        }

        [Test]
        public void TypeMapsBackToOriginalScopeAfterLeavingInnerScope()
        {
            _sut.DefineVariable("t", Type("T"), DefinitionByThis());
            _sut.RegisterCallsite("t", Method(Type("T"), "M1"));

            _sut.EnterNewScope();
            _sut.DefineVariable("t", Type("T"), DefinitionByReturn("X", Type("T")));
            _sut.RegisterCallsite("t", Method(Type("T"), "M2"));
            _sut.LeaveCurrentScope();

            _sut.RegisterCallsite("t", Method(Type("T"), "M3"));

            AssertQueries(
                CreateQueryWithDefaults(
                    Type("T"),
                    DefinitionByThis(),
                    Method(Type("T"), "M1"),
                    Method(Type("T"), "M3")),
                CreateQueryWithDefaults(
                    Type("T"),
                    DefinitionByReturn("X", Type("T")),
                    Method(Type("T"), "M2")));
        }

        [Test, ExpectedException(typeof (AssertException))]
        public void CannotLeaveMainScope()
        {
            _sut.LeaveCurrentScope();
        }

        #region test helper

        private Query CreateQueryWithDefaults(ITypeName type, DefinitionSite def, params IMethodName[] methods)
        {
            var query = new Query
            {
                type = type.ToCoReName(),
                classCtx = SomeType.ToCoReName(),
                methodCtx = SomeMethodName.ToCoReName(),
                definition = def
            };
            foreach (IMethodName method in methods)
            {
                query.sites.Add(CallSites.CreateReceiverCallSite(method));
            }
            return query;
        }

        private void AssertQueries(params Query[] queries)
        {
            var expected = Lists.NewList(queries);
            var actual = _sut.AllQueries;
            Assert.AreEqual(expected, actual);
        }

        private static ITypeName Type(string name)
        {
            return TypeName.Get(name + ", P");
        }

        private static IMethodName Method(ITypeName type, string methodName)
        {
            return MethodName.Get(string.Format("[T,P] [{0},P].{1}()", type, methodName));
        }

        private static CallSite CallSite(ITypeName type, string methodName)
        {
            var method = Method(type, methodName);
            return CallSites.CreateReceiverCallSite(method);
        }

        private static DefinitionSite DefinitionByReturn(string methodName, ITypeName returnType)
        {
            return DefinitionSites.CreateDefinitionByReturn(Method(returnType, methodName));
        }

        private static DefinitionSite DefinitionByThis()
        {
            return DefinitionSites.CreateDefinitionByThis();
        }

        private static DefinitionSite DefinitionByUnknown()
        {
            return DefinitionSites.CreateUnknownDefinitionSite();
        }

        #endregion
    }
}