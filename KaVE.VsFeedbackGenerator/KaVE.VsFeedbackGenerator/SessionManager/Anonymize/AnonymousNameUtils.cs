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
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using JetBrains.Annotations;
using JetBrains.Util;
using KaVE.Model.Names;
using KaVE.Model.Names.CSharp;
using KaVE.Model.Names.VisualStudio;
using KaVE.Utils;
using KaVE.Utils.Assertion;
using KaVE.VsFeedbackGenerator.Utils;
using KaVE.VsFeedbackGenerator.Utils.Names;

namespace KaVE.VsFeedbackGenerator.SessionManager.Anonymize
{
    internal static class AnonymousNameUtils
    {
        [NotNull]
        public static string ToHash([NotNull] this string value)
        {
            var tmpSource = value.AsBytes();
            var hash = new MD5CryptoServiceProvider().ComputeHash(tmpSource);
            return Convert.ToBase64String(hash).Replace('+', '-').Replace('/', '_');
        }

        [ContractAnnotation("notnull => notnull")]
        public static TName ToAnonymousName<TName>(this TName name) where TName : class, IName
        {
            if (name == null) return null;
            return ToAnonymousName<DocumentName, TName>(name, ToAnonymousName) ??
                   ToAnonymousName<WindowName, TName>(name, ToAnonymousName) ??
                   ToAnonymousName<SolutionName, TName>(name, ToAnonymousName) ??
                   ToAnonymousName<ProjectName, TName>(name, ToAnonymousName) ??
                   ToAnonymousName<ProjectItemName, TName>(name, ToAnonymousName) ??
                   ToAnonymousName<AliasName, TName>(name, ToAnonymousName) ??
                   ToAnonymousName<IAssemblyName, TName>(name, ToAnonymousName) ??
                   ToAnonymousName<ITypeName, TName>(name, ToAnonymousName) ??
                   ToAnonymousName<LocalVariableName, TName>(name, ToAnonymousName) ??
                   ToAnonymousName<IFieldName, TName>(name, ToAnonymousName) ??
                   ToAnonymousName<IPropertyName, TName>(name, ToAnonymousName) ??
                   ToAnonymousName<IEventName, TName>(name, ToAnonymousName) ??
                   ToAnonymousName<IParameterName, TName>(name, ToAnonymousName) ??
                   ToAnonymousName<IMethodName, TName>(name, ToAnonymousName) ??
                   ToAnonymousName<INamespaceName, TName>(name, ToAnonymousName) ??
                   ToAnonymousName<IName, TName>(name, ToAnonymousName) ??
                   Asserts.Fail<TName>("unhandled name type: {0}", name.GetType());
        }

        private static TR ToAnonymousName<TName, TR>(TR name, Func<TName, TName> anonymizer)
            where TName : class, IName where TR : class, IName
        {
            var concreteName = name as TName;
            return (concreteName != null ? anonymizer(concreteName) : null) as TR;
        }

        private static INamespaceName ToAnonymousName(INamespaceName @namespace)
        {
            return NamespaceName.Get(@namespace.Identifier.ToHash());
        }

        private static IMethodName ToAnonymousName(IMethodName method)
        {
            var identifier = new StringBuilder();
            identifier.AppendAnonymousMemberName(method, method.ReturnType);
            identifier.AppendIf(method.HasTypeParameters, "`" + method.TypeParameters.Count);
            identifier.AppendTypeParameters(method);
            identifier.Append("(");
            identifier.Append(string.Join(", ", method.Parameters.Select(p => p.ToAnonymousName())));
            identifier.Append(")");
            return MethodName.Get(identifier.ToString());
        }

        private static IParameterName ToAnonymousName(IParameterName parameter)
        {
            var identifier = new StringBuilder();
            identifier.AppendIf(parameter.IsParameterArray, ParameterName.VarArgsModifier + " ");
            identifier.AppendIf(parameter.IsOutput, ParameterName.OutputModifier + " ");
            identifier.AppendIf(parameter.IsOptional, ParameterName.OptionalModifier + " ");
            identifier.AppendIf(parameter.HasPassByReferenceModifier(), ParameterName.PassByReferenceModifier + " ");
            identifier.AppendAnonymousTypeName(parameter.ValueType).Append(' ');
            identifier.Append(parameter.Name.ToHash());
            return ParameterName.Get(identifier.ToString());
        }

