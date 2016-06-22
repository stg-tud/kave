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
    public class CsTypeName : IName, ITypeName, IDelegateTypeName, IArrayTypeName, ITypeParameterName
    {
        protected TypeNamingParser.TypeContext ctx;
        private static readonly UnknownName UnknownName = new UnknownName();

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
                else if (ctx.typeParameter() != null)
                {
                    if (ctx.typeParameter().notTypeParameter() != null)
                    {
                        return CsNameUtil.GetTypeName(ctx.typeParameter().notTypeParameter().GetText()).Assembly;
                    }
                }
                return new UnknownName();
            }
        }

        public INamespaceName Namespace
        {
            get
            {
                if (ctx.regularType() != null && ctx.regularType().resolvedType() != null &&
                    ctx.regularType().resolvedType().@namespace() != null)
                {
                    return new CsNamespaceName(ctx.regularType().resolvedType().@namespace());
                }
                else if (ctx.regularType() != null && ctx.regularType().nestedType() != null)
                {
                    var n = RecursiveNested(ctx.regularType().nestedType().nestedTypeName()).@namespace();
                    if (n != null)
                    {
                        return new CsNamespaceName(n);
                    }
                }
                else if (ctx.delegateType() != null)
                {
                    return new CsMethodName(ctx.delegateType().method()).DeclaringType.Namespace;
                }
                else if (ctx.arrayType() != null && ctx.arrayType().type() != null)
                {
                    return new CsTypeName(ctx.arrayType().type()).Namespace;
                }
                else if (ctx.typeParameter() != null && ctx.typeParameter().notTypeParameter() != null)
                {
                    return CsNameUtil.GetTypeName(ctx.typeParameter().notTypeParameter().GetText()).Namespace;
                }
                return new UnknownName();
            }
        }

        private TypeNamingParser.ResolvedTypeContext RecursiveNested(
            TypeNamingParser.NestedTypeNameContext nestedTypeNameContext)
        {
            while (nestedTypeNameContext.resolvedType() == null)
            {
                nestedTypeNameContext = nestedTypeNameContext.nestedType().nestedTypeName();
            }
            return nestedTypeNameContext.resolvedType();
        }

        public ITypeName DeclaringType
        {
            get
            {
                if (ctx.regularType() != null && IsNestedType)
                {
                    return CsNameUtil.GetTypeName(ctx.regularType().nestedType().nestedTypeName().GetText() + "," + ctx.regularType().assembly().GetText());
                }
                else if (ctx.delegateType() != null)
                {
                    return new CsMethodName(ctx.delegateType().method()).DeclaringType;
                }
                else if (ctx.arrayType() != null)
                {
                    return new CsTypeName(ctx.arrayType().type()).DeclaringType;
                }
                return UnknownName;
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
                if (ctx.regularType() != null)
                {
                    if (!IsNestedType)
                    {
                        return ctx.regularType().resolvedType().GetText();
                    }
                    return ctx.regularType().nestedType().GetText();
                }
                else if (ctx.delegateType() != null)
                {
                    return DeclaringType.FullName;
                }
                else if (ctx.arrayType() != null)
                {
                    return "arr(" + ctx.arrayType().POSNUM().GetText() + "):" + new CsTypeName(ctx.arrayType().type()).FullName;
                }
                else if (ctx.typeParameter() != null)
                {
                    if (ctx.typeParameter().notTypeParameter() != null)
                    {
                        return CsNameUtil.GetTypeName(ctx.typeParameter().notTypeParameter().GetText()).FullName;
                    }
                }
                else if (ctx.UNKNOWN() != null)
                {
                    return ctx.UNKNOWN().GetText();
                }
                return UnknownName.FullName;
            }
        }

        public string Name
        {
            get
            {
                if (ctx.regularType() != null)
                {
                    if (!IsNestedType)
                    {
                        return ExtractNameFromTypeName(ctx.regularType().resolvedType().typeName());
                    }
                    return ExtractNameFromTypeName(ctx.regularType().nestedType().typeName());
                }
                else if (ctx.delegateType() != null)
                {
                    return DeclaringType.Name;
                }
                else if (ctx.arrayType() != null)
                {
                    return "arr(" + ctx.arrayType().POSNUM().GetText() + "):" + new CsTypeName(ctx.arrayType().type()).Name;
                }
                else if (ctx.typeParameter() != null)
                {
                    if (ctx.typeParameter().notTypeParameter() != null)
                    {
                        return CsNameUtil.GetTypeName(ctx.typeParameter().notTypeParameter().GetText()).Name;
                    }
                    return UnknownName.Identifier;
                }
                return ctx.UNKNOWN().GetText();
            }
        }

        private string ExtractNameFromTypeName(TypeNamingParser.TypeNameContext c)
        {
            if (IsEnumType)
            {
                return c.enumTypeName().GetText();
            }
            else
            {
                if (IsInterfaceType)
                {
                    return
                        c.possiblyGenericTypeName()
                           .interfaceTypeName()
                           .GetText();
                }
                else if (IsStructType)
                {
                    return c.possiblyGenericTypeName()
                           .structTypeName()
                           .GetText();
                }
                else
                {
                    return c.possiblyGenericTypeName().simpleTypeName().GetText();
                }
            }
        }

        public bool IsUnknownType
        {
            get { return ctx.UNKNOWN() != null || ctx.typeParameter() != null; }
        }

        public bool IsVoidType
        {
            get
            {
                return IsVoidTypeRegularType(ctx.regularType()) || (ctx.arrayType() != null && ArrayBaseType.IsVoidType);
            }
        }

        private bool IsVoidTypeRegularType(TypeNamingParser.RegularTypeContext ctx)
        {
            if (ctx != null)
            {
                if (ctx.resolvedType() != null)
                {
                    return ctx.resolvedType().GetText().StartsWith("System.Void");
                }
                return ctx.nestedType().typeName().GetText().StartsWith("System.Void");
            }
            return false;
        }

        public bool IsValueType
        {
            get { return IsStructType || IsEnumType || IsVoidType; }
        }

        public bool IsSimpleType
        {
            get
            {
                if (ctx.regularType() != null)
                {
                    if (ctx.regularType().resolvedType() != null)
                    {
                        return IsSimpleTypeIdentifier(ctx.regularType().GetText());
                    }
                    return IsSimpleTypeIdentifier(ctx.regularType().nestedType().typeName().GetText());
                }
                else if (ctx.arrayType() != null)
                {
                    return ArrayBaseType.IsSimpleType;
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

        public bool IsEnumType
        {
            get { return IsRegularTypeEnum() || (IsArrayType && ArrayBaseType.IsEnumType); }
        }

        private bool IsRegularTypeEnum()
        {
            if (ctx.regularType() != null)
            {
                if (ctx.regularType().resolvedType() != null)
                {
                    return ctx.regularType().resolvedType().typeName().enumTypeName() != null;
                }
                return ctx.regularType().nestedType().typeName().enumTypeName() != null;
            }
            return false;
        }

        public bool IsStructType
        {
            get
            {
                return IsSimpleType || IsVoidType || IsNullableType || IsRegularTypeStruct() ||
                    (IsArrayType && ArrayBaseType.IsStructType);
            }
        }

        private bool IsRegularTypeStruct()
        {
            if (ctx.regularType() != null)
            {
                if (ctx.regularType().resolvedType() != null)
                {
                    return ctx.regularType().resolvedType().typeName().possiblyGenericTypeName() != null &&
                           ctx.regularType().resolvedType().typeName().possiblyGenericTypeName().structTypeName() !=
                           null;
                }
                return ctx.regularType().nestedType().typeName().possiblyGenericTypeName() != null &&
                       ctx.regularType().nestedType().typeName().possiblyGenericTypeName().structTypeName() != null;
            }
            return false;
        }

        public bool IsNullableType
        {
            get
            {
                return IsNotNestedRegular()
                  && ctx.regularType().resolvedType().GetText().StartsWith("System.Nullable'1[[");
            }
        }

        public bool IsReferenceType
        {
            get { return IsClassType || IsInterfaceType || IsArrayType || IsDelegateType; }
        }

        public bool IsClassType
        {
            get
            {
                return !IsValueType && !IsInterfaceType && !IsArrayType && !IsDelegateType && !IsUnknownType;
            }
        }

        public bool IsInterfaceType
        {
            get
            {
                return (ctx.regularType() != null && (IsRegularInterface() || IsNestedInterface())) || (IsArrayType && ArrayBaseType.IsInterfaceType);
            }
        }

        private bool IsNestedInterface()
        {
            return ctx.regularType().nestedType() != null &&
                   ctx.regularType().nestedType().typeName().possiblyGenericTypeName() != null &&
                   ctx.regularType().nestedType().typeName().possiblyGenericTypeName().interfaceTypeName() != null;
        }

        private bool IsRegularInterface()
        {
            return (IsNotNestedRegular() &&
                    ctx.regularType().resolvedType().typeName().possiblyGenericTypeName() != null &&
                    ctx.regularType().resolvedType().typeName().possiblyGenericTypeName().interfaceTypeName() != null);
        }

        private bool IsNotNestedRegular()
        {
            return ctx.regularType() != null && ctx.regularType().resolvedType() != null;
        }

        public bool IsDelegateType
        {
            get { return ctx.delegateType() != null || (ctx.arrayType() != null && ArrayBaseType.IsDelegateType); }
        }

        public bool IsNestedType
        {
            get
            {
                if (ctx.regularType() != null)
                {
                    return ctx.regularType().nestedType() != null;
                }
                else if (ctx.arrayType() != null)
                {
                    return ArrayBaseType.IsNestedType;
                }
                return false;
            }
        }

        public bool IsArrayType
        {
            get { return ctx.arrayType() != null; }
        }

        public ITypeName ArrayBaseType
        {
            get { return IsArrayType ? new CsTypeName(ctx.arrayType().type()) : null; }
        }

        public bool IsTypeParameter
        {
            get { return ctx.typeParameter() != null; }
        }

        public string TypeParameterShortName
        {
            get
            {
                if (IsTypeParameter)
                {
                    return ctx.typeParameter().id().GetText();
                }
                return "";
            }
        }

        public ITypeName TypeParameterType
        {
            get
            {
                if (IsTypeParameter)
                {
                    if (ctx.typeParameter().notTypeParameter() != null)
                    {
                        return CsNameUtil.GetTypeName(ctx.typeParameter().notTypeParameter().GetText());
                    }
                    return UnknownName;
                }
                return null;
            }
        }

        public bool IsGenericEntity
        {
            get { return RegularHasTypeParameters() || DelegateHasTypeParameters() || ArrayTypeNameHasTypeParameters(); }
        }

        public bool HasTypeParameters
        {
            get { return RegularHasTypeParameters() || DelegateHasTypeParameters() || ArrayTypeNameHasTypeParameters(); }
        }

        private bool ArrayTypeNameHasTypeParameters()
        {
            return (ctx.arrayType() != null && ((
                IGenericName)new CsTypeName(ctx.arrayType().type())).HasTypeParameters);
        }

        private bool DelegateHasTypeParameters()
        {
            return (ctx.delegateType() != null && new CsMethodName(ctx.delegateType().method()).IsGenericEntity);
        }

        private bool RegularHasTypeParameters()
        {
            return (ctx.regularType() != null && (IsResolvedGeneric() || IsNestedGeneric()));
        }

        private bool IsResolvedGeneric()
        {
            return ctx.regularType().resolvedType() != null && (ctx.regularType().resolvedType().typeName().possiblyGenericTypeName() != null && ctx.regularType().resolvedType().typeName().possiblyGenericTypeName().genericTypePart() != null);
        }

        private bool IsNestedGeneric()
        {
            if (ctx.regularType() != null && ctx.regularType().nestedType() != null)
            {
                var c = RecursiveNested(ctx.regularType().nestedType().nestedTypeName());
                return c.typeName().possiblyGenericTypeName() != null && c.typeName().possiblyGenericTypeName().genericTypePart() != null || ctx.regularType().nestedType().typeName().possiblyGenericTypeName().genericTypePart() != null;
            }
            return false;
        }

        public IList<ITypeName> TypeParameters
        {
            get
            {
                List<ITypeName> typePara = new KaVEList<ITypeName>();
                if (HasTypeParameters)
                {
                    if (RegularHasTypeParameters())
                    {
                        var genericParams = GetGenericParams();
                        foreach (var p in genericParams)
                        {
                            typePara.Add(new TypeParameterName(p.GetText()));
                        }
                    }
                    else if (ArrayTypeNameHasTypeParameters())
                    {
                        return new CsTypeName(ctx.arrayType().type()).TypeParameters;
                    }
                    else if (DelegateHasTypeParameters())
                    {
                        return new CsMethodName(ctx.delegateType().method()).TypeParameters;
                    }
                }
                return typePara;
            }
        }

        private TypeNamingParser.GenericParamContext[] GetGenericParams()
        {
            if (IsNestedGeneric())
            {
                KaVEList<TypeNamingParser.GenericParamContext> param = new KaVEList<TypeNamingParser.GenericParamContext>();
                var genericTypePartContext = RecursiveNested(ctx.regularType().nestedType().nestedTypeName())
                    .typeName()
                    .possiblyGenericTypeName()
                    .genericTypePart();
                if (genericTypePartContext != null)
                {
                    param.AddAll(genericTypePartContext.genericParam());
                }
                if (ctx.regularType().nestedType().typeName().possiblyGenericTypeName().genericTypePart() != null)
                {
                    param.AddAll(ctx.regularType().nestedType().typeName().possiblyGenericTypeName().genericTypePart().genericParam());
                }
                return param.ToArray();
            }
            return ctx.regularType()
                      .resolvedType()
                      .typeName()
                      .possiblyGenericTypeName()
                      .genericTypePart()
                      .genericParam();
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
                return IsDelegateType ? new CsMethodName(ctx.delegateType().method()).DeclaringType : null;
            }
        }

        public string Signature
        {
            get
            {
                return IsDelegateType ? new CsMethodName(ctx.delegateType().method()).Signature : "";
            }
        }

        public IList<IParameterName> Parameters
        {
            get
            {
                return IsDelegateType ? new CsMethodName(ctx.delegateType().method()).Parameters : Lists.NewList<IParameterName>();
            }
        }

        public bool HasParameters
        {
            get
            {
                return IsDelegateType && new CsMethodName(ctx.delegateType().method()).HasParameters;
            }
        }

        public ITypeName ReturnType
        {
            get { return IsDelegateType ? new CsMethodName(ctx.delegateType().method()).ReturnType : null; }
        }

        public int Rank
        {
            get
            {
                if (IsArrayType)
                {
                    return Int32.Parse(ctx.arrayType().POSNUM().GetText());
                }
                return 0;
            }
        }

        public IDelegateTypeName ToDelegateTypeName
        {
            get
            {
                if (IsDelegateType)
                {
                    return ((IDelegateTypeName)this);
                }
                return null;
            }
        }

        public IArrayTypeName ToArrayTypeName
        {
            get
            {
                if (IsArrayType)
                {
                    return ((IArrayTypeName)this);
                }
                return null;
            }
        }

        public override bool Equals(object other)
        {
            var otherName = other as IName;
            return otherName != null && Equals(otherName);
        }

        private bool Equals(IName other)
        {
            return string.Equals(Identifier, other.Identifier);
        }

        public override int GetHashCode()
        {
            return (Identifier != null ? Identifier.GetHashCode() : 0);
        }
    }
}