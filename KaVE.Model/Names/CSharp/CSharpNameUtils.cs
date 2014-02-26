using System.Collections.Generic;

namespace KaVE.Model.Names.CSharp
{
    public static class CSharpNameUtils
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
            {"string","System.String"}
        };

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
                return "System.Nullable`1[[" + underlyingType + "]]";
            }

            if (SimpleTypeToFullNameMap.ContainsKey(alias))
            {
                return SimpleTypeToFullNameMap[alias];
            }

            return alias;
        }
    }
}