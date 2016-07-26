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

using KaVE.Commons.Model.Events.CompletionEvents;
using KaVE.Commons.Utils;
using KaVE.Commons.Utils.Json;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Utils.Json.JsonSerializationSuite.CompletionEventSuite
{
    internal class ProposalCollectionSerializationTest
    {
        private const string SingleProposal = "{\"$type\":\"KaVE.Model.Events.CompletionEvents.Proposal, KaVE.Model\",\"Name\":\"CSharp.MethodName:[T1,P1] [T1,P2].M1()\",\"Relevance\":42}";

        [Test]
        public void DeserializeCurrent()
        {
            "[{0}]".FormatEx(SingleProposal).ParseJsonTo<ProposalCollection>();
        }

        [Test]
        public void DeserializeLegacy()
        {
            "{{\"$type\":\"KaVE.Model.Events.CompletionEvent.ProposalCollection, KaVE.Model\",\"Proposals\":[{0}]}}".FormatEx(SingleProposal)
                .ParseJsonTo<ProposalCollection>();
        }
    }
}