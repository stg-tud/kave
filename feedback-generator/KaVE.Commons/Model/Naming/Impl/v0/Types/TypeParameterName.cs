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

using System;
using KaVE.Commons.Model.Naming.Types;
using KaVE.Commons.Model.Naming.Types.Organization;
using KaVE.Commons.Utils.Assertion;
using KaVE.Commons.Utils.Collections;
using KaVE.JetBrains.Annotations;

namespace KaVE.Commons.Model.Naming.Impl.v0.Types
{
    public class TypeParameterName : BaseName, ITypeParameterName, IArrayTypeName
    {
        /// <summary>
        ///     The separator between the parameter type's short name and its type.
        /// </summary>
        public const string ParameterNameTypeSeparator = " -> ";

        internal TypeParameterName(string identifier) : base(identifier)
        {
            Asserts.Not(TypeUtils.IsUnknownTypeIdentifier(identifier));
        }

        public override bool IsUnknown
        {
            get { return false; }
        }

        public bool IsGenericEntity
        {
            get { return TypeParameterType.IsGenericEntity; }
        }

        public bool HasTypeParameters
        {
            get { return TypeParameterType.HasTypeParameters; }
        }

        public IKaVEList<ITypeParameterName> TypeParameters
        {
            get { return TypeParameterType.TypeParameters; }
        }

        public IAssemblyName Assembly
        {
            get { return TypeParameterType.Assembly; }
        }

        public INamespaceName Namespace
        {
            get { return TypeParameterType.Namespace; }
        }

        public ITypeName DeclaringType
        {
            get { return TypeParameterType.DeclaringType; }
        }

        public string FullName
        {
            get { return TypeParameterType.FullName; }
        }

        public string Name
        {
            get { return TypeParameterType.Name; }
        }

        public bool IsVoidType
        {
            get { return TypeParameterType.IsVoidType; }
        }

        public bool IsValueType
        {
            get { return TypeParameterType.IsValueType; }
        }

        public bool IsSimpleType
        {
            get { return TypeParameterType.IsSimpleType; }
        }

        public bool IsEnumType
        {
            get { return TypeParameterType.IsEnumType; }
        }

        public bool IsStructType
        {
            get { return TypeParameterType.IsStructType; }
        }

        public bool IsNullableType
        {
            get { return TypeParameterType.IsNullableType; }
        }

        public bool IsReferenceType
        {
            get { return TypeParameterType.IsReferenceType; }
        }

        public bool IsClassType
        {
            get { return TypeParameterType.IsClassType; }
        }

        public bool IsInterfaceType
        {
            get { return TypeParameterType.IsInterfaceType; }
        }

        public bool IsDelegateType
        {
            get { return TypeParameterType.IsDelegateType; }
        }

        public bool IsNestedType
        {
            get { return TypeParameterType.IsNestedType; }
        }

        public bool IsArrayType
        {
            get { return IsThisArrayType || TypeParameterType.IsArrayType; }
        }

        private bool IsThisArrayType
        {
            get { return TypeParameterShortName.Contains("[") && TypeParameterShortName.Contains("]"); }
        }

        // TODO test this method
        public ITypeName DeriveArrayTypeName(int rank)
        {
            Asserts.That(rank > 0, "rank smaller than 1");
            var typeParameterShortName = TypeParameterShortName;
            var suffix = Identifier.Substring(typeParameterShortName.Length);
            return
                new TypeParameterName(
                    string.Format("{0}[{1}]{2}", typeParameterShortName, new string(',', rank - 1), suffix));
        }

        public bool IsTypeParameter
        {
            get { return true; }
        }

        public IDelegateTypeName AsDelegateTypeName
        {
            get
            {
                Asserts.Fail("impossible");
                return null;
            }
        }

        public IArrayTypeName AsArrayTypeName
        {
            get { return this; }
        }

        public ITypeParameterName AsTypeParameterName
        {
            get { return this; }
        }

        [NotNull]
        public string TypeParameterShortName
        {
            get
            {
                var endOfTypeParameterName = Identifier.IndexOf(' ');
                return endOfTypeParameterName == -1 ? Identifier : Identifier.Substring(0, endOfTypeParameterName);
            }
        }

        [NotNull]
        public ITypeName TypeParameterType
        {
            get
            {
                var startOfTypeName = TypeParameterShortName.Length + ParameterNameTypeSeparator.Length;
                return startOfTypeName >= Identifier.Length
                    ? new TypeName()
                    : TypeUtils.CreateTypeName(Identifier.Substring(startOfTypeName));
            }
        }

        public int Rank
        {
            get
            {
                Asserts.That(IsThisArrayType);
                var start = TypeParameterShortName.IndexOf("[", StringComparison.Ordinal);
                var end = TypeParameterShortName.IndexOf("]", StringComparison.Ordinal);
                return end - start;
            }
        }

        public ITypeName ArrayBaseType
        {
            get
            {
                Asserts.That(IsThisArrayType);
                var tpn = Identifier.Substring(0, TypeParameterShortName.IndexOf("[", StringComparison.Ordinal));
                var rest = Identifier.Substring(TypeParameterShortName.Length);
                var newId = string.Format("{0}{1}{2}", tpn, ParameterNameTypeSeparator, rest);
                return new TypeParameterName(newId);
            }
        }
    }
}