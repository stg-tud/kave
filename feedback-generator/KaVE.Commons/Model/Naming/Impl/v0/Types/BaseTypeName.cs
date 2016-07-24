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

using System.Linq;
using KaVE.Commons.Model.Naming.Types;
using KaVE.Commons.Model.Naming.Types.Organization;
using KaVE.Commons.Utils.Assertion;
using KaVE.Commons.Utils.Collections;
using KaVE.JetBrains.Annotations;

namespace KaVE.Commons.Model.Naming.Impl.v0.Types
{
    public abstract class BaseTypeName : BaseName, ITypeName
    {
        public const string UnknownTypeIdentifier = "?";
        public const string PrefixEnum = "e:";
        public const string PrefixInterface = "i:";
        public const string PrefixStruct = "s:";
        public const string PrefixDelegate = "d:";


        protected BaseTypeName([NotNull] string identifier) : base(identifier) {}

        public override bool IsUnknown
        {
            get { return UnknownTypeIdentifier.Equals(Identifier); }
        }

        public abstract string Name { get; }
        public abstract string FullName { get; }
        public abstract IAssemblyName Assembly { get; }
        public abstract INamespaceName Namespace { get; }
        public abstract bool IsNestedType { get; }
        public abstract ITypeName DeclaringType { get; }

        // delegating the execution
        public bool IsVoidType
        {
            get { return IsVoidTypeIdentifier(Identifier); }
        }

        public bool IsSimpleType
        {
            get { return IsSimpleTypeIdentifier(Identifier); }
        }

        public bool IsNullableType
        {
            get { return IsNullableTypeIdentifier(Identifier); }
        }

        // composed checks
        public bool IsValueType
        {
            get { return IsStructType || IsEnumType || IsVoidType; }
        }

        public bool IsReferenceType
        {
            get { return IsClassType || IsInterfaceType || IsArray || IsDelegateType; }
        }

        public bool IsClassType
        {
            get { return !IsValueType && !IsInterfaceType && !IsArray && !IsDelegateType && !IsUnknown; }
        }

        // checks for prefix
        public bool IsEnumType
        {
            get { return !IsArray && Identifier.StartsWith(PrefixEnum); }
        }

        public bool IsInterfaceType
        {
            get { return !IsArray && Identifier.StartsWith(PrefixInterface); }
        }

        public bool IsStructType
        {
            get { return !IsArray && IsStructTypeIdentifier(Identifier); }
        }

        // generic info
        public bool HasTypeParameters
        {
            get { return TypeParameters.Count > 0; }
        }

        private IKaVEList<ITypeParameterName> _typeParameters;

        public IKaVEList<ITypeParameterName> TypeParameters
        {
            get
            {
                if (_typeParameters == null)
                {
                    if (IsArray || IsDelegateType)
                    {
                        _typeParameters = Lists.NewList<ITypeParameterName>();
                    }
                    else
                    {
                        _typeParameters = FullName.ParseTypeParameters();
                    }
                }
                return _typeParameters;
            }
        }

        // checks for interface
        public bool IsDelegateType
        {
            get { return this is IDelegateTypeName; }
        }

        public bool IsArray
        {
            get { return this is IArrayTypeName; }
        }

        public bool IsTypeParameter
        {
            // ReSharper disable once SuspiciousTypeConversion.Global
            get { return this is ITypeParameterName; }
        }

        // casts
        public IDelegateTypeName AsDelegateTypeName
        {
            get
            {
                var dtn = this as IDelegateTypeName;
                Asserts.NotNull(dtn);
                return dtn;
            }
        }

        public IArrayTypeName AsArrayTypeName
        {
            get
            {
                var atn = this as IArrayTypeName;
                Asserts.NotNull(atn);
                return atn;
            }
        }

        public ITypeParameterName AsTypeParameterName
        {
            get
            {
                // ReSharper disable once SuspiciousTypeConversion.Global
                var tpn = this as ITypeParameterName;
                Asserts.NotNull(tpn);
                return tpn;
            }
        }

        #region static helper

        private static bool IsStructTypeIdentifier(string identifier)
        {
            if (ArrayTypeName.IsArrayTypeNameIdentifier(identifier))
            {
                return false;
            }
            return identifier.StartsWith(PrefixStruct) ||
                   IsSimpleTypeIdentifier(identifier) ||
                   IsNullableTypeIdentifier(identifier) ||
                   IsVoidTypeIdentifier(identifier);
        }


        private static bool IsVoidTypeIdentifier(string identifier)
        {
            return identifier.StartsWith("System.Void,");
        }

        private static bool IsSimpleTypeIdentifier(string identifier)
        {
            return IsNumericTypeName(identifier) || identifier.StartsWith("System.Boolean,");
        }

        private static bool IsNumericTypeName(string identifier)
        {
            return IsIntegralTypeName(identifier) ||
                   IsFloatingPointTypeName(identifier) ||
                   identifier.StartsWith("System.Decimal,");
        }

        private static readonly string[] IntegralTypeNames =
        {
            "System.SByte,",
            "System.Byte,",
            "System.Int16,",
            "System.UInt16,",
            "System.Int32,",
            "System.UInt32,",
            "System.Int64,",
            "System.UInt64,",
            "System.Char,"
        };

        private static bool IsIntegralTypeName(string identifier)
        {
            return IntegralTypeNames.Any(identifier.StartsWith);
        }

        private static readonly string[] FloatingPointTypeNames =
        {
            "System.Single,",
            "System.Double,"
        };

        private static bool IsFloatingPointTypeName(string identifier)
        {
            return FloatingPointTypeNames.Any(identifier.StartsWith);
        }

        internal static bool IsNullableTypeIdentifier(string identifier)
        {
            return identifier.StartsWith("System.Nullable`1[[");
        }

        #endregion
    }
}