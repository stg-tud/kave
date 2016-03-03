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
using JetBrains.Util;
using KaVE.Commons.Model.Names;
using KaVE.Commons.Model.Names.CSharp;
using KaVE.Commons.Utils;
using KaVE.Commons.Utils.Assertion;

namespace KaVE.RS.Commons.Utils.Names
{
    public static class ReSharperDeclaredElementNameFactory
    {
        [NotNull]
        public static TName GetName<TName>([NotNull] this DeclaredElementInstance instance) where TName : class, IName
        {
            return (TName) instance.GetName();
        }

        [NotNull]
        public static IName GetName([NotNull] this DeclaredElementInstance instance)
        {
            return instance.Element.GetName(instance.Substitution);
        }

        [NotNull]
        public static TName GetName<TName>([NotNull] this IClrDeclaredElement element) where TName : class, IName
        {
            return (TName) element.GetName();
        }

        [NotNull]
        public static TName GetName<TName>([NotNull] this IDeclaredElement element, [NotNull] ISubstitution substitution)
            where TName : class, IName
        {
            return (TName) element.GetName(substitution);
        }

        [NotNull]
        public static IName GetName([NotNull] this IClrDeclaredElement element)
        {
            return element.GetName(element.IdSubstitution);
        }

        [NotNull]
        public static IName GetName([NotNull] this IDeclaredElement element, [NotNull] ISubstitution substitution)
        {
            return element.GetName(substitution, new Dictionary<DeclaredElementInstance, IName>());
        }

        [NotNull]
        internal static IName GetName([NotNull] this IDeclaredElement elem,
            [NotNull] ISubstitution substitution,
            IDictionary<DeclaredElementInstance, IName> seen)
        {
            return
                IfElementIs<INamespaceName, INamespace>(elem, GetName, substitution, NamespaceName.UnknownName, seen) ??
                IfElementIs<ITypeName, ITypeParameter>(elem, GetName, substitution, TypeName.UnknownName, seen) ??
                IfElementIs<ITypeName, ITypeElement>(elem, GetName, substitution, TypeName.UnknownName, seen) ??
                IfElementIs<IMethodName, IFunction>(elem, GetName, substitution, MethodName.UnknownName, seen) ??
                IfElementIs<IParameterName, IParameter>(elem, GetName, substitution, ParameterName.UnknownName, seen) ??
                IfElementIs<IFieldName, IField>(elem, GetName, substitution, FieldName.UnknownName, seen) ??
                IfElementIs<IPropertyName, IProperty>(elem, GetName, substitution, PropertyName.UnknownName, seen) ??
                IfElementIs<IEventName, IEvent>(elem, GetName, substitution, EventName.UnknownName, seen) ??
                IfElementIs<IName, ITypeOwner>(elem, GetName, substitution, LocalVariableName.UnknownName, seen) ??
                IfElementIs<IName, IAlias>(elem, GetName, substitution, AliasName.UnknownName, seen) ??
                IfElementIs<IName, IDeclaredElement>(elem, (e, s, a) => null, substitution, Name.UnknownName, seen) ??
                FallbackHandler(elem, substitution);
        }

        private static IName FallbackHandler(IDeclaredElement element, ISubstitution substitution)
        {
            var typeName = element.GetType().FullName;
            var elementName = element.ShortName;
            var id = string.Format("UnknownDeclaredElement: {0}, '{1}', {2}", typeName, elementName, substitution);
            return Name.Get(id);
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
        private static ITypeName GetName(this ITypeElement typeElement,
            ISubstitution substitution,
            IDictionary<DeclaredElementInstance, IName> seenElements)
        {
            return
                IfElementIs<IDelegate>(typeElement, GetName, substitution, seenElements, DelegateTypeName.UnknownName) ??
                IfElementIs<IEnum>(typeElement, GetName, substitution, seenElements, TypeName.UnknownName) ??
                IfElementIs<IInterface>(typeElement, GetName, substitution, seenElements, TypeName.UnknownName) ??
                IfElementIs<IStruct>(typeElement, GetName, substitution, seenElements, TypeName.UnknownName) ??
                TypeName.Get(typeElement.GetAssemblyQualifiedName(substitution, seenElements));
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
            var invokeMethod = delegateElement.InvokeMethod;
            var identifier = new StringBuilder("d:");
            identifier.AppendType(invokeMethod.ReturnType, seenElements)
                      .Append(" [")
                      .Append(delegateElement.GetAssemblyQualifiedName(substitution, seenElements))
                      .Append("].")
                      .AppendParameters(invokeMethod, substitution, seenElements);
            return TypeName.Get(identifier.ToString());
        }

        [NotNull]
        private static ITypeName GetName(this IEnum enumElement,
            ISubstitution substitution,
            IDictionary<DeclaredElementInstance, IName> seenElements)
        {
            return TypeName.Get("e:" + enumElement.GetAssemblyQualifiedName(substitution, seenElements));
        }

        [NotNull]
        private static ITypeName GetName(this IInterface interfaceElement,
            ISubstitution substitution,
            IDictionary<DeclaredElementInstance, IName> seenElements)
        {
            return TypeName.Get("i:" + interfaceElement.GetAssemblyQualifiedName(substitution, seenElements));
        }

        [NotNull]
        private static ITypeName GetName(this IStruct structElement,
            ISubstitution substitution,
            IDictionary<DeclaredElementInstance, IName> seenElements)
        {
            var structName = structElement.GetAssemblyQualifiedName(substitution, seenElements);
            var typeNameCandidate = TypeName.Get(structName);
            // predefined structs are recognized as such without flagging them
            var isPredefinedStruct = typeNameCandidate.IsStructType;
            return isPredefinedStruct ? typeNameCandidate : TypeName.Get("s:" + structName);
        }

        [NotNull]
        private static ITypeName GetName(this ITypeParameter typeParameter,
            ISubstitution substitution,
            IDictionary<DeclaredElementInstance, IName> seenElements)
        {
            return TypeParameterName.Get(
                typeParameter.ShortName,
                typeParameter.GetAssemblyQualifiedNameFromActualType(substitution, seenElements));
        }

        private static string GetAssemblyQualifiedNameFromActualType(this ITypeParameter typeParameter,
            ISubstitution substitution,
            IDictionary<DeclaredElementInstance, IName> seenElements)
        {
            if (substitution.Domain.Contains(typeParameter))
            {
                var type = substitution[typeParameter];
                return type.GetName(seenElements).Identifier;
            }
            return UnknownTypeName.Identifier;
        }

        [NotNull]
        private static INamespaceName GetName(this INamespace ns,
            ISubstitution substitution,
            IDictionary<DeclaredElementInstance, IName> seenElements)
        {
            return NamespaceName.Get(ns.QualifiedName);
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
            return ParameterName.Get(identifier.ToString());
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
                identifier.Append('`').Append(typeParametersOwner.TypeParameters.Count);
                identifier.Append(typeParametersOwner.GetTypeParametersList(substitution, seenElements));
            }
            identifier.AppendParameters(function, substitution, seenElements);
            return MethodName.Get(identifier.ToString());
        }

