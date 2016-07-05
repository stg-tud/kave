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

using KaVE.Commons.Model.Naming;
using KaVE.JetBrains.Annotations;

namespace KaVE.Commons.Model.Events.CompletionEvents
{
    /// <summary>
    ///     A completion proposal.
    /// </summary>
    public interface IProposal
    {
        /// <summary>
        ///     The name of the element that is proposed for completion.
        /// </summary>
        [CanBeNull]
        IName Name { get; set; }

        /// <summary>
        ///     The relevance this proposal was ranked with by the code completion. Bigger better.
        ///     Optional if no relevance is known.
        /// </summary>
        [CanBeNull]
        int? Relevance { get; set; }
    }
}