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
using KaVE.Commons.Model.Names;
using KaVE.Commons.Model.Names.CSharp;
using KaVE.Commons.Model.SSTs.Declarations;
using KaVE.Commons.Model.SSTs.References;

namespace KaVE.Commons.Utils
{
    public static class NameCreator
    {
        public static IMethodName CreateMethodName(this IMethodDeclaration methodDeclaration)
        {
            var methodModifier = methodDeclaration.IsStatic ? "static " : "";
            var returnType = methodDeclaration.ReturnType.TypeName.Identifier;
            var declaringType = methodDeclaration.DeclaringType.TypeName.Identifier;
            var methodName = methodDeclaration.MethodName.Name;
            var typeParameters = GetTypeParameterString(methodDeclaration.TypeParameters.ToList());
            var parameters = GetParameterString(methodDeclaration.Parameters);

            return MethodName.Get(
                "{0}[{1}] [{2}].{3}{4}({5})",
                methodModifier,
                returnType,
                declaringType,
                methodName,
                typeParameters,
                parameters);
        }

        private static string GetTypeParameterString(ICollection<ITypeReference> typeParameters)
        {
            var result = "";
            var numberOfTypeParameters = typeParameters.Count;
            if (numberOfTypeParameters > 0)
            {
                result = string.Format(
                    "`{0}[{1}]",
                    numberOfTypeParameters,
                    string.Join(
                        ",",
                        typeParameters.Select(
                            typeParameter => string.Format("[{0}]", typeParameter.TypeName.Identifier))));
            }
            return result;
        }

        private static string GetParameterString(IEnumerable<IParameterDeclaration> parameterDeclarations)
        {
            var sb = new StringBuilder();
            var parameterDeclarationList = parameterDeclarations.ToList();
            foreach (var parameterDeclaration in parameterDeclarationList)
            {
                if (parameterDeclaration.IsOptional)
                {
                    sb.AppendFormat("{0} ", ParameterName.OptionalModifier);
                }
                if (parameterDeclaration.IsOutput)
                {
                    sb.AppendFormat("{0} ", ParameterName.OutputModifier);
                }
                if (parameterDeclaration.IsParameterArray)
                {
                    sb.AppendFormat("{0} ", ParameterName.VarArgsModifier);
                }
                if (parameterDeclaration.IsPassedByReference && parameterDeclaration.Type.TypeName.IsValueType)
                {
                    sb.AppendFormat("{0} ", ParameterName.PassByReferenceModifier);
                }

                sb.AppendFormat(
                    "[{0}] {1}",
                    parameterDeclaration.Type.TypeName.Identifier,
                    parameterDeclaration.Name.Name);

                sb.Append(", ");
            }

            var parameterString = sb.ToString();
            if (parameterDeclarationList.Count != 0)
            {
                parameterString = parameterString.Remove(parameterString.LastIndexOf(", ", StringComparison.Ordinal));
            }
            return parameterString;
        }

        public static IFieldName CreateFieldName(this IFieldDeclaration fieldDeclaration)
        {
            string fieldModifier = fieldDeclaration.IsStatic ? "static " : "";
            string valueType = fieldDeclaration.ValueType.TypeName.Identifier;
            string declaringType = fieldDeclaration.DeclaringType.TypeName.Identifier;
            string fieldName = fieldDeclaration.FieldName.Name;

            return FieldName.Get("{0}[{1}] [{2}].{3}", fieldModifier, valueType, declaringType, fieldName);
        }
    }
}