using System;
using System.Collections.Generic;
using KaVE.JetBrains.Annotations;
using KaVE.Utils.Assertion;

namespace KaVE.Model.Names.CSharp
{
    public class TypeParameterName : Name, ITypeName
    {
        /// <summary>
        /// The separator between the parameter type's short name and its type.
        /// </summary>
        private const string ParameterNameTypeSeparater = " -> ";

        /// <summary>
        /// Constructor for reflective recreation of names. See <see cref="Get(string,string)"/> for details on how to
        /// instantiate type-parameter names.
        /// </summary>
        [UsedImplicitly]
        public new static ITypeName Get(string identifier)
        {
            return TypeName.Get(identifier);
        }

        /// <summary>
        /// Gets the <see cref="ITypeName"/> for the identifer
        /// <code>'short name' -&gt; 'actual-type identifier'</code>.
        /// </summary>
        public static ITypeName Get(string typeParameterShortName, string actualTypeIdentifier)
        {
            if (actualTypeIdentifier == TypeName.UnknownTypeIdentifier)
            {
                return Get(typeParameterShortName);
            }
            return Get(typeParameterShortName + ParameterNameTypeSeparater + actualTypeIdentifier);
        }

        internal static bool IsTypeParameterIdentifier(string identifier)
        {
            if (IsUnknownTypeIdentifier(identifier))
            {
                return false;
            }
            return IsFreeTypeParameterIdentifier(identifier) || IsBoundTypeParameterIdentifier(identifier);
        }

        private static bool IsUnknownTypeIdentifier(string identifier)
        {
            return identifier == TypeName.UnknownTypeIdentifier;
        }

        private static bool IsFreeTypeParameterIdentifier(string identifier)
        {
            return !identifier.Contains(",");
        }

        private static bool IsBoundTypeParameterIdentifier(string identifier)
        {
            // "T -> System.Void, mscorlib, ..." is a type parameter, because it contains the separator.
            // "System.Nullable`1[[T -> System.Int32, mscorlib, 4.0.0.0]], ..." is not, because
            // the separator is only in the type's parameter-type list, i.e., after the '`'.
            var indexOfMapping = identifier.IndexOf(ParameterNameTypeSeparater, StringComparison.Ordinal);
            var endOfTypeName = identifier.IndexOf('`');
            return indexOfMapping >= 0 && (endOfTypeName == -1 || endOfTypeName > indexOfMapping);
        }

        internal TypeParameterName(string identifier) : base(identifier) {}

        public bool IsGenericType
        {
            get { return TypeParameterType.IsGenericType; }
        }

        public bool HasTypeParameters
        {
            get { return TypeParameterType.HasTypeParameters; }
        }

        public IList<ITypeName> TypeParameters
        {
            get { return TypeParameterType.TypeParameters; }
        }

        public IAssemblyName Assembly
        {
            get { return TypeParameterType.Assembly; }
        }

        public INamespaceName Namespace
        {
            get { return TypeParameterType.Namespace; }
        }

        public ITypeName DeclaringType
        {
            get { return TypeParameterType.DeclaringType; }
        }

        public string FullName
        {
            get { return TypeParameterType.FullName; }
        }

        public string Name
        {
            get { return TypeParameterType.Name; }
        }

        public bool IsUnknownType
        {
            get { return TypeParameterType.IsUnknownType; }
        }

        public bool IsVoidType
        {
            get { return TypeParameterType.IsVoidType; }
        }

        public bool IsValueType
        {
            get { return TypeParameterType.IsValueType; }
        }

        public bool IsSimpleType
        {
            get { return TypeParameterType.IsSimpleType; }
        }

        public bool IsEnumType
        {
            get { return TypeParameterType.IsEnumType; }
        }

        public bool IsStructType
        {
            get { return TypeParameterType.IsStructType; }
        }

        public bool IsNullableType
        {
            get { return TypeParameterType.IsNullableType; }
        }

        public bool IsReferenceType
        {
            get { return TypeParameterType.IsReferenceType; }
        }

        public bool IsClassType
        {
            get { return TypeParameterType.IsClassType; }
        }

        public bool IsInterfaceType
        {
            get { return TypeParameterType.IsInterfaceType; }
        }

        public bool IsDelegateType
        {
            get { return TypeParameterType.IsDelegateType; }
        }

        public bool IsNestedType
        {
            get { return TypeParameterType.IsNestedType; }
        }

        public bool IsArrayType
        {
            get { return TypeParameterType.IsArrayType; }
        }

        // TODO test this method
        public ITypeName DeriveArrayTypeName(int rank)
        {
            Asserts.That(rank > 0, "rank smaller than 1");
            var typeParameterShortName = TypeParameterShortName;
            var suffix = Identifier.Substring(typeParameterShortName.Length);
            return TypeName.Get(string.Format("{0}[{1}]{2}", typeParameterShortName, new string(',', rank - 1), suffix));
        }

        public bool IsTypeParameter
        {
            get { return true; }
        }

        public string TypeParameterShortName
        {
            get
            {
                var endOfTypeParameterName = Identifier.IndexOf(' ');
                return endOfTypeParameterName == -1 ? Identifier : Identifier.Substring(0, endOfTypeParameterName);
            }
        }

        public ITypeName TypeParameterType
        {
            get
            {
                var startOfTypeName = TypeParameterShortName.Length + ParameterNameTypeSeparater.Length;
                return
                    TypeName.Get(
                        startOfTypeName >= Identifier.Length
                            ? TypeName.UnknownTypeIdentifier
                            : Identifier.Substring(startOfTypeName));
            }
        }
    }
}