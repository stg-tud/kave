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
using System.Security.Cryptography;
using System.Text;
using KaVE.Commons.Model.Naming;
using KaVE.Commons.Model.Naming.CodeElements;
using KaVE.Commons.Model.Naming.IDEComponents;
using KaVE.Commons.Model.Naming.Impl.v0.CodeElements;
using KaVE.Commons.Model.Naming.Types;
using KaVE.Commons.Model.Naming.Types.Organization;
using KaVE.Commons.Utils.Assertion;
using KaVE.JetBrains.Annotations;

namespace KaVE.Commons.Utils.Naming
{
    public static class AnonymousNameUtils
    {
        [ContractAnnotation("notnull => notnull")]
        public static string ToHash(this string value)
        {
            if (value == null || "".Equals(value))
            {
                return value;
            }
            var tmpSource = value.AsBytes();
            var hash = new MD5CryptoServiceProvider().ComputeHash(tmpSource);
            return Convert.ToBase64String(hash).Replace('+', '-').Replace('/', '_');
        }

        [ContractAnnotation("notnull => notnull")]
        public static TName ToAnonymousName<TName>(this TName name) where TName : class, IName
        {
            if (name == null || name.IsUnknown)
            {
                return name;
            }
            return ToAnonymousName<IDocumentName, TName>(name, ToAnonymousName) ??
                   ToAnonymousName<IWindowName, TName>(name, ToAnonymousName) ??
                   ToAnonymousName<ISolutionName, TName>(name, ToAnonymousName) ??
                   ToAnonymousName<IProjectName, TName>(name, ToAnonymousName) ??
                   ToAnonymousName<IProjectItemName, TName>(name, ToAnonymousName) ??
                   ToAnonymousName<IAliasName, TName>(name, ToAnonymousName) ??
                   ToAnonymousName<IAssemblyName, TName>(name, ToAnonymousName) ??
                   ToAnonymousName<ITypeName, TName>(name, ToAnonymousName) ??
                   ToAnonymousName<ILocalVariableName, TName>(name, ToAnonymousName) ??
                   ToAnonymousName<IFieldName, TName>(name, ToAnonymousName) ??
                   ToAnonymousName<IPropertyName, TName>(name, ToAnonymousName) ??
                   ToAnonymousName<IEventName, TName>(name, ToAnonymousName) ??
                   ToAnonymousName<IParameterName, TName>(name, ToAnonymousName) ??
                   ToAnonymousName<ILambdaName, TName>(name, ToAnonymousName) ??
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
            return Names.Namespace(@namespace.Identifier.ToHash());
        }

        private static ILambdaName ToAnonymousName(ILambdaName lambda)
        {
            var identifier = new StringBuilder();
            identifier.AppendFormat("[{0}] ", lambda.ReturnType.ToAnonymousName())
                      .AppendParameters(lambda.Parameters, true);
            return Names.Lambda(identifier.ToString());
        }

        private static IMethodName ToAnonymousName(IMethodName method)
        {
            var identifier = new StringBuilder();
            identifier.AppendAnonymousMemberName(method, method.ReturnType);
            if (method.HasTypeParameters)
            {
                identifier.Append('`').Append(method.TypeParameters.Count);
            }
            var anonymize = method.DeclaringType.IsUnknown || method.DeclaringType.Assembly.IsLocalProject;
            identifier.AppendTypeParameters(method, anonymize);
            identifier.AppendParameters(method.Parameters, anonymize);
            return Names.Method(identifier.ToString());
        }

        private static void AppendParameters(this StringBuilder identifier,
            IEnumerable<IParameterName> parameterNames,
            bool anonymizeNames)
        {
            identifier.Append("(");
            identifier.Append(string.Join(", ", parameterNames.Select(p => ToAnonymousName(p, anonymizeNames))));
            identifier.Append(")");
        }

        private static IParameterName ToAnonymousName(IParameterName parameter)
        {
            return ToAnonymousName(parameter, true);
        }

        // TODO NameUpdate: put modifier to central place
        private static IParameterName ToAnonymousName(IParameterName parameter, bool anonymizeName)
        {
            var identifier = new StringBuilder();
            identifier.AppendIf(parameter.IsParameterArray, ParameterName.VarArgsModifier + " ");
            identifier.AppendIf(parameter.IsOutput, ParameterName.OutputModifier + " ");
            identifier.AppendIf(parameter.IsOptional, ParameterName.OptionalModifier + " ");
            identifier.AppendIf(parameter.HasPassByReferenceModifier(), ParameterName.PassByReferenceModifier + " ");
            identifier.AppendAnonymousTypeName(parameter.ValueType, anonymizeName).Append(' ');
            identifier.Append(anonymizeName ? parameter.Name.ToHash() : parameter.Name);
            return Names.Parameter(identifier.ToString());
        }

