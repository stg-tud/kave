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
using KaVE.Commons.Utils.Assertion;
using KaVE.Commons.Utils.Collections;
using KaVE.Commons.Utils.ObjectUsageExport;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Utils.ObjectUsageExport
{
    internal class UsageContextTest
    {
        private static readonly ITypeName SomeType = Names.Type("T,P");
        private static readonly IMethodName SomeMethodName = Names.Method("[A,P] [B,P].M()");

        private UsageContext _sut;

        [SetUp]
        public void SetUp()
        {
            _sut = new UsageContext
            {
                Enclosings =
                {
                    Type = SomeType,
                    Method = SomeMethodName
                }
            };
        }

        [Test]
        public void DefaultValues()
        {
            var sut = new UsageContext();
            Assert.AreEqual(Names.UnknownType, sut.Enclosings.Type);
            Assert.AreEqual(Names.UnknownMethod, sut.Enclosings.Method);
            Assert.AreEqual(Lists.NewList<Query>(), sut.AllQueries);
            Assert.AreEqual(new ScopedNameResolver(), sut.NameResolver);
        }

        [Test]
        public void SettingValues()
        {
            var sut = new UsageContext
            {
                Enclosings =
                {
                    Type = SomeType,
                    Method = SomeMethodName
                }
            };
            Assert.AreEqual(SomeType, sut.Enclosings.Type);
            Assert.AreEqual(SomeMethodName, sut.Enclosings.Method);
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

        [Test, ExpectedException(typeof(AssertException))]
        public void CannotLeaveMainScope()
        {
            _sut.LeaveCurrentScope();
        }

        [Test]
        public void EnteringLambdasClonesReachableVariablesAndAdaptsContext()
        {
            _sut.Enclosings.Type = Type("T1");
            _sut.Enclosings.Method = Method(Type("T2"), "M");

            _sut.DefineVariable("v", Type("V"), DefinitionByThis());
            _sut.EnterNewLambdaScope();

            AssertQueries(
                new Query
                {
                    type = Type("V").ToCoReName(),
                    classCtx = Type("T1").ToCoReName(),
                    methodCtx = Method(Type("T2"), "M").ToCoReName(),
                    definition = DefinitionByThis()
                },
                new Query
                {
                    type = Type("V").ToCoReName(),
                    classCtx = Type("T1$Lambda").ToCoReName(),
                    methodCtx = Method(Type("T2"), "M$Lambda").ToCoReName(),
                    definition = DefinitionByThis()
                });
        }

        [Test]
        public void EnteringLambdasTwiceAppendsMarkerTwice()
        {
            _sut.Enclosings.Type = Type("T1");
            _sut.Enclosings.Method = Method(Type("T2"), "M");

            _sut.DefineVariable("v", Type("V"), DefinitionByThis());
            _sut.EnterNewLambdaScope();
            _sut.EnterNewLambdaScope();

            AssertQueries(
                new Query
                {
                    type = Type("V").ToCoReName(),
                    classCtx = Type("T1").ToCoReName(),
                    methodCtx = Method(Type("T2"), "M").ToCoReName(),
                    definition = DefinitionByThis()
                },
                new Query
                {
                    type = Type("V").ToCoReName(),
                    classCtx = Type("T1$Lambda").ToCoReName(),
                    methodCtx = Method(Type("T2"), "M$Lambda").ToCoReName(),
                    definition = DefinitionByThis()
                },
                new Query
                {
                    type = Type("V").ToCoReName(),
                    classCtx = Type("T1$Lambda$Lambda").ToCoReName(),
                    methodCtx = Method(Type("T2"), "M$Lambda$Lambda").ToCoReName(),
                    definition = DefinitionByThis()
                });
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
            return Names.Type(name + ", P");
        }

        private static IMethodName Method(ITypeName type, string methodName)
        {
            return Names.Method(string.Format("[T,P] [{0}].{1}()", type, methodName));
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