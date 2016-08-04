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
using System.Text.RegularExpressions;
using KaVE.Commons.Model.Naming.CodeElements;
using KaVE.Commons.Model.Naming.Impl.v0.CodeElements;
using KaVE.Commons.Model.Naming.Impl.v0.Types;
using KaVE.Commons.Model.Naming.Types;
using KaVE.Commons.Utils;
using KaVE.Commons.Utils.Assertion;
using KaVE.Commons.Utils.Collections;
using KaVE.JetBrains.Annotations;

namespace KaVE.Commons.Model.Naming.Impl.v0
{
    public static class NameUtils
    {
        public static string FixLegacyFormats([NotNull] this string id)
        {
            return
                id.FixPredefinedTypes().FixLegacyNullable()
                  .FixLegacyTypeParameterLists()
                  .FixLegacyDelegateNames()
                  .FixMissingGenericTicks()
                  .FixJaggedArrays();
        }

        private static readonly Regex LegacyNullableMatcher = new Regex("(^|[^:])System.Nullable`1\\[");

        [NotNull]
        private static string FixLegacyNullable([NotNull] this string id)
        {
            if (!LegacyNullableMatcher.IsMatch(id))
            {
                return id;
            }
            return id.Replace("System.Nullable`1", "s:System.Nullable`1");
        }

        private static readonly IDictionary<string, string> OldToNew = new Dictionary<string, string>
        {
            {"Boolean", "p:bool"},
            {"Byte", "p:byte"},
            {"Char", "p:char"},
            {"Decimal", "p:decimal"},
            {"Double", "p:double"},
            {"Int16", "p:short"},
            {"Int32", "p:int"},
            {"Int64", "p:long"},
            {"Object", "p:object"},
            {"SByte", "p:sbyte"},
            {"Single", "p:float"},
            {"String", "p:string"},
            {"UInt16", "p:ushort"},
            {"UInt32", "p:uint"},
            {"UInt64", "p:ulong"},
            {"Void", "p:void"}
        };

        private static readonly Regex PredefTypesMatcher =
            new Regex(
                "(System.(Boolean|Byte|Char|Decimal|Double|Int16|Int32|Int64|Object|SByte|Single|String|UInt16|UInt32|UInt64|Void)((\\[,*\\])?),\\smscorlib,\\s\\d\\.\\d\\.\\d\\.\\d)");

        //new Regex("(System.(Boolean|Byte)((\\[,*\\])?),\\smscorlib,\\s\\d\\.\\d\\.\\d\\.\\d)");

        // Initially, we captured predefined types as regular types (e.g., System.Int32, mscorlib, 4.0.0.0)
        // Please note the missing "s:", even though most of them are struct types
        [NotNull]
        private static string FixPredefinedTypes([NotNull] this string id)
        {
            var match = PredefTypesMatcher.Match(id);
            for (; match.Success; match = match.NextMatch())
            {
                var newId = OldToNew[match.Groups[2].ToString()];
                var arrPart = match.Groups[3].ToString();

                var needle = match.Groups[0].ToString();
                var repl = newId + arrPart;

                id = id.Replace(needle, repl);
            }

            return id;
        }

        private static readonly Regex JaggedArrays = new Regex("(\\[,*\\](\\[,*\\])+)");

        // initially, we captured jagged arrays, but simplified all of them to [,,] later
        [NotNull]
        private static string FixJaggedArrays([NotNull] this string id)
        {
            var matches = JaggedArrays.Match(id);
            for (; matches.Success; matches = matches.NextMatch())
            {
                var arr = matches.Groups[1].ToString();
                var rank = arr.Count(c => c.Equals('[') || c.Equals(','));
                var newArr = '[' + new string(',', rank - 1) + ']';
                id = id.Replace(arr, newArr);
            }

            return id;
        }

        private static readonly Dictionary<string, string> ManualTypeParameterFixes = new Dictionary<string, string>
        {
            {
                "s:System.Data.Entity.Core.Metadata.Edm.ReadOnlyMetadataCollection`1+Enumerator, EntityFramework, 6.0.0.0",
                "s:System.Data.Entity.Core.Metadata.Edm.ReadOnlyMetadataCollection`1[[T]]+Enumerator, EntityFramework, 6.0.0.0"
            },
            {
                "System.Collections.Generic.Dictionary`2+KeyCollection, mscorlib, 4.0.0.0",
                "System.Collections.Generic.Dictionary`2[[TKey],[TValue]]+KeyCollection, mscorlib, 4.0.0.0"
            },
            {
                "System.Collections.Generic.Dictionary`2+ValueCollection, mscorlib, 4.0.0.0",
                "System.Collections.Generic.Dictionary`2[[TKey],[TValue]]+ValueCollection, mscorlib, 4.0.0.0"
            },
            {
                "System.Collections.ObjectModel.ReadOnlyDictionary`2+KeyCollection, mscorlib, 4.0.0.0",
                "System.Collections.ObjectModel.ReadOnlyDictionary`2[[TKey],[TValue]]+KeyCollection, mscorlib, 4.0.0.0"
            },
            {
                "System.Collections.ObjectModel.ReadOnlyDictionary`2+ValueCollection, mscorlib, 4.0.0.0",
                "System.Collections.ObjectModel.ReadOnlyDictionary`2[[TKey],[TValue]]+ValueCollection, mscorlib, 4.0.0.0"
            },
            {
                "s:System.Collections.Generic.Dictionary`2+Enumerator, mscorlib, 4.0.0.0",
                "s:System.Collections.Generic.Dictionary`2[[TKey],[TValue]]+Enumerator, mscorlib, 4.0.0.0"
            },
            {
                "s:System.Collections.Immutable.ImmutableArray`1+Enumerator, System.Collections.Immutable, 1.1.37.0",
                "s:System.Collections.Immutable.ImmutableArray`1[[T]]+Enumerator, System.Collections.Immutable, 1.1.37.0"
            }
        };

