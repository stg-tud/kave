﻿/*
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
using System.Diagnostics.CodeAnalysis;
using KaVE.Commons.Model.Naming.CodeElements;
using KaVE.Commons.Model.Naming.Impl.v0.CodeElements;
using KaVE.Commons.Model.Naming.Impl.v0.Types;
using KaVE.Commons.Model.Naming.Types;
using KaVE.Commons.Utils;
using KaVE.Commons.Utils.Assertion;
using KaVE.Commons.Utils.Collections;

namespace KaVE.Commons.Model.Naming
{
    public static class NameUtils
    {
        /// <summary>
        ///     Parses the type parameter list from a type's full name or a method's signature.
        /// </summary>
        /// <param name="fullNameOrSignature">Expected to contain a type-parameter list.</param>
        public static IKaVEList<ITypeParameterName> ParseTypeParameters(this string fullNameOrSignature)
        {
            var parameters = Lists.NewList<ITypeParameterName>();
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
                        var parameterTypeName = new TypeParameterName(descriptor);
                        parameters.Add(parameterTypeName);
                        startIndex = fullNameOrSignature.IndexOf('[', currentIndex);
                    }
                }
            }
            return parameters;
        }

        public static IMethodName RemoveGenerics(this IMethodName name)
        {
            return Names.Method(RemoveGenerics(name.Identifier));
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

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public static IKaVEList<IParameterName> GetParameterNamesFromMethod(this IMethodName name)
        {
            var id = name.Identifier;
            var openRT = id.IndexOf("[", StringComparison.Ordinal);
            var closeRT = id.FindCorrespondingCloseBracket(openRT);

            var openDT = id.FindNext(closeRT, '[');
            var closeDT = id.FindCorrespondingCloseBracket(openDT);


            var nextGeneric = id.FindNext(closeDT, '`');
            if (nextGeneric == -1)
            {
                nextGeneric = id.Length;
            }
            var nextParam = id.FindNext(closeDT, '(');
            var isGeneric = nextGeneric < nextParam;

            int openParams;
            if (isGeneric)
            {
                var openGeneric = id.FindNext(closeDT, '[');
                var closeGeneric = id.FindCorrespondingCloseBracket(openGeneric);

                openParams = id.FindNext(closeGeneric, '(');
            }
            else
            {
                openParams = id.FindNext(closeDT, '(');
            }
            var closeParams = id.FindCorrespondingCloseBracket(openParams);

            return id.GetParameterNamesFromSignature(openParams, closeParams);
        }

        private static IKaVEList<IParameterName> GetParameterNamesFromSignature(this string identifierWithParameters,
            int idxOpeningBrace,
            int idxClosingBrace)
        {
            // remove opening bracket
            idxOpeningBrace++;

            // strip leading whitespace
            while (identifierWithParameters[idxOpeningBrace] == ' ')
            {
                idxOpeningBrace++;
            }

            var parameters = Lists.NewList<IParameterName>();
            var hasNoParams = idxOpeningBrace == idxClosingBrace;
            if (hasNoParams)
            {
                return parameters;
            }

            var current = idxOpeningBrace;
            while (current < idxClosingBrace)
            {
                var startOfParam = current;

                if (identifierWithParameters[current] != '[')
                {
                    current = identifierWithParameters.FindNext(current, '[');
                }
                current = identifierWithParameters.FindCorrespondingCloseBracket(current);
                current = identifierWithParameters.FindNext(current, ',', ')');
                var endOfParam = current;

                var lengthOfSubstring = endOfParam - startOfParam;
                var paramSubstring = identifierWithParameters.Substring(startOfParam, lengthOfSubstring);
                parameters.Add(new ParameterName(paramSubstring.Trim()));

                // ignore comma
                current++;
            }

            return parameters;
        }

        public static IKaVEList<IParameterName> GetParameterNamesFromLambda(this ILambdaName name)
        {
            if (name.IsUnknown)
            {
                return Lists.NewList<IParameterName>();
            }
            var identifierWithParameters = name.Identifier;
            var endOfParameters = identifierWithParameters.LastIndexOf(')');
            Asserts.That(endOfParameters != -1);
            var startOfParameters = identifierWithParameters.FindCorrespondingOpenBracket(endOfParameters);

            return identifierWithParameters.GetParameterNamesFromSignature(startOfParameters, endOfParameters);
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