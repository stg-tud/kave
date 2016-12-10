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
using System.Globalization;
using System.Linq;
using KaVE.Commons.Model.Events;
using KaVE.Commons.Model.Events.CompletionEvents;
using KaVE.Commons.Model.Naming;
using KaVE.Commons.Model.Naming.CodeElements;
using KaVE.Commons.Model.SSTs;
using KaVE.Commons.Model.SSTs.Impl.Declarations;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Assignable;
using KaVE.Commons.Model.SSTs.Impl.Statements;
using KaVE.Commons.Model.TypeShapes;
using KaVE.Commons.TestUtils.Model.Naming;
using KaVE.Commons.Utils.Collections;

namespace KaVE.Commons.TestUtils.Model.Events.CompletionEvent
{
    public static class CompletionEventTestFactory
    {
        private static int _counter;

        private static int NextCounter()
        {
            return ++_counter;
        }

        public static void Reset()
        {
            _counter = 0;
        }

        public static Commons.Model.Events.CompletionEvents.CompletionEvent CreateAnonymousCompletionEvent(int duration)
        {
            var now = DateTime.Now;
            return new Commons.Model.Events.CompletionEvents.CompletionEvent
            {
                TriggeredAt = now,
                TriggeredBy = EventTrigger.Shortcut,
                TerminatedAt = now.AddTicks(duration*TimeSpan.TicksPerMillisecond)
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
            return new Proposal {Name = Names.General(Guid.NewGuid().ToString())};
        }

        private static IEnumerable<Proposal> CreatePredictableProposals(uint numberOfProposals)
        {
            var proposals = new List<Proposal>();
            for (var i = 0; i < numberOfProposals; i++)
            {
                proposals.Add(CreatePredictableProposal());
            }
            return proposals;
        }

        public static IList<ProposalSelection> CreatePredictableProposalSelections(uint numberOfProposals)
        {
            return CreatePredictableProposals(numberOfProposals).Select(p => new ProposalSelection(p)).ToList();
        }

        private static Proposal CreatePredictableProposal()
        {
            return new Proposal {Name = Names.General(NextCounter().ToString(CultureInfo.InvariantCulture))};
        }

        public static ITypeHierarchy GetAnonymousTypeHierarchy()
        {
            return new TypeHierarchy(TestNameFactory.GetAnonymousTypeName().Identifier);
        }

        public static ISet<IMemberHierarchy<IMethodName>> GetAnonymousMethodHierarchies(uint numberOfElements)
        {
            var hierarchies = Sets.NewHashSet<IMemberHierarchy<IMethodName>>();
            for (var i = 0; i < numberOfElements; i++)
            {
                hierarchies.Add(GetAnonymousMethodHierarchy());
            }
            return hierarchies;
        }

        private static MethodHierarchy GetAnonymousMethodHierarchy()
        {
            return new MethodHierarchy(TestNameFactory.GetAnonymousMethodName());
        }

        public static void AddPrefix(this ICompletionEvent completionEvent, string newPrefix)
        {
            var methodDeclarationContainingCompletionExpression = new MethodDeclaration
            {
                Body =
                    Lists.NewList<IStatement>(
                        new ExpressionStatement
                        {
                            Expression = new CompletionExpression {Token = newPrefix}
                        })
            };

            completionEvent.Context2.SST.Methods.Add(methodDeclarationContainingCompletionExpression);
        }
    }
}