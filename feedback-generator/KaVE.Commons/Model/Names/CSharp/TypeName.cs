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
 * 
 * Contributors:
 *    - Sven Amann
 */

using System;
using System.Collections.Generic;
using System.Linq;
using KaVE.Commons.Utils.Assertion;
using KaVE.Commons.Utils.Collections;
using KaVE.JetBrains.Annotations;

namespace KaVE.Commons.Model.Names.CSharp
{
    public class TypeName : Name, ITypeName
    {
        private static readonly WeakNameCache<ITypeName> Registry = WeakNameCache<ITypeName>.Get(CreateTypeName);

        public new static ITypeName UnknownName
        {
            get { return UnknownTypeName.Instance; }
        }

        public override bool IsUnknown
        {
            get { return Equals(this, UnknownName); }
        }

        private static ITypeName CreateTypeName(string identifier)
        {
            // checked first, because it's a special case
            if (UnknownTypeName.IsUnknownTypeIdentifier(identifier))
            {
                return new UnknownTypeName(identifier);
            }
            // checked second, since type parameters can have any kind of type
            if (TypeParameterName.IsTypeParameterIdentifier(identifier))
            {
                return new TypeParameterName(identifier);
            }
            // checked third, since the array's value type can have any kind of type
            if (ArrayTypeName.IsArrayTypeIdentifier(identifier))
            {
                return new ArrayTypeName(identifier);
            }
            if (InterfaceTypeName.IsInterfaceTypeIdentifier(identifier))
            {
                return new InterfaceTypeName(identifier);
            }
            if (StructTypeName.IsStructTypeIdentifier(identifier))
            {
                return new StructTypeName(identifier);
            }
            if (EnumTypeName.IsEnumTypeIdentifier(identifier))
            {
                return new EnumTypeName(identifier);
            }
            if (DelegateTypeName.IsDelegateTypeIdentifier(identifier))
            {
                return new DelegateTypeName(identifier);
            }
            return new TypeName(identifier);
        }


        /// <summary>
        ///     Type names follow the scheme
        ///     <code>'fully-qualified type name''generic type parameters', 'assembly identifier'</code>.
        ///     Examples of type names are:
        ///     <list type="bullet">
        ///         <item>
        ///             <description>
        ///                 <code>System.Int32, mscore, 4.0.0.0</code>
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <code>System.Nullable`1[[T -> System.Int32, mscore, 4.0.0.0]], mscore, 4.0.0.0</code>
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <code>System.Collections.Dictionary`2[[TKey -> System.Int32, mscore, 4.0.0.0],[TValue -> System.String, mscore, 4.0.0.0]], mscore, 4.0.0.0</code>
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <code>Namespace.OuterType+InnerType, Assembly, 1.2.3.4</code>
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <code>enum EnumType, Assembly, 1.2.3.4</code>
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <code>interface InterfaceType, Assembly, 1.2.3.4</code>
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <code>struct StructType, Assembly, 1.2.3.4</code>
        ///             </description>
        ///         </item>
        ///     </list>
        ///     parameter-type names follow the scheme <code>'short-name' -> 'actual-type identifier'</code>, with actual-type
        ///     identifier being either the identifier of a type name, as declared above, or another parameter-type name.
        /// </summary>
        [NotNull]
        public new static ITypeName Get(string identifier)
        {
            if (identifier == String.Empty)
            {
                return UnknownTypeName.Instance;
            }
            identifier = FixLegacyNameFormat(identifier);

            return Registry.GetOrCreate(identifier);
        }

        private static string FixLegacyNameFormat(string identifier)
        {
            if (DelegateTypeName.IsDelegateTypeIdentifier(identifier))
            {
                return DelegateTypeName.FixLegacyDelegateNames(identifier);
            }
            return identifier;
        }

        protected TypeName(string identifier) : base(identifier) {}

        public virtual bool IsUnknownType
        {
            get { return false; }
        }

        public virtual string FullName
        {
            get
            {
                var length = GetLengthOfTypeName(Identifier);
                return Identifier.Substring(0, length);
            }
        }

        protected static int GetLengthOfTypeName(string identifier)
        {
            if (UnknownTypeName.IsUnknownTypeIdentifier(identifier))
            {
                return identifier.Length;
            }
            var length = identifier.LastIndexOf(']') + 1;
            if (length > 0)
            {
                return length;
            }
            return identifier.IndexOf(',');
        }

        public string RawFullName
        {
            get
            {
                var fullName = FullName;
                var indexOfGenericList = fullName.IndexOf("[[", StringComparison.Ordinal);
                if (indexOfGenericList < 0)
                {
                    indexOfGenericList = fullName.IndexOf(", ", StringComparison.Ordinal);
                }
                return indexOfGenericList < 0 ? fullName : fullName.Substring(0, indexOfGenericList);
            }
        }

