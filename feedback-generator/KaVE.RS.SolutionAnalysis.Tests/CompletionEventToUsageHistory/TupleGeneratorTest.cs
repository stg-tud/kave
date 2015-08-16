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

using System;
using System.Globalization;
using KaVE.Commons.Model.Events.CompletionEvents;
using KaVE.Commons.Model.Names;
using KaVE.Commons.Model.Names.CSharp;
using KaVE.Commons.Model.ObjectUsage;
using KaVE.Commons.Model.SSTs.Impl;
using KaVE.Commons.Utils.Assertion;
using KaVE.Commons.Utils.Collections;
using KaVE.Commons.Utils.ObjectUsageExport;
using KaVE.RS.SolutionAnalysis.CompletionEventToUsageHistory;
using Moq;
using NUnit.Framework;

namespace KaVE.RS.SolutionAnalysis.Tests.CompletionEventToUsageHistory
{
    internal class TupleGeneratorTest
    {
        private IUsageExtractor _usageExtractor;
        private TupleGenerator _sut;

        [SetUp]
        public void Setup()
        {
            _usageExtractor = Mock.Of<IUsageExtractor>();
            _sut = new TupleGenerator(_usageExtractor);
        }

        [Test]
        public void GetIndex()
        {
            AssertIndex("123", Date(1970, 1, 2), Type(1234), "123_19700102_1234");
            AssertIndex("234", null, Type(2345), "234_00010101_2345");
        }

        [Test]
        public void FindFirstAndLast()
        {
            var ctx1 = Mock.Of<Context>();
            var ctx4 = Mock.Of<Context>();

            var e1 = Event(1970, 1, 1, ctx1);
            var e2 = Event(1970, 1, 2);
            var e3 = Event(1970, 1, 3);
            var e4 = Event(1970, 1, 4, ctx4);

            var actualRegular = _sut.FindFirstAndLast(Lists.NewList(e1, e2, e3, e4));
            var actualReverse = _sut.FindFirstAndLast(Lists.NewList(e4, e3, e2, e1));
            var actualRandom = _sut.FindFirstAndLast(Lists.NewList(e3, e1, e4, e2));

            var expected = Tuple.Create(ctx1, ctx4);
            Assert.AreEqual(expected, actualRegular);
            Assert.AreEqual(expected, actualReverse);
            Assert.AreEqual(expected, actualRandom);
        }

        [Test, ExpectedException(typeof (AssertException))]
        public void FindFirstAndLast_SanityCheckDifferentEvents()
        {
            var e1 = Event(1970, 1, 1);
            _sut.FindFirstAndLast(Lists.NewList(e1));
        }

        [Test]
        public void GenerateTuples_MethodIsAddedAndRemoved()
        {
            var ctxStart = CreateContext("T1");
            var ctxEnd = CreateContext("T2");

            var u1 = UsageA1("a1");
            var u2 = UsageB("b1", "b2");
            SetupUsageExport(ctxStart, u1, u2);

            var u3 = UsageA1("a1", "a2");
            var u4 = UsageB("b2");
            SetupUsageExport(ctxEnd, u3, u4);

            var actuals = _sut.GenerateTuples(ctxStart, ctxEnd);
            var expecteds = Lists.NewList(T(u1, u3), T(u2, u4));
            Assert.AreEqual(expecteds, actuals);
        }

        [Test]
        public void GenerateTuples_NoStartOrNoEnd()
        {
            var ctxStart = CreateContext("T1");
            var ctxEnd = CreateContext("T2");

            var u1 = UsageA1("a1");
            SetupUsageExport(ctxStart, u1);

            var u2 = UsageB("b1");
            SetupUsageExport(ctxEnd, u2);

            var artificialA = UsageA1();
            var artificialB = UsageB();

            var actuals = _sut.GenerateTuples(ctxStart, ctxEnd);
            var expecteds = Lists.NewList(T(u1, artificialA), T(artificialB, u2));
            Assert.AreEqual(expecteds, actuals);
        }

        [Test]
        public void GenerateTuples_NoChange()
        {
            var ctxStart = CreateContext("T1");
            var ctxEnd = CreateContext("T2");

            var u1 = UsageA1("a1");
            SetupUsageExport(ctxStart, u1);

            var u2 = UsageA1("a1");
            SetupUsageExport(ctxEnd, u2);

            var actuals = _sut.GenerateTuples(ctxStart, ctxEnd);
            var expecteds = Lists.NewList<Tuple<Query, Query>>();
            Assert.AreEqual(expecteds, actuals);
        }

