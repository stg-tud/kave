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
using System.Linq;
using System.Numerics;
using System.Text.RegularExpressions;
using JetBrains.Application;
using KaVE.Commons.Model.Events;
using KaVE.Commons.Model.Events.CompletionEvents;
using KaVE.JetBrains.Annotations;
using KaVE.VS.FeedbackGenerator.MessageBus;
using KaVE.VS.Statistics.Calculators.BaseClasses;
using KaVE.VS.Statistics.Filters;
using KaVE.VS.Statistics.StatisticListing;
using KaVE.VS.Statistics.Statistics;
using KaVE.VS.Statistics.Utils;

namespace KaVE.VS.Statistics.Calculators
{
    [ShellComponent]
    public class CompletionCalculator : StatisticCalculator
    {
        public CompletionCalculator(IStatisticListing statisticListing,
            IMessageBus messageBus,
            IErrorHandler errorHandler)
            : base(statisticListing, messageBus, errorHandler, typeof (CompletionStatistic), new CompletionFilter()) {}

        protected override IStatistic Process(IDEEvent @event)
        {
            var completionEvent = @event as CompletionEvent;
            if (completionEvent == null)
            {
                return null;
            }

            var completionStatistic = (CompletionStatistic) StatisticListing.GetStatistic(StatisticType);

            completionStatistic.TotalCompletions++;

            if (completionEvent.Duration != null)
            {
                completionStatistic.TotalTime += completionEvent.Duration.Value;
            }

            if (IsCompleted(completionEvent))
            {
                SetTotalCompleted(completionStatistic);
                SetTotalTimeCompleted(completionStatistic, GetEventDuration(completionEvent));
                SetSavedKeystrokes(completionStatistic, GetSavedKeystrokes(completionEvent));
            }
            if (IsCancelled(completionEvent))
            {
                SetTotalCancelled(completionStatistic);
                SetTotalTimeCancelled(completionStatistic, GetEventDuration(completionEvent));
            }

            SetTotalProposals(completionStatistic, GetNumberOfProposals(completionEvent));

            return completionStatistic;
        }

        private static bool IsCompleted(ICompletionEvent completionEvent)
        {
            return completionEvent.TerminatedState.Equals(TerminationState.Applied);
        }

        private static bool IsCancelled(ICompletionEvent completionEvent)
        {
            return completionEvent.TerminatedState.Equals(TerminationState.Cancelled);
        }

        /// <summary>
        ///     Prevents overflow
        /// </summary>
        private static void SetTotalCompleted(CompletionStatistic completionStatistic)
        {
            if (completionStatistic.TotalCompleted < int.MaxValue)
            {
                completionStatistic.TotalCompleted++;
            }
        }

        /// <summary>
        ///     Prevents overflow
        /// </summary>
        private static void SetTotalTimeCompleted(CompletionStatistic completionStatistic, TimeSpan duration)
        {
            if (completionStatistic.TotalTimeCompleted < TimeSpan.MaxValue - duration)
            {
                completionStatistic.TotalTimeCompleted += duration;
            }
        }

        /// <summary>
        ///     Prevents overflow
        /// </summary>
        private static void SetTotalCancelled(CompletionStatistic completionStatistic)
        {
            if (completionStatistic.TotalCancelled < int.MaxValue)
            {
                completionStatistic.TotalCancelled++;
            }
        }

        /// <summary>
        ///     Prevents overflow
        /// </summary>
        private static void SetTotalTimeCancelled(CompletionStatistic completionStatistic, TimeSpan duration)
        {
            if (completionStatistic.TotalTimeCancelled < TimeSpan.MaxValue - duration)
            {
                completionStatistic.TotalTimeCancelled += duration;
            }
        }

        /// <summary>
        ///     Prevents overflow
        /// </summary>
        private static void SetSavedKeystrokes(CompletionStatistic completionStatistic, int keystrokes)
        {
            if (completionStatistic.SavedKeystrokes < int.MaxValue - keystrokes)
            {
                completionStatistic.SavedKeystrokes += keystrokes;
            }
        }

        private static int GetSavedKeystrokes(ICompletionEvent completionEvent)
        {
            var p = completionEvent.Selections.LastOrDefault();
            if (p == null ||
                p.Proposal.Name == null)
            {
                return 0;
            }

            var prefix = completionEvent.Prefix;
            var proposalIdentifier = p.Proposal.Name.Identifier;

            var fullName = ExtractFullName(prefix, proposalIdentifier);

            return fullName.Length - prefix.Length;
        }

        /// <summary>
        ///     Returns the actual code added by the proposal using the given <see cref="prefix" />
        /// </summary>
        private static string ExtractFullName(string prefix, string proposalIdentifier)
        {
            const string namePattern = @"[\w|.]";
            var completionPattern = string.Format(
                @"{0}{1}+",
                prefix,
                namePattern);
            var regex = new Regex(completionPattern);

            var matches = regex.Matches(proposalIdentifier);
            if (matches.Count == 0)
            {
                return "";
            }

            var lastMatchFound = matches[matches.Count - 1].Value;
            lastMatchFound = RemoveLeadingDot(lastMatchFound);

            if (IsMethodIdentifier(proposalIdentifier) &&
                !IsPropertyIdentifier(proposalIdentifier))
            {
                lastMatchFound += "()";
            }
            if (IsGenericTypeIdentifier(proposalIdentifier))
            {
                lastMatchFound += "<>";
            }

            return lastMatchFound;
        }

        [Pure]
        private static string RemoveLeadingDot(string lastMatchFound)
        {
            return lastMatchFound.TrimStart('.');
        }

        [Pure]
        public static bool IsMethodIdentifier(string proposalIdentifier)
        {
            // Example: "Foo_bar()"
            const string methodPattern = @"([\w]+\(.*\))";
            var methodRegex = new Regex(methodPattern);
            return methodRegex.IsMatch(proposalIdentifier);
        }


        [Pure]
        public static bool IsGenericTypeIdentifier(string proposalIdentifier)
        {
            // Example: "[[T1 -> T1],[T2 -> T2]]"
            const string genericTypePattern = @"\[[\w]+ -> [\w]+\]";
            var genericTypeRegex = new Regex(string.Format(@"\[({0},)*{0}\]", genericTypePattern));
            return genericTypeRegex.IsMatch(proposalIdentifier);
        }

        [Pure]
        public static bool IsPropertyIdentifier(string proposalIdentifier)
        {
            // Example: "get [System.Int32, mscore, 4.0.0.0] [MyClass, MyAssembly, 1.2.3.4].Count"
            return proposalIdentifier.StartsWith("set") || proposalIdentifier.StartsWith("get");
        }

        private static void SetTotalProposals(CompletionStatistic completionStatistic, BigInteger numberOfProposals)
        {
            completionStatistic.TotalProposals += numberOfProposals;
        }

        private static BigInteger GetNumberOfProposals(ICompletionEvent completionEvent)
        {
            return completionEvent.ProposalCollection.Proposals.Count;
        }

        [Pure]
        private static TimeSpan GetEventDuration(IDEEvent @event)
        {
            return @event.Duration ?? TimeSpan.Zero;
        }
    }
}