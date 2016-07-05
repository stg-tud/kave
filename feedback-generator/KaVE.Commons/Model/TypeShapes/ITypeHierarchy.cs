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

using KaVE.Commons.Model.Naming.Types;
using KaVE.Commons.Utils.Collections;
using KaVE.JetBrains.Annotations;

namespace KaVE.Commons.Model.TypeShapes
{
    /// <summary>
    ///     Represents one level of a type hierarchy.
    /// </summary>
    public interface ITypeHierarchy
    {
        /// <summary>
        ///     The type at this level in the type hierarchy.
        /// </summary>
        [NotNull]
        ITypeName Element { get; }

        /// <summary>
        ///     The direct superclass of the type at this level.
        /// </summary>
        [CanBeNull]
        ITypeHierarchy Extends { get; }

        /// <summary>
        ///     The interfaces directly implemented by the type at this level.
        /// </summary>
        [NotNull]
        IKaVESet<ITypeHierarchy> Implements { get; }

        /// <summary>
        ///     <returns>Whether this type extends some superclass or implements any interfaces</returns>
        /// </summary>
        bool HasSupertypes { get; }

        /// <summary>
        ///     <returns>Whether this type extends some superclass</returns>
        /// </summary>
        bool HasSuperclass { get; }

        /// <summary>
        ///     <returns>Whether this type implements any interfaces</returns>
        /// </summary>
        bool IsImplementingInterfaces { get; }
    }
}