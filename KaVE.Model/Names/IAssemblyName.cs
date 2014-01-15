namespace KaVE.Model.Names
{
    public interface IAssemblyName : IName
    {
        IAssemblyVersion AssemblyVersion { get; }

        string Name { get; }
    }
}