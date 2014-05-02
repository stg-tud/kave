using System.Collections.Generic;

namespace KaVE.Model.Names
{
    public interface IGenericName : IName
    {
        bool IsGenericEntity { get; }

        /// <summary>
        /// Whether the name contains a list of type parameters.
        /// </summary>
        bool HasTypeParameters { get; }

        IList<ITypeName> TypeParameters { get; }
    }
}
