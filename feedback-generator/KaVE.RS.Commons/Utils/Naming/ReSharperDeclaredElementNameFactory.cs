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
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using JetBrains.Metadata.Utils;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.Model2.Assemblies.Interfaces;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.DeclaredElements;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.ExtensionsAPI;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Util;
using JetBrains.Util;
using KaVE.Commons.Model.Naming;
using KaVE.Commons.Model.Naming.CodeElements;
using KaVE.Commons.Model.Naming.Impl.v0.CodeElements;
using KaVE.Commons.Model.Naming.Impl.v0.Types;
using KaVE.Commons.Model.Naming.Types;
using KaVE.Commons.Model.Naming.Types.Organization;
using KaVE.Commons.Utils;
using KaVE.Commons.Utils.Assertion;

namespace KaVE.RS.Commons.Utils.Naming
{
    public static class ReSharperDeclaredElementNameFactory
    {
        // TODO NameUpdate: find better solution for this and all "modifier" references in this file
        private const string TypeParameterArrow = " -> ";

        [NotNull]
        public static IName GetName([NotNull] this DeclaredElementInstance instance)
        {
            return instance.GetName<IName>();
        }

        [NotNull]
        public static TName GetName<TName>([NotNull] this DeclaredElementInstance instance) where TName : class, IName
        {
            return instance.Element.GetName<TName>(instance.Substitution);
        }

        [NotNull]
        public static IName GetName([NotNull] this IClrDeclaredElement element)
        {
            return element.GetName<IName>();
        }

        [NotNull]
        public static TName GetName<TName>([NotNull] this IClrDeclaredElement element) where TName : class, IName
        {
            return element.GetName<TName>(element.IdSubstitution);
        }

        [NotNull]
        public static TName GetName<TName>([NotNull] this IDeclaredElement element, [NotNull] ISubstitution substitution)
            where TName : class, IName
        {
            var seen = new Dictionary<DeclaredElementInstance, IName>();
            var name = element.GetName(substitution, seen);
            var typedName = name as TName;
            if (typedName != null)
            {
                return typedName;
            }

            // in case of unresolved types in using directives...
            // maybe this will be replaced when I have a better understanding of how aliases are handled in the R# AST
            if (typeof(AliasName) == name.GetType() && typeof(TName) == typeof(ITypeName))
            {
                return (TName) Names.UnknownType;
            }

            throw new InvalidCastException(
                "Cannot cast {0}({1}) to {2}.".FormatEx(name.GetType().Name, name.Identifier, typeof(TName).Name));
        }

        [NotNull]
        internal static IName GetName([NotNull] this IDeclaredElement elem,
            [NotNull] ISubstitution substitution,
            IDictionary<DeclaredElementInstance, IName> seen)
        {
            return
                IfElementIs<INamespaceName, INamespace>(elem, GetName, substitution, Names.UnknownNamespace, seen) ??
                IfElementIs<ITypeName, ITypeParameter>(elem, GetName, substitution, Names.UnknownType, seen) ??
                IfElementIs<ITypeName, ITypeElement>(elem, GetName, substitution, Names.UnknownType, seen) ??
                IfElementIs<IMethodName, IFunction>(elem, GetName, substitution, Names.UnknownMethod, seen) ??
                IfElementIs<IParameterName, IParameter>(elem, GetName, substitution, Names.UnknownParameter, seen) ??
                IfElementIs<IFieldName, IField>(elem, GetName, substitution, Names.UnknownField, seen) ??
                IfElementIs<IPropertyName, IProperty>(elem, GetName, substitution, Names.UnknownProperty, seen) ??
                IfElementIs<IEventName, IEvent>(elem, GetName, substitution, Names.UnknownEvent, seen) ??
                IfElementIs<IName, ITypeOwner>(elem, GetName, substitution, Names.UnknownLocalVariable, seen) ??
                IfElementIs<IName, IAlias>(elem, GetName, substitution, Names.UnknownAlias, seen) ??
                IfElementIs<IName, IDeclaredElement>(elem, (e, s, a) => null, substitution, Names.UnknownGeneral, seen) ??
                FallbackHandler(elem, substitution);
        }

        private static IName FallbackHandler(IDeclaredElement element, ISubstitution substitution)
        {
            var typeName = element.GetType().FullName;
            var elementName = element.ShortName;
            var id = string.Format("UnknownDeclaredElement: {0}, '{1}', {2}", typeName, elementName, substitution);
            return Names.General(id);
        }

