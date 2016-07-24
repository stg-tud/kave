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

using System.Collections.Generic;
using System.Text.RegularExpressions;
using KaVE.Commons.Model.Naming.CodeElements;
using KaVE.Commons.Model.Naming.Impl.v0.CodeElements;
using KaVE.Commons.Model.Naming.Impl.v0.Types;
using KaVE.Commons.Model.Naming.Types;
using KaVE.Commons.Utils;
using KaVE.Commons.Utils.Assertion;
using KaVE.Commons.Utils.Collections;

namespace KaVE.Commons.Model.Naming.Impl.v0
{
    public static class NameUtils
    {
        public static string FixLegacyFormats(this string id)
        {
            return id.FixLegacyDelegateNames().FixMissingGenericTicks();
        }

        private static string FixLegacyDelegateNames(this string id)
        {
            if (!TypeUtils.IsDelegateTypeIdentifier(id))
            {
                return id;
            }

            // originally, we did not store delegates as method signatures, but only the "delegate type"
            if (!id.Contains("("))
            {
                id = string.Format(
                    "{0}[{1}] [{2}].()",
                    BaseTypeName.PrefixDelegate,
                    new TypeName().Identifier,
                    id.Substring(BaseTypeName.PrefixDelegate.Length));
            }
            return id;
        }

        private static readonly Regex MissingTicks = new Regex("\\+([a-zA-Z0-9]+)(\\[\\[.*)");

        private static string FixMissingGenericTicks(this string id)
        {
            var match = MissingTicks.Match(id);
            if (!match.Success)
            {
                return id;
            }

            var type = string.Format("+{0}[[", match.Groups[1]);
            var numTicks = FindNumTicksInRest(match.Groups[2].ToString());
            var newType = string.Format("+{0}`{1}[[", match.Groups[1], numTicks);
            var newId = id.Replace(type, newType);
            return newId.FixMissingGenericTicks();
        }

        private static int FindNumTicksInRest(string rest)
        {
            Asserts.Not(string.IsNullOrEmpty(rest));
            Asserts.That(rest[0] == '[');

            var endGenerics = rest.FindCorrespondingCloseBracket(0);

            var numTicks = 0;
            var current = 1; // skip opening bracket
            while (current != -1 && current < endGenerics)
            {
                numTicks++;
                var tpOpen = rest.FindNext(current, '[');
                var tpClose = rest.FindCorrespondingCloseBracket(tpOpen);
                tpClose++; // skip closing bracket
                current = rest.FindNext(tpClose, ',', ']');
            }

            return numTicks;
        }

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

        /// <summary>
        ///     Parses contents of a "ParameterListHolder"... just pass the complete identifier and the indices of the brackets
        /// </summary>
        public static IKaVEList<IParameterName> GetParameterNamesFromSignature(this string identifierWithParameters,
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

        public static IMethodName RemoveGenerics(this IMethodName name)
        {
            return new MethodName(RemoveGenerics(name.Identifier));
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
                var numStr = id.Substring(tick + 1, length).Trim();
                var numGenerics = int.Parse(numStr);

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
    }
}