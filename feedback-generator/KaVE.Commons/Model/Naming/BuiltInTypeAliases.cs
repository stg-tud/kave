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

namespace KaVE.Commons.Model.Naming
{
    public static class BuiltInTypeAliases
    {
        private static readonly IDictionary<string, string> SimpleTypeToFullNameMap = new Dictionary<string, string>
        {
            {"sbyte", "System.SByte"},
            {"byte", "System.Byte"},
            {"short", "System.Int16"},
            {"ushort", "System.UInt16"},
            {"int", "System.Int32"},
            {"uint", "System.UInt32"},
            {"long", "System.Int64"},
            {"ulong", "System.UInt64"},
            {"char", "System.Char"},
            {"float", "System.Single"},
            {"double", "System.Double"},
            {"bool", "System.Boolean"},
            {"decimal", "System.Decimal"},
            {"void", "System.Void"},
            {"object", "System.Object"},
            {"string", "System.String"}
        };

        private static readonly IDictionary<string, string> FullNameToSimpleTypeMap = new Dictionary<string, string>();

        static BuiltInTypeAliases()
        {
            foreach (var entry in SimpleTypeToFullNameMap)
            {
                FullNameToSimpleTypeMap.Add(entry.Value, entry.Key);
            }
        }

        /// <summary>
        ///     Translates type aliases, like <code>int</code>, <code>object</code>, or <code>short?</code> to the
        ///     respective, fully-qualified type names. All type aliases refer to types from the mscore assembly.
        /// </summary>
        /// <param name="alias">the alias to get the type name from</param>
        /// <returns>the aliased type name</returns>
        public static string GetFullTypeNameFromTypeAlias(string alias)
        {
            var indexOfBraces = alias.IndexOf('[');
            if (indexOfBraces > -1)
            {
                var rawAlias = alias.Substring(0, indexOfBraces);
                var suffix = alias.Substring(indexOfBraces);
                return GetFullTypeNameFromTypeAlias(rawAlias) + suffix;
            }

            if (alias.EndsWith("?"))
            {
                var underlyingType = GetFullTypeNameFromTypeAlias(alias.TrimEnd('?'));
                return "System.Nullable`1[[T -> " + underlyingType + "]]";
            }

            if (SimpleTypeToFullNameMap.ContainsKey(alias))
            {
                return SimpleTypeToFullNameMap[alias];
            }

            return alias;
        }

        /// <summary>
        ///     Translates fully qualified type names of simple types (e.g. "System.Int32") back to their alias.
        /// </summary>
        /// <param name="typeName">Fully qualified type name.</param>
        /// <returns>The alias, if it exists.</returns>
        public static string GetTypeAliasFromFullTypeName(string typeName)
        {
            if (FullNameToSimpleTypeMap.ContainsKey(typeName))
            {
                return FullNameToSimpleTypeMap[typeName];
            }

            return typeName;
        }
    }
}