        private static bool HasPassByReferenceModifier(this IParameterName parameter)
        {
            return parameter.IsPassedByReference && !parameter.ValueType.IsReferenceType;
        }

        private static StringBuilder AppendAnonymousTypeName(this StringBuilder identifier,
            ITypeName type,
            bool parameterNameWasAnonymized = false)
        {
            identifier.Append('[');
            if (parameterNameWasAnonymized && type.IsTypeParameter && !type.AsTypeParameterName.IsBound)
            {
                identifier.Append(type.AsTypeParameterName.TypeParameterShortName.ToHash());
            }
            else
            {
                identifier.Append(type.ToAnonymousName());
            }
            identifier.Append(']');
            return identifier;
        }

        private static IEventName ToAnonymousName(IEventName @event)
        {
            var identifier = new StringBuilder();
            identifier.AppendAnonymousMemberName(@event, @event.HandlerType);
            return Names.Event(identifier.ToString());
        }

        private static IPropertyName ToAnonymousName(IPropertyName property)
        {
            var identifier = new StringBuilder();
            identifier.AppendIf(property.HasSetter, PropertyName.SetterModifier + " ");
            identifier.AppendIf(property.HasGetter, PropertyName.GetterModifier + " ");
            identifier.AppendAnonymousMemberName(property, property.ValueType);
            var anonymize = property.DeclaringType.IsUnknown || property.DeclaringType.Assembly.IsLocalProject;
            identifier.AppendParameters(property.Parameters, anonymize);
            return Names.Property(identifier.ToString());
        }

        private static IFieldName ToAnonymousName(IFieldName field)
        {
            var identifier = new StringBuilder();
            identifier.AppendAnonymousMemberName(field, field.ValueType);
            return Names.Field(identifier.ToString());
        }

        private static void AppendAnonymousMemberName(this StringBuilder identifier,
            IMemberName member,
            ITypeName valueType)
        {
            if (member.IsStatic)
            {
                identifier.Append(MemberName.StaticModifier).Append(" ");
            }
            identifier.AppendAnonymousTypeName(valueType).Append(' ');
            identifier.AppendAnonymousTypeName(member.DeclaringType).Append('.');
            var originatesInAssembly = !member.DeclaringType.IsUnknown && !member.DeclaringType.Assembly.IsLocalProject;
            var isCtor = member.Name.Equals(".ctor");
            var isCctor = member.Name.Equals(".cctor");
            var nameShouldNotBeHashed = originatesInAssembly || isCctor || isCtor;
            identifier.Append(nameShouldNotBeHashed ? member.Name : member.Name.ToHash());
        }

        private static ILocalVariableName ToAnonymousName(ILocalVariableName variable)
        {
            var identifier = new StringBuilder();
            identifier.AppendAnonymousTypeName(variable.ValueType).Append(' ');
            identifier.Append(variable.Name.ToHash());
            return Names.LocalVariable(identifier.ToString());
        }

        private static IName ToAnonymousName(IName name)
        {
            return Names.General(name.Identifier.ToHash());
        }

        private static ITypeName ToAnonymousName(ITypeName type)
        {
            if (type.IsUnknown)
            {
                return type;
            }
            if (type.IsPredefined)
            {
                return type;
            }
            if (type.IsDelegateType)
            {
                return ToAnonymousType_Delegate(type.AsDelegateTypeName);
            }
            if (type.IsArray)
            {
                return ToAnonymousType_Array(type.AsArrayTypeName);
            }
            if (type.IsTypeParameter)
            {
                return ToAnonymousType_TypeParameter(type.AsTypeParameterName);
            }

            if (!"TypeName".Equals(type.GetType().Name))
            {
                Asserts.Fail<ITypeName>("unknown type implementation");
            }

            return ToAnonymousType_Regular(type);
        }

        private static IArrayTypeName ToAnonymousType_Array(IArrayTypeName type)
        {
            var rank = type.Rank;
            var anonymousBaseName = type.ArrayBaseType.ToAnonymousName();
            return Names.ArrayType(rank, anonymousBaseName);
        }

        private static IDelegateTypeName ToAnonymousType_Delegate(IDelegateTypeName type)
        {
            var recursionSafeId = type.Identifier.Substring(2); // strip "d:"
            var mid = Names.Method(recursionSafeId);
            var anonId = mid.ToAnonymousName().Identifier;
            return Names.Type("d:{0}", anonId).AsDelegateTypeName;
        }

        private static ITypeName ToAnonymousType_Regular(ITypeName type)
        {
            var identifier = new StringBuilder();
            identifier.AppendTypeKindPrefix(type);
            var inEnclosingProject = type.Assembly.IsLocalProject;
            identifier.Append(type.AnonymizedFullName(inEnclosingProject));
            identifier.AppendTypeParameters(type, inEnclosingProject).Append(", ");
            identifier.Append(type.Assembly.ToAnonymousName());
            return Names.Type(identifier.ToString());
        }

