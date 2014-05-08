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
using System.Collections.Generic;
using KaVE.Model.Events;
using KaVE.Model.Events.CompletionEvent;
using KaVE.Model.Names.CSharp;

namespace KaVE.TestUtils.Model.Events.CompletionEvent
{
    public static class CompletionEventTestFactory
    {
        public static KaVE.Model.Events.CompletionEvent.CompletionEvent CreateAnonymousCompletionEvent(int duration)
        {
            var now = DateTime.Now;
            return new KaVE.Model.Events.CompletionEvent.CompletionEvent
            {
                TriggeredAt = now,
                TriggeredBy = IDEEvent.Trigger.Shortcut,
                TerminatedAt = now.AddTicks(duration * TimeSpan.TicksPerMillisecond)
            };
        }

        public static IList<Proposal> CreateAnonymousProposals(uint numberOfProposals)
        {
            var proposals = new List<Proposal>();
            for (var i = 0; i < numberOfProposals; i++)
            {
                proposals.Add(CreateAnonymousProposal());
            }
            return proposals;
        }

        public static Proposal CreateAnonymousProposal()
        {
            return new Proposal { Name = Name.Get(Guid.NewGuid().ToString()) };
        }
    }
}
