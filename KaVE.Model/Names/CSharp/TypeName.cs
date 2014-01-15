using System;
using System.Collections.Generic;
using System.Linq;
using KaVE.Model.Utils;
using KaVE.Utils.Assertion;

namespace KaVE.Model.Names.CSharp
{
    public class TypeName : Name, ITypeName
    {
        public const string UnknownTypeIdentifier = "?";

        private static readonly WeakNameCache<ITypeName> Registry = WeakNameCache<ITypeName>.Get(CreateTypeName);

        private static ITypeName CreateTypeName(string identifier)
        {
            return TypeParameterName.IsTypeParameterIdentifier(identifier)
                ? (ITypeName) new TypeParameterName(identifier)
                : new TypeName(identifier);
        }


        /// <summary>
        /// Type names follow the scheme <code>'fully-qualified type name''generic type parameters', 'assembly identifier'</code>.
        /// Examples of type names are:
        /// <list type="bullet">
        ///     <item><description><code>System.Int32, mscore, Version=4.0.0.0</code></description></item>
        ///     <item><description><code>System.Nullable`1[[T -> System.Int32, mscore, Version=4.0.0.0]], mscore, Version=4.0.0.0</code></description></item>
        ///     <item><description><code>System.Collections.Dictionary`2[[TKey -> System.Int32, mscore, Version=4.0.0.0],[TValue -> System.String, mscore, Version=4.0.0.0]], mscore, Version=4.0.0.0</code></description></item>
        ///     <item><description><code>Namespace.OuterType+InnerType, Assembly, Version=1.2.3.4</code></description></item>
        ///     <item><description><code>enum EnumType, Assembly, Version=1.2.3.4</code></description></item>
        ///     <item><description><code>interface InterfaceType, Assembly, Version=1.2.3.4</code></description></item>
        ///     <item><description><code>struct StructType, Assembly, Version=1.2.3.4</code></description></item>
        /// </list>
        /// parameter-type names follow the scheme <code>'short-name' -> 'actual-type identifier'</code>, with actual-type identifier being
        /// either the identifier of a type name, as declared above, or another parameter-type name.
        /// </summary>
        public new static ITypeName Get(string identifier)
        {
            if (identifier == String.Empty)
            {
                identifier = UnknownTypeIdentifier;
            }

            return Registry.GetOrCreate(identifier);
        }

        private static ITypeName Get(string fullName, AssemblyName assemblyName)
        {
            return Get(fullName + ", " + assemblyName);
        }

        private TypeName(string identifier, bool isStruct = false, bool isEnum = false, bool isInterface = false)
            : base(identifier)
        {
            IsCustomStruct = isStruct;
            IsEnumType = isEnum;
            IsInterfaceType = isInterface;
        }

        public bool IsUnknownType
        {
            get { return Identifier.EndsWith(UnknownTypeIdentifier); }
        }

        public string FullName
        {
            get
            {
                var length = EndOfTypeName - StartOfTypeName;
                return Identifier.Substring(StartOfTypeName, length);
            }
        }

        private int StartOfTypeName
        {
            get { return IsTypeParameter ? Identifier.IndexOf(">", StringComparison.Ordinal) + 2 : 0; }
        }

        private int EndOfTypeName
        {
            get
            {
                var length = Identifier.LastIndexOf(']') + 1;
                if (length == 0)
                {
                    length = Identifier.IndexOf(',');
                    if (length == -1)
                    {
                        length = Identifier.Length;
                    }
                }
                return length;
            }
        }

        private string RawFullName
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

        public IAssemblyName Assembly
        {
            get
            {
                if (IsUnknownType)
                {
                    return null;
                }
                var endOfTypeIdentifier = FullName.Length;
                var assemblyIdentifier = Identifier.Substring(endOfTypeIdentifier).Trim(new[] {' ', ','});
                return AssemblyName.Get(assemblyIdentifier);
            }
        }

        public INamespaceName Namespace
        {
            get
            {
                if (IsUnknownType)
                {
                    return null;
                }
                var id = RawFullName;
                var endIndexOfNamespaceIdentifier = IsNestedType ? id.LastIndexOf('+') : id.LastIndexOf('.');
                return NamespaceName.Get(id.Substring(0, endIndexOfNamespaceIdentifier));
            }
        }

