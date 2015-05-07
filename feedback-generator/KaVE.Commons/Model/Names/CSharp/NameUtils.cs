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
 *    - Sven Amann
 */

using System.Collections.Generic;

namespace KaVE.Commons.Model.Names.CSharp
{
    internal static class NameUtils
    {
        public static bool HasParameters(this string identifierWithparameters)
        {
            var startOfParameters = identifierWithparameters.IndexOf('(');
            return (startOfParameters > 0 && identifierWithparameters[startOfParameters + 1] != ')');
        }

        public static IList<IParameterName> GetParameterNames(this string identifierWithParameters)
        {
            var parameters = new List<IParameterName>();
            if (identifierWithParameters.HasParameters())
            {
                var endOfParameters = identifierWithParameters.LastIndexOf(')');
                var startOfParameter = identifierWithParameters.IndexOf('(') + 1;
                int endOfParameter;
                for (; startOfParameter < endOfParameters; startOfParameter = endOfParameter + 1)
                {
                    endOfParameter = EndOfParameter(identifierWithParameters, startOfParameter, endOfParameters);
                    var lengthOfParameter = endOfParameter - startOfParameter;
                    var identifier = identifierWithParameters.Substring(startOfParameter, lengthOfParameter).Trim();
                    parameters.Add(ParameterName.Get(identifier));
                }
            }
            return parameters;
        }

        private static int EndOfParameter(string identifierWithParameters, int startOfParameter, int endOfParameters)
        {
            var endOfParameterType = EndOfNextTypeIdentifier(identifierWithParameters, startOfParameter);
            var endOfParameter = identifierWithParameters.IndexOf(',', endOfParameterType);
            if (0 <= endOfParameter && endOfParameter <= endOfParameters)
            {
                return endOfParameter;
            }
            return endOfParameters;
        }

        /// <summary>
        ///     Returns the index after the ']' that corresponds to the first '[' in the identifier, starting at the given offset.
        /// </summary>
        public static int EndOfNextTypeIdentifier(this string identifier, int offset)
        {
            var index = StartOfNextTypeIdentifier(identifier, offset);
            var brackets = 0;
            do
            {
                if (identifier[index] == '[')
                {
                    brackets++;
                }
                else if (identifier[index] == ']')
                {
                    brackets--;
                }
                index++;
            } while (index < identifier.Length && brackets > 0);
            return index;
        }

        /// <summary>
        /// Returns the index of the next '[' in the identifier, starting at the given offset.
        /// </summary>
        public static int StartOfNextTypeIdentifier(this string identifier, int offset)
        {
            return identifier.IndexOf('[', offset);
        }
    }
}