        private static bool IsMissingDeclaration(IDeclaredElement element)
        {
            return element.ShortName == SharedImplUtil.MISSING_DECLARATION_NAME;
        }

        private static TN IfElementIs<TN, TE>(IDeclaredElement element,
            DeclaredElementToName<TN, TE> map,
            ISubstitution substitution,
            TN unknownName,
            IDictionary<DeclaredElementInstance, IName> seenElements)
            where TE : class, IDeclaredElement
            where TN : class, IName
        {
            var specificElement = element as TE;
            if (specificElement == null)
            {
                return null;
            }
            var dei = new DeclaredElementInstance(element, substitution);
            // exit if we encounter a recursive type, e.g., delegate IList<D> D();
            if (seenElements.ContainsKey(dei))
            {
                return (TN) seenElements[dei];
            }
            // this makes us default to the unknownName, if we reencounter an element while resolving it
            seenElements[dei] = unknownName;
            // after this call we have resolved the element and cached the result
            seenElements[dei] = IsMissingDeclaration(specificElement)
                ? unknownName
                : map(specificElement, substitution, seenElements);
            return (TN) seenElements[dei];
        }

        private delegate TN DeclaredElementToName<out TN, in TE>(
            TE element,
            ISubstitution substitution,
            IDictionary<DeclaredElementInstance, IName> seenElements)
            where TE : class, IDeclaredElement
            where TN : class, IName;

        [NotNull]
        private static ITypeName GetName(this ITypeElement typeElem,
            ISubstitution substitution,
            IDictionary<DeclaredElementInstance, IName> seenElems)
        {
            if (typeElem == null)
            {
                return Names.UnknownType;
            }

            var name = typeElem.GetSimplePredefinedType() ??
                       IfElementIs<IDelegate>(typeElem, GetName, substitution, seenElems, Names.UnknownDelegateType) ??
                       IfElementIs<IEnum>(typeElem, GetName, substitution, seenElems, Names.UnknownType) ??
                       IfElementIs<IInterface>(typeElem, GetName, substitution, seenElems, Names.UnknownType) ??
                       IfElementIs<IStruct>(typeElem, GetName, substitution, seenElems, Names.UnknownType) ??
                       Names.Type(typeElem.GetAssemblyQualifiedName(substitution, seenElems));

            var dei = new DeclaredElementInstance(typeElem, substitution);
            seenElems[dei] = name;
            return name;
        }

        private static ITypeName GetSimplePredefinedType([NotNull] this ITypeElement typeElem)
        {
            var predefinedType = typeElem.Module.GetPredefinedType();

            return IfTypeIsPredefined(typeElem, predefinedType.Object, "p:object") ??
                   IfTypeIsPredefined(typeElem, predefinedType.String, "p:string") ??
                   //
                   IfTypeIsPredefined(typeElem, predefinedType.Bool, "p:bool") ??
                   //
                   IfTypeIsPredefined(typeElem, predefinedType.Decimal, "p:decimal") ??
                   IfTypeIsPredefined(typeElem, predefinedType.Char, "p:char") ??
                   IfTypeIsPredefined(typeElem, predefinedType.Float, "p:float") ??
                   IfTypeIsPredefined(typeElem, predefinedType.Double, "p:double") ??
                   IfTypeIsPredefined(typeElem, predefinedType.Sbyte, "p:sbyte") ??
                   IfTypeIsPredefined(typeElem, predefinedType.Byte, "p:byte") ??
                   IfTypeIsPredefined(typeElem, predefinedType.Short, "p:short") ??
                   IfTypeIsPredefined(typeElem, predefinedType.Ushort, "p:ushort") ??
                   IfTypeIsPredefined(typeElem, predefinedType.Int, "p:int") ??
                   IfTypeIsPredefined(typeElem, predefinedType.Uint, "p:uint") ??
                   IfTypeIsPredefined(typeElem, predefinedType.Long, "p:long") ??
                   IfTypeIsPredefined(typeElem, predefinedType.Ulong, "p:ulong") ??
                   //
                   IfTypeIsPredefined(typeElem, predefinedType.Void, "p:void");
        }

        private static ITypeName IfTypeIsPredefined(ITypeElement typeElem, IDeclaredType targetElemen, string id)
        {
            return Equals(typeElem, targetElemen.GetTypeElement()) ? Names.Type(id) : null;
        }

