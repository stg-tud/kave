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

using KaVE.Commons.Utils.Collections;
using KaVE.JetBrains.Annotations;

namespace KaVE.Commons.Model.Events.CompletionEvents
{
    /// <summary>
    ///     A completion event represents one cycle through the code completion
    ///     process:
    ///     <list type="number">
    ///         <item>
    ///             <description>The completion is triggered.</description>
    ///         </item>
    ///         <item>
    ///             <description>The user browses the code completion.</description>
    ///         </item>
    ///         <item>
    ///             <description>The user applies a completion proposal or cancels the code completion.</description>
    ///         </item>
    ///     </list>
    /// </summary>
    public interface ICompletionEvent : IIDEEvent
    {
        /// <summary>
        ///     The context in which the completion takes place. These information
        ///     is what is used as the query to the intelligent code completion.
        /// </summary>
        [NotNull]
        Context Context2 { get; }

        /// <summary>
        ///     The completion proposals shown to the user.
        /// </summary>
        [NotNull]
        IProposalCollection ProposalCollection { get; }

        /// <summary>
        ///     The list of proposals from the ProposalCollection that where
        ///     selected while the code completion was active. Proposals appear in
        ///     the order of selection.
        /// </summary>
        [NotNull]
        IKaVEList<IProposalSelection> Selections { get; }

        [CanBeNull]
        IProposal LastSelectedProposal { get; }

        /// <summary>
        ///     The kind of interaction that termined the completion, e.g., by a mouse click.
        /// </summary>
        EventTrigger TerminatedBy { get; }

        /// <summary>
        ///     The status with which the completion terminated, e.g., as cancelled.
        /// </summary>
        TerminationState TerminatedState { get; }
    }

    public enum TerminationState
    {
        Applied,
        Cancelled,
        Filtered,
        Unknown
    }
}