        private static readonly Regex IsLegacyTypeParameterList = new Regex("([^+.]+`([0-9]+))[^\\[]");
        private static readonly Regex AllLegacyTypeParameterLists = new Regex("([^+.]+`([0-9]+))");

        // initially, we used markers on the types (e.g., T`1) and only had a single typeParameterList at the end
        [NotNull]
        private static string FixLegacyTypeParameterLists([NotNull] this string id)
        {
            if (id.StartsWith("vsWindowTypeDocument ") || id.StartsWith("CSharp ") || id.EndsWith(".cs") ||
                !IsLegacyTypeParameterList.IsMatch(id))
            {
                return id;
            }

            var matches = AllLegacyTypeParameterLists.Match(id);

            var endParams = id.LastIndexOf(']');
            if (endParams == -1)
            {
                if (!ManualTypeParameterFixes.Keys.Contains(id))
                {
                    Console.WriteLine("has tick, but no type parameters: '{0}'", id);
                }
                foreach (var invalidId in ManualTypeParameterFixes.Keys)
                {
                    id = id.Replace(invalidId, ManualTypeParameterFixes[invalidId]);
                }
                return id;
            }
            var startParams = id.FindCorrespondingOpenBracket(endParams);
            if (startParams == -1)
            {
                return id;
            }
            var parameters = id.ParseParams(startParams, endParams);
            var before = id.Substring(0, startParams);
            var after = id.Substring(endParams + 1, id.Length - endParams - 1);

            var alreadyTaken = 0;
            for (; matches.Success; matches = matches.NextMatch())
            {
                var hit = matches.Groups[1].ToString();
                var tick = matches.Groups[2].ToString();
                var tickNum = int.Parse(tick);

                var sb = new StringBuilder().Append('[');
                var takeUntil = alreadyTaken + tickNum;
                var shouldAddComma = false;
                for (var i = alreadyTaken; i < takeUntil && i < parameters.Count; i++)
                {
                    if (shouldAddComma)
                    {
                        sb.Append(',');
                    }
                    shouldAddComma = true;
                    sb.Append('[').Append(parameters[i]).Append(']');
                    alreadyTaken++;
                }
                var paramList = sb.Append(']').ToString();

                var replacement = "{0}{1}".FormatEx(hit, paramList);
                before = before.Replace(hit, replacement);
            }

            return before + after;
        }

        private static IList<string> ParseParams(this string id, int open, int close)
        {
            var parameters = Lists.NewList<string>();
            var cur = open + 1; // skip outer bracket
            while (cur != -1 && cur < close)
            {
                var openParam = id.FindNext(cur, '[', ']');
                if (id[openParam] == ']')
                {
                    cur = close;
                }
                else
                {
                    var closeParam = id.FindCorrespondingCloseBracket(openParam);
                    openParam++; // skip bracket
                    var paramId = id.Substring(openParam, closeParam - openParam);
                    parameters.Add(paramId);
                    cur = closeParam + 1;
                }
            }
            return parameters;
        }

        private static readonly Regex LegacyDelegateMatcher = new Regex("(d:[^\\[])");

        // initially, we did not store delegates as method signatures, but only the "delegate type"
        [NotNull]
        private static string FixLegacyDelegateNames([NotNull] this string id)
        {
            var delegateInBrackets = LegacyDelegateMatcher.Match(id);

            if (!delegateInBrackets.Success)
            {
                return id;
            }
            var matchingDelegate = delegateInBrackets.Groups[1].ToString();
            var startDelegate = id.IndexOf(matchingDelegate, StringComparison.Ordinal);
            var closeBracket = id.FindNext(startDelegate, ']');
            var startType = startDelegate + BaseTypeName.PrefixDelegate.Length;
            var endType = closeBracket != -1 ? closeBracket : id.Length;
            var oldDelegateId = id.Substring(startType, endType - startType);
            var newDelegateId = ToFixedDelegate(oldDelegateId);
            var newId = id.Replace(BaseTypeName.PrefixDelegate + oldDelegateId, newDelegateId);
            return newId.FixLegacyDelegateNames();
        }

        private static string ToFixedDelegate(string substring)
        {
            return "{0}[{1}] [{2}].()".FormatEx(BaseTypeName.PrefixDelegate, new TypeName().Identifier, substring);
        }

        private static readonly Regex MissingTicks = new Regex("(?:\\+|^|\\.)([a-zA-Z0-9_]+)(\\[,*\\])?(\\[\\[.*)");

        [NotNull]
        private static string FixMissingGenericTicks([NotNull] this string id)
        {
            var match = MissingTicks.Match(id);
            if (!match.Success)
            {
                return id;
            }

            var type = string.Format("{0}{1}[[", match.Groups[1], match.Groups[2]);
            var numTicks = FindNumTicksInRest(match.Groups[3].ToString());
            var newType = string.Format("{0}`{1}{2}[[", match.Groups[1], numTicks, match.Groups[2]);
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