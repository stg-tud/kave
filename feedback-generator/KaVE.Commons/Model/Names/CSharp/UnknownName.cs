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
using KaVE.Commons.Utils.Collections;

namespace KaVE.Commons.Model.Names.CSharp
{
    public class UnknownName : IArrayTypeName, IAssemblyName, IAssemblyVersion, IMethodName, IDelegateTypeName, ITypeParameterName, IParameterName, INamespaceName, IFieldName, IPropertyName, IEventName, ILambdaName
    {
        public UnknownName()
        {

        }

        public string Identifier { get { return "?"; } }
        public bool IsUnknown { get { return true; } }
        public bool IsHashed { get { return false; } }
        public bool IsGenericEntity { get { return false; } }
        public bool HasTypeParameters { get { return false; } }
        public IList<ITypeName> TypeParameters { get { return Lists.NewList<ITypeName>(); } }
        public IAssemblyName Assembly { get { return new UnknownName(); } }
        public INamespaceName Namespace { get { return new UnknownName(); } }

        ITypeName ITypeName.DeclaringType
        {
            get { return new UnknownName(); }
        }

        public bool IsStatic { get { return false; } }
        public bool HasSetter { get { return false; } }
        public bool HasGetter { get { return false; } }
        public ITypeName ValueType { get { return new UnknownName(); } }
        public INamespaceName ParentNamespace { get { return new UnknownName(); } }

        string INamespaceName.Name
        {
            get { return Identifier; }
        }

        public bool IsGlobalNamespace { get { return false; } }

        string IParameterName.Name
        {
            get { return Identifier; }
        }

        public bool IsPassedByReference { get { return false; } }
        public bool IsOutput { get { return false; } }
        public bool IsParameterArray { get { return false; } }
        public bool IsOptional { get { return false; } }
        public bool IsExtensionMethodParameter { get { return false; } }

        string IMemberName.Name
        {
            get { return Identifier; }
        }

        public string FullName { get { return Identifier; } }
        public IAssemblyVersion AssemblyVersion { get { return new UnknownName(); } }

        ITypeName IMemberName.DeclaringType
        {
            get { return new UnknownName(); }
        }

        string IAssemblyName.Name
        {
            get { return Identifier; }
        }

        string ITypeName.Name
        {
            get { return Identifier; }
        }

        public bool IsUnknownType { get { return false; } }
        public bool IsVoidType { get { return false; } }
        public bool IsValueType { get { return false; } }
        public bool IsSimpleType { get { return false; } }
        public bool IsEnumType { get { return false; } }
        public bool IsStructType { get { return false; } }
        public bool IsNullableType { get { return false; } }
        public bool IsReferenceType { get { return false; } }
        public bool IsClassType { get { return false; } }
        public bool IsInterfaceType { get { return false; } }
        public bool IsDelegateType { get { return false; } }
        public bool IsNestedType { get { return false; } }
        public bool IsArrayType { get { return false; } }
        public ITypeName ArrayBaseType { get { return new UnknownName(); } }
        public bool IsTypeParameter { get { return false; } }
        public string TypeParameterShortName { get { return "?"; } }
        public ITypeName TypeParameterType { get { return new UnknownName(); } }
        public IDelegateTypeName ToDelegateTypeName { get { return new UnknownName(); } }
        public IArrayTypeName ToArrayTypeName { get { return new UnknownName(); } }
        public int Rank { get { return -1; } }
        public int CompareTo(IAssemblyVersion other)
        {
            return -1;
        }

        public int Major { get { return -1; } }
        public int Minor { get { return -1; } }
        public int Build { get { return -1; } }
        public int Revision { get { return -1; } }
        public ITypeName DelegateType { get { return new UnknownName(); } }

        string IDelegateTypeName.Signature
        {
            get { return Identifier; }
        }

        public IList<IParameterName> Parameters { get { return Lists.NewList<IParameterName>(); } }
        public bool HasParameters { get { return false; } }
        public ITypeName ReturnType { get { return new UnknownName(); } }

        public string Signature { get { return Identifier; } }

        IList<IParameterName> IDelegateTypeName.Parameters
        {
            get { return Lists.NewList<IParameterName>(); }
        }

        bool IDelegateTypeName.HasParameters
        {
            get { return false; }
        }

        ITypeName IDelegateTypeName.ReturnType
        {
            get { return new UnknownName(); }
        }

        string IMethodName.Signature
        {
            get { return Identifier; }
        }

        IList<IParameterName> IMethodName.Parameters
        {
            get { return Lists.NewList<IParameterName>(); }
        }

        bool IMethodName.HasParameters
        {
            get { return false; }
        }

        public bool IsConstructor { get { return false; } }

        ITypeName IMethodName.ReturnType
        {
            get { return new UnknownName(); }
        }

        public bool IsExtensionMethod { get { return false; } }
        public ITypeName HandlerType { get { return new UnknownName(); } }
    }
}