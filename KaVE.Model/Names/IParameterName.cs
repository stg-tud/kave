namespace KaVE.Model.Names
{
    public interface IParameterName : IName
    {
        ITypeName ValueType { get; }
        string Name { get; }
        bool IsPassedByReference { get; }
        bool IsOutput { get; }
        bool IsParameterArray { get; }
        bool IsOptional { get; }
    }
}