        private static ITypeName IfElementIs<TE>(ITypeElement typeElement,
            DeclaredElementToName<ITypeName, TE> map,
            ISubstitution substitution,
            IDictionary<DeclaredElementInstance, IName> seenElements,
            ITypeName unknownName)
            where TE : class, IDeclaredElement
        {
            var specificElement = typeElement as TE;
            if (specificElement == null)
            {
                return null;
            }
            return IsMissingDeclaration(specificElement)
                ? unknownName
                : map(specificElement, substitution, seenElements);
        }

        [NotNull]
        private static ITypeName GetName(this IDelegate delegateElement,
            ISubstitution substitution,
            IDictionary<DeclaredElementInstance, IName> seenElements)
        {
            var dei = new DeclaredElementInstance(delegateElement, substitution);
            var elemTypeId = delegateElement.GetAssemblyQualifiedName(substitution, seenElements);
            seenElements[dei] = TypeUtils.CreateTypeName(elemTypeId);

            var invokeMethod = delegateElement.InvokeMethod;

            var identifier = new StringBuilder("d:");
            identifier.AppendType(invokeMethod.ReturnType, seenElements);
            identifier.Append(" [");
            identifier.Append(elemTypeId);
            identifier.Append("].");
            identifier.AppendParameters(invokeMethod, substitution, seenElements);
            return Names.Type(identifier.ToString());
        }

        [NotNull]
        private static ITypeName GetName(this IEnum enumElement,
            ISubstitution substitution,
            IDictionary<DeclaredElementInstance, IName> seenElements)
        {
            return Names.Type("e:" + enumElement.GetAssemblyQualifiedName(substitution, seenElements));
        }

        [NotNull]
        private static ITypeName GetName(this IInterface interfaceElement,
            ISubstitution substitution,
            IDictionary<DeclaredElementInstance, IName> seenElements)
        {
            return Names.Type("i:" + interfaceElement.GetAssemblyQualifiedName(substitution, seenElements));
        }

        [NotNull]
        private static ITypeName GetName(this IStruct structElement,
            ISubstitution substitution,
            IDictionary<DeclaredElementInstance, IName> seenElements)
        {
            var structName = structElement.GetAssemblyQualifiedName(substitution, seenElements);
            return Names.Type("s:{0}", structName);
        }

        [NotNull]
        private static ITypeName GetName(this ITypeParameter typeParameter,
            ISubstitution substitution,
            IDictionary<DeclaredElementInstance, IName> seenElements)
        {
            string id;
            if (typeParameter.IsBound(substitution))
            {
                var type = substitution[typeParameter];
                var target = type.GetName(seenElements).Identifier;
                id = "{0} -> {1}".FormatEx(typeParameter.ShortName, target);
            }
            else
            {
                id = typeParameter.ShortName;
            }
            return Names.Type(id);
        }

        private static bool IsBound(this ITypeParameter typeParameter, ISubstitution substitution)
        {
            if (!substitution.Domain.Contains(typeParameter))
            {
                return false;
            }
            if (substitution.IsId())
            {
                return false;
            }
            var targetType = substitution[typeParameter];
            var targetTypeParameter = targetType.GetTypeElement<ITypeParameter>();
            if (targetTypeParameter == null)
            {
                return true;
            }
            var o1 = typeParameter.Owner;
            var o2 = targetTypeParameter.Owner;
            return !o1.Equals(o2);
        }

        [NotNull]
        private static INamespaceName GetName(this INamespace ns,
            ISubstitution substitution,
            IDictionary<DeclaredElementInstance, IName> seenElements)
        {
            return Names.Namespace(ns.QualifiedName);
        }

        [NotNull]
        private static IParameterName GetName(this IParameter parameter,
            ISubstitution substitution,
            IDictionary<DeclaredElementInstance, IName> seenElements)
        {
            var identifier = new StringBuilder();
            identifier.AppendIf(parameter.IsParameterArray, ParameterName.VarArgsModifier + " ");
            identifier.AppendIf(parameter.Kind == ParameterKind.OUTPUT, ParameterName.OutputModifier + " ");
            identifier.AppendIf(parameter.IsExtensionFirstParameter(), ParameterName.ExtensionMethodModifier + " ");
            identifier.AppendIf(parameter.IsOptional, ParameterName.OptionalModifier + " ");
            identifier.AppendIf(parameter.Kind == ParameterKind.REFERENCE, ParameterName.PassByReferenceModifier + " ");
            identifier.AppendType(parameter.Type, seenElements).Append(" ").Append(parameter.ShortName);
            return Names.Parameter(identifier.ToString());
        }

