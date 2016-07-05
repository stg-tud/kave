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

using KaVE.JetBrains.Annotations;

namespace KaVE.Commons.Model.Naming.Types
{
    /// <summary>
    ///     Represents a full-qualified type name. Such names can have
    ///     generic type parameters.
    /// </summary>
    public interface ITypeName : IGenericName
    {
        /// <summary>
        ///     The name of the bundle (e.g., assembly) this type is declared in.
        /// </summary>
        [NotNull]
        IAssemblyName Assembly { get; }

        /// <summary>
        ///     A full-qualified identifier of the namespace
        ///     containing the type.
        /// </summary>
        [NotNull]
        INamespaceName Namespace { get; }

        /// <summary>
        ///     The name of the type declaring this type or
        ///     <code>null</code> if this type is not nested.
        /// </summary>
        ITypeName DeclaringType { get; }

        [NotNull]
        string FullName { get; }

        [NotNull]
        string Name { get; }

        bool IsUnknownType { get; }

        bool IsVoidType { get; }

        /// <summary>
        ///     Value types are simple (or primitive) types, enum types,
        ///     struct types and nullable types (the extension of all other
        ///     value types with the <code>null</code> value).
        /// </summary>
        bool IsValueType { get; }

        /// <summary>
        ///     <returns>Wheather this is a simple (or primitive) type</returns>
        /// </summary>
        bool IsSimpleType { get; }

        bool IsEnumType { get; }

        bool IsStructType { get; }

        /// <summary>
        ///     <returns>
        ///         Wheather this is a value type that can also take the
        ///         <code>null</code> value
        ///     </returns>
        /// </summary>
        bool IsNullableType { get; }

        /// <summary>
        ///     Reference types are (abstract) class types, interface types,
        ///     array types, and delegate types.
        /// </summary>
        bool IsReferenceType { get; }

        bool IsClassType { get; }
        bool IsInterfaceType { get; }
        bool IsDelegateType { get; }
        bool IsNestedType { get; }

        bool IsArrayType { get; }
        ITypeName ArrayBaseType { get; }

        bool IsTypeParameter { get; }

        IDelegateTypeName AsDelegateTypeName { get; }
        IArrayTypeName AsArrayTypeName { get; }
        ITypeParameterName AsTypeParameterName { get; }
    }
}