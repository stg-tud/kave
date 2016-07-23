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
using System.Text;
using KaVE.Commons.Model.Naming.CodeElements;
using KaVE.Commons.Model.Naming.Impl.v0;
using KaVE.Commons.Model.Naming.Impl.v0.CodeElements;
using KaVE.Commons.Model.Naming.Impl.v1;
using KaVE.Commons.Model.Naming.Types;
using KaVE.Commons.Model.Naming.Types.Organization;

namespace KaVE.Commons.Model.Naming
{
    public class NameSerialization
    {
        private enum Types
        {
            TypeName,
            MethodName,
            AliasName,
            AssemblyName,
            EventName,
            FieldName,
            LambdaName,
            LocalVariableName,
            Name,
            NamespaceName,
            ParameterName,
            PropertyName
        }

        private static readonly Dictionary<string, Types> IdToType = new Dictionary<string, Types>
        {
            {"CSharp.AliasName", Types.AliasName},
            {"1a", Types.AliasName},
            {"CSharp.AssemblyName", Types.AssemblyName},
            {"1b", Types.AssemblyName},
            {"CSharp.EventName", Types.EventName},
            {"1c", Types.EventName},
            {"CSharp.FieldName", Types.FieldName},
            {"1d", Types.FieldName},
            {"CSharp.LambdaName", Types.LambdaName},
            {"1e", Types.LambdaName},
            {"CSharp.LocalVariableName", Types.LocalVariableName},
            {"1f", Types.LocalVariableName},
            {"CSharp.MethodName", Types.MethodName},
            {"1g", Types.MethodName},
            {"CSharp.Name", Types.Name},
            {"1h", Types.Name},
            {"CSharp.NamespaceName", Types.NamespaceName},
            {"1i", Types.NamespaceName},
            {"CSharp.ParameterName", Types.ParameterName},
            {"1j", Types.ParameterName},
            {"CSharp.PropertyName", Types.PropertyName},
            {"1k", Types.PropertyName},
            {"CSharp.TypeName", Types.TypeName},
            {"1l", Types.TypeName}
        };

        private static readonly Dictionary<Type, string> TypeToJson = new Dictionary<Type, string>
        {
            {typeof(AliasName), "1a"},
            {typeof(IAssemblyName), "1b"},
            {typeof(IEventName), "1c"},
            {typeof(IFieldName), "1d"},
            {typeof(ILambdaName), "1e"},
            {typeof(LocalVariableName), "1f"},
            {typeof(IMethodName), "1g"},
            {typeof(Impl.v1.MethodName), "1g"},
            {typeof(GeneralName), "1h"},
            {typeof(INamespaceName), "1i"},
            {typeof(IParameterName), "1j"},
            {typeof(IPropertyName), "1k"},
            {typeof(Impl.v0.Types.TypeName), "1l"},
            {typeof(TypeName), "1l"},
            {typeof(ITypeName), "1l"},
            {typeof(AssemblyName), "1b"}
        };

        public static IName ParseJson(string input)
        {
            var splitPos = input.IndexOf(":", StringComparison.Ordinal);
            var key = input.Substring(0, splitPos);
            var identifier = input.Substring(splitPos + 1, (input.Length - splitPos) - 1);
            if (!IdToType.ContainsKey(key))
                return null;
            var type = IdToType[key];
            switch (type)
            {
                case Types.AliasName:
                    return Names.Alias(identifier);
                case Types.AssemblyName:
                    return Names.Assembly(identifier);
                case Types.EventName:
                    return Names.Event(identifier);
                case Types.FieldName:
                    return Names.Field(identifier);
                case Types.LambdaName:
                    return Names.Lambda(identifier);
                case Types.LocalVariableName:
                    return Names.LocalVariable(identifier);
                case Types.MethodName:
                    return Names.Method(identifier);
                case Types.Name:
                    return Names.General(identifier);
                case Types.NamespaceName:
                    return Names.Namespace(identifier);
                case Types.ParameterName:
                    return Names.Parameter(identifier);
                case Types.PropertyName:
                    return Names.Property(identifier);
                case Types.TypeName:
                    return Names.Type(identifier);
            }
            return null;
        }

        public static string ToJson(IName type)
        {
            var s = new StringBuilder();
            var name = type as UnknownName;
            if (name != null)
            {
                s.Append(TypeToJson[name.GetUnknownType()]);
            }
            else
            {
                s.Append(TypeToJson[type.GetType()]);
            }
            s.Append(":");
            s.Append(type.Identifier);
            return s.ToString();
        }
    }
}