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
using System.Linq;
using KaVE.Commons.Model.Naming.Types;

namespace KaVE.Commons.Model.Naming.Impl.v0.Types
{
    public class TypeUtils
    {
        private static readonly ITypeName UnknownTypeInstance = new TypeName();

        public static ITypeName CreateTypeName(string identifier)
        {
            // checked first, because it's a special case
            if (identifier == string.Empty || IsUnknownTypeIdentifier(identifier))
            {
                return UnknownTypeInstance;
            }
            // checked second, since type parameters can have any kind of type
            if (IsTypeParameterIdentifier(identifier))
            {
                return new TypeParameterName(identifier);
            }
            // checked third, since the array's value type can have any kind of type
            if (IsArrayTypeIdentifier(identifier))
            {
                return new ArrayTypeName(identifier);
            }
            if (IsDelegateTypeIdentifier(identifier))
            {
                return new DelegateTypeName(identifier);
            }
            return new TypeName(identifier);
        }

        public static bool IsUnknownTypeIdentifier(string identifier)
        {
            return BaseTypeName.UnknownTypeIdentifier.Equals(identifier);
        }

        internal static bool IsTypeParameterIdentifier(string identifier)
        {
            if (IsUnknownTypeIdentifier(identifier))
            {
                return false;
            }
            return IsFreeTypeParameterIdentifier(identifier) || IsBoundTypeParameterIdentifier(identifier);
        }

        private static bool IsFreeTypeParameterIdentifier(string identifier)
        {
            return !identifier.Contains(",") && !identifier.Contains("[");
        }

        private static bool IsBoundTypeParameterIdentifier(string identifier)
        {
            // "T -> System.Void, mscorlib, ..." is a type parameter, because it contains the separator.
            // "System.Nullable`1[[T -> System.Int32, mscorlib, 4.0.0.0]], ..." is not, because
            // the separator is only in the type's parameter-type list, i.e., after the '`'.
            var indexOfMapping = identifier.IndexOf(
                TypeParameterName.ParameterNameTypeSeparater,
                StringComparison.Ordinal);
            var endOfTypeName = identifier.IndexOf('`');
            return indexOfMapping >= 0 && (endOfTypeName == -1 || endOfTypeName > indexOfMapping);
        }

        internal static bool IsDelegateTypeIdentifier(string identifier)
        {
            return identifier.StartsWith(BaseTypeName.PrefixDelegate);
        }


        internal static bool IsArrayTypeIdentifier(string id)
        {
            if (id.StartsWith("d:"))
            {
                var idx = id.LastIndexOf(')');
                if (id.Length > (idx + 1) && id[idx + 1] == '[')
                {
                    return true;
                }
            }
            else
            {
                var idx = id.IndexOf('[');
                if (idx == -1)
                {
                    return false;
                }
                while ((idx + 1) < id.Length && id[++idx] == ',') {}
                if (id[idx] == ']')
                {
                    return true;
                }
            }

            return false;
        }

        internal static bool IsStructTypeIdentifier(string identifier)
        {
            return identifier.StartsWith(BaseTypeName.PrefixStruct) ||
                   IsSimpleTypeIdentifier(identifier) ||
                   IsNullableTypeIdentifier(identifier) ||
                   IsVoidTypeIdentifier(identifier);
        }


        internal static bool IsVoidTypeIdentifier(string identifier)
        {
            return identifier.StartsWith("System.Void,");
        }

        internal static bool IsSimpleTypeIdentifier(string identifier)
        {
            return IsNumericTypeName(identifier) || identifier.StartsWith("System.Boolean,");
        }

        private static bool IsNumericTypeName(string identifier)
        {
            return IsIntegralTypeName(identifier) ||
                   IsFloatingPointTypeName(identifier) ||
                   identifier.StartsWith("System.Decimal,");
        }

        private static readonly string[] IntegralTypeNames =
        {
            "System.SByte,",
            "System.Byte,",
            "System.Int16,",
            "System.UInt16,",
            "System.Int32,",
            "System.UInt32,",
            "System.Int64,",
            "System.UInt64,",
            "System.Char,"
        };

        private static bool IsIntegralTypeName(string identifier)
        {
            return IntegralTypeNames.Any(identifier.StartsWith);
        }

        private static readonly string[] FloatingPointTypeNames =
        {
            "System.Single,",
            "System.Double,"
        };

        private static bool IsFloatingPointTypeName(string identifier)
        {
            return FloatingPointTypeNames.Any(identifier.StartsWith);
        }

        internal static bool IsNullableTypeIdentifier(string identifier)
        {
            return identifier.StartsWith("System.Nullable`1[[");
        }
    }
}