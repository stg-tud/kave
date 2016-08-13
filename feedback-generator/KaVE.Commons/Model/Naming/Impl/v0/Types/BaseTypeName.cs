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
using KaVE.Commons.Utils;
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
            get { return false; }
        }

        public bool IsSimpleType
        {
            get { return false; }
        }

        public bool IsNullableType
        {
            get { return Identifier.StartsWith("s:System.Nullable`1[["); }
        }

        // composed checks
        public bool IsValueType
        {
            get { return IsStructType || IsEnumType; }
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
            get { return !IsArray && Identifier.StartsWith(PrefixStruct); }
        }

        // generic info
        public bool HasTypeParameters
        {
            get { return TypeParameters.Count > 0; }
        }

        private IKaVEList<ITypeParameterName> _typeParameters;

        public virtual IKaVEList<ITypeParameterName> TypeParameters
        {
            get
            {
                if (_typeParameters == null)
                {
                    Asserts.Not(IsDelegateType);
                    var close = FullName.FindPrevious(FullName.Length - 1, '+', ']');
                    if (IsArray || close == -1 || FullName[close] == '+')
                    {
                        _typeParameters = Lists.NewList<ITypeParameterName>();
                    }
                    else
                    {
                        var open = FullName.FindCorrespondingOpenBracket(close);
                        _typeParameters = FullName.ParseTypeParameterList(open, close);
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

        public bool IsPredefined
        {
            // ReSharper disable once SuspiciousTypeConversion.Global
            get { return this is IPredefinedTypeName; }
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

        public IPredefinedTypeName AsPredefinedTypeName
        {
            get
            {
                // ReSharper disable once SuspiciousTypeConversion.Global
                var pdt = this as IPredefinedTypeName;
                Asserts.NotNull(pdt);
                return pdt;
            }
        }
    }
}