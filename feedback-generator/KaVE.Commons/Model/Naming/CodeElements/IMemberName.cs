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

namespace KaVE.Commons.Model.Naming.CodeElements
{
    public interface IMemberName : IName
    {
        ITypeName DeclaringType { get; }
        ITypeName ValueType { get; }
        bool IsStatic { get; }

        /// <summary>
        ///     short name (e.g., _f)
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     full name (e.g.: n.C._f)
        /// </summary>
        string FullName { get; }
    }
}