        private static bool HasPassByReferenceModifier(this IParameterName parameter)
        {
            return parameter.IsPassedByReference && !parameter.ValueType.IsReferenceType;
        }

        private static StringBuilder AppendAnonymousTypeName(this StringBuilder identifier, ITypeName type)
        {
            return identifier.Append('[').Append(type.ToAnonymousName()).Append(']');
        }

        private static IEventName ToAnonymousName(IEventName @event)
        {
            var identifier = new StringBuilder();
            identifier.AppendAnonymousMemberName(@event, @event.HandlerType);
            return EventName.Get(identifier.ToString());
        }

        private static IPropertyName ToAnonymousName(IPropertyName property)
        {
            var identifier = new StringBuilder();
            identifier.AppendIf(property.HasSetter, PropertyName.SetterModifier + " ");
            identifier.AppendIf(property.HasGetter, PropertyName.GetterModifier + " ");
            identifier.AppendAnonymousMemberName(property, property.ValueType);
            return PropertyName.Get(identifier.ToString());
        }

        private static IFieldName ToAnonymousName(IFieldName field)
        {
            var identifier = new StringBuilder();
            identifier.AppendAnonymousMemberName(field, field.ValueType);
            return FieldName.Get(identifier.ToString());
        }

        private static void AppendAnonymousMemberName(this StringBuilder identifier,
            IMemberName member,
            ITypeName valueType)
        {
            identifier.AppendIf(member.IsStatic, MemberName.StaticModifier + " ");
            identifier.AppendAnonymousTypeName(valueType).Append(' ');
            identifier.AppendAnonymousTypeName(member.DeclaringType).Append('.');
            identifier.Append(member.DeclaringType.IsDeclaredInEnclosingProjectOrUnknown() ? member.Name.ToHash() : member.Name);
        }

        private static LocalVariableName ToAnonymousName(LocalVariableName variable)
        {
            var identifier = new StringBuilder();
            identifier.AppendAnonymousTypeName(variable.ValueType).Append(' ');
            identifier.Append(variable.Name.ToHash());
            return LocalVariableName.Get(identifier.ToString());
        }

        private static IName ToAnonymousName(IName name)
        {
            return Name.Get(name.Identifier.ToHash());
        }

        private static ITypeName ToAnonymousName(ITypeName type)
        {
            return ToAnonymousName<UnknownTypeName, ITypeName>(type, ToAnonymousName) ??
                   ToAnonymousName<TypeName, ITypeName>(type, ToAnonymousName) ??
                   ToAnonymousName<TypeParameterName, ITypeName>(type, ToAnonymousName) ??
                   Asserts.Fail<ITypeName>("unknown type implementation");
        }

        private static UnknownTypeName ToAnonymousName(UnknownTypeName type)
        {
            return type;
        }

        private static TypeName ToAnonymousName(TypeName type)
        {
            var identifier = new StringBuilder();
            identifier.AppendTypeKindPrefix(type);
            identifier.Append(type.IsDeclaredInEnclosingProjectOrUnknown() ? type.AnonymizedRawFullName() : type.RawFullName);
            identifier.AppendTypeParameters(type).Append(", ");
            identifier.Append(type.Assembly.ToAnonymousName());
            return (TypeName) TypeName.Get(identifier.ToString());
        }

        private static void AppendTypeKindPrefix(this StringBuilder identifier, TypeName type)
        {
            var prefix = type.Identifier.Substring(0, 2);
            if (prefix.EndsWith(":"))
            {
                identifier.Append(prefix);
            }
        }