        private static void AppendTypeKindPrefix(this StringBuilder identifier, ITypeName type)
        {
            var prefix = type.Identifier.Substring(0, 2);
            if (prefix.EndsWith(":"))
            {
                identifier.Append(prefix);
            }
        }

        private static string AnonymizedFullName(this ITypeName type, bool anonymize)
        {
            var sb = new StringBuilder();
            if (type.IsNestedType)
            {
                Asserts.NotNull(type.DeclaringType);
                var dt = anonymize ? type.DeclaringType.AnonymizedFullName(true) : type.DeclaringType.Identifier;
                sb.Append(dt).Append('+');
            }
            else
            {
                var ns = type.Namespace;
                if (!ns.IsGlobalNamespace)
                {
                    sb.Append(anonymize ? ns.ToAnonymousName().Identifier : ns.Identifier).Append(".");
                }
            }

            sb.Append(anonymize ? type.Name.ToHash() : type.Name);
            if (type.HasTypeParameters)
            {
                sb.Append("`{0}".FormatEx(type.TypeParameters.Count));
            }

            return sb.ToString();
        }

        private static StringBuilder AppendTypeParameters(this StringBuilder identifier,
            IGenericName type,
            bool anonymizeShortNames)
        {
            if (type.HasTypeParameters)
            {
                bool isFirst = true;
                identifier.Append("[");
                foreach (var tp in type.TypeParameters)
                {
                    if (!isFirst)
                    {
                        identifier.Append(',');
                    }
                    isFirst = false;
                    identifier.Append('[');
                    identifier.Append(ToAnonymousTypeParameter(tp, anonymizeShortNames));
                    identifier.Append("]");
                }
                identifier.Append("]");
            }
            return identifier;
        }

        private static ITypeParameterName ToAnonymousTypeParameter(ITypeParameterName typeParameter,
            bool anonymizeShortNames)
        {
            var l = anonymizeShortNames
                ? typeParameter.TypeParameterShortName.ToHash()
                : typeParameter.TypeParameterShortName;

            if (!typeParameter.IsBound)
            {
                return Names.TypeParameter(l);
            }

            string r;
            if (typeParameter.TypeParameterType.IsTypeParameter &&
                !typeParameter.TypeParameterType.AsTypeParameterName.IsBound)
            {
                r = typeParameter.TypeParameterType.AsTypeParameterName.TypeParameterShortName.ToHash();
            }
            else
            {
                r = typeParameter.TypeParameterType.ToAnonymousName().Identifier;
            }

            return Names.TypeParameter(l, r);
        }

        private static ITypeParameterName ToAnonymousType_TypeParameter(ITypeParameterName typeParameter)
        {
            return ToAnonymousTypeParameter(typeParameter, false);
        }

        private static IAssemblyName ToAnonymousName(IAssemblyName assembly)
        {
            return assembly.IsLocalProject ? Names.Assembly(assembly.Identifier.ToHash()) : assembly;
        }

        private static IAliasName ToAnonymousName(IAliasName alias)
        {
            return Names.Alias(alias.Identifier.ToHash());
        }

        [ContractAnnotation("notnull => notnull")]
        public static IDocumentName ToAnonymousName([CanBeNull] this IDocumentName document)
        {
            return document == null
                ? null
                : Names.Document("{0} {1}", document.Language, document.FileName.HashIfFile());
        }

        [ContractAnnotation("notnull => notnull")]
        public static IProjectItemName ToAnonymousName([CanBeNull] this IProjectItemName projectItem)
        {
            return projectItem == null
                ? null
                : Names.ProjectItem("{0} {1}", projectItem.Type, projectItem.Name.HashIfFile());
        }

        [ContractAnnotation("notnull => notnull")]
        public static IProjectName ToAnonymousName([CanBeNull] this IProjectName project)
        {
            return project == null
                ? null
                : Names.Project("{0} {1}", project.Type, project.Name.HashIfFile());
        }

        [ContractAnnotation("notnull => notnull")]
        public static ISolutionName ToAnonymousName([CanBeNull] this ISolutionName solution)
        {
            return solution == null
                ? null
                : Names.Solution(solution.Identifier.ToHash());
        }

        [ContractAnnotation("notnull => notnull")]
        public static IWindowName ToAnonymousName([CanBeNull] this IWindowName window)
        {
            return window == null
                ? null
                : Names.Window("{0} {1}", window.Type, window.Caption.HashIfFile());
        }

        private static string HashIfFile(this string name)
        {
            var isFileName = name.Contains("\\") || name.Contains(".");
            if (isFileName)
            {
                name = name.ToHash();
            }
            return name;
        }
    }
}