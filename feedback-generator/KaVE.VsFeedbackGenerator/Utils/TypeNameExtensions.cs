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
 *    - Dennis Albrecht
 */

using System;
using KaVE.JetBrains.Annotations;
using KaVE.Model.Names;
using KaVE.Model.Names.CSharp.TypeNames;

namespace KaVE.VsFeedbackGenerator.Utils
{
    internal static class TypeNameExtensions
    {
        // TODO @Dennis: testen oder raus
        public static string ToTypeCategory([NotNull] this ITypeName elem)
        {
            if (elem.IsReferenceType)
            {
                if (elem.IsClassType)
                {
                    return "class";
                }
                if (elem.IsInterfaceType)
                {
                    return "interface";
                }
                if (elem.IsArrayType)
                {
                    return "array";
                }
                if (elem.IsDelegateType)
                {
                    return "delegate";
                }
                throw new ArgumentException(
                    @"Given ITypeName claims to be a ReferenceType but does not match any subtype",
                    "elem");
            }
            if (elem.IsValueType)
            {
                if (elem.IsVoidType)
                {
                    return "void";
                }
                if (elem.IsEnumType)
                {
                    return "enum";
                }
                if (elem.IsStructType)
                {
                    return elem.IsNullableType ? "nullable" : (elem.IsSimpleType ? "simple" : "struct");
                }
                throw new ArgumentException(
                    @"Given ITypeName claims to be a ValueType but does not match any subtype",
                    "elem");
            }
            if (elem.IsUnknownType)
            {
                // TODO @Dennis: this is not a type category, but a type identifier
                return UnknownTypeName.Identifier;
            }
            throw new ArgumentException(@"Given ITypeName does not match any type", "elem");
        }
    }
}