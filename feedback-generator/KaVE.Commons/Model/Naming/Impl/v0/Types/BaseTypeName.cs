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
            get { return TypeUtils.IsVoidTypeIdentifier(Identifier); }
        }

        public bool IsSimpleType
        {
            get { return TypeUtils.IsSimpleTypeIdentifier(Identifier); }
        }

        public bool IsNullableType
        {
            get { return TypeUtils.IsNullableTypeIdentifier(Identifier); }
        }

        // composed checks
        public bool IsValueType
        {
            get { return IsStructType || IsEnumType || IsVoidType; }
        }

        public bool IsReferenceType
        {
            get { return IsClassType || IsInterfaceType || IsArrayType || IsDelegateType; }
        }

        public bool IsClassType
        {
            get { return !IsValueType && !IsInterfaceType && !IsArrayType && !IsDelegateType && !IsUnknown; }
        }

        // checks for prefix
        public bool IsEnumType
        {
            get { return !IsArrayType && Identifier.StartsWith(PrefixEnum); }
        }

        public bool IsInterfaceType
        {
            get { return !IsArrayType && Identifier.StartsWith(PrefixInterface); }
        }

        public bool IsStructType
        {
            get { return !IsArrayType && TypeUtils.IsStructTypeIdentifier(Identifier); }
        }

        // generic info
        public bool IsGenericEntity
        {
            get { return Identifier.IndexOf('`') > 0; }
        }

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
                    if (IsArrayType || IsDelegateType)
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

        public bool IsArrayType
        {
            get { return this is IArrayTypeName; }
        }

        public bool IsTypeParameter
        {
            get { return this is ITypeParameterName; }
        }

        // casts
        public IDelegateTypeName AsDelegateTypeName
        {
            get
            {
                Asserts.That(IsDelegateType);
                return this as IDelegateTypeName;
            }
        }

        public IArrayTypeName AsArrayTypeName
        {
            get
            {
                Asserts.That(IsDelegateType);
                return this as IArrayTypeName;
            }
        }

        public ITypeParameterName AsTypeParameterName
        {
            get
            {
                Asserts.That(IsDelegateType);
                return this as ITypeParameterName;
            }
        }
    }
}