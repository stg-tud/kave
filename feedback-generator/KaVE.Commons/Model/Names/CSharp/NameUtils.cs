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

namespace KaVE.Commons.Model.Names.CSharp
{
    public static class NameUtils
    {
        public static IMethodName RemoveGenerics(this IMethodName name)
        {
            return MethodName.Get(RemoveGenerics(name.Identifier));
        }

        private static string RemoveGenerics(string id)
        {
            var startIdx = id.IndexOf('`');
            if (startIdx == -1)
            {
                return id;
            }

            var replacements = new Dictionary<string, string>();
            var tick = id.FindNext(0, '`');

            while (tick != -1)
            {
                var open = id.FindNext(tick, '[');
                var length = open - tick - 1;
                var numStr = length > 0
                    ? id.Substring(tick + 1, length).Trim()
                    : "0";

                if (length < 1)
                {
                    // TODO fix name creation, this should not happen!
                    Console.WriteLine("\nEE: cannot remove generic (no tick number): {0}", id);
                }

                int numGenerics = 0;
                try
                {
                    numGenerics = int.Parse(numStr);
                }
                catch (FormatException)
                {
                    // TODO fix name creation, this should not happen!
                    Console.WriteLine(
                        "\nEE: cannot remove generic (invalid tick number between {1} and {2}): {0}",
                        id,
                        tick,
                        open);
                }

                for (var i = 0; i < numGenerics; i++)
                {
                    open = id.FindNext(open + 1, '[');
                    var close = id.FindCorrespondingCloseBracket(open);

                    var arrowStart = id.FindNext(open, '-');
                    if (arrowStart != -1 && arrowStart < close)
                    {
                        var param = id.Substring(open, arrowStart - open).Trim();
                        var complete = id.Substring(open, close - open);
                        replacements[complete] = param;
                    }
                }
                tick = id.FindNext(tick + 1, '`');
            }
            var res = id;
            foreach (var k in replacements.Keys)
            {
                var with = replacements[k];
                res = res.Replace(k, with);
            }
            return res;
        }

        public static bool HasParameters(this string identifierWithparameters)
        {
            var startOfParameters = identifierWithparameters.IndexOf('(');
            return (startOfParameters > 0 && identifierWithparameters[startOfParameters + 1] != ')');
        }

        public static IList<IParameterName> GetParameterNames(this string identifierWithParameters)
        {
            var parameters = new List<IParameterName>();
            var endOfParameters = identifierWithParameters.LastIndexOf(')');
            var hasNoBrackets = endOfParameters == -1;
            if (hasNoBrackets)
            {
                return parameters;
            }

            var startOfParameters = identifierWithParameters.FindCorrespondingOpenBracket(endOfParameters);
            var current = startOfParameters;

            var hasNoParams = startOfParameters == (endOfParameters - 1);
            if (hasNoParams)
            {
                return parameters;
            }

            while (current != endOfParameters)
            {
                var startOfParam = ++current;

                if (identifierWithParameters[current] != '[')
                {
                    current = identifierWithParameters.FindNext(current, '[');
                }
                current = identifierWithParameters.FindCorrespondingCloseBracket(current);
                current = identifierWithParameters.FindNext(current, ',', ')');
                var endOfParam = current;

                var lengthOfSubstring = endOfParam - startOfParam;
                var paramSubstring = identifierWithParameters.Substring(startOfParam, lengthOfSubstring);
                parameters.Add(ParameterName.Get(paramSubstring.Trim()));
            }

            return parameters;
        }


        public static int FindNext(this string str, int currentIndex, params char[] characters)
        {
            for (var i = currentIndex; i < str.Length; i++)
            {
                var c = str[i];
                if (characters.Contains(c))
                {
                    return i;
                }
            }
            return -1;
        }

        public static int FindPrevious(this string str, int currentIndex, char character)
        {
            for (var i = currentIndex; i >= 0; i--)
            {
                if (str[i] == character)
                {
                    return i;
                }
            }
            return -1;
        }

        public static int FindCorrespondingOpenBracket(this string str, int currentIndex)
        {
            var open = str[currentIndex];
            var close = open.GetCorresponding();

            var depth = 0;
            for (var i = currentIndex; i > 0; i--)
            {
                depth = UpdateDepth(depth, open, close, str[i]);
                if (depth == 0)
                {
                    return i;
                }
            }
            return -1;
        }

        private static int UpdateDepth(int depth, char open, char close, char current)
        {
            if (current == open)
            {
                return depth + 1;
            }
            if (current == close)
            {
                return depth - 1;
            }
            return depth;
        }

        public static int FindCorrespondingCloseBracket(this string str, int currentIndex)
        {
            var open = str[currentIndex];
            var close = open.GetCorresponding();

            var depth = 0;
            for (var i = currentIndex; i < str.Length; i++)
            {
                depth = UpdateDepth(depth, open, close, str[i]);
                if (depth == 0)
                {
                    return i;
                }
            }
            return -1;
        }

        public static char GetCorresponding(this char c)
        {
            switch (c)
            {
                case '(':
                    return ')';
                case ')':
                    return '(';
                case '{':
                    return '}';
                case '}':
                    return '{';
                case '[':
                    return ']';
                case ']':
                    return '[';
                case '<':
                    return '>';
                case '>':
                    return '<';
                default:
                    throw new ArgumentException(string.Format("no supported bracket type: {0}", c));
            }
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
        ///     Returns the index of the next '[' in the identifier, starting at the given offset.
        /// </summary>
        public static int StartOfNextTypeIdentifier(this string identifier, int offset)
        {
            return identifier.IndexOf('[', offset);
        }
    }
}