        public string Name
        {
            get
            {
                var rawFullName = RawFullName;
                var startIndexOfSimpleName = rawFullName.LastIndexOf('.');
                return rawFullName.Substring(startIndexOfSimpleName + 1);
            }
        }

        public IList<ITypeName> TypeParameters
        {
            get
            {
                var parameters = new List<ITypeName>();
                var fullName = FullName;
                var indexOfParameterList = fullName.IndexOf('[');
                if (indexOfParameterList > -1)
                {
                    var braces = 0;
                    var startIndex = indexOfParameterList + 1;
                    var endIndex = startIndex;
                    while (endIndex < fullName.Length)
                    {
                        var c = fullName[endIndex];

                        if (c == '[')
                        {
                            braces++;
                        }
                        else if (c == ']')
                        {
                            braces--;

                            if (braces == 0)
                            {
                                var indexAfterOpeningBrace = startIndex + 1;
                                var lengthToBeforeClosingBrace = endIndex - startIndex - 1;
                                var descriptor = fullName.Substring(indexAfterOpeningBrace, lengthToBeforeClosingBrace);
                                var parameterTypeName = Get(descriptor);
                                parameters.Add(parameterTypeName);
                                startIndex = fullName.IndexOf('[', endIndex);
                            }
                        }

                        endIndex++;
                    }
                }
                return parameters;
            }
        }

        public bool IsGenericType
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
                if (!IsNestedType)
                {
                    return null;
                }

                var fullName = FullName;
                var indexOf = fullName.LastIndexOf('+');
                var declaringTypeName = fullName.Substring(0, indexOf);
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
                    declaringTypeName += "[[" + String.Join("],[", outerTypeParameters.Select(t => t.Identifier)) + "]]";
                }
                return Get(declaringTypeName, (AssemblyName) Assembly);
            }
        }

        public bool IsVoidType
        {
            get { return Identifier.StartsWith("System.Void"); }
        }

        public bool IsValueType
        {
            get { return IsStructType || IsEnumType || IsVoidType; }
        }

        public bool IsStructType
        {
            get { return IsCustomStruct || IsSimpleType || IsNullableType; }
        }

        private bool IsCustomStruct { get; set; }

        public bool IsSimpleType
        {
            get { return IsNumericType() || Identifier.StartsWith("System.Boolean"); }
        }

        private bool IsNumericType()
        {
            return IsIntegralType() || IsFloatingPointType() || Identifier.StartsWith("System.Decimal");
        }

        private bool IsIntegralType()
        {
            switch (RawFullName)
            {
                case "System.SByte":
                case "System.Byte":
                case "System.Int16":
                case "System.UInt16":
                case "System.Int32":
                case "System.UInt32":
                case "System.Int64":
                case "System.UInt64":
                case "System.Char":
                    return true;
                default:
                    return false;
            }
        }

        private bool IsFloatingPointType()
        {
            switch (RawFullName)
            {
                case "System.Single":
                case "System.Double":
                    return true;
                default:
                    return false;
            }
        }

        public bool IsEnumType { get; private set; }

        public bool IsNullableType
        {
            get { return Identifier.StartsWith("System.Nullable"); }
        }

        public bool IsReferenceType
        {
            get { return IsClassType || IsInterfaceType || IsArrayType || IsDelegateType; }
        }

        public bool IsClassType
        {
            get { return !IsValueType && !IsInterfaceType && !IsArrayType && !IsDelegateType && !IsUnknownType; }
        }

        public bool IsInterfaceType { get; private set; }

        public bool IsArrayType
        {
            get { return RawFullName.Contains("["); }
        }

        public ITypeName DeriveArrayTypeName(int rank)
        {
            Asserts.That(rank > 0, "rank smaller than 1");
            var rawFullName = RawFullName;
            var suffix = Identifier.Substring(rawFullName.Length);
            return Get(string.Format("{0}[{1}]{2}", rawFullName, new string(',', rank - 1), suffix));
        }

        public bool IsDelegateType
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