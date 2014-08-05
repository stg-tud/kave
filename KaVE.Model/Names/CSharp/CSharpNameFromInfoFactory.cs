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
using System.Reflection;
using System.Text;
using KaVE.JetBrains.Annotations;
using KaVE.Utils;
using KaVE.Utils.Assertion;

namespace KaVE.Model.Names.CSharp
{
    /// <summary>
    /// This implementation lacks support for the following:
    /// <list type="bullet">
    ///     <item><description>proper handling of type parameters</description></item>
    ///     <item><description>generation of constructor names</description></item>
    ///     <item><description>generation of array-type names</description></item>
    ///     <item><description>stripping of additional assembly information, i.e., Culture and PublicKeyToken, from assembly names</description></item>
    /// </list>
    /// </summary>
    [Obsolete("This implementation is outdated and should not be used.")]
    public static class CSharpNameFromInfoFactory
    {
        [NotNull]
        public static ITypeName GetName([NotNull] this Type type)
        {
            Asserts.Not(type.IsGenericParameter, "cannot create name for generic parameter");
            // ReSharper disable once PossibleNullReferenceException
            return TypeName.Get(type.AssemblyQualifiedName.Replace("][", ","));
        }

        [NotNull]
        public static IMethodName GetName([NotNull] this MethodInfo method)
        {
            var identifier = new StringBuilder();
            identifier.AppendIf(method.IsStatic, MemberName.StaticModifier);
            identifier.Append(" ");
            identifier.Append(method, method.ReturnType);
            identifier.Append("(").Append(String.Join(", ", method.GetParameters().GetNames())).Append(")");
            return MethodName.Get(identifier.ToString());
        }

        private static IEnumerable<IParameterName> GetNames(this IEnumerable<ParameterInfo> parameters)
        {
            return parameters.Select(GetName);
        }

        [NotNull]
        public static IParameterName GetName([NotNull] this ParameterInfo parameter)
        {
            var identifier = new StringBuilder();
            identifier.AppendIf(parameter.IsParameterArray(), ParameterName.VarArgsModifier + " ");
            identifier.AppendIf(parameter.IsOut, ParameterName.OutputModifier + " ");
            identifier.AppendIf(parameter.IsOptional, ParameterName.OptionalModifier + " ");
            identifier.AppendIf(parameter.ParameterType.IsByRef, ParameterName.PassByReferenceModifier + " ");
            identifier.AppendType(parameter.ParameterType).Append(" ").Append(parameter.Name);
            return ParameterName.Get(identifier.ToString());
        }

        private static bool IsParameterArray(this ParameterInfo parameter)
        {
            return Attribute.IsDefined(parameter, typeof (ParamArrayAttribute));
        }

        [NotNull]
        public static IFieldName GetName([NotNull] this FieldInfo field)
        {
            var identifier = new StringBuilder();
            identifier.AppendIf(field.IsStatic, MemberName.StaticModifier + " ");
            identifier.Append(field, field.FieldType);
            return FieldName.Get(identifier.ToString());
        }

        [NotNull]
        public static IPropertyName GetName([NotNull] this PropertyInfo property)
        {
            var identifier = new StringBuilder();
            var getterInfo = property.GetGetMethod();
            var setterInfo = property.GetSetMethod();
            identifier.AppendIf(
                getterInfo != null && getterInfo.IsStatic || setterInfo != null && setterInfo.IsStatic,
                MemberName.StaticModifier + " ");
            identifier.AppendIf(getterInfo != null, PropertyName.GetterModifier + " ");
            identifier.AppendIf(setterInfo != null, PropertyName.SetterModifier + " ");
            identifier.Append(property, property.PropertyType);
            return PropertyName.Get(identifier.ToString());
        }

        private static void Append(this StringBuilder identifier, MemberInfo property, Type valueType)
        {
            identifier.AppendType(valueType)
                .Append(" ")
                .AppendType(property.DeclaringType)
                .Append(".")
                .Append(property.Name);
        }

        private static StringBuilder AppendType(this StringBuilder identifier, Type type)
        {
            return identifier.Append("[").Append(type.AssemblyQualifiedName).Append("]");
        }

        [NotNull]
        public static IEventName GetName([NotNull] this EventInfo evt)
        {
            var identifier = new StringBuilder();
            identifier.AppendIf(evt.GetAddMethod(true).IsStatic, MemberName.StaticModifier + " ");
            identifier.Append(evt, evt.EventHandlerType);
            return EventName.Get(identifier.ToString());
        }
    }
}