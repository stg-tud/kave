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
using System.Text.RegularExpressions;
using KaVE.Commons.Model.Naming.Types;
using KaVE.Commons.Utils;

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
            if (TypeParameterName.IsTypeParameterIdentifier(identifier))
            {
                return new TypeParameterName(identifier);
            }
            // checked third, since the array's value type can have any kind of type
            if (ArrayTypeName.IsArrayTypeNameIdentifier(identifier))
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


        internal static bool IsDelegateTypeIdentifier(string identifier)
        {
            return identifier.StartsWith(BaseTypeName.PrefixDelegate) &&
                   !ArrayTypeName.IsArrayTypeNameIdentifier(identifier);
        }

        internal static bool IsStructTypeIdentifier(string identifier)
        {
            if (ArrayTypeName.IsArrayTypeNameIdentifier(identifier))
            {
                return false;
            }
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