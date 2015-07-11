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
using System.Text;
using System.Text.RegularExpressions;
using KaVE.Commons.Model.Events;
using KaVE.Commons.Model.Events.CompletionEvents;
using KaVE.Commons.Utils;
using KaVE.Commons.Utils.Json;
using KaVE.Commons.Utils.SSTPrinter;
using KaVE.VS.FeedbackGenerator.SessionManager.Presentation;

namespace KaVE.VS.FeedbackGenerator.SessionManager
{
    public class EventViewModel : ViewModelBase<EventViewModel>
    {
        private string _xamlContext;
        private string _xamlDetails;
        private string _xamlRaw;
        private string _xamlProposals;
        private string _xamlSelections;

        public EventViewModel(IDEEvent evt)
        {
            Event = evt;
        }

        public IDEEvent Event { get; private set; }

        public string EventType
        {
            get
            {
                var eventTypeName = Event.GetType().Name;
                if (eventTypeName.EndsWith("Event"))
                {
                    eventTypeName = eventTypeName.Remove(eventTypeName.Length - 5);
                }
                return eventTypeName;
            }
        }

        public string Trigger
        {
            get { return Event.TriggeredBy.ToString(); }
        }

        public DateTime? StartDateTime
        {
            get { return Event.TriggeredAt; }
        }

        public double DurationInMilliseconds
        {
            get { return Event.Duration.HasValue ? Event.Duration.Value.TotalMilliseconds : 0; }
        }

        public string XamlProposalsRepresentation
        {
            get { return _xamlProposals ?? (_xamlProposals = GetProposalsFormattedAsBulletList()); }
        }

        private string GetProposalsFormattedAsBulletList()
        {
            var completionEvent = Event as CompletionEvent;

            if (completionEvent != null)
            {
                var proposalRepresentation = new StringBuilder();

                foreach (var proposal in completionEvent.ProposalCollection)
                {
                    proposalRepresentation.Append("• ");

                    AppendProposalName(proposalRepresentation, proposal, completionEvent.Prefix);

                    proposalRepresentation.AppendLine();
                }

                return proposalRepresentation.ToString();
            }

            return null;
        }

        public string XamlSelectionsRepresentation
        {
            get { return _xamlSelections ?? (_xamlSelections = GetFormattedSelections()); }
        }

        private string GetFormattedSelections()
        {
            var completionEvent = Event as CompletionEvent;

            if (completionEvent != null && completionEvent.Selections.Any())
            {
                var proposalRepresentation = new StringBuilder();

                foreach (var selection in completionEvent.Selections)
                {
                    proposalRepresentation.Append("• ");

                    if (selection.SelectedAfter != null)
                    {
                        proposalRepresentation.Append("<Bold>").Append(selection.SelectedAfter).Append("</Bold> ");
                    }

                    AppendProposalName(proposalRepresentation, selection.Proposal, completionEvent.Prefix);

                    proposalRepresentation.AppendLine();
                }

                return proposalRepresentation.ToString();
            }

            return null;
        }

        private static void AppendProposalName(StringBuilder proposalRepresentation, IProposal proposal, string prefix)
        {
            if (proposal.Name != null)
            {
                var identifier = proposal.Name.Identifier;

                if (!String.IsNullOrEmpty(prefix))
                {
                    identifier = Regex.Replace(identifier, prefix, "<Bold>$0</Bold>", RegexOptions.IgnoreCase);
                }

                proposalRepresentation.Append(identifier);
            }
            else
            {
                proposalRepresentation.Append("???");
            }
        }

        public string XamlContextRepresentation
        {
            get { return _xamlContext ?? (_xamlContext = GetContextAsXaml()); }
        }

        private string GetContextAsXaml()
        {
            var completionEvent = Event as CompletionEvent;

            if (completionEvent != null)
            {
                var visitor = new SSTPrintingVisitor();
                var context = new XamlSSTPrintingContext {TypeShape = completionEvent.Context2.TypeShape};
                visitor.Visit(completionEvent.Context2.SST, context);

                var usingListContext = new XamlSSTPrintingContext();
                context.SeenNamespaces.FormatAsUsingList(usingListContext);

                return String.Concat(
                    usingListContext.ToString(),
                    Environment.NewLine,
                    Environment.NewLine,
                    context.ToString());
            }

            return null;
        }

        public string Details
        {
            get { return _xamlDetails ?? (_xamlDetails = AddSyntaxHighlightingIfNotTooLong(Event.GetDetailsAsJson())); }
        }

        public string XamlRawRepresentation
        {
            get { return _xamlRaw ?? (_xamlRaw = AddSyntaxHighlightingIfNotTooLong(Event.ToFormattedJson())); }
        }


        /// <summary>
        ///     Adds syntax highlighting (Xaml formatting) to the json, if the json ist not too long.
        ///     This condition is because formatting very long strings takes ages.
        /// </summary>
        private static string AddSyntaxHighlightingIfNotTooLong(string json)
        {
            return json.Length > 50000 ? json : json.AddJsonSyntaxHighlightingWithXaml();
        }

        protected bool Equals(EventViewModel other)
        {
            return string.Equals(Event, other.Event);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj, Equals);
        }

        public override int GetHashCode()
        {
            return Event.GetHashCode();
        }
    }
}