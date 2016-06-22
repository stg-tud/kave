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
using KaVE.Commons.Model.Names.CSharp;
using KaVE.Commons.Model.Names.CSharp.Parser;
using KaVE.Commons.Utils;
using KaVE.Commons.Utils.Assertion;
using KaVE.Commons.Utils.Collections;

namespace KaVE.Commons.Model.Names
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

        private static Dictionary<Type, string> TypeToJson = new Dictionary<Type, string>()
        {
            {typeof(AliasName), "1a"},
            {typeof(AssemblyName), "1b"},
            {typeof(EventName), "1c"},
            {typeof(FieldName), "1d"},
            {typeof(LambdaName), "1e"},
            {typeof(LocalVariableName), "1f"},
            {typeof(MethodName), "1g"},
            {typeof(CsMethodName), "1g"},
            {typeof(Name), "1h"},
            {typeof(NamespaceName), "1i"},
            {typeof(ParameterName), "1j"},
            {typeof(PropertyName), "1k"},
            {typeof(TypeName), "1l"},
            {typeof(CsTypeName), "1l"}
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
                    return GetMethodName(identifier);
                case Types.Name:
                    return Name.Get(identifier);
                case Types.NamespaceName:
                    return NamespaceName.Get(identifier);
                case Types.ParameterName:
                    return ParameterName.Get(identifier);
                case Types.PropertyName:
                    return GetPropertyName(identifier);
                case Types.TypeName:
                    return GetTypeName(identifier);
                default:
                    break;
            };
            return null;
        }

        public static string ToJson(IName type)
        {

            return TypeToJson[type.GetType()] + ":" + type.Identifier;
        }

        public static ITypeName GetTypeName(string input)
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
                    var ctx = TypeNameParseUtil.ValidateTypeName(CsNameFixer.HandleOldTypeNames(input));
                    return new CsTypeName(ctx);
                }
                catch (Exception e2)
                {
                    return new CsTypeName(TypeNameParseUtil.ValidateTypeName("?"));
                }
            }
        }

        public static IMethodName GetMethodName(string input)
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
                    var ctx = TypeNameParseUtil.ValidateMethodName(CsNameFixer.HandleOldMethodNames(input));
                    return new CsMethodName(ctx);
                }
                catch (Exception e2)
                {
                    return new CsMethodName(TypeNameParseUtil.ValidateMethodName("?"));
                }
            }
        }

        public static INamespaceName GetNamespaceName(string input)
        {
            try
            {
                var ctx = TypeNameParseUtil.ValidateNamespaceName(input);
                return new CsNamespaceName(ctx);
            }
            catch (Exception e)
            {
                return new UnknownName();
            }
        }

        public static IAssemblyName GetAssemblyName(string input)
        {
            try
            {
                var ctx = TypeNameParseUtil.ValidateAssemblyName(input);
                return new CsAssemblyName(ctx);
            }
            catch (Exception e)
            {
                return new UnknownName();
            }
        }

        public static IParameterName GetParameterName(string input)
        {
            try
            {
                var ctx = TypeNameParseUtil.ValidateParameterName(input);
                return new CsParameterName(ctx);
            }
            catch (AssertException e)
            {
                return new UnknownName();
            }
        }

        private static CsMemberName GetMemberName(string input)
        {
            try
            {
                var ctx = TypeNameParseUtil.ValidateMemberName(input);
                return new CsMemberName(ctx);
            }
            catch (AssertException e)
            {
                return null;
            }
        }

        public static IFieldName GetFieldName(string input)
        {
            var name = GetMemberName(input);
            if (name != null)
            {
                return name;
            }
            return new UnknownName();
        }

        public static IEventName GetEventName(string input)
        {
            var name = GetMemberName(input);
            if (name != null)
            {
                return name;
            }
            return new UnknownName();
        }

        public static IPropertyName GetPropertyName(string input)
        {
            var name = GetMemberName(input);
            if (name != null)
            {
                return name;
            }
            return new UnknownName();
        }
    }
}