using System.Collections.Generic;

namespace KaVE.Model.Names
{
    public interface IGenericName : IName
    {
        bool IsGenericType { get; }
        bool HasTypeParameters { get; }
        IList<ITypeName> TypeParameters { get; }
    }
}
