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
using System.Text.RegularExpressions;
using KaVE.Commons.Model.Naming.Impl.v0.Types.Organization;
using KaVE.Commons.Model.Naming.Types;
using KaVE.Commons.Model.Naming.Types.Organization;
using KaVE.Commons.Utils;
using KaVE.Commons.Utils.Assertion;
using KaVE.Commons.Utils.Collections;
using KaVE.JetBrains.Annotations;

namespace KaVE.Commons.Model.Naming.Impl.v0.Types
{
    public class TypeParameterName : BaseName, ITypeParameterName, IArrayTypeName
    {
        public const string ParameterNameTypeSeparator = " -> ";

        internal TypeParameterName([NotNull] string identifier) : base(identifier)
        {
            Asserts.Not(TypeUtils.IsUnknownTypeIdentifier(identifier));
        }

        public override bool IsUnknown
        {
            get { return false; }
        }

        public bool HasTypeParameters
        {
            get { return false; }
        }

        public IKaVEList<ITypeParameterName> TypeParameters
        {
            get { return Lists.NewList<ITypeParameterName>(); }
        }

        public IAssemblyName Assembly
        {
            get { return new AssemblyName(); }
        }

        public INamespaceName Namespace
        {
            get { return new NamespaceName(); }
        }

        public ITypeName DeclaringType
        {
            get { return null; }
        }

        public string FullName
        {
            get { return TypeParameterShortName; }
        }

        public string Name
        {
            get { return TypeParameterShortName; }
        }

        public bool IsVoidType
        {
            get { return false; }
        }

        public bool IsValueType
        {
            get { return false; }
        }

        public bool IsSimpleType
        {
            get { return false; }
        }

        public bool IsEnumType
        {
            get { return false; }
        }

        public bool IsStructType
        {
            get { return false; }
        }

        public bool IsNullableType
        {
            get { return false; }
        }

        public bool IsReferenceType
        {
            get { return IsArray; }
        }

        public bool IsClassType
        {
            get { return false; }
        }

        public bool IsInterfaceType
        {
            get { return false; }
        }

        public bool IsDelegateType
        {
            get { return false; }
        }

        public bool IsNestedType
        {
            get { return false; }
        }

        public bool IsArray
        {
            get { return TypeParameterShortName.Contains("[") && TypeParameterShortName.Contains("]"); }
        }

        public bool IsTypeParameter
        {
            get { return !IsArray; }
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
            get
            {
                Asserts.That(IsArray);
                return this;
            }
        }

        public ITypeParameterName AsTypeParameterName
        {
            get
            {
                Asserts.That(IsTypeParameter);
                return this;
            }
        }

        public bool IsPredefined
        {
            get { return false; }
        }

        public IPredefinedTypeName AsPredefinedTypeName
        {
            get
            {
                Asserts.Fail("impossible");
                return null;
            }
        }

        [NotNull]
        public string TypeParameterShortName
        {
            get
            {
                var endOfTypeParameterName = Identifier.IndexOf(ParameterNameTypeSeparator, StringComparison.Ordinal);
                return endOfTypeParameterName == -1 ? Identifier : Identifier.Substring(0, endOfTypeParameterName);
            }
        }

        public bool IsBound
        {
            get { return Identifier.Contains(ParameterNameTypeSeparator); }
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
                Asserts.That(IsArray);
                var start = TypeParameterShortName.IndexOf("[", StringComparison.Ordinal);
                var end = TypeParameterShortName.IndexOf("]", StringComparison.Ordinal);
                return end - start;
            }
        }

        public ITypeName ArrayBaseType
        {
            get
            {
                Asserts.That(IsArray);
                var tpn = Identifier.Substring(0, TypeParameterShortName.IndexOf("[", StringComparison.Ordinal));
                var rest = Identifier.Substring(TypeParameterShortName.Length);
                return new TypeParameterName("{0}{1}".FormatEx(tpn, rest));
            }
        }

        private static readonly Regex FreeTypeParameterMatcher = new Regex("^[^ ,0-9\\[\\](){}<>-][^ ,\\[\\](){}<>-]*$");

        public static bool IsTypeParameterNameIdentifier([NotNull] string identifier)
        {
            if (TypeUtils.IsUnknownTypeIdentifier(identifier))
            {
                return false;
            }
            if (identifier.StartsWith("?")) // e.g., unknown arrays
            {
                return false;
            }
            if (identifier.StartsWith("p:"))
            {
                return false;
            }
            var idxArrow = identifier.IndexOf(ParameterNameTypeSeparator, StringComparison.Ordinal);
            if (idxArrow != -1)
            {
                var before = identifier.Substring(0, idxArrow);
                return IsTypeParameterNameIdentifier(before);
            }

            return FreeTypeParameterMatcher.IsMatch(identifier) || IsTypeParameterArray(identifier);
        }

        private static bool IsTypeParameterArray(string id)
        {
            id = id.Trim();

            var arrClose = id.LastIndexOf(']');
            if (arrClose == -1)
            {
                return false;
            }
            var cur = arrClose;
            while (cur - 1 > 0 && id[--cur] == ',') {}
            if (id[cur] != '[')
            {
                return false;
            }

            var before = id.Substring(0, cur);
            if (before.ContainsAny(" ", ",") || arrClose != id.Length - 1)
            {
                return false;
            }
            return FreeTypeParameterMatcher.IsMatch(before.Trim());
        }
    }
}