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
using KaVE.Commons.Model.Naming.Impl.v1.Parser;
using KaVE.Commons.Model.Naming.Types;
using KaVE.Commons.Model.Naming.Types.Organization;
using KaVE.Commons.Utils.Assertion;
using KaVE.Commons.Utils.Collections;

namespace KaVE.Commons.Model.Naming.Impl.v1
{
    public class TypeName : IDelegateTypeName, IArrayTypeName, ITypeParameterName
    {
        protected TypeNamingParser.TypeContext Ctx;

        public TypeName(TypeNamingParser.TypeContext ctx)
        {
            Asserts.Null(ctx.UNKNOWN(), "ctx.UNKNOWN() != null");
            Ctx = ctx;
        }

        public IAssemblyName Assembly
        {
            get
            {
                if (Ctx.regularType() != null)
                {
                    return new AssemblyName(Ctx.regularType().assembly());
                }
                if (Ctx.arrayType() != null)
                {
                    return new TypeName(Ctx.arrayType().type()).Assembly;
                }
                if (Ctx.delegateType() != null)
                {
                    return new MethodName(Ctx.delegateType().method()).DeclaringType.Assembly;
                }
                if (Ctx.typeParameter() != null)
                {
                    if (Ctx.typeParameter().notTypeParameter() != null)
                    {
                        return Names.Type(Ctx.typeParameter().notTypeParameter().GetText()).Assembly;
                    }
                }
                return UnknownName.Get(typeof(IAssemblyName));
            }
        }

        public INamespaceName Namespace
        {
            get
            {
                if (Ctx.regularType() != null && Ctx.regularType().resolvedType() != null &&
                    Ctx.regularType().resolvedType().@namespace() != null)
                {
                    return new NamespaceName(Ctx.regularType().resolvedType().@namespace());
                }
                if (Ctx.regularType() != null && Ctx.regularType().nestedType() != null)
                {
                    var n = RecursiveNested(Ctx.regularType().nestedType().nestedTypeName()).@namespace();
                    if (n != null)
                    {
                        return new NamespaceName(n);
                    }
                }
                else if (Ctx.delegateType() != null)
                {
                    return new MethodName(Ctx.delegateType().method()).DeclaringType.Namespace;
                }
                else if (Ctx.arrayType() != null && Ctx.arrayType().type() != null)
                {
                    return new TypeName(Ctx.arrayType().type()).Namespace;
                }
                else if (Ctx.typeParameter() != null && Ctx.typeParameter().notTypeParameter() != null)
                {
                    return Names.Type(Ctx.typeParameter().notTypeParameter().GetText()).Namespace;
                }
                return UnknownName.Get(typeof(INamespaceName));
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
                if (Ctx.regularType() != null && IsNestedType)
                {
                    return
                        Names.Type(
                            Ctx.regularType().nestedType().nestedTypeName().GetText() + "," +
                            Ctx.regularType().assembly().GetText());
                }
                if (Ctx.delegateType() != null)
                {
                    return new MethodName(Ctx.delegateType().method()).DeclaringType;
                }
                if (Ctx.arrayType() != null)
                {
                    return new TypeName(Ctx.arrayType().type()).DeclaringType;
                }
                return UnknownName.Get(typeof(ITypeName));
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
                if (Ctx.regularType() != null)
                {
                    if (!IsNestedType)
                    {
                        return Ctx.regularType().resolvedType().GetText();
                    }
                    return Ctx.regularType().nestedType().GetText();
                }
                if (Ctx.delegateType() != null)
                {
                    return DeclaringType.FullName;
                }
                if (Ctx.arrayType() != null)
                {
                    return "arr(" + Ctx.arrayType().POSNUM().GetText() + "):" +
                           new TypeName(Ctx.arrayType().type()).FullName;
                }
                if (Ctx.typeParameter() != null)
                {
                    if (Ctx.typeParameter().notTypeParameter() != null)
                    {
                        return Names.Type(Ctx.typeParameter().notTypeParameter().GetText()).FullName;
                    }
                }
                else if (Ctx.UNKNOWN() != null)
                {
                    return Ctx.UNKNOWN().GetText();
                }
                return UnknownName.Get(typeof(ITypeName)).Identifier;
            }
        }

