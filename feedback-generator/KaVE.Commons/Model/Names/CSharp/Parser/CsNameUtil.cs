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
using KaVE.Commons.Utils.Collections;

namespace KaVE.Commons.Model.Names.CSharp.Parser
{
    public static class CsNameUtil
    {
        enum Types
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

        private static Dictionary<string, Types> IdToType = new Dictionary<string, Types>()
        {
            {"CSharp.AliasName", Types.AliasName},
            {"CSharp.AssemblyName", Types.AssemblyName},
            {"CSharp.EventName", Types.EventName},
            {"CSharp.FieldName", Types.FieldName},
            {"CSharp.LambdaName", Types.LambdaName},
            {"CSharp.LocalVariableName", Types.LocalVariableName},
            {"CSharp.MethodName", Types.MethodName},
            {"CSharp.Name", Types.Name},
            {"CSharp.NamespaceName", Types.NamespaceName},
            {"CSharp.ParameterName", Types.ParameterName},
            {"CSharp.PropertyName", Types.PropertyName},
            {"CSharp.TypeName", Types.TypeName}
        };

        private static Dictionary<Type, string> TypeToJson = new Dictionary<Type, string>()
        {
            {typeof(AliasName), ""},
            {typeof(AssemblyName), ""},
            {typeof(EventName), ""},
            {typeof(FieldName), ""},
            {typeof(LambdaName), ""},
            {typeof(LocalVariableName), ""},
            {typeof(MethodName), ""},
            {typeof(Name), ""},
            {typeof(NamespaceName), ""},
            {typeof(ParameterName), ""},
            {typeof(PropertyName), ""},
            {typeof(TypeName), ""}
        };

        public static KaVEList<string> Names = new KaVEList<string>();

        public static void AddName(string json)
        {
            Names.Add(json);
        }

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
                    return AliasName.Get(identifier);
                case Types.AssemblyName:
                    return AssemblyName.Get(identifier);
                case Types.EventName:
                    return EventName.Get(identifier);
                case Types.FieldName:
                    return FieldName.Get(identifier);
                case Types.LambdaName:
                    return LambdaName.Get(identifier);
                case Types.LocalVariableName:
                    return LocalVariableName.Get(identifier);
                case Types.MethodName:
                    return ParseMethodName(identifier);
                case Types.Name:
                    return Name.Get(identifier);
                case Types.NamespaceName:
                    return NamespaceName.Get(identifier);
                case Types.ParameterName:
                    return ParameterName.Get(identifier);
                case Types.PropertyName:
                    return PropertyName.Get(identifier);
                case Types.TypeName:
                    return ParseTypeName(identifier);
                default:
                    break;
            };
            return null;
        }

        public static string toJson(IName type)
        {

            return TypeToJson[type.GetType()] + type.Identifier;
        }

        public static ITypeName ParseTypeName(string input)
        {
            try
            {
                var ctx = TypeNameParseUtil.ValidateTypeName(input);
                return new CsTypeName(ctx);
            }
            catch (Exception e)
            {
                try
                {
                    var ctx = TypeNameParseUtil.ValidateTypeName(HandleOldTypeNames(input));
                    return new CsTypeName(ctx);
                }
                catch (Exception e2)
                {
                    return new CsTypeName(TypeNameParseUtil.ValidateTypeName("?"));
                }
            }
        }

        public static IMethodName ParseMethodName(string input)
        {
            try
            {
                var ctx = TypeNameParseUtil.ValidateMethodName(input);
                return new CsMethodName(ctx);
            }
            catch (Exception e)
            {
                try
                {
                    var ctx = TypeNameParseUtil.ValidateMethodName(HandleOldMethodNames(input));
                    return new CsMethodName(ctx);
                }
                catch (Exception e2)
                {
                    return new CsMethodName(TypeNameParseUtil.ValidateMethodName("?"));
                }
            }
        }

        public static string HandleOldMethodNames(string input)
        {
            IMethodName m = MethodName.Get(input);
            var sig = m.Signature;
            var decl = m.DeclaringType.Identifier;
            var ret = m.ReturnType.Identifier;
            if (m.IsConstructor)
            {
                ret = "?";
            }
            else
            {
                ret = HandleOldTypeNames(ret);
            }
            decl = HandleOldTypeNames(decl);
            if (m.HasParameters)
            {
                var parameters = m.Parameters;
                sig = m.Name + "(";
                for (var i = 0; i < parameters.Count; i++)
                {
                    var keyword = GetKeyWord(parameters[i]);
                    sig += keyword + "[" + HandleOldTypeNames(parameters[i].ValueType.Identifier) + "] " + parameters[i].Name + (i < parameters.Count - 1 ? ", " : "");
                }
                sig += ")";
            }
            return "[" + ret + "] [" + decl + "]." + sig;
        }

        private static object GetKeyWord(IParameterName parameterName)
        {
            return parameterName.IsOptional
                ? "opt "
                : parameterName.IsOutput
                    ? "out "
                    : parameterName.IsParameterArray ? "params " : parameterName.IsPassedByReference ? "ref " : "";
        }

        public static string HandleNestedTypeNames(string input)
        {
            string result = "";
            int nestedCount = input.Split('+').Length - 1;
            for (int i = 0; i < nestedCount; i++)
            {
                result += "n:";
            }
            result += input;
            return result;
        }

        public static string HandleTypeIdentifier(string input)
        {
            var type = TypeName.Get(input);
            var identifier = type.IsEnumType ? "e:" : type.IsStructType ? "s:" : type.IsInterfaceType ? "i:" : "?";
            return type.Namespace + "." + identifier + type.FullName.Substring(type.FullName.LastIndexOf(".") + 1, type.FullName.Length - (type.FullName.LastIndexOf(".") + 1)) + ", " + type.Assembly;
        }

        public static string HandleOldTypeNames(string input)
        {
            ITypeName typeName = TypeName.Get(input);
            string result = input;
            if (typeName.IsDelegateType)
            {
                return "d:" + HandleOldMethodNames(input.Substring(2, input.Length - 2));
            }
            else if (typeName.IsGenericEntity)
            {
                var paras = typeName.TypeParameters;
                var n = typeName.Namespace.Identifier;
                var a = typeName.Assembly.Identifier;
                if (!n.Equals(""))
                {
                    n += ".";
                }
                if (!a.Contains("+"))
                {
                    a = ", " + a;
                }
                result = n + GetTypeNameIdentifier(typeName) + typeName.Name + "'" + paras.Count + "[";
                for (var i = 0; i < paras.Count; i++)
                {
                    result += "[" + paras[i].TypeParameterShortName + " -> " + HandleOldTypeNames(paras[i].TypeParameterType.Identifier) + "]" + (i < paras.Count - 1 ? "," : "")
                    ;
                }
                result += "]" + a;
            }
            if ((typeName.IsEnumType || (typeName.IsStructType && !typeName.IsSimpleType && !typeName.IsVoidType) || typeName.IsInterfaceType) && !typeName.Namespace.Identifier.Equals(""))
            {
                result = HandleTypeIdentifier(result);
            }
            if (typeName.IsNestedType || typeName.Identifier.Contains("+"))
            {
                result = HandleNestedTypeNames(result);
            }
            return result;
        }

        private static string GetTypeNameIdentifier(ITypeName typeName)
        {
            return typeName.IsInterfaceType ? "i:" : typeName.IsStructType ? "s:" : typeName.IsEnumType ? "e:" : "";
        }
    }
}