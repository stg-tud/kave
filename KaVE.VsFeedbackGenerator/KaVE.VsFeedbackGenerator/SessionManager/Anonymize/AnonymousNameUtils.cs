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
        [JetBrains.Annotations.NotNull]
        public static string ToHash([JetBrains.Annotations.NotNull] this string value)
        {
            var tmpSource = value.AsBytes();
            var hash = new MD5CryptoServiceProvider().ComputeHash(tmpSource);
            return Convert.ToBase64String(hash);
        }

        [ContractAnnotation("notnull => notnull")]
        public static IName ToAnonymousName<TName>(this TName name) where TName : class, IName
        {
            return ToAnonymousName<DocumentName>(name, ToAnonymousName) ??
                   ToAnonymousName<WindowName>(name, ToAnonymousName) ??
                   ToAnonymousName<SolutionName>(name, ToAnonymousName) ??
                   ToAnonymousName<ProjectName>(name, ToAnonymousName) ??
                   ToAnonymousName<ProjectItemName>(name, ToAnonymousName) ??
                   ToAnonymousName<AliasName>(name, ToAnonymousName) ??
                   ToAnonymousName<IAssemblyName>(name, ToAnonymousName) ??
                   ToAnonymousName<ITypeName>(name, ToAnonymousName) ??
                   ToAnonymousName<LocalVariableName>(name, ToAnonymousName) ??
                   ToAnonymousName<IFieldName>(name, ToAnonymousName) ??
                   ToAnonymousName<IPropertyName>(name, ToAnonymousName) ??
                   ToAnonymousName<IEventName>(name, ToAnonymousName) ??
                   ToAnonymousName<IParameterName>(name, ToAnonymousName) ??
                   ToAnonymousName<Name>(name, ToAnonymousName) ??
                   Asserts.Fail<IName>("unhandled name type");
        }

        private static TName ToAnonymousName<TName>(IName name, Func<TName, TName> anonymizer)
            where TName : class, IName
        {
            var concreteName = name as TName;
            return concreteName != null ? anonymizer(concreteName) : null;
        }

        private static IParameterName ToAnonymousName(IParameterName parameter)
        {
            var identifier = new StringBuilder();
            identifier.AppendIf(parameter.IsParameterArray, ParameterName.VarArgsModifier + " ");
            identifier.AppendIf(parameter.IsOutput, ParameterName.OutputModifier + " ");
            identifier.AppendIf(parameter.IsOptional, ParameterName.OptionalModifier + " ");
            var isNonReferenceTypePassedByReference = parameter.IsPassedByReference && !parameter.ValueType.IsReferenceType;
            identifier.AppendIf(isNonReferenceTypePassedByReference, ParameterName.PassByReferenceModifier + " ");
            identifier.Append("[");
            identifier.Append(parameter.ValueType.ToAnonymousName());
            identifier.Append("] ");
            identifier.Append(parameter.Name.ToHash());
            return ParameterName.Get(identifier.ToString());
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

        private static void AppendAnonymousMemberName(this StringBuilder identifier, IMemberName member, ITypeName valueType)
        {
            identifier.AppendIf(member.IsStatic, MemberName.StaticModifier + " ");
            identifier.Append('[');
            identifier.Append(valueType.ToAnonymousName());
            identifier.Append("] [");
            identifier.Append(member.DeclaringType.ToAnonymousName());
            identifier.Append("].");
            identifier.Append(member.DeclaringType.IsDeclaredInEnclosingProject() ? member.Name.ToHash() : member.Name);
        }

        private static LocalVariableName ToAnonymousName(LocalVariableName variable)
        {
            return LocalVariableName.Get("[" + variable.ValueType.ToAnonymousName() + "] " + variable.Name.ToHash());
        }

        private static Name ToAnonymousName(Name name)
        {
            return Name.Get(name.Identifier.ToHash());
        }

        private static ITypeName ToAnonymousName(ITypeName type)
        {
            return ToAnonymousName<TypeName>(type, ToAnonymousName) ??
                   ToAnonymousName<TypeParameterName>(type, ToAnonymousName) ??
                   Asserts.Fail<ITypeName>("unknown type implementation");
        }
        private static TypeName ToAnonymousName(TypeName type)
        {
            var identifier = new StringBuilder();

            if (type.IsDeclaredInEnclosingProject())
            {
                identifier.Append(type.RawFullName.ToHash());
            }
            else
            {
                identifier.Append(type.RawFullName);
            }
            if (type.HasTypeParameters)
            {
                identifier.Append("[[");
                identifier.Append(string.Join("],[", type.TypeParameters.Select(typeParameter => typeParameter.ToAnonymousName())));
                identifier.Append("]]");
            }
            identifier.Append(", ");
            identifier.Append(type.Assembly.ToAnonymousName());
            return (TypeName)TypeName.Get(identifier.ToString());
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

        private static bool IsDeclaredInEnclosingProject(this ITypeName type)
        {
            return type.Assembly.IsEnclosingProject();
        }

        private static bool IsEnclosingProject(this IAssemblyName assembly)
        {
            return assembly.AssemblyVersion == null;
        }

        private static AliasName ToAnonymousName(AliasName alias)
        {
            return AliasName.Get(alias.Identifier.ToHash());
        }

        [ContractAnnotation("notnull => notnull")]
        public static DocumentName ToAnonymousName([JetBrains.Annotations.CanBeNull] this DocumentName document)
        {
            return document == null
                ? null
                : CreateAnonymizedName(VsComponentNameFactory.GetDocumentName, document.Language, document.FileName);
        }

        [ContractAnnotation("notnull => notnull")]
        public static ProjectItemName ToAnonymousName([JetBrains.Annotations.CanBeNull] this ProjectItemName projectItem)
        {
            return projectItem == null
                ? null
                : CreateAnonymizedName(VsComponentNameFactory.GetProjectItemName, projectItem.Type, projectItem.Name);
        }

        [ContractAnnotation("notnull => notnull")]
        public static ProjectName ToAnonymousName([JetBrains.Annotations.CanBeNull] this ProjectName project)
        {
            return project == null
                ? null
                : CreateAnonymizedName(VsComponentNameFactory.GetProjectName, project.Type, project.Name);
        }

        [ContractAnnotation("notnull => notnull")]
        public static SolutionName ToAnonymousName([JetBrains.Annotations.CanBeNull] this SolutionName solution)
        {
            return solution == null
                ? null
                : VsComponentNameFactory.GetSolutionName(solution.Identifier.ToHash());
        }

        [ContractAnnotation("notnull => notnull")]
        public static WindowName ToAnonymousName([JetBrains.Annotations.CanBeNull] this WindowName window)
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