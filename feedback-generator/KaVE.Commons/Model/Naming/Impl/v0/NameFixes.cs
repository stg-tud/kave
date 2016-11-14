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
using KaVE.Commons.Model.Naming.Impl.v0.Types;
using KaVE.Commons.Utils;
using KaVE.Commons.Utils.Assertion;
using KaVE.Commons.Utils.Collections;
using KaVE.JetBrains.Annotations;

namespace KaVE.Commons.Model.Naming.Impl.v0
{
    public static class NameFixes
    {
        /// <summary>
        ///     repairs legacy formats in the serialized form (i.e., incl. the prefix)
        /// </summary>
        public static string FixIdentifiers([NotNull] this string id, [CanBeNull] string prefix = null)
        {
            if (!string.IsNullOrEmpty(prefix))
            {
                id =
                    id.FixSerializedNames_BrokenFixes(prefix)
                      .FixSerializedNames_PropertiesWithoutSetterAndGetter(prefix);
            }
            return id.FixGeneralIdentifiers();
        }


        // unfortunately, name fixing was broken at some point... this is fixed here for one common case
        [NotNull]
        private static string FixSerializedNames_BrokenFixes([NotNull] this string id,
            [NotNull] string prefix)
        {
            if (!"0M".Equals(prefix))
            {
                return id;
            }

            const string fail =
                "[d:[TResult] [System.Func`10[[T9]][[TResult],[System.Func`10[[T9]][[T1],[T2],[T3],[T4],[T5],[T6],[T7],[T8],[T9],[TResult]], System.Core, 4.0.0.0],[T1],[T2],[T3],[T4],[T5],[T6],[T7],[T8]][[T1],[T2],[T3],[T4],[T5],[T6],[T7],[T8],[T9],[TResult]], System.Core, 4.0.0.0].([T1] arg1, [T2] arg2, [T3] arg3, [T4] arg4, [T5] arg5, [T6] arg6, [T7] arg7, [T8] arg8, [T9] arg9)] ..ctor()";

            if (fail.Equals(id))
            {
                const string delTypeId =
                    "d:[TResult] [System.Func`10[[T1],[T2],[T3],[T4],[T5],[T6],[T7],[T8],[T9],[TResult]], System.Core, 4.0.0.0].([T1] arg1, [T2] arg2, [T3] arg3, [T4] arg4, [T5] arg5, [T6] arg6, [T7] arg7, [T8] arg8, [T9] arg9))";
                return "[{0}] [{0}]..ctor()".FormatEx(delTypeId);
            }

            const string fail2 =
                "[s:System.Collections.Generic.List`1[][[[T -> T]]]+Enumerator, mscorlib, 4.0.0.0] .GetEnumerator()";
            if (fail2.Equals(id))
            {
                return
                    "[s:System.Collections.Generic.List`1[[T -> T]]+Enumerator, mscorlib, 4.0.0.0] [System.Collections.Generic.List`1[[T -> T]], mscorlib, 4.0.0.0].GetEnumerator()";
            }

            const string fail3 = "[p:void] ..ctor()";
            if (fail3.Equals(id))
            {
                return "[?] [?].???()";
            }
            return id;
        }


        private static readonly Regex NoSetterAndGetterMatcher = new Regex("^\\s*(static)?\\s*\\[");

        [NotNull]
        private static string FixSerializedNames_PropertiesWithoutSetterAndGetter([NotNull] this string id,
            [NotNull] string prefix)
        {
            if (!("0P".Equals(prefix) || "CSharp.PropertyName".Equals(prefix)))
            {
                return id;
            }

            if ("[?] [?].???".Equals(id))
            {
                return id;
            }

            var match = NoSetterAndGetterMatcher.Match(id);
            if (!match.Success)
            {
                return id;
            }
            var staticModifier = match.Groups[1].Success ? "static " : "";
            var open = id.IndexOf('[');
            var rest = id.Substring(open);
            return "set get {0}{1}".FormatEx(staticModifier, rest);
        }


        /// <summary>
        ///     repairs legacy formats in arbitrary strings
        /// </summary>
        public static string FixGeneralIdentifiers([NotNull] this string id)
        {
            return
                id.FixPredefinedTypes().FixLegacyNullable()
                  .FixLegacyTypeParameterLists()
                  .FixLegacyDelegateNames()
                  .FixMissingGenericTicks()
                  .FixJaggedArrays().FixMissingParenthesisForProperties();
        }

        private static readonly Regex MissingParenthesisMatcher = new Regex("^(get|set) .*[^)]$");

        [NotNull]
        private static string FixMissingParenthesisForProperties([NotNull] this string id)
        {
            var match = MissingParenthesisMatcher.Match(id);
            if (!match.Success)
            {
                return id;
            }
            return id + "()";
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
            },
            {
                "d:[TValue] [System.Runtime.CompilerServices.ConditionalWeakTable`2+CreateValueCallback, mscorlib, 4.0.0.0].([TKey] key)",
                "d:[TValue] [System.Runtime.CompilerServices.ConditionalWeakTable`2[[TKey],[TValue]]+CreateValueCallback, mscorlib, 4.0.0.0].([TKey] key)"
            },
            {
                "s:System.Collections.Generic.List`1+Enumerator, mscorlib, 4.0.0.0",
                "s:System.Collections.Generic.List`1[[T]]+Enumerator, mscorlib, 4.0.0.0"
            },
            {
                "System.Collections.Generic.List`1+SynchronizedList, mscorlib, 4.0.0.0",
                "System.Collections.Generic.List`1[[T]]+SynchronizedList, mscorlib, 4.0.0.0"
            },
            {
                "s:System.Collections.Generic.LinkedList`1+Enumerator, System, 4.0.0.0",
                "s:System.Collections.Generic.LinkedList`1[[T]]+Enumerator, System, 4.0.0.0"
            },
            {
                "System.Collections.Generic.SortedDictionary`2+KeyCollection, System, 4.0.0.0",
                "System.Collections.Generic.SortedDictionary`2[[TKey],[TValue]]+KeyCollection, System, 4.0.0.0"
            }
        };

        // second (non) numbers at the end are req, because -for some reason- regex ist not greedy
        private static readonly Regex IsLegacyTypeParameterListMatcher = new Regex("([^+.]+`([0-9]+))[^0-9\\[]");
        private static readonly Regex AllLegacyTypeParameterLists = new Regex("([^+.]+`([0-9]+))[^0-9]");

        // initially, we used markers on the types (e.g., T`1) and only had a single typeParameterList at the end
        [NotNull]
        private static string FixLegacyTypeParameterLists([NotNull] this string id)
        {
            if (IsNoLegacyTypeParameterList(id))
            {
                return id;
            }

            foreach (var invalidId in ManualTypeParameterFixes.Keys)
            {
                var validId = ManualTypeParameterFixes[invalidId];
                id = id.Replace(invalidId, validId);
            }

            if (IsNoLegacyTypeParameterList(id))
            {
                return id;
            }

            var matches = AllLegacyTypeParameterLists.Match(id);

            var endParams = id.LastIndexOf("]]", StringComparison.Ordinal);
            if (endParams == -1)
            {
                if (!ManualTypeParameterFixes.Keys.Contains(id))
                {
                    Console.WriteLine("has tick, but no type parameters: '{0}'", id);
                    return id;
                }
            }
            endParams++; // outer bracket
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

        private static bool IsNoLegacyTypeParameterList(string id)
        {
            return id.StartsWith("vsWindowTypeDocument ") || id.StartsWith("CSharp ") || id.EndsWith(".cs") ||
                   !IsLegacyTypeParameterListMatcher.IsMatch(id);
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
    }
}