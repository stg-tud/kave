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

using System.Collections.Generic;
using System.Linq;
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
            string methodModifier = methodDeclaration.IsStatic ? "static " : "";
            string returnType = methodDeclaration.ReturnType.TypeName.Identifier;
            string declaringType = methodDeclaration.DeclaringType.TypeName.Identifier;
            string methodName = methodDeclaration.MethodName.Name;
            string parameters = GetParameterString(methodDeclaration.Parameters);
            string typeParameters = GetTypeParameterString(methodDeclaration.TypeParameters);

            return MethodName.Get(string.Format("{0}[{1}] [{2}].{3}{4}({5})", methodModifier, returnType, declaringType, methodName, typeParameters, parameters));
        }

        private static string GetTypeParameterString(IEnumerable<ITypeReference> typeParameters)
        {
            var typeParameterList = typeParameters.ToList();
            var numberOfTypeParameters = typeParameterList.Count();
            string result = numberOfTypeParameters != 0 ? string.Format("`{0}[", numberOfTypeParameters) : "";
            
            foreach (var typeParameter in typeParameterList)
            {
                result += string.Format("[{0}]", typeParameter.TypeName.Identifier);
                if (!typeParameter.Equals(typeParameterList.Last()))
                {
                    result += ",";
                }
                else
                {
                    result += "]";
                }
            }
            return result;
        }

        private static string GetParameterString(IEnumerable<IParameterDeclaration> parameterDeclarations)
        {
            string result = "";
            var parameterDeclarationList = parameterDeclarations.ToList();
            foreach (var parameterDeclaration in parameterDeclarationList)
            {
                string parameter = "";

                if (parameterDeclaration.IsOptional)
                {
                    parameter += ParameterName.OptionalModifier + " ";
                }
                if (parameterDeclaration.IsOutput)
                {
                    parameter += ParameterName.OutputModifier + " ";
                }
                if (parameterDeclaration.IsParameterArray)
                {
                    parameter += ParameterName.VarArgsModifier + " ";
                }
                if (parameterDeclaration.IsPassedByReference && parameterDeclaration.Type.TypeName.IsValueType)
                {
                    parameter += ParameterName.PassByReferenceModifier + " ";
                }

                parameter += string.Format("[{0}] ", parameterDeclaration.Type.TypeName.Identifier);
                parameter += parameterDeclaration.Name.Name;
                if (!parameterDeclaration.Equals(parameterDeclarationList.Last()))
                {
                    parameter += ", ";
                }

                result += parameter;
            }
            return result;
        }

        public static IFieldName CreateFieldName(this IFieldDeclaration fieldDeclaration)
        {
            string fieldModifier = fieldDeclaration.IsStatic ? "static " : "";
            string valueType = fieldDeclaration.ValueType.TypeName.Identifier;
            string declaringType = fieldDeclaration.DeclaringType.TypeName.Identifier;
            string fieldName = fieldDeclaration.FieldName.Name;

            return FieldName.Get(string.Format("{0}[{1}] [{2}].{3}", fieldModifier, valueType, declaringType, fieldName));
        }
    }
}