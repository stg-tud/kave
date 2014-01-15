namespace KaVE.Model.Names
{
    public interface IFieldName : IMemberName
    {
        ITypeName ValueType { get; }
    }
}