        public virtual IAssemblyName Assembly
        {
            get
            {
                var endOfTypeName = GetLengthOfTypeName(Identifier);
                var assemblyIdentifier = Identifier.Substring(endOfTypeName).Trim(new[] {' ', ','});
                return AssemblyName.Get(assemblyIdentifier);
            }
        }

        public virtual INamespaceName Namespace
        {
            get
            {
                var id = RawFullName;
                var endIndexOfNamespaceIdentifier = id.LastIndexOf('.');
                return endIndexOfNamespaceIdentifier < 0
                    ? NamespaceName.GlobalNamespace
                    : NamespaceName.Get(id.Substring(0, endIndexOfNamespaceIdentifier));
            }
        }

        public string Name
        {
            get
            {
                var rawFullName = RawFullName;
                var endOfOutTypeName = rawFullName.LastIndexOf('+');
                if (endOfOutTypeName > -1)
                {
                    rawFullName = rawFullName.Substring(endOfOutTypeName + 1);
                }
                var endOfTypeName = rawFullName.LastIndexOf('`');
                if (endOfTypeName > -1)
                {
                    rawFullName = rawFullName.Substring(0, endOfTypeName);
                }
                var startIndexOfSimpleName = rawFullName.LastIndexOf('.');
                return rawFullName.Substring(startIndexOfSimpleName + 1);
            }
        }

        public IList<ITypeName> TypeParameters
        {
            get { return HasTypeParameters ? FullName.ParseTypeParameters() : new List<ITypeName>(); }
        }

        public bool IsGenericEntity
        {
            get { return Identifier.IndexOf('`') > 0; }
        }

        public bool HasTypeParameters
        {
            get { return FullName.IndexOf("[[", StringComparison.Ordinal) > -1; }
        }

        public ITypeName DeclaringType
        {
            get
            {
                if (!IsNestedType)
                {
                    return null;
                }

                var fullName = FullName;
                var endOfDeclaringTypeName = fullName.LastIndexOf('+');
                var declaringTypeName = fullName.Substring(0, endOfDeclaringTypeName);
                if (declaringTypeName.IndexOf('`') > -1 && HasTypeParameters)
                {
                    var startIndex = 0;
                    var numberOfParameters = 0;
                    while ((startIndex = declaringTypeName.IndexOf('`', startIndex) + 1) > 0)
                    {
                        var endIndex = declaringTypeName.IndexOf('+', startIndex);
                        if (endIndex > -1)
                        {
                            numberOfParameters +=
                                int.Parse(declaringTypeName.Substring(startIndex, endIndex - startIndex));
                        }
                        else
                        {
                            numberOfParameters += int.Parse(declaringTypeName.Substring(startIndex));
                        }
                    }
                    var outerTypeParameters = TypeParameters.Take(numberOfParameters).ToList();
                    declaringTypeName += "[[" + String.Join("],[", outerTypeParameters.Select(t => t.Identifier)) + "]]";
                }
                return Get(declaringTypeName + ", " + Assembly);
            }
        }

        public virtual bool IsVoidType
        {
            get { return false; }
        }

        public bool IsValueType
        {
            get { return IsStructType || IsEnumType || IsVoidType; }
        }

        public virtual bool IsStructType
        {
            get { return false; }
        }

        public virtual bool IsSimpleType
        {
            get { return false; }
        }

        public virtual bool IsEnumType
        {
            get { return false; }
        }

        public virtual bool IsNullableType
        {
            get { return false; }
        }

        public bool IsReferenceType
        {
            get { return IsClassType || IsInterfaceType || IsArrayType || IsDelegateType; }
        }

        public bool IsClassType
        {
            get { return !IsValueType && !IsInterfaceType && !IsArrayType && !IsDelegateType && !IsUnknownType; }
        }

        public virtual bool IsInterfaceType
        {
            get { return false; }
        }

        public virtual bool IsArrayType
        {
            get { return false; }
        }

        public virtual ITypeName ArrayBaseType
        {
            get { return null; }
        }

        public ITypeName DeriveArrayTypeName(int rank)
        {
            return ArrayTypeName.From(this, rank);
        }

        public virtual bool IsDelegateType
        {
            get { return false; }
        }

        public bool IsNestedType
        {
            get { return RawFullName.Contains("+"); }
        }

        public bool IsTypeParameter
        {
            get { return false; }
        }

        public string TypeParameterShortName
        {
            get { return null; }
        }

        public ITypeName TypeParameterType
        {
            get { return null; }
        }
    }
}