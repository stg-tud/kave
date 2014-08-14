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
 *    - Dennis Albrecht
 */

using System.Collections.Generic;
using KaVE.Model.ObjectUsage;
using KaVE.VsFeedbackGenerator.CodeCompletion;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.CodeCompletion
{
    [TestFixture]
    internal class UsageModelTest
    {
        [Test, Ignore]
        public void SaveFixtureToDisk()
        {
            UsageModelFixture.Network().WriteFile("c:/.../Network.xdsl");
        }

        [Test]
        public void ShouldNotProduceAnyProposalsIfAllMethodsAreAlreadyCalled()
        {
            var net = UsageModelFixture.Network();
            var model = new UsageModel(net);
            var query = new Query();
            query.sites.Add(new CallSite {call = new CoReMethodName("LType.Init()LReturn;")});
            query.sites.Add(new CallSite {call = new CoReMethodName("LType.Execute()LReturn;")});
            query.sites.Add(new CallSite {call = new CoReMethodName("LType.Finish()LReturn;")});
            var expected = new Dictionary<CoReMethodName, double>();

            var actual = model.Query(query);

            UsageModelFixture.AssertEquivalenceIgnoringRoundingErrors(expected, actual);
        }

        [Test]
        public void ShouldProduceAllProposalsIfNoMethodsAreAlreadyCalled()
        {
            var net = UsageModelFixture.Network();
            var model = new UsageModel(net);
            var query = new Query();
            var expected = new Dictionary<CoReMethodName, double>
            {
                {new CoReMethodName("LType.Init()LReturn;"), 0.55},
                {new CoReMethodName("LType.Execute()LReturn;"), 0.475},
                {new CoReMethodName("LType.Finish()LReturn;"), 0.425}
            };

            var actual = model.Query(query);

            UsageModelFixture.AssertEquivalenceIgnoringRoundingErrors(expected, actual);
        }

        [Test]
        public void ShouldProduceSomeProposalsIfSomeMethodsAreAlreadyCalled()
        {
            var net = UsageModelFixture.Network();
            var model = new UsageModel(net);
            var query = new Query();
            query.sites.Add(new CallSite { call = new CoReMethodName("LType.Init()LReturn;") });
            var expected = new Dictionary<CoReMethodName, double>
            {
                {new CoReMethodName("LType.Execute()LReturn;"), 0.639},
                {new CoReMethodName("LType.Finish()LReturn;"), 0.152}
            };

            var actual = model.Query(query);

            UsageModelFixture.AssertEquivalenceIgnoringRoundingErrors(expected, actual);
        }

        [Test]
        public void ShouldStillWorkIfUnknownMethodsAreCalled()
        {
            var net = UsageModelFixture.Network();
            var model = new UsageModel(net);
            var query = new Query();
            query.sites.Add(new CallSite { call = new CoReMethodName("LType.Init()LReturn;") });
            query.sites.Add(new CallSite { call = new CoReMethodName("LOtherType.SomeMethod()LResult;") });
            var expected = new Dictionary<CoReMethodName, double>
            {
                {new CoReMethodName("LType.Execute()LReturn;"), 0.639},
                {new CoReMethodName("LType.Finish()LReturn;"), 0.152}
            };

            var actual = model.Query(query);

            UsageModelFixture.AssertEquivalenceIgnoringRoundingErrors(expected, actual);
        }
    }
}