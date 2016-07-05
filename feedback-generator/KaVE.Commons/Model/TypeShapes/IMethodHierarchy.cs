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
using KaVE.JetBrains.Annotations;

namespace KaVE.Commons.Model.TypeShapes
{
    public interface IMethodHierarchy
    {
        /// <summary>
        ///     The name of a method.
        /// </summary>
        [NotNull]
        IMethodName Element { get; set; }

        /// <summary>
        ///     The implementation of the enclosing method that is referred to by calling
        ///     <code>super.'methodName'(...)</code>.
        /// </summary>
        [CanBeNull]
        IMethodName Super { get; set; }

        /// <summary>
        ///     The declarations of the enclosing method, i.e., the method names specified in interfaces or the highest
        ///     parent class that the enclosing method is an implementation of.
        /// </summary>
        [CanBeNull]
        IMethodName First { get; set; }

        /// <summary>
        ///     Whether or not this is a hierarchy of a method that overrides or implements a declaration from further up in the
        ///     type hierarchy.
        /// </summary>
        bool IsDeclaredInParentHierarchy { get; }
    }
}