        [NotNull]
        private static IMethodName GetName(this IFunction function,
            ISubstitution substitution,
            IDictionary<DeclaredElementInstance, IName> seenElements)
        {
            var identifier = new StringBuilder();
            identifier.Append(function.GetMemberIdentifier(substitution, function.ReturnType, seenElements));
            var typeParametersOwner = function as ITypeParametersOwner;
            if (typeParametersOwner != null && typeParametersOwner.TypeParameters.Any())
            {
                identifier.Append("`{0}".FormatEx(typeParametersOwner.TypeParameters.Count));
                identifier.Append(typeParametersOwner.GetTypeParametersList(substitution, seenElements));
            }
            identifier.AppendParameters(function, substitution, seenElements);
            return Names.Method(identifier.ToString());
        }

        [NotNull]
        private static IFieldName GetName(this IField field,
            ISubstitution substitution,
            IDictionary<DeclaredElementInstance, IName> seenElements)
        {
            return Names.Field(field.GetMemberIdentifier(substitution, field.Type, seenElements));
        }

        [NotNull]
        private static IEventName GetName(this IEvent evt,
            ISubstitution substitution,
            IDictionary<DeclaredElementInstance, IName> seenElements)
        {
            return Names.Event(evt.GetMemberIdentifier(substitution, evt.Type, seenElements));
        }

        [NotNull]
        private static IPropertyName GetName(this IProperty property,
            ISubstitution substitution,
            IDictionary<DeclaredElementInstance, IName> seenElements)
        {
            var identifier = new StringBuilder();
            identifier.AppendIf(property.IsWritable, PropertyName.SetterModifier + " ");
            identifier.AppendIf(property.IsReadable, PropertyName.GetterModifier + " ");
            identifier.Append(property.GetMemberIdentifier(substitution, property.ReturnType, seenElements));
            identifier.AppendParameters(property, substitution, seenElements);
            return Names.Property(identifier.ToString());
        }

        private static string GetMemberIdentifier(this ITypeMember member,
            ISubstitution substitution,
            IType valueType,
            IDictionary<DeclaredElementInstance, IName> seenElements)
        {
            var identifier = new StringBuilder();
            identifier.AppendIf(member.IsStatic, MemberName.StaticModifier + " ");
            identifier.AppendMemberBase(member, substitution, valueType, seenElements);
            return identifier.ToString();
        }

        [NotNull]
        private static ILocalVariableName GetName(this ITypeOwner variable,
            ISubstitution substitution,
            IDictionary<DeclaredElementInstance, IName> seenElements)
        {
            var identifier = new StringBuilder();
            identifier.AppendType(variable.Type, seenElements).Append(' ').Append(variable.ShortName);
            return Names.LocalVariable(identifier.ToString());
        }

        [NotNull]
        private static IName GetName(this IAlias alias,
            ISubstitution substitution,
            IDictionary<DeclaredElementInstance, IName> seenElements)
        {
            return Names.Alias(alias.ShortName);
        }

        [NotNull]
        public static ILambdaName GetName([NotNull] this ILambdaExpression lambdaExpression)
        {
            var seen = new Dictionary<DeclaredElementInstance, IName>();

            var identifier = new StringBuilder();
            identifier.AppendType(lambdaExpression.ReturnType, seen);
            identifier.Append(' ');
            identifier.AppendParameters(lambdaExpression.DeclaredParametersOwner, EmptySubstitution.INSTANCE, seen);
            return Names.Lambda(identifier.ToString());
        }

        [NotNull]
        public static ILambdaName GetName([NotNull] this IAnonymousMethodExpression anonymousMethodDecl)
        {
            var seen = new Dictionary<DeclaredElementInstance, IName>();

            var identifier = new StringBuilder();
            identifier.AppendType(anonymousMethodDecl.ReturnType, seen);
            identifier.Append(' ');
            identifier.AppendParameters(anonymousMethodDecl.DeclaredParametersOwner, EmptySubstitution.INSTANCE, seen);
            return Names.Lambda(identifier.ToString());
        }

        private static void AppendMemberBase(this StringBuilder identifier,
            IClrDeclaredElement member,
            ISubstitution substitution,
            IType valueType,
            IDictionary<DeclaredElementInstance, IName> seenElements)
        {
            identifier.AppendType(valueType, seenElements);
            identifier.Append(' ');
            var containingType = member.GetContainingType();
            identifier.AppendType(containingType, substitution, seenElements);
            identifier.Append('.');
            identifier.Append(member.ShortName);
        }

