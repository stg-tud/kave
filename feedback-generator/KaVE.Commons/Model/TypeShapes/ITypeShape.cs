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

using KaVE.Commons.Model.Naming.CodeElements;
using KaVE.Commons.Model.Naming.Types;
using KaVE.Commons.Utils.Collections;
using KaVE.JetBrains.Annotations;

namespace KaVE.Commons.Model.TypeShapes
{
    public interface ITypeShape
    {
        /// <summary>
        ///     A description of the enclosing class, including its parent class and implemented interfaces.
        /// </summary>
        [NotNull]
        ITypeHierarchy TypeHierarchy { get; set; }

        /// <summary>
        ///     All Nested Types in the enclosing class
        /// </summary>
        [NotNull]
        IKaVESet<ITypeName> NestedTypes { get; set; }

        /// <summary>
        ///     All Delegates in the enclosing class
        /// </summary>
        [NotNull]
        IKaVESet<IDelegateTypeName> Delegates { get; set; }

        /// <summary>
        ///     All Events in the enclosing class (including information about the first and super declarations)
        /// </summary>
        [NotNull]
        IKaVESet<IMemberHierarchy<IEventName>> EventHierarchies { get; set; }

        /// <summary>
        ///     All Fields in the enclosing class
        /// </summary>
        [NotNull]
        IKaVESet<IFieldName> Fields { get; set; }

        /// <summary>
        ///     All Methods that are overridden in the class under edit (including information about the first and super
        ///     declaration).
        /// </summary>
        [NotNull]
        IKaVESet<IMemberHierarchy<IMethodName>> MethodHierarchies { get; set; }

        /// <summary>
        ///     All Properties in the enclosing class (including information about the first and super
        ///     declaration).
        /// </summary>
        [NotNull]
        IKaVESet<IMemberHierarchy<IPropertyName>> PropertyHierarchies { get; set; }
    }
}