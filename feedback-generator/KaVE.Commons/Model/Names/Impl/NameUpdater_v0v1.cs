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

using System.Linq;
using System.Runtime.Serialization.Configuration;
using KaVE.Commons.Model.Names.CSharp;

namespace KaVE.Commons.Model.Names
{
    public class CsNameFixer
    {
        public static string HandleOldMethodNames(string input)
        {
            IMethodName m = MethodName.Get(input);
            var name = m.Name;
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
            if (name == "")
            {
                name = m.DeclaringType.Name;
                decl = HandleOldTypeNames(m.DeclaringType.IsNestedType ? m.DeclaringType.DeclaringType.Identifier : "?, " + m.DeclaringType.Assembly);
                sig = name + "()";
            }
            else
            {
                decl = HandleOldTypeNames(decl);
            }
            if (m.HasParameters)
            {
                var parameters = m.Parameters;
                sig = name + "(";
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
            else if (typeName.IsArrayType)
            {
                ArrayTypeName name = (ArrayTypeName) typeName;
                return "arr(" + name.Rank + "):" + HandleOldTypeNames(name.ArrayBaseType.Identifier);
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
                    result += "[" + ((TypeParameterName)paras[i]).TypeParameterShortName + " -> " + HandleOldTypeNames(((TypeParameterName)paras[i]).TypeParameterType.Identifier) + "]" + (i < paras.Count - 1 ? "," : "")
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