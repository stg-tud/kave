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

using System.Collections.Generic;
using KaVE.Commons.Model.Naming.CodeElements;
using KaVE.Commons.Model.Naming.Impl.v0.CodeElements;
using KaVE.Commons.Model.Naming.Types;
using KaVE.JetBrains.Annotations;

namespace KaVE.Commons.Model.Naming.Impl.v0.Types
{
    public class DelegateTypeName : Name, IDelegateTypeName
    {
        public new static IDelegateTypeName UnknownName
        {
            get { return Get("d:[?] [?].()"); }
        }

        private const string Prefix = "d:";

        internal static bool IsDelegateTypeIdentifier(string identifier)
        {
            return identifier.StartsWith(Prefix);
        }

        [UsedImplicitly, NotNull]
        public new static IDelegateTypeName Get(string identifier)
        {
            return (IDelegateTypeName) TypeName.Get(identifier);
        }

        internal static string FixLegacyDelegateNames(string identifier)
        {
            // fix legacy delegate names
            if (!identifier.Contains("("))
            {
                identifier = string.Format(
                    "{0}[{1}] [{2}].()",
                    Prefix,
                    UnknownTypeName.Identifier,
                    identifier.Substring(Prefix.Length));
            }
            return identifier;
        }

        internal DelegateTypeName(string identifier) : base(identifier) {}

        private IMethodName DelegateMethod
        {
            get { return MethodName.Get(Identifier.Substring(Prefix.Length)); }
        }

        public ITypeName DelegateType
        {
            get { return DelegateMethod.DeclaringType; }
        }

        public bool IsInterfaceType
        {
            get { return false; }
        }

        public bool IsDelegateType
        {
            get { return true; }
        }

        public bool IsNestedType
        {
            get { return DelegateType.IsNestedType; }
        }

        public bool IsArrayType
        {
            get { return false; }
        }

        public ITypeName ArrayBaseType
        {
            get { return null; }
        }

        public ITypeName DeriveArrayTypeName(int rank)
        {
            return ArrayTypeName.From(this, rank);
        }

        public bool IsTypeParameter
        {
            get { return false; }
        }

        public IDelegateTypeName AsDelegateTypeName { get; private set; }
        public IArrayTypeName AsArrayTypeName { get; private set; }
        public ITypeParameterName AsTypeParameterName { get; private set; }

        public string TypeParameterShortName
        {
            get { return null; }
        }

        public ITypeName TypeParameterType
        {
            get { return null; }
        }

        public IAssemblyName Assembly
        {
            get { return DelegateType.Assembly; }
        }

        public INamespaceName Namespace
        {
            get { return DelegateType.Namespace; }
        }

        public ITypeName DeclaringType
        {
            get { return DelegateType.DeclaringType; }
        }

        public string FullName
        {
            get { return DelegateMethod.DeclaringType.FullName; }
        }

        public string Name
        {
            get { return DelegateType.Name; }
        }

        public bool IsUnknownType
        {
            get { return false; }
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
            get { return true; }
        }

        public bool IsClassType
        {
            get { return false; }
        }

        public string Signature
        {
            get
            {
                var endOfValueType = Identifier.EndOfNextTypeIdentifier(2);
                var endOfDelegateType = Identifier.EndOfNextTypeIdentifier(endOfValueType);
                return Name + Identifier.Substring(endOfDelegateType + 1);
            }
        }

        public IList<IParameterName> Parameters
        {
            get { return DelegateMethod.Parameters; }
        }

        public bool HasParameters
        {
            get { return DelegateMethod.HasParameters; }
        }

        public ITypeName ReturnType
        {
            get { return DelegateMethod.ReturnType; }
        }

        public bool IsGenericEntity
        {
            get { return DelegateType.IsGenericEntity; }
        }

        public bool HasTypeParameters
        {
            get { return DelegateType.HasTypeParameters; }
        }

        public IList<ITypeName> TypeParameters
        {
            get { return DelegateType.TypeParameters; }
        }
    }
}