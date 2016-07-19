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
using KaVE.Commons.Utils.Assertion;
using KaVE.Commons.Utils.Collections;

namespace KaVE.Commons.Model.Naming.Impl.v0.Types
{
    public class TypeName : BaseName, ITypeName
    {
        public override bool IsUnknown
        {
            get { return Equals(this, UnknownTypeName.Instance); }
        }

        public TypeName() : base(UnknownTypeName.Identifier) {}

        public TypeName(string identifier) : base(identifier) {}

        public virtual bool IsUnknownType
        {
            get { return false; }
        }

        public virtual string FullName
        {
            get
            {
                var length = GetLengthOfTypeName(Identifier);
                return Identifier.Substring(0, length);
            }
        }

        protected static int GetLengthOfTypeName(string identifier)
        {
            if (UnknownTypeName.IsUnknownTypeIdentifier(identifier))
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

        public string RawFullName
        {
            get
            {
                var fullName = FullName;
                var indexOfGenericList = fullName.IndexOf("[[", StringComparison.Ordinal);
                if (indexOfGenericList < 0)
                {
                    indexOfGenericList = fullName.IndexOf(", ", StringComparison.Ordinal);
                }
                return indexOfGenericList < 0 ? fullName : fullName.Substring(0, indexOfGenericList);
            }
        }

        public virtual IAssemblyName Assembly
        {
            get
            {
                var endOfTypeName = GetLengthOfTypeName(Identifier);
                var assemblyIdentifier = Identifier.Substring(endOfTypeName).Trim(' ', ',');
                return new AssemblyName(assemblyIdentifier);
            }
        }

        public virtual INamespaceName Namespace
        {
            get
            {
                var id = RawFullName;
                var endIndexOfNamespaceIdentifier = id.LastIndexOf('.');
                return endIndexOfNamespaceIdentifier < 0
                    ? NamespaceName.GlobalNamespace
                    // TODO NAmeUpdate: use fixer/updater and then "Names" factory
                    : new NamespaceName(id.Substring(0, endIndexOfNamespaceIdentifier));
            }
        }

        public string Name
        {
            get
            {
                var rawFullName = RawFullName;
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

        public IKaVEList<ITypeParameterName> TypeParameters
        {
            get { return HasTypeParameters ? FullName.ParseTypeParameters() : Lists.NewList<ITypeParameterName>(); }
        }

        public bool IsGenericEntity
        {
            get { return Identifier.IndexOf('`') > 0; }
        }

        public bool HasTypeParameters
        {
            get { return FullName.IndexOf("[[", StringComparison.Ordinal) > -1; }
        }

        public ITypeName DeclaringType
        {
            get
            {
                try
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
                        return UnknownTypeName.Instance;
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
                    // TODO NameUpdate: breaks caching
                    return new TypeName(declaringTypeName + ", " + Assembly);
                }
                catch (Exception e)
                {
                    // TODO @seb: fix analyse and remove try/catch
                    Console.WriteLine("TypeName.DeclaringType: exception caught, falling back to unknown TypeName");
                    Console.WriteLine(e);
                    return UnknownTypeName.Instance;
                }
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

        public virtual bool IsVoidType
        {
            get { return false; }
        }

        public bool IsValueType
        {
            get { return IsStructType || IsEnumType || IsVoidType; }
        }

        public virtual bool IsStructType
        {
            get { return false; }
        }

        public virtual bool IsSimpleType
        {
            get { return false; }
        }

        public virtual bool IsEnumType
        {
            get { return false; }
        }

        public virtual bool IsNullableType
        {
            get { return false; }
        }

        public bool IsReferenceType
        {
            get { return IsClassType || IsInterfaceType || IsArrayType || IsDelegateType; }
        }

        public bool IsClassType
        {
            get { return !IsValueType && !IsInterfaceType && !IsArrayType && !IsDelegateType && !IsUnknownType; }
        }

        public virtual bool IsInterfaceType
        {
            get { return false; }
        }

        public virtual bool IsArrayType
        {
            get { return false; }
        }

        public virtual ITypeName ArrayBaseType
        {
            get { return null; }
        }

        public ITypeName DeriveArrayTypeName(int rank)
        {
            return ArrayTypeName.From(this, rank);
        }

        public virtual bool IsDelegateType
        {
            get { return false; }
        }

        public bool IsNestedType
        {
            get { return RawFullName.Contains("+"); }
        }

        public bool IsTypeParameter
        {
            get { return false; }
        }

        public IDelegateTypeName AsDelegateTypeName
        {
            get { throw new NotImplementedException(); }
        }

        public IArrayTypeName AsArrayTypeName
        {
            get
            {
                var atn = this as IArrayTypeName;
                Asserts.NotNull(atn);
                return atn;
            }
        }

        public ITypeParameterName AsTypeParameterName
        {
            get { throw new NotImplementedException(); }
        }

        public string TypeParameterShortName
        {
            get { return null; }
        }

        public ITypeName TypeParameterType
        {
            get { return null; }
        }
    }
}