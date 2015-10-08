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

using KaVE.Commons.Model.Events;

namespace KaVE.VS.Statistics.Filters
{
    /// <summary>
    ///     Interface for filtering IDEEvents;
    ///     <para>
    ///         This may be used by StatisticCalculators to
    ///         implement filter, merging or replacement logic for their events
    ///     </para>
    /// </summary>
    public interface IEventPreprocessor
    {
        /// <summary>
        ///     Processes <see cref="@event" />;
        ///     <para>Returns the processed event or null if event is filtered</para>
        /// </summary>
        IDEEvent Preprocess(IDEEvent @event);
    }
}