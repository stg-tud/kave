namespace KaVE.Model.Names
{
    public interface IMemberName : IName
    {
        ITypeName DeclaringType { get; }
        bool IsStatic { get; }
        string Name { get; }
    }
}