        [NotNull]
        private static IFieldName GetName(this IField field,
            ISubstitution substitution,
            IDictionary<DeclaredElementInstance, IName> seenElements)
        {
            return FieldName.Get(field.GetMemberIdentifier(substitution, field.Type, seenElements));
        }

        [NotNull]
        private static IEventName GetName(this IEvent evt,
            ISubstitution substitution,
            IDictionary<DeclaredElementInstance, IName> seenElements)
        {
            return EventName.Get(evt.GetMemberIdentifier(substitution, evt.Type, seenElements));
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
            return PropertyName.Get(identifier.ToString());
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
        private static LocalVariableName GetName(this ITypeOwner variable,
            ISubstitution substitution,
            IDictionary<DeclaredElementInstance, IName> seenElements)
        {
            var identifier = new StringBuilder();
            identifier.AppendType(variable.Type, seenElements).Append(' ').Append(variable.ShortName);
            return LocalVariableName.Get(identifier.ToString());
        }

        [NotNull]
        private static IName GetName(this IAlias alias,
            ISubstitution substitution,
            IDictionary<DeclaredElementInstance, IName> seenElements)
        {
            return AliasName.Get(alias.ShortName);
        }

        [NotNull]
        public static ILambdaName GetName([NotNull] this ILambdaExpression lambdaExpression)
        {
            var seen = new Dictionary<DeclaredElementInstance, IName>();

            var identifier = new StringBuilder();
            identifier.AppendType(lambdaExpression.ReturnType, seen);
            identifier.Append(' ');
            identifier.AppendParameters(lambdaExpression.DeclaredParametersOwner, EmptySubstitution.INSTANCE, seen);
            return LambdaName.Get(identifier.ToString());
        }

        [NotNull]
        public static ILambdaName GetName([NotNull] this IAnonymousMethodExpression anonymousMethodDecl)
        {
            var seen = new Dictionary<DeclaredElementInstance, IName>();

            var identifier = new StringBuilder();
            identifier.AppendType(anonymousMethodDecl.ReturnType, seen);
            identifier.Append(' ');
            identifier.AppendParameters(anonymousMethodDecl.DeclaredParametersOwner, EmptySubstitution.INSTANCE, seen);
            return LambdaName.Get(identifier.ToString());
        }

        private static void AppendMemberBase(this StringBuilder identifier,
            IClrDeclaredElement member,
            ISubstitution substitution,
            IType valueType,
            IDictionary<DeclaredElementInstance, IName> seenElements)
        {
            identifier.AppendType(valueType, seenElements)
                      .Append(' ')
                      .AppendType(member.GetContainingType(), substitution, seenElements)
                      .Append('.')
                      .Append(member.ShortName);
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
        private static String GetAssemblyQualifiedName(this ITypeElement type,
            ISubstitution substitution,
            IDictionary<DeclaredElementInstance, IName> seenElements)
        {
            if (type == null)
            {
                return UnknownTypeName.Identifier;
            }

            var clrTypeName = type.GetClrName();
            var containingModule = type.Module.ContainingProjectModule;
            Asserts.NotNull(containingModule, "module is null");
            var moduleName = containingModule.GetQualifiedName();

            var typeParameters = type.GetTypeParametersList(substitution, seenElements);

            string myName;
            var parent = type.GetContainingType();
            if (parent != null)
            {
                var parentName = parent.GetName<ITypeName>(substitution).FullName;
                myName = string.Format("{0}+{1}", parentName, clrTypeName.ShortName);
            }
            else
            {
                myName = clrTypeName.FullName;
            }

            return String.Format(
                "{0}{1}, {2}",
                myName,
                typeParameters,
                moduleName);
        }

        private static String GetTypeParametersList(this ITypeParametersOwner typeParametersOwner,
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
            return "[[" + tps.Join("],[") + "]]";
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