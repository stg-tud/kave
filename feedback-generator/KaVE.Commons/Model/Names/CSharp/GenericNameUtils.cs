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

namespace KaVE.Model.Names.CSharp
{
    internal static class GenericNameUtils
    {
        /// <summary>
        ///     Parses the type parameter list from a type's full name or a method's signature.
        /// </summary>
        /// <param name="fullNameOrSignature">Expected to contain a type-parameter list.</param>
        public static List<ITypeName> ParseTypeParameters(this string fullNameOrSignature)
        {
            var parameters = new List<ITypeName>();
            var openBraces = 0;
            var startIndex = fullNameOrSignature.IndexOf('[') + 1;
            for (var currentIndex = startIndex; currentIndex < fullNameOrSignature.Length; currentIndex++)
            {
                var c = fullNameOrSignature[currentIndex];

                if (c == '[')
                {
                    openBraces++;
                }
                else if (c == ']')
                {
                    openBraces--;

                    if (openBraces == 0)
                    {
                        var indexAfterOpeningBrace = startIndex + 1;
                        var lengthToBeforeClosingBrace = currentIndex - startIndex - 1;
                        var descriptor = fullNameOrSignature.Substring(
                            indexAfterOpeningBrace,
                            lengthToBeforeClosingBrace);
                        var parameterTypeName = TypeName.Get(descriptor);
                        parameters.Add(parameterTypeName);
                        startIndex = fullNameOrSignature.IndexOf('[', currentIndex);
                    }
                }
            }
            return parameters;
        }
    }
}