        public string Name
        {
            get
            {
                if (Ctx.regularType() != null)
                {
                    if (!IsNestedType)
                    {
                        return ExtractNameFromTypeName(Ctx.regularType().resolvedType().typeName());
                    }
                    return ExtractNameFromTypeName(Ctx.regularType().nestedType().typeName());
                }
                if (Ctx.delegateType() != null)
                {
                    return DeclaringType.Name;
                }
                if (Ctx.arrayType() != null)
                {
                    return "arr(" + Ctx.arrayType().POSNUM().GetText() + "):" +
                           new TypeName(Ctx.arrayType().type()).Name;
                }
                if (Ctx.typeParameter() != null)
                {
                    if (Ctx.typeParameter().notTypeParameter() != null)
                    {
                        return Names.Type(Ctx.typeParameter().notTypeParameter().GetText()).Name;
                    }
                    return UnknownName.Get(typeof(ITypeName)).FullName;
                }
                return UnknownName.Get(typeof(ITypeName)).FullName;
            }
        }

        private string ExtractNameFromTypeName(TypeNamingParser.TypeNameContext c)
        {
            if (IsEnumType)
            {
                return c.enumTypeName().GetText();
            }
            if (IsInterfaceType)
            {
                return
                    c.possiblyGenericTypeName()
                     .interfaceTypeName()
                     .GetText();
            }
            if (IsStructType)
            {
                return c.possiblyGenericTypeName()
                        .structTypeName()
                        .GetText();
            }
            return c.possiblyGenericTypeName().simpleTypeName().GetText();
        }

        public bool IsUnknownType
        {
            get { return false; }
        }