        private static StringBuilder AppendType(this StringBuilder identifier,
            IType type,
            IDictionary<DeclaredElementInstance, IName> seenElements)
        {
            return identifier.Append('[').Append(type.GetName(seenElements).Identifier).Append(']');
        }

        [NotNull]
        private static StringBuilder AppendType(this StringBuilder identifier,
            ITypeElement type,
            ISubstitution substitution,
            IDictionary<DeclaredElementInstance, IName> seenElements)
        {
            return identifier.Append('[').Append(type.GetName(substitution, seenElements).Identifier).Append(']');
        }

        [NotNull]
        private static string GetAssemblyQualifiedName(this ITypeElement type,
            ISubstitution substitution,
            IDictionary<DeclaredElementInstance, IName> seenElements)
        {
            if (type == null)
            {
                return Names.UnknownType.Identifier;
            }

            var clrTypeName = type.GetClrName();
            var containingModule = type.Module.ContainingProjectModule;
            Asserts.NotNull(containingModule, "module is null");
            var moduleName = containingModule.GetQualifiedName();

            var typeParameters = type.GetTypeParametersList(substitution, seenElements);

            string myName;
            var parent = type.GetContainingType();
            var myFullName = clrTypeName.FullName;
            if (parent != null)
            {
                var parentName = parent.GetName<ITypeName>(substitution);
                // including the generic `N ticks
                var parentFullName = parentName.FullName;
                // shortName does not include the generic `N ticks, so we have to find it in the fullname...
                var startOfShortName = myFullName.LastIndexOf("+", StringComparison.Ordinal) + 1;
                //  ... and ignore the leading part
                var fullShortName = myFullName.Substring(startOfShortName);
                myName = string.Format("{0}+{1}", parentFullName, fullShortName);
            }
            else
            {
                myName = myFullName;
            }

            return String.Format(
                "{0}{1}, {2}",
                myName,
                typeParameters,
                moduleName);
        }

        private static string GetTypeParametersList(this ITypeParametersOwner typeParametersOwner,
            ISubstitution substitution,
            IDictionary<DeclaredElementInstance, IName> seenElements)
        {
            if (typeParametersOwner.TypeParameters.IsEmpty())
            {
                return "";
            }
            var tps = new List<string>();
            foreach (var tp in typeParametersOwner.TypeParameters)
            {
                var name = tp.GetName(substitution, seenElements);
                tps.Add(name.Identifier);
            }
            return "[[{0}]]".FormatEx(tps.Join("],["));
        }

        /// <summary>
        ///     Retrieves the module's assembly-qualified name (including the assembly name and version). If the module
        ///     is a project returned name will only contain the project's name and not its version. According to
        ///     http://devnet.jetbrains.com/message/5503864#5503864 it is not generally possibly to retrieve the version
        ///     for projects. Therefore, we decided for this consistent solution.
        /// </summary>
        [NotNull]
        private static string GetQualifiedName([NotNull] this IModule module)
        {
            AssemblyNameInfo assembly = null;
            var containingAssembly = module as IAssembly;
            if (containingAssembly != null)
            {
                assembly = containingAssembly.AssemblyName;
            }
            return assembly != null ? assembly.NameAndVersion() : SanitizeAssemblyName(module.Name);
        }

        [NotNull]
        private static string NameAndVersion([NotNull] this AssemblyNameInfo assemblyName)
        {
            return string.Format("{0}, {1}", SanitizeAssemblyName(assemblyName.Name), assemblyName.Version);
        }

        public static string SanitizeAssemblyName(string input)
        {
            return Regex.Replace(input, "[^a-zA-Z0-9._-]", "_");
        }

        private static void AppendParameters(this StringBuilder identifier,
            IParametersOwner parametersOwner,
            ISubstitution substitution,
            IDictionary<DeclaredElementInstance, IName> seenElements)
        {
            identifier.Append('(')
                      .Append(
                          parametersOwner.Parameters.GetNames(substitution, seenElements)
                                         .Select(p => p.Identifier)
                                         .Join(", "))
                      .Append(')');
        }

        [NotNull]
        private static IEnumerable<IParameterName> GetNames(this IEnumerable<IParameter> parameters,
            ISubstitution substitution,
            IDictionary<DeclaredElementInstance, IName> seenElements)
        {
            return parameters.Select(param => param.GetName(substitution, seenElements));
        }
    }
}