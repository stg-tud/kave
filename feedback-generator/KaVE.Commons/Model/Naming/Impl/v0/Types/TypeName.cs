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
using KaVE.Commons.Model.Naming.Impl.v0.Types.Organization;
using KaVE.Commons.Model.Naming.Types;
using KaVE.Commons.Model.Naming.Types.Organization;
using KaVE.Commons.Utils;

namespace KaVE.Commons.Model.Naming.Impl.v0.Types
{
    public class TypeName : BaseTypeName
    {
        public TypeName() : this(UnknownTypeIdentifier) {}

        public TypeName(string identifier) : base(identifier) {}

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
                var endOfTypeName = GetLengthOfTypeName(Identifier);
                var assemblyIdentifier = Identifier.Substring(endOfTypeName).Trim(' ', ',');
                return new AssemblyName(assemblyIdentifier);
            }
        }

        protected static int GetLengthOfTypeName(string identifier)
        {
            if (TypeUtils.IsUnknownTypeIdentifier(identifier))
            {
                return identifier.Length;
            }
            var length = identifier.LastIndexOf(']') + 1;
            if (length > 0)
            {
                return length;
            }
            return identifier.IndexOf(',');
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

        public override string FullName
        {
            get
            {
                var length = GetLengthOfTypeName(Identifier);
                var fn = Identifier.Substring(0, length);
                if (IsEnumType || IsInterfaceType || IsStructType)
                {
                    var startIdx = fn.IndexOf(":", StringComparison.Ordinal) + 1;
                    return fn.Substring(startIdx);
                }
                return fn;
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
                if (!IsNestedType)
                {
                    return null;
                }

                var fullName = FullName;
                if (HasTypeParameters)
                {
                    fullName = TakeUntilChar(fullName, new[] {'[', ','});
                }
                var endOfDeclaringTypeName = fullName.LastIndexOf('+');
                if (endOfDeclaringTypeName == -1)
                {
                    return new TypeName();
                }

                var declaringTypeName = fullName.Substring(0, endOfDeclaringTypeName);
                if (declaringTypeName.IndexOf('`') > -1 && HasTypeParameters)
                {
                    var startIndex = 0;
                    var numberOfParameters = 0;
                    while ((startIndex = declaringTypeName.IndexOf('`', startIndex) + 1) > 0)
                    {
                        var endIndex = declaringTypeName.IndexOf('+', startIndex);
                        if (endIndex > -1)
                        {
                            numberOfParameters +=
                                int.Parse(declaringTypeName.Substring(startIndex, endIndex - startIndex));
                        }
                        else
                        {
                            numberOfParameters += int.Parse(declaringTypeName.Substring(startIndex));
                        }
                    }
                    var outerTypeParameters = TypeParameters.Take(numberOfParameters).ToList();
                    declaringTypeName += "[[" + String.Join("],[", outerTypeParameters.Select(t => t.Identifier)) +
                                         "]]";
                }
                return TypeUtils.CreateTypeName(declaringTypeName + ", " + Assembly);
            }
        }

        private static string TakeUntilChar(string fullName, char[] stopChars)
        {
            var i = 0;
            foreach (var c in fullName)
            {
                if (stopChars.Contains(c))
                {
                    break;
                }
                i++;
            }

            return fullName.Substring(0, i);
        }

        public ITypeName DeriveArrayTypeName(int rank)
        {
            return ArrayTypeName.From(this, rank);
        }

        public override bool IsNestedType
        {
            get { return RemoveTypeParameterListButKeepTicks(FullName).Contains("+"); }
        }
    }
}