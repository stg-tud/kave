namespace KaVE.Model.Names
{
    public interface IPropertyName : IMemberName
    {
        // TODO add support for parameters
        bool HasSetter { get; }
        bool HasGetter { get; }
        ITypeName ValueType { get; }
    }
}