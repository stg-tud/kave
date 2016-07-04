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

using System.Linq;
using KaVE.JetBrains.Annotations;

namespace KaVE.Commons.Model.Names.CSharp
{
    public class StructTypeName : TypeName
    {
        private const string NamePrefix = "s:";

        internal static bool IsStructTypeIdentifier(string identifier)
        {
            return identifier.StartsWith(NamePrefix) ||
                   IsSimpleTypeIdentifier(identifier) ||
                   IsNullableTypeIdentifier(identifier) ||
                   IsVoidTypeIdentifier(identifier);
        }

        [UsedImplicitly]
        internal new static ITypeName Get(string identifier)
        {
            return TypeName.Get(identifier);
        }

        internal StructTypeName(string identifier) : base(identifier) {}

        public override string FullName
        {
            get
            {
                var fullName = base.FullName;
                if (fullName.StartsWith(NamePrefix))
                {
                    fullName = fullName.Substring(NamePrefix.Length);
                }
                return fullName;
            }
        }

        public override bool IsUnknownType
        {
            get { return false; }
        }

        public override bool IsVoidType
        {
            get { return IsVoidTypeIdentifier(Identifier); }
        }

        private static bool IsVoidTypeIdentifier(string identifier)
        {
            return identifier.StartsWith("System.Void,");
        }

        public override bool IsStructType
        {
            get { return true; }
        }

        public override bool IsEnumType
        {
            get { return false; }
        }

        public override bool IsSimpleType
        {
            get { return IsSimpleTypeIdentifier(Identifier); }
        }

        private static bool IsSimpleTypeIdentifier(string identifier)
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

        public override bool IsNullableType
        {
            get { return IsNullableTypeIdentifier(Identifier); }
        }

        private static bool IsNullableTypeIdentifier(string identifier)
        {
            return identifier.StartsWith("System.Nullable`1[[");
        }

        public override bool IsArrayType
        {
            get { return false; }
        }
    }
}