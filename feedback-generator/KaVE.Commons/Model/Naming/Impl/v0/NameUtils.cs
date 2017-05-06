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
        public static IKaVEList<ITypeParameterName> ParseTypeParameterList(this string id, int open, int close)
        {
            if (string.IsNullOrEmpty(id) || open < 0 || close >= id.Length || close < open || id[open] != '[' ||
                id[close] != ']')
            {
                Asserts.Fail("error parsing parameters from '{0}' ({1}, {2})", id, open, close);
            }

            var parameters = Lists.NewList<ITypeParameterName>();
            for (var cur = open; cur < close;)
            {
                cur++; // skip open bracket or comma

                cur = id.FindNext(cur, '[');
                var closeParam = id.FindCorrespondingCloseBracket(cur);

                cur++; // skip bracket

                var tpId = id.Substring(cur, closeParam - cur);
                parameters.Add(new TypeParameterName(tpId));

                closeParam++; // skip bracket

                cur = id.FindNext(closeParam, ',', ']');
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

        public static ITypeName RemoveGenerics(this ITypeName name)
        {
            return new TypeName(RemoveGenerics(name.Identifier));
        }

        public static IMethodName RemoveGenerics(this IMethodName name)
        {
            return new MethodName(RemoveGenerics(name.Identifier));
        }

        public static IFieldName RemoveGenerics(this IFieldName name)
        {
            return new FieldName(RemoveGenerics(name.Identifier));
        }

        public static IPropertyName RemoveGenerics(this IPropertyName name)
        {
            return new PropertyName(RemoveGenerics(name.Identifier));
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

                while (IsArray(id, open))
                {
                    open = id.FindNext(open + 1, '[');
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

                    open = close + 1; // skip everything in between
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

        private static bool IsArray(string id, int open)
        {
            Asserts.That(id[open] == '[', "not pointed to opening brace");
            var is1DArr = id[open + 1] == ']';
            var isNdArr = id[open + 1] == ',';
            return is1DArr || isNdArr;
        }
    }
}