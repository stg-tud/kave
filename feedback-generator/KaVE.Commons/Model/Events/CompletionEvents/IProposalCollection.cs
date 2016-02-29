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

using System.Collections.Generic;
using KaVE.Commons.Utils.Collections;
using KaVE.JetBrains.Annotations;

namespace KaVE.Commons.Model.Events.CompletionEvents
{
    /// <summary>
    ///     An ordered collection of completion proposals as, for example,
    ///     presented to the code-completion user in the code-completion
    ///     dropdown.
    /// </summary>
    public interface IProposalCollection : IEnumerable<IProposal>
    {
        /// <summary>
        ///     The proposals contained in this collection.
        /// </summary>
        [NotNull]
        IKaVEList<IProposal> Proposals { get; }

        int Count { get; }

        /// <summary>
        ///     Convenience method for collection initialization.
        /// </summary>
        void Add(IProposal proposal);

        /// <returns>The 0-based position of the given proposal in this collection.</returns>
        int GetPosition(IProposal proposal);
    }
}