        public bool IsVoidType
        {
            get
            {
                return IsVoidTypeRegularType(Ctx.regularType()) || (Ctx.arrayType() != null && ArrayBaseType.IsVoidType);
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
                if (Ctx.regularType() != null)
                {
                    if (Ctx.regularType().resolvedType() != null)
                    {
                        return IsSimpleTypeIdentifier(Ctx.regularType().GetText());
                    }
                    return IsSimpleTypeIdentifier(Ctx.regularType().nestedType().typeName().GetText());
                }
                if (Ctx.arrayType() != null)
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

        private static readonly String[] IntegralTypeNames =
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

        private static readonly String[] FloatingPointTypeNames = {"System.Single,", "System.Double,"};

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
            get { return IsRegularTypeEnum() || (IsArray && ArrayBaseType.IsEnumType); }
        }

        private bool IsRegularTypeEnum()
        {
            if (Ctx.regularType() != null)
            {
                if (Ctx.regularType().resolvedType() != null)
                {
                    return Ctx.regularType().resolvedType().typeName().enumTypeName() != null;
                }
                return Ctx.regularType().nestedType().typeName().enumTypeName() != null;
            }
            return false;
        }

        public bool IsStructType
        {
            get
            {
                return IsSimpleType || IsVoidType || IsNullableType || IsRegularTypeStruct() ||
                       (IsArray && ArrayBaseType.IsStructType);
            }
        }

        private bool IsRegularTypeStruct()
        {
            if (Ctx.regularType() != null)
            {
                if (Ctx.regularType().resolvedType() != null)
                {
                    return Ctx.regularType().resolvedType().typeName().possiblyGenericTypeName() != null &&
                           Ctx.regularType().resolvedType().typeName().possiblyGenericTypeName().structTypeName() !=
                           null;
                }
                return Ctx.regularType().nestedType().typeName().possiblyGenericTypeName() != null &&
                       Ctx.regularType().nestedType().typeName().possiblyGenericTypeName().structTypeName() != null;
            }
            return false;
        }

        public bool IsNullableType
        {
            get
            {
                return IsNotNestedRegular()
                       && Ctx.regularType().resolvedType().GetText().StartsWith("System.Nullable'1[[");
            }
        }

        public bool IsReferenceType
        {
            get { return IsClassType || IsInterfaceType || IsArray || IsDelegateType; }
        }

        public bool IsClassType
        {
            get { return !IsValueType && !IsInterfaceType && !IsArray && !IsDelegateType && !IsUnknownType; }
        }

        public bool IsInterfaceType
        {
            get
            {
                return (Ctx.regularType() != null && (IsRegularInterface() || IsNestedInterface())) ||
                       (IsArray && ArrayBaseType.IsInterfaceType);
            }
        }

        private bool IsNestedInterface()
        {
            return Ctx.regularType().nestedType() != null &&
                   Ctx.regularType().nestedType().typeName().possiblyGenericTypeName() != null &&
                   Ctx.regularType().nestedType().typeName().possiblyGenericTypeName().interfaceTypeName() != null;
        }

        private bool IsRegularInterface()
        {
            return (IsNotNestedRegular() &&
                    Ctx.regularType().resolvedType().typeName().possiblyGenericTypeName() != null &&
                    Ctx.regularType().resolvedType().typeName().possiblyGenericTypeName().interfaceTypeName() != null);
        }

        private bool IsNotNestedRegular()
        {
            return Ctx.regularType() != null && Ctx.regularType().resolvedType() != null;
        }

        public bool IsDelegateType
        {
            get { return Ctx.delegateType() != null || (Ctx.arrayType() != null && ArrayBaseType.IsDelegateType); }
        }

        public bool IsNestedType
        {
            get
            {
                if (Ctx.regularType() != null)
                {
                    return Ctx.regularType().nestedType() != null;
                }
                if (Ctx.arrayType() != null)
                {
                    return ArrayBaseType.IsNestedType;
                }
                return false;
            }
        }

        public bool IsArray
        {
            get { return Ctx.arrayType() != null; }
        }

        public ITypeName ArrayBaseType
        {
            get { return IsArray ? new TypeName(Ctx.arrayType().type()) : null; }
        }

        public bool IsTypeParameter
        {
            get { return Ctx.typeParameter() != null; }
        }

        public string TypeParameterShortName
        {
            get
            {
                if (IsTypeParameter)
                {
                    return Ctx.typeParameter().id().GetText();
                }
                return "";
            }
        }

        public bool IsBound
        {
            get { throw new NotImplementedException(); }
        }

        public ITypeName TypeParameterType
        {
            get
            {
                if (IsTypeParameter)
                {
                    if (Ctx.typeParameter().notTypeParameter() != null)
                    {
                        return Names.Type(Ctx.typeParameter().notTypeParameter().GetText());
                    }
                    return UnknownName.Get(typeof(ITypeParameterName));
                }
                return null;
            }
        }

        public bool HasTypeParameters
        {
            get
            {
                return RegularHasTypeParameters() || DelegateHasTypeParameters() || ArrayTypeNameHasTypeParameters();
            }
        }

        private bool ArrayTypeNameHasTypeParameters()
        {
            return (Ctx.arrayType() != null && ((
                IGenericName) new TypeName(Ctx.arrayType().type())).HasTypeParameters);
        }

        private bool DelegateHasTypeParameters()
        {
            return (Ctx.delegateType() != null && new MethodName(Ctx.delegateType().method()).HasTypeParameters);
        }

        private bool RegularHasTypeParameters()
        {
            return (Ctx.regularType() != null && (IsResolvedGeneric() || IsNestedGeneric()));
        }

        private bool IsResolvedGeneric()
        {
            return Ctx.regularType().resolvedType() != null &&
                   (Ctx.regularType().resolvedType().typeName().possiblyGenericTypeName() != null &&
                    Ctx.regularType().resolvedType().typeName().possiblyGenericTypeName().genericTypePart() != null);
        }

        private bool IsNestedGeneric()
        {
            if (Ctx.regularType() != null && Ctx.regularType().nestedType() != null)
            {
                var c = RecursiveNested(Ctx.regularType().nestedType().nestedTypeName());
                return c.typeName().possiblyGenericTypeName() != null &&
                       c.typeName().possiblyGenericTypeName().genericTypePart() != null ||
                       Ctx.regularType().nestedType().typeName().possiblyGenericTypeName().genericTypePart() != null;
            }
            return false;
        }

        public IKaVEList<ITypeParameterName> TypeParameters
        {
            get
            {
                var typePara = new KaVEList<ITypeParameterName>();
                if (HasTypeParameters)
                {
                    if (RegularHasTypeParameters())
                    {
                        var genericParams = GetGenericParams();
                        foreach (var p in genericParams)
                        {
                            typePara.Add(Names.Type(p.typeParameter().GetText()).AsTypeParameterName);
                        }
                    }
                    else if (ArrayTypeNameHasTypeParameters())
                    {
                        return new TypeName(Ctx.arrayType().type()).TypeParameters;
                    }
                    else if (DelegateHasTypeParameters())
                    {
                        return new MethodName(Ctx.delegateType().method()).TypeParameters;
                    }
                }
                return typePara;
            }
        }

        private TypeNamingParser.GenericParamContext[] GetGenericParams()
        {
            if (IsNestedGeneric())
            {
                KaVEList<TypeNamingParser.GenericParamContext> param =
                    new KaVEList<TypeNamingParser.GenericParamContext>();
                var genericTypePartContext = RecursiveNested(Ctx.regularType().nestedType().nestedTypeName())
                    .typeName()
                    .possiblyGenericTypeName()
                    .genericTypePart();
                if (genericTypePartContext != null)
                {
                    param.AddAll(genericTypePartContext.genericParam());
                }
                if (Ctx.regularType().nestedType().typeName().possiblyGenericTypeName().genericTypePart() != null)
                {
                    param.AddAll(
                        Ctx.regularType()
                           .nestedType()
                           .typeName()
                           .possiblyGenericTypeName()
                           .genericTypePart()
                           .genericParam());
                }
                return param.ToArray();
            }
            return Ctx.regularType()
                      .resolvedType()
                      .typeName()
                      .possiblyGenericTypeName()
                      .genericTypePart()
                      .genericParam();
        }

        public string Identifier
        {
            get { return Ctx.GetText(); }
        }

        bool IName.IsUnknown
        {
            get { return false; }
        }

        public bool IsHashed
        {
            get { throw new NotImplementedException(); }
        }

        public ITypeName DelegateType
        {
            get { return IsDelegateType ? new MethodName(Ctx.delegateType().method()).DeclaringType : null; }
        }

        public string Signature
        {
            get { return IsDelegateType ? new MethodName(Ctx.delegateType().method()).Signature : ""; }
        }

        public IList<IParameterName> Parameters
        {
            get
            {
                return IsDelegateType
                    ? new MethodName(Ctx.delegateType().method()).Parameters
                    : Lists.NewList<IParameterName>();
            }
        }

        public bool HasParameters
        {
            get { return IsDelegateType && new MethodName(Ctx.delegateType().method()).HasParameters; }
        }

        public bool IsRecursive { get; private set; }

        public ITypeName ReturnType
        {
            get { return IsDelegateType ? new MethodName(Ctx.delegateType().method()).ReturnType : null; }
        }

        public int Rank
        {
            get
            {
                if (IsArray)
                {
                    return Int32.Parse(Ctx.arrayType().POSNUM().GetText());
                }
                return 0;
            }
        }

        public IDelegateTypeName AsDelegateTypeName
        {
            get
            {
                if (IsDelegateType)
                {
                    return this;
                }
                return null;
            }
        }

        public IArrayTypeName AsArrayTypeName
        {
            get
            {
                if (IsArray)
                {
                    return this;
                }
                return null;
            }
        }

        public ITypeParameterName AsTypeParameterName
        {
            get
            {
                if (IsTypeParameter)
                {
                    return this;
                }
                return null;
            }
        }

        public bool IsPredefined { get; private set; }
        public IPredefinedTypeName AsPredefinedTypeName { get; private set; }

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