        [Test]
        public void GenerateTuples_NoChangeOnlyDefSite()
        {
            var ctxStart = CreateContext("T1");
            var ctxEnd = CreateContext("T2");

            var u1 = UsageA1("a1");
            SetupUsageExport(ctxStart, u1);

            var u2 = UsageA2("a1");
            SetupUsageExport(ctxEnd, u2);

            var actuals = _sut.GenerateTuples(ctxStart, ctxEnd);
            var expecteds = Lists.NewList<Tuple<Query, Query>>();
            Assert.AreEqual(expecteds, actuals);
        }

        [Test]
        public void GenerateTuples_UnknownType()
        {
            var ctxStart = CreateContext("T1");
            var ctxEnd = CreateContext("T2");

            var u1 = Usage(TypeName.UnknownName, DefinitionSites.CreateUnknownDefinitionSite(), "a1");
            SetupUsageExport(ctxStart, u1);

            var u2 = Usage(TypeName.UnknownName, DefinitionSites.CreateUnknownDefinitionSite(), "a1", "a2");
            SetupUsageExport(ctxEnd, u2);

            var actuals = _sut.GenerateTuples(ctxStart, ctxEnd);
            var expecteds = Lists.NewList<Tuple<Query, Query>>();
            Assert.AreEqual(expecteds, actuals);
        }

        private void SetupUsageExport(Context ctx, params Query[] usages)
        {
            Mock.Get(_usageExtractor).Setup(u => u.Export(ctx)).Returns(Lists.NewListFrom(usages));
        }

        private static Context CreateContext(string type)
        {
            return new Context
            {
                SST = new SST
                {
                    EnclosingType = TypeName.Get(type + ",P")
                }
            };
        }

        private static Tuple<Query, Query> T(Query u1, Query u2)
        {
            var t1 = Tuple.Create(u1, u2);
            return t1;
        }

        private Query UsageA1(params string[] calls)
        {
            return Usage(TypeName.Get("A,P"), DefinitionSites.CreateDefinitionByConstant(), calls);
        }

        private Query UsageA2(params string[] calls)
        {
            return Usage(TypeName.Get("A,P"), DefinitionSites.CreateDefinitionByThis(), calls);
        }

        private Query UsageB(params string[] calls)
        {
            return Usage(TypeName.Get("B,P"), DefinitionSites.CreateUnknownDefinitionSite(), calls);
        }

        private Query Usage(ITypeName type, DefinitionSite defSite, params string[] calls)
        {
            var q = new Query
            {
                type = type.ToCoReName(),
                definition = defSite
            };
            foreach (var shortCall in calls)
            {
                var mStr = string.Format("[{0}] [{1}].{2}()", "System.Void", "A,P", shortCall);
                var m = MethodName.Get(mStr);
                var call = CallSites.CreateReceiverCallSite(m);
                q.sites.Add(call);
            }
            return q;
        }

        private CompletionEvent Event(int year, int month, int day, Context ctx = null)
        {
            if (ctx == null)
            {
                ctx = new Context();
            }
            return new CompletionEvent
            {
                TriggeredAt = Date(year, month, day),
                Context2 = ctx
            };
        }

        private DateTime Date(int year, int month, int day)
        {
            var dateStr = string.Format("{0}-{1:00}-{2:00}", year, month, day);
            return DateTime.ParseExact(dateStr, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        }

        private ITypeName Type(int hashcode)
        {
            return new TestTypeName(hashcode);
        }

        private void AssertIndex(string sessionId, DateTime? date, ITypeName type, string expected)
        {
            var e = new CompletionEvent
            {
                IDESessionUUID = sessionId,
                TriggeredAt = date,
                Context2 = new Context
                {
                    SST = new SST
                    {
                        EnclosingType = type
                    }
                }
            };

            var actual = _sut.GetTemporalIndex(e);
            Assert.AreEqual(expected, actual);
        }


        internal class TestTypeName : TypeName
        {
            private readonly int _hashCode;

            public TestTypeName(int hashCode)
                : base(string.Format("T{0},P", hashCode))
            {
                _hashCode = hashCode;
            }

            public override int GetHashCode()
            {
                return _hashCode;
            }
        }
    }
}