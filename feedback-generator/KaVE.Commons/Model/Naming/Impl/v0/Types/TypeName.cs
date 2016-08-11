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
using KaVE.Commons.Model.Naming.Impl.v0.Types.Organization;
using KaVE.Commons.Model.Naming.Types;
using KaVE.Commons.Model.Naming.Types.Organization;
using KaVE.Commons.Utils;
using KaVE.Commons.Utils.Exceptions;

namespace KaVE.Commons.Model.Naming.Impl.v0.Types
{
    public class TypeName : BaseTypeName
    {
        private static readonly string[] InvalidIds =
        {
            "System.Boolean, mscorlib,",
            "System.Decimal, mscorlib,",
            "System.SByte, mscorlib,",
            "System.Byte, mscorlib,",
            "System.Int16, mscorlib,",
            "System.UInt16, mscorlib,",
            "System.Int32, mscorlib,",
            "System.UInt32, mscorlib,",
            "System.Int64, mscorlib,",
            "System.UInt64, mscorlib,",
            "System.Char, mscorlib,",
            "System.Single, mscorlib,",
            "System.Double, mscorlib,",
            //"System.String, mscorlib,",
            //"System.Object, mscorlib,",
            "System.Void, mscorlib"
        };

        public TypeName() : this(UnknownTypeIdentifier) {}

        public TypeName(string id) : base(id)
        {
            if (UnknownTypeIdentifier.Equals(id))
            {
                return;
            }

            if (InvalidIds.Any(id.StartsWith))
            {
                throw new ValidationException("rejecting a predefined type: '{0}'".FormatEx(id), null);
            }

            var hasComma = id.Contains(",") && id.LastIndexOf(',') > id.LastIndexOf(']');
            if (!hasComma)
            {
                throw new ValidationException("does not contain a correct assembly name: '{0}'".FormatEx(id), null);
            }
        }

        public override IAssemblyName Assembly
        {
            get
            {
                if (IsUnknown)
                {
                    return new AssemblyName();
                }
                if (IsDelegateType)
                {
                    return DeclaringType.Assembly;
                }
                var endOfTypeName = GetLengthOfTypeName();
                var assemblyIdentifier = Identifier.Substring(endOfTypeName).Trim(' ', ',');
                return new AssemblyName(assemblyIdentifier);
            }
        }

        protected int GetLengthOfTypeName()
        {
            var id = Identifier;
            if (TypeUtils.IsUnknownTypeIdentifier(id))
            {
                return id.Length;
            }

            var lastComma = id.LastIndexOf(',');
            var x = id.FindPrevious(lastComma - 1, ',', ']', '+');
            if (x == -1)
            {
                return lastComma;
            }
            return id[x] == ',' ? x : id.FindNext(x, ',');
        }

        public override INamespaceName Namespace
        {
            get
            {
                if (IsUnknown)
                {
                    return new NamespaceName();
                }
                if (IsDelegateType)
                {
                    return DeclaringType.Namespace;
                }

                var id = RemoveTypeParameterListButKeepTicks(FullName);

                var endIndexOfNamespaceIdentifier = id.LastIndexOf('.');
                return endIndexOfNamespaceIdentifier < 0
                    ? new NamespaceName("")
                    : new NamespaceName(id.Substring(0, endIndexOfNamespaceIdentifier));
            }
        }

        private string _fullName;

        public override string FullName
        {
            get
            {
                if (_fullName == null)
                {
                    var length = GetLengthOfTypeName();
                    _fullName = Identifier.Substring(0, length);
                    if (IsEnumType || IsInterfaceType || IsStructType)
                    {
                        var startIdx = _fullName.IndexOf(":", StringComparison.Ordinal) + 1;
                        _fullName = _fullName.Substring(startIdx);
                    }
                }
                return _fullName;
            }
        }

        private static string RemoveTypeParameterListButKeepTicks(string fullName)
        {
            var startIdx = fullName.IndexOf('[');
            if (startIdx != -1)
            {
                var endIdx = fullName.FindCorrespondingCloseBracket(startIdx);
                var genericInfo = fullName.Substring(startIdx, endIdx - startIdx);
                return fullName.Replace(genericInfo, "");
            }
            return fullName;
        }

        public override string Name
        {
            get
            {
                var rawFullName = RemoveTypeParameterListButKeepTicks(FullName);
                var endOfOutTypeName = rawFullName.LastIndexOf('+');
                if (endOfOutTypeName > -1)
                {
                    rawFullName = rawFullName.Substring(endOfOutTypeName + 1);
                }
                var endOfTypeName = rawFullName.LastIndexOf('`');
                if (endOfTypeName > -1)
                {
                    rawFullName = rawFullName.Substring(0, endOfTypeName);
                }
                var startIndexOfSimpleName = rawFullName.LastIndexOf('.');
                return rawFullName.Substring(startIndexOfSimpleName + 1);
            }
        }

        public override ITypeName DeclaringType
        {
            get
            {
                var plus = FindPlus(Identifier);
                if (plus == -1)
                {
                    return null;
                }

                var start = Identifier.StartsWith(PrefixEnum)
                    ? PrefixEnum.Length
                    : Identifier.StartsWith(PrefixInterface)
                        ? PrefixInterface.Length
                        : Identifier.StartsWith(PrefixStruct) ? PrefixStruct.Length : 0;

                var declTypeId = Identifier.Substring(start, plus - start);

                return new TypeName("{0}, {1}".FormatEx(declTypeId, Assembly.Identifier));
            }
        }

        public override bool IsNestedType
        {
            get { return FindPlus(Identifier) != -1; }
        }

        private int FindPlus(string id)
        {
            var comma = id.Length - Assembly.Identifier.Length;
            // unknown type
            if (comma < 0)
            {
                return -1;
            }
            var plus = id.FindPrevious(comma, '+', ']');
            if (plus == -1)
            {
                return -1;
            }
            // is generic
            if (id[plus] == ']')
            {
                var closeGeneric = id.FindCorrespondingOpenBracket(plus);
                plus = id.FindPrevious(closeGeneric, '+');
            }
            return plus;
        }

        private static readonly Regex MissingTickForGenericsMatcher = new Regex("[a-zA-Z]\\[\\[");

        public static bool IsTypeNameIdentifier(string id)
        {
            if (TypeUtils.IsUnknownTypeIdentifier(id) ||
                PredefinedTypeName.IsPredefinedTypeNameIdentifier(id) ||
                TypeParameterName.IsTypeParameterNameIdentifier(id) ||
                ArrayTypeName.IsArrayTypeNameIdentifier(id) ||
                DelegateTypeName.IsDelegateTypeNameIdentifier(id))
            {
                return false;
            }

            // unbalanced brackets
            foreach (var pair in new[] {"[]", "()"})
            {
                if (Count(id, pair[0]) != Count(id, pair[1]))
                {
                    return false;
                }
            }

            if (MissingTickForGenericsMatcher.IsMatch(id))
            {
                return false;
            }

            return true;
        }

        private static int Count(string id, char needle)
        {
            return id.Count(c => c == needle);
        }
    }
}