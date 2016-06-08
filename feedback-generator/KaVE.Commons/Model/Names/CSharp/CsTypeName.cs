/**
 * Copyright 2016 Technische Universität Darmstadt
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using System;
using System.Collections.Generic;
using KaVE.Commons.Model.Names.CSharp.Parser;
using KaVE.Commons.Utils.Collections;

namespace KaVE.Commons.Model.Names.CSharp
{
    class CsTypeName : ITypeName, IDelegateTypeName
    {
        private static string UNKNOWN_IDENTIFIER = "???";
        protected TypeNamingParser.TypeContext ctx;

        public CsTypeName(String type)
        {
            TypeNamingParser.TypeContext ctx = TypeNameParseUtil.ValidateTypeName(type);
            this.ctx = ctx;
        }

        public CsTypeName(TypeNamingParser.TypeContext ctx)
        {
            this.ctx = ctx;
        }

        public IAssemblyName Assembly
        {
            get
            {
                if (ctx.regularType() != null)
                {
                    return new CsAssemblyName(ctx.regularType().assembly());
                }
                else if (ctx.arrayType() != null)
                {
                    return new CsTypeName(ctx.arrayType().type()).Assembly;
                }
                else if (ctx.delegateType() != null)
                {
                    return new CsMethodName(ctx.delegateType().method()).DeclaringType.Assembly;
                }
                return AssemblyName.UnknownName;
            }
        }

        public INamespaceName Namespace
        {
            get
            {
                var identifier = "???";
                if (ctx.regularType() != null && ctx.regularType().resolvedType() != null &&
                    ctx.regularType().resolvedType().@namespace() != null)
                {
                    identifier = ctx.regularType().resolvedType().@namespace().GetText();
                }
                else if (ctx.regularType() != null && ctx.regularType().nestedType() != null)
                {
                    identifier = RecursiveNested(ctx.regularType().nestedType().nestedTypeName());
                }
                else if (ctx.delegateType() != null)
                {
                    return new CsMethodName(ctx.delegateType().method()).DeclaringType.Namespace;
                }
                return NamespaceName.Get(identifier);
            }
        }

        private string RecursiveNested(TypeNamingParser.NestedTypeNameContext nestedTypeNameContext)
        {
            while (nestedTypeNameContext.resolvedType() == null)
            {
                nestedTypeNameContext = nestedTypeNameContext.nestedType().nestedTypeName();
            }
            return nestedTypeNameContext.resolvedType().@namespace() != null ? nestedTypeNameContext.resolvedType().@namespace().GetText() : UNKNOWN_IDENTIFIER;
        }

        public ITypeName DeclaringType
        {
            get
            {
                if (ctx.regularType() != null && ((ITypeName) this).IsNestedType)
                {
                    String identifier = GetWithoutLastTypeName(ctx.regularType().resolvedType()) + ","
                            + (ctx.regularType().WS() != null ? ctx.regularType().WS().GetText() : "")
                            + ctx.regularType().assembly().GetText();
                    return CsNameUtil.ParseTypeName(identifier);
                }
                else if (ctx.delegateType() != null)
                {
                    return new CsMethodName(ctx.delegateType().method()).DeclaringType;
                }
                return CsNameUtil.ParseTypeName("?");
            }
        }

        private String GetWithoutLastTypeName(TypeNamingParser.ResolvedTypeContext resolvedType)
        {
            String typeName = resolvedType.@namespace() != null
                    ? resolvedType.@namespace().GetText() + resolvedType.typeName().GetText()
                    : resolvedType.typeName().GetText();
            List<TypeNamingParser.TypeNameContext> typeNames = new KaVEList<TypeNamingParser.TypeNameContext>();
            for (int i = 1; i < typeNames.Count - 1; i++)
            {
                typeName += "+" + typeNames[i].GetText();
            }
            return typeName;
        }

        public string FullName
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string Name
        {
            get { throw new NotImplementedException(); }
        }

        bool ITypeName.IsUnknownType
        {
            get { return ctx.UNKNOWN() != null || ctx.typeParameter() != null; }
        }

        bool ITypeName.IsVoidType
        {
            get { return ctx.regularType() != null && ctx.regularType().resolvedType().GetText().StartsWith("System.Void"); }
        }

        bool ITypeName.IsValueType
        {
            get { return ((ITypeName)this).IsStructType || ((ITypeName)this).IsEnumType || ((ITypeName)this).IsVoidType; }
        }

        bool ITypeName.IsSimpleType
        {
            get
            {
                if (ctx.regularType() != null)
                {
                    return IsSimpleTypeIdentifier(ctx.regularType().GetText());
                }
                return false;
            }
        }

        private static bool IsSimpleTypeIdentifier(String identifier)
        {
            return IsNumericTypeName(identifier) || identifier.StartsWith("System.Boolean,");
        }

        private static bool IsNumericTypeName(String identifier)
        {
            return IsIntegralTypeName(identifier) || IsFloatingPointTypeName(identifier)
                    || identifier.StartsWith("System.Decimal,");
        }

        private static String[] IntegralTypeNames = { "System.SByte,", "System.Byte,", "System.Int16,",
			"System.UInt16,", "System.Int32,", "System.UInt32,", "System.Int64,", "System.UInt64,", "System.Char," };

        private static bool IsIntegralTypeName(String identifier)
        {
            for (int i = 0; i < IntegralTypeNames.Length; i++)
            {
                if (identifier.StartsWith(IntegralTypeNames[i]))
                {
                    return true;
                }
            }
            return false;
        }

        private static String[] FloatingPointTypeNames = { "System.Single,", "System.Double," };

        private static bool IsFloatingPointTypeName(String identifier)
        {
            for (int i = 0; i < FloatingPointTypeNames.Length; i++)
            {
                if (identifier.StartsWith(FloatingPointTypeNames[i]))
                {
                    return true;
                }
            }
            return false;
        }

        bool ITypeName.IsEnumType
        {
            get { return ctx.regularType() != null && ctx.regularType().resolvedType().typeName().enumTypeName() != null; }
        }

        bool ITypeName.IsStructType
        {
            get
            {
                return ((ITypeName)this).IsSimpleType || ((ITypeName)this).IsVoidType || ((ITypeName)this).IsNullableType
                || (ctx.regularType() != null &&
                       ctx.regularType().resolvedType().typeName().possiblyGenericTypeName() != null &&
                       ctx.regularType().resolvedType().typeName().possiblyGenericTypeName().interfaceTypeName() != null);
            }
        }

        bool ITypeName.IsNullableType
        {
            get
            {
                return ctx.regularType() != null
                  && ctx.regularType().resolvedType().GetText().StartsWith("System.Nullable'1[[");
            }
        }

        bool ITypeName.IsReferenceType
        {
            get { throw new NotImplementedException(); }
        }

        bool ITypeName.IsClassType
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        bool ITypeName.IsInterfaceType
        {
            get { throw new NotImplementedException(); }
        }

        bool ITypeName.IsDelegateType
        {
            get { return ctx.delegateType() != null; }
        }

        bool ITypeName.IsNestedType
        {
            get
            {
                if (ctx.regularType() != null)
                {
                    return ctx.regularType().nestedType() != null;
                }
                return false;
            }
        }

        bool ITypeName.IsArrayType
        {
            get { return ctx.arrayType() != null; }
        }

        public ITypeName ArrayBaseType
        {
            get { throw new NotImplementedException(); }
        }

        bool ITypeName.IsTypeParameter
        {
            get { return ctx.typeParameter() != null; }
        }

        public string TypeParameterShortName
        {
            get { throw new NotImplementedException(); }
        }

        public ITypeName TypeParameterType
        {
            get { throw new NotImplementedException(); }
        }

        bool IGenericName.IsGenericEntity
        {
            get { throw new NotImplementedException(); }
        }

        bool IGenericName.HasTypeParameters
        {
            get { throw new NotImplementedException(); }
        }

        public IList<ITypeName> TypeParameters
        {
            get { throw new NotImplementedException(); }
        }

        public string Identifier
        {
            get { return ctx.GetText(); }
        }

        bool IName.IsUnknown
        {
            get { return ctx.UNKNOWN() != null; }
        }

        public bool IsHashed
        {
            get { throw new NotImplementedException(); }
        }

        public ITypeName DelegateType
        {
            get
            {
                return ((ITypeName)this).IsDelegateType ? new CsMethodName(ctx.delegateType().method()).DeclaringType : null;
            }
        }

        public string Signature
        {
            get
            {
                return ((ITypeName)this).IsDelegateType ? new CsMethodName(ctx.delegateType().method()).Signature : "";
            }
        }

        public IList<IParameterName> Parameters
        {
            get
            {
                return ((ITypeName)this).IsDelegateType ? new CsMethodName(ctx.delegateType().method()).Parameters : Lists.NewList<IParameterName>();
            }
        }

        public bool HasParameters
        {
            get
            {
                return ((ITypeName)this).IsDelegateType && new CsMethodName(ctx.delegateType().method()).HasParameters;
            }
        }

        public ITypeName ReturnType
        {
            get { return ((ITypeName) this).IsDelegateType ? new CsMethodName(ctx.delegateType().method()).ReturnType : null; }
        }
    }
}