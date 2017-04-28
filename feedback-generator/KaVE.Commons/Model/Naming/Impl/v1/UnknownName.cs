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
using System.Collections.Generic;
using KaVE.Commons.Model.Naming.CodeElements;
using KaVE.Commons.Model.Naming.Types;
using KaVE.Commons.Model.Naming.Types.Organization;
using KaVE.Commons.Utils.Collections;

namespace KaVE.Commons.Model.Naming.Impl.v1
{
    public class UnknownName : IArrayTypeName, IAssemblyName, IAssemblyVersion, IMethodName, IDelegateTypeName,
        ITypeParameterName, IParameterName, INamespaceName, IFieldName, IPropertyName, IEventName, ILambdaName
    {
        private static readonly Dictionary<Type, UnknownName> NameCache = new Dictionary<Type, UnknownName>();

        public static UnknownName Get(Type t)
        {
            if (NameCache.ContainsKey(t))
            {
                return NameCache[t];
            }
            var name = new UnknownName(t);
            NameCache.Add(t, name);
            return name;
        }

        public bool IsBound
        {
            get { throw new NotImplementedException(); }
        }

        public static IMethodName Method()
        {
            return Get(typeof(IMethodName));
        }

        public static ITypeName Type()
        {
            return Get(typeof(ITypeName));
        }


        private readonly Type _t;

        private UnknownName(Type t)
        {
            _t = t;
        }

        public Type GetUnknownType()
        {
            return _t;
        }

        public string Identifier
        {
            get { return "?"; }
        }

        public bool IsUnknown
        {
            get { return true; }
        }

        public bool IsHashed
        {
            get { return false; }
        }

        public bool IsGenericEntity
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
            get { return Get(typeof(IAssemblyName)); }
        }

        public INamespaceName Namespace
        {
            get { return Get(typeof(INamespaceName)); }
        }

        ITypeName ITypeName.DeclaringType
        {
            get { return Get(typeof(ITypeName)); }
        }

        public bool IsStatic
        {
            get { return false; }
        }

        public bool HasSetter
        {
            get { return false; }
        }

        public bool HasGetter
        {
            get { return false; }
        }

        public ITypeName ValueType
        {
            get { return Get(typeof(ITypeName)); }
        }

        public INamespaceName ParentNamespace
        {
            get { return Get(typeof(INamespaceName)); }
        }

        string INamespaceName.Name
        {
            get { return Identifier; }
        }

        public bool IsLocalProject
        {
            get { return false; }
        }

        public bool IsGlobalNamespace
        {
            get { return false; }
        }

        string IParameterName.Name
        {
            get { return Identifier; }
        }

        public bool IsPassedByReference
        {
            get { return false; }
        }

        public bool IsOutput
        {
            get { return false; }
        }

        public bool IsParameterArray
        {
            get { return false; }
        }

        public bool IsOptional
        {
            get { return false; }
        }

        public bool IsExtensionMethodParameter
        {
            get { return false; }
        }

        string IMemberName.Name
        {
            get { return Identifier; }
        }

        public string FullName
        {
            get { return Identifier; }
        }

        public IAssemblyVersion Version
        {
            get { return Get(typeof(IAssemblyVersion)); }
        }

        ITypeName IMemberName.DeclaringType
        {
            get { return Get(typeof(ITypeName)); }
        }

        string IAssemblyName.Name
        {
            get { return Identifier; }
        }

        string ITypeName.Name
        {
            get { return Identifier; }
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
            get { return false; }
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
            get { return false; }
        }

        public ITypeName ArrayBaseType
        {
            get { return Get(typeof(ITypeName)); }
        }

        public bool IsTypeParameter
        {
            get { return false; }
        }

        public string TypeParameterShortName
        {
            get { return "?"; }
        }

        public ITypeName TypeParameterType
        {
            get { return Get(typeof(ITypeName)); }
        }

        public IDelegateTypeName AsDelegateTypeName
        {
            get { return Get(typeof(IDelegateTypeName)); }
        }

        public IArrayTypeName AsArrayTypeName
        {
            get { return Get(typeof(IArrayTypeName)); }
        }

        public ITypeParameterName AsTypeParameterName
        {
            get { return Get(typeof(ITypeParameterName)); }
        }

        public bool IsPredefined { get; private set; }
        public IPredefinedTypeName AsPredefinedTypeName { get; private set; }

        public int Rank
        {
            get { return -1; }
        }

        public int CompareTo(IAssemblyVersion other)
        {
            return -1;
        }

        public int Major
        {
            get { return -1; }
        }

        public int Minor
        {
            get { return -1; }
        }

        public int Build
        {
            get { return -1; }
        }

        public int Revision
        {
            get { return -1; }
        }

        public ITypeName DelegateType
        {
            get { return Get(typeof(ITypeName)); }
        }

        public IKaVEList<IParameterName> Parameters
        {
            get { return Lists.NewList<IParameterName>(); }
        }

        public bool HasParameters
        {
            get { return false; }
        }

        public bool IsRecursive { get; private set; }

        public bool IsInit { get; private set; }

        public ITypeName ReturnType
        {
            get { return Get(typeof(ITypeName)); }
        }

        public string Signature
        {
            get { return Identifier; }
        }

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
            get { return Get(typeof(ITypeName)); }
        }

        public bool IsConstructor
        {
            get { return false; }
        }

        ITypeName IMethodName.ReturnType
        {
            get { return Get(typeof(ITypeName)); }
        }

        public bool IsExtensionMethod
        {
            get { return false; }
        }

        public ITypeName HandlerType
        {
            get { return Get(typeof(ITypeName)); }
        }

        public bool IsIndexer
        {
            get { return false; }
        }

        public bool IsDelegateInvocation { get; private set; }
    }
}