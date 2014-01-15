using System.Collections.Generic;

namespace KaVE.Model.Names
{
    public interface IMethodName : IMemberName, IGenericName
    {
        IList<IParameterName> Parameters { get; }
        bool HasParameters { get; }
        bool IsConstructor { get; }
        ITypeName ReturnType { get; }
    }
}