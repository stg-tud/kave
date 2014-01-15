using System;

namespace KaVE.Model.Names
{
    public interface IAssemblyVersion : IName, IComparable<IAssemblyVersion>
    {
    }
}