        private static string AnonymizedRawFullName(this TypeName type)
        {
            // We want to keep the number of type parameters (`1), array braces ([]), nesting markers (+), and the
            // separation between the namespace and the class name. Examples of raw names in the cases we,
            // therefore, handle are:
            // * Namespace.TypeName`1
            // * OuterType+InnerType[]
            // * TypeName`2[,]
            var @namespace = type.Namespace;
            var rawFullName = type.RawFullName;
            rawFullName = rawFullName.Substring(@namespace.Identifier.Length);
            var baseName = rawFullName;
            var suffix = "";
            var indexOfDelimiter = rawFullName.IndexOfAny(new[] {'`', '['});
            if (indexOfDelimiter > -1)
            {
                suffix = rawFullName.Substring(indexOfDelimiter);
                baseName = rawFullName.Substring(0, indexOfDelimiter);
            }
            var baseNameParts = baseName.Split('+');
            if (baseNameParts.Length > 0)
            {
                baseName = baseNameParts.Select(ToHash).Join("+");
            }
            else
            {
                baseName = baseName.ToHash();
            }
            if (@namespace.IsGlobalNamespace)
            {
                return baseName + suffix;
            }
            return @namespace.ToAnonymousName() + "." + baseName + suffix;
        }

        private static StringBuilder AppendTypeParameters(this StringBuilder identifier, IGenericName type)
        {
            if (type.HasTypeParameters)
            {
                identifier.Append("[[");
                identifier.Append(
                    string.Join("],[", type.TypeParameters.Select(typeParameter => typeParameter.ToAnonymousName())));
                identifier.Append("]]");
            }
            return identifier;
        }

        private static TypeParameterName ToAnonymousName(TypeParameterName typeParameter)
        {
            return (TypeParameterName) TypeParameterName.Get(
                typeParameter.TypeParameterShortName,
                typeParameter.TypeParameterType.ToAnonymousName().Identifier);
        }

        private static IAssemblyName ToAnonymousName(IAssemblyName assembly)
        {
            return assembly.IsEnclosingProject() ? AssemblyName.Get(assembly.Identifier.ToHash()) : assembly;
        }

        private static bool IsDeclaredInEnclosingProjectOrUnknown(this ITypeName type)
        {
            return type.IsUnknownType || type.Assembly.IsEnclosingProject();
        }

        private static bool IsEnclosingProject(this IAssemblyName assembly)
        {
            return assembly.AssemblyVersion == AssemblyVersion.UnknownName;
        }

        private static AliasName ToAnonymousName(AliasName alias)
        {
            return AliasName.Get(alias.Identifier.ToHash());
        }

        [ContractAnnotation("notnull => notnull")]
        public static DocumentName ToAnonymousName([CanBeNull] this DocumentName document)
        {
            return document == null
                ? null
                : CreateAnonymizedName(VsComponentNameFactory.GetDocumentName, document.Language, document.FileName);
        }

        [ContractAnnotation("notnull => notnull")]
        public static ProjectItemName ToAnonymousName([CanBeNull] this ProjectItemName projectItem)
        {
            return projectItem == null
                ? null
                : CreateAnonymizedName(VsComponentNameFactory.GetProjectItemName, projectItem.Type, projectItem.Name);
        }

        [ContractAnnotation("notnull => notnull")]
        public static ProjectName ToAnonymousName([CanBeNull] this ProjectName project)
        {
            return project == null
                ? null
                : CreateAnonymizedName(VsComponentNameFactory.GetProjectName, project.Type, project.Name);
        }

        [ContractAnnotation("notnull => notnull")]
        public static SolutionName ToAnonymousName([CanBeNull] this SolutionName solution)
        {
            return solution == null
                ? null
                : VsComponentNameFactory.GetSolutionName(solution.Identifier.ToHash());
        }

        [ContractAnnotation("notnull => notnull")]
        public static WindowName ToAnonymousName([CanBeNull] this WindowName window)
        {
            return window == null
                ? null
                : CreateAnonymizedName(VsComponentNameFactory.GetWindowName, window.Type, window.Caption);
        }

        private static TName CreateAnonymizedName<TName>(Func<string, string, TName> factory, string type, string name)
        {
            var isFileName = name.Contains("\\") || name.Contains(".");
            if (isFileName)
            {
                name = name.ToHash();
            }
            return factory(type, name);
        }
    }
}