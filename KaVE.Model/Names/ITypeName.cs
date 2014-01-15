namespace KaVE.Model.Names
{
    /// <summary>
    /// Represents a full-qualified type name. Such names can have
    /// generic type parameters.
    /// </summary>
    public interface ITypeName : IGenericName
    {
        /// <summary>
        /// The name of the bundle (e.g., assembly) this type is declared in.
        /// </summary>
        IAssemblyName Assembly { get; }

        /// <summary>
        /// A full-qualified identifier of the namespace
        /// containing the type.
        /// </summary>
        INamespaceName Namespace { get; }

        /// <summary>
        /// The name of the type declaring this type or
        /// <code>null</code> if this type is not nested.
        /// </summary>
        ITypeName DeclaringType { get; }

        string FullName { get; }

        string Name { get; }

        bool IsUnknownType { get; }

        bool IsVoidType { get; }

        /// <summary>
        /// Value types are simple (or primitive) types, enum types,
        /// struct types and nullable types (the extension of all other
        /// value types with the <code>null</code> value).
        /// </summary>
        bool IsValueType { get; }

        /// <summary>
        /// <returns>Wheather this is a simple (or primitive) type</returns>
        /// </summary>
        bool IsSimpleType { get; }

        bool IsEnumType { get; }

        bool IsStructType { get; }

        /// <summary>
        /// <returns>Wheather this is a value type that can also take the
        /// <code>null</code> value</returns>
        /// </summary>
        bool IsNullableType { get; }

        /// <summary>
        /// Reference types are (abstract) class types, interface types,
        /// array types, and delegate types.
        /// </summary>
        bool IsReferenceType { get; }

        bool IsClassType { get; }
        bool IsInterfaceType { get; }
        bool IsDelegateType { get; }
        bool IsNestedType { get; }

        bool IsArrayType { get; }

        /// <summary>
        /// Creates an array-type name from this name.
        /// </summary>
        /// <param name="rank">the rank of the array; must be greater than 0</param>
        ITypeName DeriveArrayTypeName(int rank);

        bool IsTypeParameter { get; }
        string TypeParameterShortName { get; }
        ITypeName TypeParameterType { get; }
    }
}