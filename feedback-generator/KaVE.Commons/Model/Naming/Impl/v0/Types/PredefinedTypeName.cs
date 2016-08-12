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
    public class PredefinedTypeName : BaseName, IPredefinedTypeName, IArrayTypeName
    {
        private static readonly IDictionary<string, string> IdToFullName = new Dictionary<string, string>
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

        private static readonly Regex ValidNameMatcher = new Regex("^p:[a-z]+(\\[,*\\])?$");

        public PredefinedTypeName([NotNull] string identifier) : base(identifier)
        {
            Validate(ValidNameMatcher.IsMatch(identifier), "invalid id '{0}'".FormatEx(identifier));
            Validate(!IsArray || !ArrayBaseType.IsVoidType, "impossible to create void array");
            var baseId = IsArray ? ArrayBaseType.Identifier : Identifier;
            Validate(IdToFullName.ContainsKey(baseId), "uknown id '{0}'".FormatEx(identifier));
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
            get { return new AssemblyName("mscorlib, {0}".FormatEx(new AssemblyVersion().Identifier)); }
        }

        public INamespaceName Namespace
        {
            get { return new NamespaceName("System"); }
        }

        public string FullName
        {
            get
            {
                if (IsArray)
                {
                    return "{0}[{1}]".FormatEx(IdToFullName[ArrayBaseType.Identifier], new string(',', Rank - 1));
                }
                return IdToFullName[Identifier];
            }
        }

        public string Name
        {
            get { return Identifier.Substring(Identifier.IndexOf(':') + 1); }
        }

        public bool IsVoidType
        {
            get { return Is("p:void"); }
        }

        public bool IsValueType
        {
            get { return IsStructType; }
        }

        public bool IsSimpleType
        {
            get { return Is("p:bool") || IsNumeric; }
        }

        #region helper

        private bool Is([NotNull] string id)
        {
            return id.Equals(Identifier);
        }

        private bool IsNumeric
        {
            get { return Is("p:decimal") || IsIntegral || IsFloatingPoint; }
        }

        private bool IsFloatingPoint
        {
            get { return Is("p:float") || Is("p:double"); }
        }

        private bool IsIntegral
        {
            get
            {
                return Is("p:sbyte") || Is("p:byte") || Is("p:short") || Is("p:ushort") || Is("p:int") || Is("p:uint") ||
                       Is("p:long") || Is("p:ulong") || Is("p:char");
            }
        }

        #endregion

        public bool IsEnumType
        {
            get { return false; }
        }

        public bool IsStructType
        {
            get { return !IsReferenceType; }
        }

        public bool IsNullableType
        {
            get { return false; }
        }

        public bool IsReferenceType
        {
            get { return IsArray || IsClassType; }
        }

        public bool IsClassType
        {
            get { return Is("p:object") || Is("p:string"); }
        }

        public bool IsInterfaceType
        {
            get { return false; }
        }


        public bool IsNestedType
        {
            get { return false; }
        }

        public ITypeName DeclaringType
        {
            get { return null; }
        }

        public bool IsDelegateType
        {
            get { return false; }
        }

        public IDelegateTypeName AsDelegateTypeName
        {
            get
            {
                Asserts.Fail("impossible");
                return null;
            }
        }

        public bool IsArray
        {
            get { return Identifier.EndsWith("]"); }
        }

        public IArrayTypeName AsArrayTypeName
        {
            get
            {
                Asserts.That(IsArray);
                return this;
            }
        }

        public bool IsTypeParameter
        {
            get { return false; }
        }

        public ITypeParameterName AsTypeParameterName
        {
            get
            {
                Asserts.Fail("impossible");
                return null;
            }
        }

        public bool IsPredefined
        {
            get { return !IsArray; }
        }

        public IPredefinedTypeName AsPredefinedTypeName
        {
            get
            {
                Asserts.That(IsPredefined);
                return this;
            }
        }

        public ITypeName FullType
        {
            get
            {
                Asserts.That(!IsArray);
                var id = "{0}{1}, {2}".FormatEx(IsStructType ? "s:" : "", FullName, Assembly.Identifier);
                return new TypeName(id);
            }
        }

        public int Rank
        {
            get
            {
                var openArr = Identifier.IndexOf('[');
                var closeArr = Identifier.IndexOf(']');
                return closeArr - openArr;
            }
        }

        public ITypeName ArrayBaseType
        {
            get
            {
                var openArr = Identifier.IndexOf('[');
                return new PredefinedTypeName(Identifier.Substring(0, openArr));
            }
        }

        public static bool IsPredefinedTypeNameIdentifier([NotNull] string id)
        {
            return !string.IsNullOrEmpty(id) && ValidNameMatcher.IsMatch(id);
        }
    }
}