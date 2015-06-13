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
 * 
 * Contributors:
 *    - Sven Amann
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using JetBrains.Metadata.Utils;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.Model2.Assemblies.Interfaces;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.DeclaredElements;
using JetBrains.ReSharper.Psi.ExtensionsAPI;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.Util;
using KaVE.JetBrains.Annotations;
using KaVE.Commons.Model.Names;
using KaVE.Commons.Model.Names.CSharp;
using KaVE.Commons.Utils;
using KaVE.Commons.Utils.Assertion;

namespace KaVE.VsFeedbackGenerator.Utils.Names
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
            return element.GetName(substitution, new Dictionary<IDeclaredElement, IName>());
        }
    
        [NotNull]
        internal static IName GetName([NotNull] this IDeclaredElement element, [NotNull] ISubstitution substitution, IDictionary<IDeclaredElement, IName> seenElements)
        {
            return IfElementIs<INamespaceName, INamespace>(element, GetName, substitution, NamespaceName.UnknownName, seenElements) ??
                   IfElementIs<ITypeName, ITypeParameter>(element, GetName, substitution, TypeName.UnknownName, seenElements) ??
                   IfElementIs<ITypeName, ITypeElement>(element, GetName, substitution, TypeName.UnknownName, seenElements) ??
                   IfElementIs<IMethodName, IFunction>(element, GetName, substitution, MethodName.UnknownName, seenElements) ??
                   IfElementIs<IParameterName, IParameter>(element, GetName, substitution, ParameterName.UnknownName, seenElements) ??
                   IfElementIs<IFieldName, IField>(element, GetName, substitution, FieldName.UnknownName, seenElements) ??
                   IfElementIs<IPropertyName, IProperty>(element, GetName, substitution, PropertyName.UnknownName, seenElements) ??
                   IfElementIs<IEventName, IEvent>(element, GetName, substitution, EventName.UnknownName, seenElements) ??
                   IfElementIs<IName, ITypeOwner>(element, GetName, substitution, LocalVariableName.UnknownName, seenElements) ??
                   IfElementIs<IName, IAlias>(element, GetName, substitution, AliasName.UnknownName, seenElements) ??
                   IfElementIs<IName, IDeclaredElement>(element, (e, s, a) => null, substitution, Name.UnknownName, seenElements) ??
                   FallbackHandler(element, substitution);
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
            IDictionary<IDeclaredElement, IName> seenElements)
            where TE : class, IDeclaredElement
            where TN : class, IName
        {
            var specificElement = element as TE;
            if (specificElement == null)
            {
                return null;
            }
            // exit if we encounter a recursive type, e.g., delegate IList<D> D();
            if (seenElements.ContainsKey(element))
            {
                return (TN) seenElements[element];
            }
            // this makes us default to the unknownName, if we reencounter an element while resolving it
            seenElements[element] = unknownName;
            // after this call we have resolved the element and cached the result
            seenElements[element] = IsMissingDeclaration(specificElement) ? unknownName : map(specificElement, substitution, seenElements);
            return (TN) seenElements[element];
        }

        private delegate TN DeclaredElementToName<out TN, in TE>(TE element, ISubstitution substitution, IDictionary<IDeclaredElement, IName> seenElements)
            where TE : class, IDeclaredElement
            where TN : class, IName;

        [NotNull]
        private static ITypeName GetName(this ITypeElement typeElement, ISubstitution substitution, IDictionary<IDeclaredElement, IName> seenElements)
        {
            return IfElementIs<IDelegate>(typeElement, GetName, substitution, seenElements, DelegateTypeName.UnknownName) ??
                   IfElementIs<IEnum>(typeElement, GetName, substitution, seenElements, TypeName.UnknownName) ??
                   IfElementIs<IInterface>(typeElement, GetName, substitution, seenElements, TypeName.UnknownName) ??
                   IfElementIs<IStruct>(typeElement, GetName, substitution, seenElements, TypeName.UnknownName) ??
                   TypeName.Get(typeElement.GetAssemblyQualifiedName(substitution, seenElements));
        }

        private static ITypeName IfElementIs<TE>(ITypeElement typeElement,
            DeclaredElementToName<ITypeName, TE> map,
            ISubstitution substitution, IDictionary<IDeclaredElement, IName> seenElements, ITypeName unknowName)
            where TE : class, IDeclaredElement
        {
            return IfElementIs(typeElement, map, substitution, unknowName, seenElements);
        }

        [NotNull]
        private static ITypeName GetName(this IDelegate delegateElement, ISubstitution substitution, IDictionary<IDeclaredElement, IName> seenElements)
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
        private static ITypeName GetName(this IEnum enumElement, ISubstitution substitution, IDictionary<IDeclaredElement, IName> seenElements)
        {
            return TypeName.Get("e:" + enumElement.GetAssemblyQualifiedName(substitution, seenElements));
        }

        [NotNull]
        private static ITypeName GetName(this IInterface interfaceElement, ISubstitution substitution, IDictionary<IDeclaredElement, IName> seenElements)
        {
            return TypeName.Get("i:" + interfaceElement.GetAssemblyQualifiedName(substitution, seenElements));
        }

        [NotNull]
        private static ITypeName GetName(this IStruct structElement, ISubstitution substitution, IDictionary<IDeclaredElement, IName> seenElements)
        {
            var structName = structElement.GetAssemblyQualifiedName(substitution, seenElements);
            var typeNameCandidate = TypeName.Get(structName);
            // predefined structs are recognized as such without flagging them
            var isPredefinedStruct = typeNameCandidate.IsStructType;
            return isPredefinedStruct ? typeNameCandidate : TypeName.Get("s:" + structName);
        }

        [NotNull]
        private static ITypeName GetName(this ITypeParameter typeParameter, ISubstitution substitution, IDictionary<IDeclaredElement, IName> seenElements)
        {
            return TypeParameterName.Get(
                typeParameter.ShortName,
                typeParameter.GetAssemblyQualifiedNameFromActualType(substitution, seenElements));
        }

        private static string GetAssemblyQualifiedNameFromActualType(this ITypeParameter typeParameter,
            ISubstitution substitution, IDictionary<IDeclaredElement, IName> seenElements)
        {
            return substitution.Domain.Contains(typeParameter)
                ? substitution[typeParameter].GetName(seenElements).Identifier
                : UnknownTypeName.Identifier;
        }

        [NotNull]
        private static INamespaceName GetName(this INamespace ns, ISubstitution substitution, IDictionary<IDeclaredElement, IName> seenElements)
        {
            return NamespaceName.Get(ns.QualifiedName);
        }

        [NotNull]
        private static IParameterName GetName(this IParameter parameter, ISubstitution substitution, IDictionary<IDeclaredElement, IName> seenElements)
        {
            var identifier = new StringBuilder();
            identifier.AppendIf(parameter.IsParameterArray, ParameterName.VarArgsModifier + " ");
            identifier.AppendIf(parameter.Kind == ParameterKind.OUTPUT, ParameterName.OutputModifier + " ");
            identifier.AppendIf(parameter.IsOptional, ParameterName.OptionalModifier + " ");
            identifier.AppendIf(parameter.Kind == ParameterKind.REFERENCE, ParameterName.PassByReferenceModifier + " ");
            identifier.AppendType(parameter.Type, seenElements).Append(" ").Append(parameter.ShortName);
            return ParameterName.Get(identifier.ToString());
        }

        [NotNull]
        private static IMethodName GetName(this IFunction function, ISubstitution substitution, IDictionary<IDeclaredElement, IName> seenElements)
        {
            var identifier = new StringBuilder();
            identifier.Append(function.GetMemberIdentifier(substitution, function.ReturnType,seenElements));
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
        private static IFieldName GetName(this IField field, ISubstitution substitution, IDictionary<IDeclaredElement, IName> seenElements)
        {
            return FieldName.Get(field.GetMemberIdentifier(substitution, field.Type, seenElements));
        }

        [NotNull]
        private static IEventName GetName(this IEvent evt, ISubstitution substitution, IDictionary<IDeclaredElement, IName> seenElements)
        {
            return EventName.Get(evt.GetMemberIdentifier(substitution, evt.Type,seenElements));
        }

        [NotNull]
        private static IPropertyName GetName(this IProperty property, ISubstitution substitution, IDictionary<IDeclaredElement, IName> seenElements)
        {
            var identifier = new StringBuilder();
            identifier.AppendIf(property.IsWritable, PropertyName.SetterModifier + " ");
            identifier.AppendIf(property.IsReadable, PropertyName.GetterModifier + " ");
            identifier.Append(property.GetMemberIdentifier(substitution, property.ReturnType, seenElements));
            identifier.AppendParameters(property, substitution, seenElements);
            return PropertyName.Get(identifier.ToString());
        }

        private static string GetMemberIdentifier(this ITypeMember member, ISubstitution substitution, IType valueType, IDictionary<IDeclaredElement, IName> seenElements)
        {
            var identifier = new StringBuilder();
            identifier.AppendIf(member.IsStatic, MemberName.StaticModifier + " ");
            identifier.AppendMemberBase(member, substitution, valueType, seenElements);
            return identifier.ToString();
        }

        [NotNull]
        private static LocalVariableName GetName(this ITypeOwner variable, ISubstitution substitution, IDictionary<IDeclaredElement, IName> seenElements)
        {
            var identifier = new StringBuilder();
            identifier.AppendType(variable.Type, seenElements).Append(' ').Append(variable.ShortName);
            return LocalVariableName.Get(identifier.ToString());
        }

        [NotNull]
        private static IName GetName(this IAlias alias, ISubstitution substitution, IDictionary<IDeclaredElement, IName> seenElements)
        {
            return AliasName.Get(alias.ShortName);
        }

        private static void AppendMemberBase(this StringBuilder identifier,
            IClrDeclaredElement member,
            ISubstitution substitution,
            IType valueType, IDictionary<IDeclaredElement, IName> seenElements)
        {
            identifier.AppendType(valueType, seenElements)
                      .Append(' ')
                      .AppendType(member.GetContainingType(), substitution, seenElements)
                      .Append('.')
                      .Append(member.ShortName);
        }

        private static StringBuilder AppendType(this StringBuilder identifier, IType type, IDictionary<IDeclaredElement, IName> seenElements)
        {
            return identifier.Append('[').Append(type.GetName(seenElements).Identifier).Append(']');
        }

        [NotNull]
        private static StringBuilder AppendType(this StringBuilder identifier,
            ITypeElement type,
            ISubstitution substitution, IDictionary<IDeclaredElement, IName> seenElements)
        {
            return identifier.Append('[').Append(type.GetName(substitution, seenElements).Identifier).Append(']');
        }

        [NotNull]
        private static String GetAssemblyQualifiedName(this ITypeElement type, ISubstitution substitution, IDictionary<IDeclaredElement, IName> seenElements)
        {
            if (type == null)
            {
                return UnknownTypeName.Identifier;
            }
            var containingModule = type.Module.ContainingProjectModule;
            Asserts.NotNull(containingModule, "module is null");
            return String.Format(
                "{0}{1}, {2}",
                type.GetClrName().FullName,
                type.GetTypeParametersList(substitution, seenElements),
                containingModule.GetQualifiedName());
        }

        private static String GetTypeParametersList(this ITypeParametersOwner typeParametersOwner,
            ISubstitution substitution, IDictionary<IDeclaredElement, IName> seenElements)
        {
            var stackTrace = new StackTrace();
            if (stackTrace.FrameCount > 500)
            {
                
            }

            return typeParametersOwner.TypeParameters.IsEmpty()
                ? ""
                : "[[" +
                  typeParametersOwner.TypeParameters.Select(tp => tp.GetName(substitution, seenElements).Identifier).Join("],[") +
                  "]]";
        }

        /// <summary>
        ///     Retrieves the module's assembly-qualified name (including the assembly name and version). If the module
        ///     is a project and that project is currently not compilable (and has not been compiled ever or since the
        ///     last clear) the returned name will only contain the project's name and not its version. According to
        ///     http://devnet.jetbrains.com/message/5503864#5503864 this is a restriction of ReSharper. Note that the
        ///     project's name may differ from the project's output-assembly name.
        /// </summary>
        [NotNull]
        private static string GetQualifiedName([NotNull] this IModule module)
        {
            AssemblyNameInfo assembly = null;
            var containingProject = module as IProject;
            if (containingProject != null)
            {
                var assemblyInfo = containingProject.GetOutputAssemblyInfo();
                if (assemblyInfo != null)
                {
                    assembly = assemblyInfo.AssemblyNameInfo;
                }
            }
            var containingAssembly = module as IAssembly;
            if (containingAssembly != null)
            {
                assembly = containingAssembly.AssemblyName;
            }
            return assembly != null ? assembly.NameAndVersion() : module.Name;
        }

        [NotNull]
        private static string NameAndVersion([NotNull] this AssemblyNameInfo assemblyName)
        {
            return string.Format("{0}, {1}", assemblyName.Name, assemblyName.Version);
        }

        private static void AppendParameters(this StringBuilder identifier,
            IParametersOwner parametersOwner,
            ISubstitution substitution, IDictionary<IDeclaredElement, IName> seenElements)
        {
            identifier.Append('(')
                      .Append(parametersOwner.Parameters.GetNames(substitution, seenElements).Select(p => p.Identifier).Join(", "))
                      .Append(')');
        }

        [NotNull]
        private static IEnumerable<IParameterName> GetNames(this IEnumerable<IParameter> parameters,
            ISubstitution substitution, IDictionary<IDeclaredElement, IName> seenElements)
        {
            return parameters.Select(param => param.GetName(substitution, seenElements));
        }
    }
}