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
using KaVE.Commons.Model.Naming.Types;
using KaVE.Commons.Model.Naming.Types.Organization;
using KaVE.Commons.Utils.Assertion;
using KaVE.Commons.Utils.Collections;
using KaVE.JetBrains.Annotations;

namespace KaVE.Commons.Model.Naming.Impl.v0.Types
{
    public class PredefinedTypeName : BaseName, IPredefinedTypeName, IArrayTypeName
    {
        private static readonly IDictionary<string, string> SimpleTypeToFullNameMap = new Dictionary<string, string>
        {
            {"p:sbyte", "System.SByte"},
            {"p:byte", "System.Byte"},
            {"p:short", "System.Int16"},
            {"p:ushort", "System.UInt16"},
            {"p:int", "System.Int32"},
            {"p:uint", "System.UInt32"},
            {"p:long", "System.Int64"},
            {"p:ulong", "System.UInt64"},
            {"p:char", "System.Char"},
            {"p:float", "System.Single"},
            {"p:double", "System.Double"},
            {"p:bool", "System.Boolean"},
            {"p:decimal", "System.Decimal"},
            //
            {"p:void", "System.Void"},
            //
            {"p:object", "System.Object"},
            {"p:string", "System.String"}
        };


        public PredefinedTypeName([NotNull] string identifier) : base(identifier)
        {
            Asserts.That(identifier.StartsWith("p:"));
        }

        public override bool IsUnknown
        {
            get { return false; }
        }

        public bool HasTypeParameters
        {
            get { return false; }
        }

        public IKaVEList<ITypeParameterName> TypeParameters { get; private set; }
        public IAssemblyName Assembly { get; private set; }
        public INamespaceName Namespace { get; private set; }
        public string FullName { get; private set; }
        public string Name { get; private set; }

        public bool IsVoidType
        {
            get { return "p:void".Equals(Identifier); }
        }

        public bool IsValueType { get; private set; }
        public bool IsSimpleType { get; private set; }
        public bool IsEnumType { get; private set; }
        public bool IsStructType { get; private set; }
        public bool IsNullableType { get; private set; }
        public bool IsReferenceType { get; private set; }
        public bool IsClassType { get; private set; }
        public bool IsInterfaceType { get; private set; }

        public bool IsNestedType
        {
            get { return false; }
        }

        public ITypeName DeclaringType { get; private set; }

        public bool IsDelegateType
        {
            get { return false; }
        }

        public IDelegateTypeName AsDelegateTypeName { get; private set; }

        public bool IsArray
        {
            get { return false; }
        }

        public IArrayTypeName AsArrayTypeName { get; private set; }

        public bool IsTypeParameter
        {
            get { return false; }
        }

        public ITypeParameterName AsTypeParameterName { get; private set; }
        public bool IsPredefined { get; private set; }

        public IPredefinedTypeName AsPredefinedTypeName { get; private set; }

        public ITypeName FullType { get; private set; }

        public static bool IsPredefinedTypeNameIdentifier(string id)
        {
            return false; //SimpleTypeToFullNameMap.ContainsKey(id);
        }

        public int Rank { get; private set; }

        public ITypeName ArrayBaseType { get; private set; }
    }
}