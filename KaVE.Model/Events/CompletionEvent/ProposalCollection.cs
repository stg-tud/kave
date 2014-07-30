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
using System.Runtime.Serialization;
using KaVE.JetBrains.Annotations;
using KaVE.Model.Names;
using KaVE.Utils;

namespace KaVE.Model.Events.CompletionEvent
{
    /// <summary>
    /// An ordered collection of completion proposals as, for example,
    /// presented to the code-completion user in the code-completion
    /// dropdown.
    /// </summary>
    [DataContract]
    public class ProposalCollection
    {
        /// <summary>
        /// The proposals contained in this collection.
        /// </summary>
        [DataMember, NotNull]
        public IList<Proposal> Proposals { get; private set; }

        /// <summary>
        /// For internal use only.
        /// </summary>
        [UsedImplicitly]
        public ProposalCollection()
        {
            Proposals = new List<Proposal>();
        }

        /// <summary>
        /// Creates a collection from a list of proposals.
        /// </summary>
        public ProposalCollection([NotNull] IList<Proposal> proposals)
        {
            Proposals = proposals;
        }

        /// <summary>
        /// Creates a collection from a sequence of proposals.
        /// </summary>
        public ProposalCollection(params Proposal[] proposals) : this(proposals.ToList()) {}

        /// <returns>The 0-based position of the given proposal in this collection.</returns>
        public int GetPosition(Proposal proposal)
        {
            return Proposals.IndexOf(proposal);
        }

        /// <returns>The name of the completion engine that recommended the given proposal.</returns>
        public IName GetCompletionEngine(Proposal proposal)
        {
            return null;
        }

        /// <returns>The names of all completion engines that provided proposals to this collection.</returns>
        public ISet<IName> GetActiveCompletionEngines()
        {
            return null;
        }


        /// <returns>All proposals contributed by a specific completion engine, in the same order as in this collection.</returns>
        public IList<Proposal> GetProposalsFrom(IName engine)
        {
            return null;
        }

        protected bool Equals(ProposalCollection other)
        {
            return Proposals != null && Proposals.SequenceEqual(other.Proposals);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj, Equals);
        }

        public override int GetHashCode()
        {
            return (Proposals != null ? Proposals.GetHashCode() : 0);
        }

        public override string ToString()
        {
            return "[" + string.Join(", ", Proposals) + "]";
        }
    }
}