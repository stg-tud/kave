using System.Collections.Generic;

namespace KaVE.Model.Names
{
    public interface IMethodName : IMemberName, IGenericName
    {
        string Signature { get; }
        IList<IParameterName> Parameters { get; }
        bool HasParameters { get; }
        bool IsConstructor { get; }
        ITypeName ReturnType { get; }
    }
}