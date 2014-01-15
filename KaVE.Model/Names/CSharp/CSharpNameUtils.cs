namespace KaVE.Model.Names.CSharp
{
    public static class CSharpNameUtils
    {
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

            switch (alias)
            {
                case "sbyte":
                    return "System.SByte";
                case "byte":
                    return "System.Byte";
                case "short":
                    return "System.Int16";
                case "ushort":
                    return "System.UInt16";
                case "int":
                    return "System.Int32";
                case "uint":
                    return "System.UInt32";
                case "long":
                    return "System.Int64";
                case "ulong":
                    return "System.UInt64";
                case "char":
                    return "System.Char";
                case "float":
                    return "System.Single";
                case "double":
                    return "System.Double";
                case "bool":
                    return "System.Boolean";
                case "decimal":
                    return "System.Decimal";
                case "void":
                    return "System.Void";
                case "object":
                    return "System.Object";
                case "string":
                    return "System.String";
                default:
                    return alias;
            }
        }
    }
}