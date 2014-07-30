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
 *    - Sven Amann
 */

using System.Collections.Generic;
using System.Linq;
using JetBrains.Util;
using KaVE.Model.Events;
using KaVE.Model.Events.CompletionEvent;
using KaVE.Model.Names;
using KaVE.Utils.Assertion;

namespace KaVE.VsFeedbackGenerator.SessionManager.Anonymize
{
    internal class CompletionEventAnonymizer : IDEEventAnonymizer
    {
        public override void AnonymizeDurations(IDEEvent ideEvent)
        {
            var completionEvent = (CompletionEvent) ideEvent;
            completionEvent.Selections.ForEach(selection => selection.SelectedAfter = null);
            base.AnonymizeDurations(ideEvent);
        }

        public override void AnonymizeCodeNames(IDEEvent ideEvent)
        {
            var completionEvent = (CompletionEvent) ideEvent;
            completionEvent.ProposalCollection.Proposals.ForEach(AnonymizeCodeNames);
            completionEvent.Selections.ForEach(AnonymizeCodeNames);
            AnonymizeCodeNames(completionEvent.Context);
            base.AnonymizeCodeNames(completionEvent);
        }

        private static void AnonymizeCodeNames(ProposalSelection selection)
        {
            AnonymizeCodeNames(selection.Proposal);
        }

        private static void AnonymizeCodeNames(Proposal proposal)
        {
            proposal.Name = proposal.Name.ToAnonymousName();
        }

        private static void AnonymizeCodeNames(Context context)
        {
            context.EnclosingMethod = context.EnclosingMethod.ToAnonymousName();
            context.TriggerTarget = context.TriggerTarget.ToAnonymousName();
            AnonymizeCodeNames(context.TypeShape.TypeHierarchy);
            context.TypeShape.MethodHierarchies.ForEach(AnonymizeCodeNames);
            AnonymizeCodeNames(context.EntryPointToCalledMethods);
        }

        private static void AnonymizeCodeNames(ITypeHierarchy hierarchy)
        {
            hierarchy.Element = hierarchy.Element.ToAnonymousName();
            if (hierarchy.Extends != null)
            {
                AnonymizeCodeNames(hierarchy.Extends);
            }
            hierarchy.Implements.ForEach(AnonymizeCodeNames);
        }

        private static void AnonymizeCodeNames(MethodHierarchy hierarchy)
        {
            hierarchy.Element = hierarchy.Element.ToAnonymousName();
            if (hierarchy.Super != null)
            {
                hierarchy.Super = hierarchy.Super.ToAnonymousName();
            }
            if (hierarchy.First != null)
            {
                hierarchy.First = hierarchy.First.ToAnonymousName();
            }
        }

        private static void AnonymizeCodeNames(IDictionary<IMethodName, ISet<IMethodName>> entryPointToCalledMethods)
        {
            var anonymizedEntryPoints = new Dictionary<IMethodName, ISet<IMethodName>>();
            foreach (var entryPointToCalledMethod in entryPointToCalledMethods)
            {
                var entryPoint = entryPointToCalledMethod.Key.ToAnonymousName();
                Asserts.NotNull(entryPoint, "dictionary key cannot be null");
                var calledMethods = ToAnonymousNames(entryPointToCalledMethod.Value);
                anonymizedEntryPoints[entryPoint] = calledMethods;
            }
            entryPointToCalledMethods.Clear();
            entryPointToCalledMethods.AddRange(anonymizedEntryPoints);
        }

        private static ISet<IMethodName> ToAnonymousNames(IEnumerable<IMethodName> entryPointToCalledMethod)
        {
            return new HashSet<IMethodName>(entryPointToCalledMethod.Select(method => method.ToAnonymousName()));
        }
    }
}