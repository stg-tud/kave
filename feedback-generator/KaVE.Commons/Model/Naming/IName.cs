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

namespace KaVE.Commons.Model.Naming
{
    /// <summary>
    ///     Represents full-qualified names.
    /// </summary>
    public interface IName
    {
        /// <summary>
        ///     Returns a unique representation of the qualified name.
        /// </summary>
        string Identifier { get; }

        /// <summary>
        ///     Tells if this instance refers to an unknown name.
        /// </summary>
        bool IsUnknown { get; }

        /// <summary>
        ///     True if parts of the identifier have been hashed by the anonymizer.
        /// </summary>
        bool IsHashed { get; }
    }
}